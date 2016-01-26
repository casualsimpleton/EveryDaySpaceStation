//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// NetServer - Classes for server with a tcp listener. Abstract base class incase multiple connections are necessary
// Created: January 19 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 19 2016
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;
using EveryDaySpaceStation.Network;

namespace EveryDaySpaceStation.Network
{
    public abstract class NetServer
    {
        #region Vars
        /// <summary>
        /// TCP Listener
        /// </summary>
        protected TcpListener _tcpListener;

        /// <summary>
        /// Listen port
        /// </summary>
        protected int _port;

        /// <summary>
        /// IP Address for this listener
        /// </summary>
        protected IPAddress _ipAddr;

        /// <summary>
        /// Internal listener server name "Gameplay", "File", etc. Not meant for external use (e.g. client facing)
        /// </summary>
        protected string _serverName;

        /// <summary>
        /// Holding place for clients in handshake
        /// </summary>
        protected Dictionary<IPEndPoint, NetGameServerConnection> _pendingClients;

        /// <summary>
        /// Holding place for clients who have made it past handshake, and assuming approved for gameplay
        /// </summary>
        protected Dictionary<IPEndPoint, NetGameServerConnection> _connectedClients;
        
        /// <summary>
        /// A list to hold IPEndPoints that need to be removed from _pendingClients or _connectedClients
        /// </summary>
        protected List<IPEndPoint> _tempRemovalList;

        /// <summary>
        /// Is this server running?
        /// </summary>
        protected bool _isRunning;

        protected Thread _updateThread;

        public NetGameBaseConnection.NetTransmissionStats _netStats = new NetGameBaseConnection.NetTransmissionStats();

        /// <summary>
        /// How long before a connection is considered timed out
        /// </summary>
        public const int CONNECTIONTIMEOUT = 30;
        #endregion

        #region Delegates
        protected OnTcpServerClientConnect _onClientConnect;
        protected OnTcpServerClientDisconnect _onClientDisconnect;
        protected OnTcpServerDataReceived _onDataReceived;
        protected OnTcpServerDataTransmit _onDataTransmitted;
        protected OnTcpServerClientAccepted _onConnectionAccepted;
        #endregion

        #region Gets/Sets
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public IPAddress IPAddr
        {
            get { return _ipAddr; }
            set { _ipAddr = value; }
        }

        public TcpListener Listener
        {
            get { return _tcpListener; }
        }

        public string ServerName
        {
            get { return _serverName; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public int PendingNumberOfConnections
        {
            get { return _pendingClients.Count; }
        }

        public int NumberOfConnections
        {
            get { return _connectedClients.Count; }
        }

        public Dictionary<IPEndPoint, NetGameServerConnection> PendingClients
        {
            get { return _pendingClients; }
        }

        public Dictionary<IPEndPoint, NetGameServerConnection> ConnectedClients
        {
            get { return _connectedClients; }
        }

        public bool GetPendingConnection(IPEndPoint ipEP, out NetGameServerConnection connection)
        {
            return _pendingClients.TryGetValue(ipEP, out connection);
        }

        public bool GetConnection(IPEndPoint ipEP, out NetGameServerConnection connection)
        {
            return _connectedClients.TryGetValue(ipEP, out connection);
        }
        #endregion

        #region Abstracts
        public abstract void AddNewConnection(IPEndPoint ipEP, NetGameServerConnection connection);
        public abstract void OnClientConnected(NetGameServerConnection connection);
        protected abstract void CheckOnPendingConnections();
        #endregion

        #region Constructors
        public NetServer(string serverName, int port, IPAddress serverIPAddr,
            OnTcpServerClientConnect clientConnect,
            OnTcpServerClientDisconnect clientDisconnect,
            OnTcpServerDataReceived dataReceived,
            OnTcpServerDataTransmit dataTransmitted,
            OnTcpServerClientAccepted connectionAccepted)
        {
            _serverName = serverName;
            _port = port;
            _ipAddr = serverIPAddr;

            _onClientConnect = clientConnect;
            _onClientDisconnect = clientDisconnect;
            _onDataReceived = dataReceived;
            _onDataTransmitted = dataTransmitted;
            _onConnectionAccepted = connectionAccepted;

            _connectedClients = new Dictionary<IPEndPoint, NetGameServerConnection>();
            _pendingClients = new Dictionary<IPEndPoint, NetGameServerConnection>();
            _tempRemovalList = new List<IPEndPoint>();
        }

        public NetServer(string serverName, int port,
            OnTcpServerClientConnect clientConnect,
            OnTcpServerClientDisconnect clientDisconnect,
            OnTcpServerDataReceived dataReceived,
            OnTcpServerDataTransmit dataTransmitted,
            OnTcpServerClientAccepted connectionAccepted)
            : this(serverName, port, Dns.GetHostEntry("localhost").AddressList[0],
                clientConnect, clientDisconnect, dataReceived, dataTransmitted, connectionAccepted)
        {
        }
        #endregion

        #region Start/Stop Server
        public void StartServer(int port)
        {
            _port = port;
            StartServer();
        }

        public void StartServer()
        {
            if (_port < 1 || _port > ushort.MaxValue - 1)
            {
                Debug.LogError(string.Format("Port {0} is out of range for {1}. Can't start server. Please fix this.", _port, _serverName));
                return;
            }

            if (_ipAddr == null)
            {
                _ipAddr = IPAddress.Parse("0.0.0.0");
            }
            _tcpListener = new TcpListener(_ipAddr, _port);

            StartTCPListener();
        }

        /// <summary>
        /// Begins the tcplistener and set up for async operations. Make sure to do this only once per tcplistener
        /// </summary>
        private void StartTCPListener()
        {
            try
            {
                _tcpListener.Start();

                Debug.Log(string.Format("NETWORK: Server '{0}' started on {1}:{2}", _serverName, _ipAddr, _port));
            }
            catch (Exception exception)
            {
                Debug.LogError(string.Format("NETWORK: Server '{0}' encountered an exception: {1}", _serverName, exception.Message.ToString()));
            }

            _isRunning = true;
            
            //We want to start a separate thread for processing the cycle of input
            _updateThread = new Thread(ServerCycle);
            _updateThread.Start();
        }

        public void Shutdown()
        {
            Debug.Log(string.Format("NETWORK: Server '{0}' is shutting down.", _serverName));

            _isRunning = false;

            if (_updateThread != null)
            {
                _updateThread.Abort();
                _updateThread = null;
            }

            foreach (KeyValuePair<IPEndPoint, NetGameServerConnection> client in _connectedClients)
            {
                try
                {
                    client.Value.DoCloseConnection = true;
                    client.Value.Close();
                }
                catch //Don't care about catching
                {
                }
            }

            foreach (KeyValuePair<IPEndPoint, NetGameServerConnection> client in _pendingClients)
            {
                try
                {
                    client.Value.DoCloseConnection = true;
                    client.Value.Close();
                }
                catch //Don't care about catching
                {
                }
            }

            if (_tcpListener != null)
            {
                _tcpListener.Server.Close();
                _tcpListener.Stop();
            }
        }
        #endregion

        #region Main Loop
        void ServerCycle()
        {
            while (true)
            {
                while (_tcpListener != null && _tcpListener.Pending())
                {
                    NetGameServerConnection newClient = new NetGameServerConnection(_tcpListener.AcceptTcpClient(), this,
                        OnClientConnected, OnClientDisconnect, OnDataReceived, OnDataTransmitted);
                    Debug.Log(string.Format("NETWORK: Server '{0}' established connection with new client: {1}:{2}", _serverName, newClient.RemoteIPAddr, newClient.RemotePort));
                }
                Thread.Sleep(1);
            }
        }
        #endregion

        #region New Connections
        /// <summary>
        /// This is important for looping the listener inside the seperate thread
        /// </summary>
        /// <param name="threadCallback"></param>
        private void ListenForNewConnections(object threadCallback)
        {
            _tcpListener.BeginAcceptTcpClient(AcceptNewTcpConnection, _tcpListener);
            Debug.Log(string.Format("NETWORK: Server '{0}' has accepted new connection...", _serverName));
        }

        private void AcceptNewTcpConnection(IAsyncResult iar)
        {
            //Just to be sure that the listener is still going as we may be out of sync due to multi threading
            if (_tcpListener == null)
            {
                return;
            }

            TcpClient newClient;

            //This is in a try/catch to swallow up the occasional complaint from when the server is shutting down
            try
            {
                newClient = _tcpListener.EndAcceptTcpClient(iar);
            }
            catch
            {
                return;
            }

            //For some reason the server is up, but connections aren't allowed
            //So close it immediately and return
            if (!Network.AllowConnections)
            {
                newClient.GetStream().Close();
                newClient.Close();

                ListenForNewConnections(null);
                return;
            }

            NetGameServerConnection newConnection = new NetGameServerConnection(newClient, this,
                OnClientConnected, OnClientDisconnect, OnDataReceived, OnDataTransmitted);

            Debug.Log(string.Format("NETWORK: Server '{0}' established connection with new client: {1}:{2}", _serverName, newConnection.RemoteIPAddr, newConnection.RemotePort));

            ListenForNewConnections(null);
        }

        public void ConnectionApproved(NetGameServerConnection connection)
        {
            //Remove it first from the pending connections
            try
            {
                lock (_pendingClients)
                {
                    _pendingClients.Remove(connection.RemoteEndPoint);
                }
            }
            catch
            {
            }

            //Now add it to the connected clients list
            try
            {
                _connectedClients.Add(connection.RemoteEndPoint, connection);
            }
            catch
            {
            }
        }
        #endregion

        #region Delegates
        public void OnClientDisconnect(IPEndPoint ipEP, NetworkConnectionStatus connectionStatus)
        {
            if(_onClientDisconnect != null)
            {
                _onClientDisconnect(ipEP, connectionStatus);
                return;
            }

            Debug.Log(string.Format("NETWORK: Server '{0}' has disconnected from {1}:{2}", _serverName, ipEP.Address, ipEP.Port));

            NetGameServerConnection disconnectingClient;

            bool foundConnection = GetConnection(ipEP, out disconnectingClient);

            //Didn't find you in the connected clients, so check pending connections to be sure
            if (!foundConnection)
            {
                foundConnection = GetPendingConnection(ipEP, out disconnectingClient);
            }

            //Found a connection
            if (foundConnection)
            {
                lock (_connectedClients)
                {
                    _connectedClients.Remove(ipEP);
                }

                lock (_pendingClients)
                {
                    _pendingClients.Remove(ipEP);
                }

                disconnectingClient.Close();
                disconnectingClient = null;
            }
        }
        
        public void OnDataReceived(NetMessage message)
        {
            if (_onDataReceived != null)
            {
                _onDataReceived(message);
            }
        }

        public void OnDataTransmitted(NetGameServerConnection connection)
        {
            if (_onDataTransmitted != null)
            {
                _onDataTransmitted(connection);
            }
        }
        #endregion

        #region NetByteBuffers
        public void CheckByteBuffersQueue()
        {
            foreach (KeyValuePair<IPEndPoint, NetGameServerConnection> client in _connectedClients)
            {
                client.Value.ClearFinishedOutgoingBuffers();
            }
        }
        #endregion

        #region Close Connection
        public void CloseConnection(IPEndPoint ipEP, string disconnectReason, bool forceClose)
        {
            NetGameServerConnection connection = null;

            bool found = GetConnection(ipEP, out connection);

            if (!found)
            {
                Debug.LogWarning(string.Format("Can't find a NetGameServerConnection for {0}...", ipEP));
                return;
            }

            //TODO
            if (!string.IsNullOrEmpty(disconnectReason))
            {
                //We want to send a message to that client to let them know
            }

            connection.DoCloseConnection = true;

            if (forceClose)
            {
                try
                {
                    connection.Close();
                }
                catch
                {
                }
            }

            lock (_connectedClients)
            {
                _connectedClients.Remove(ipEP);
            }
        }

        public void ClosePendingConnection(IPEndPoint ipEP, string disconnectReason, bool forceClose)
        {
            NetGameServerConnection connection = null;

            bool found = GetPendingConnection(ipEP, out connection);

            if (!found)
            {
                Debug.LogWarning(string.Format("Can't find a NetGameServerConnection for {0}...", ipEP));
                return;
            }

            //TODO
            if (!string.IsNullOrEmpty(disconnectReason))
            {
                //We want to send a message to that client to let them know
            }

            connection.DoCloseConnection = true;

            if (forceClose)
            {
                try
                {
                    connection.Close();
                }
                catch
                {
                }
            }

            lock (_connectedClients)
            {
                _pendingClients.Remove(ipEP);
            }
        }
        #endregion
    }

    public class NetServerMain : NetServer
    {
        public NetServerMain(string serverName, int port, IPAddress serverIPAddr,
            OnTcpServerClientConnect clientConnect,
            OnTcpServerClientDisconnect clientDisconnect,
            OnTcpServerDataReceived dataReceived,
            OnTcpServerDataTransmit dataTransmitted,
            OnTcpServerClientAccepted connectionAccepted)
            : base(serverName, port, serverIPAddr,
                clientConnect, clientDisconnect, dataReceived, dataTransmitted, connectionAccepted)
        {
        }

        public NetServerMain(string serverName, int port,
            OnTcpServerClientConnect clientConnect,
            OnTcpServerClientDisconnect clientDisconnect,
            OnTcpServerDataReceived dataReceived,
            OnTcpServerDataTransmit dataTransmitted,
            OnTcpServerClientAccepted connectionAccepted)
            : base(serverName, port, clientConnect, clientDisconnect, dataReceived, dataTransmitted, connectionAccepted)
        {
        }

        public override void AddNewConnection(IPEndPoint ipEP, NetGameServerConnection connection)
        {
            connection.HandshakeStatus = NetGameServerConnection.ConnectionHandshakeStatus.NewConnection;

            try
            {
                lock (_pendingClients)
                {
                    _pendingClients.Add(ipEP, connection);
                }

                //Tell this new connection to advance the handshake

                Debug.Log(string.Format("NETWORK: Main Server added pending connection from {0}.", ipEP));
            }
            catch (Exception exception)
            {
                Debug.LogError(string.Format("NETWORK ERROR! Failed to add new pending connection '{0}'.", exception.Message.ToString()));
            }
        }

        public override void OnClientConnected(NetGameServerConnection connection)
        {
            AddNewConnection(connection.RemoteEndPoint, connection);

            if (_onClientConnect != null)
            {
                _onClientConnect(connection);
                return;
            }
        }

        /// <summary>
        /// Check on all pending connections. Either advance them or clean them up
        /// </summary>
        protected override void CheckOnPendingConnections()
        {
            lock (_pendingClients)
            {
                _tempRemovalList.Clear();

                foreach (KeyValuePair<IPEndPoint, NetGameServerConnection> client in _pendingClients)
                {
                    //If the last communication was too long along, disconnect them
                    if (Network.NetworkTime - client.Value.LastReceivedMessageTime > NetServer.CONNECTIONTIMEOUT)
                    {
                        Debug.Log(string.Format("NETWORK: Main Server kicking {0} for taking too long...", client.Key));

                        //TODO Send message to them

                        client.Value.DoCloseConnection = true;
                        _tempRemovalList.Add(client.Key);
                    }
                }

                //Now remove them from the dictionary
                for (int i = 0; i < _tempRemovalList.Count; i++)
                {
                    _pendingClients.Remove(_tempRemovalList[i]);
                }
            }
        }
    }
}