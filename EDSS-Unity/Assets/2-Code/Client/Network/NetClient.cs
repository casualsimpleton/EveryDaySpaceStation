//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// NetClient - Class containing TcpClient and ClientConnection, representing a client->server connection
// Created: January 19 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 19 2016
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;
using EveryDaySpaceStation.Network;

namespace EveryDaySpaceStation.Network
{
    public class NetClient
    {
        #region Vars
        /// <summary>
        /// Name of this particular connection
        /// </summary>
        protected string _clientConnectionName;

        /// <summary>
        /// Underlying TcpClient
        /// </summary>
        protected TcpClient _tcpClient;

        /// <summary>
        /// Contains game related and higher level logic for tcp client
        /// </summary>
        protected NetGameClientConnection _netGameClient;

        /// <summary>
        /// Is this connection active (either connected, attempting to connect or something similar)
        /// </summary>
        protected bool _isActive;

        /// <summary>
        /// Delegate for when connection is established to tcplistener
        /// </summary>
        protected OnTcpClientConnect _onTcpConnect;

        /// <summary>
        /// Delegate for after when data is sent
        /// </summary>
        protected OnTcpClientDataTransmit _onTcpDataTransmit;

        /// <summary>
        /// Delegate for after when data is received
        /// </summary>
        protected OnTcpClientDataReceived _onTcpDataReceived;

        /// <summary>
        /// Delegate for when a disconnect event happens
        /// </summary>
        protected OnTcpClientDisconnect _onTcpDisconnect;
        #endregion

        #region Gets/Sets
        public bool IsConnected
        {
            get
            {
                if (_tcpClient == null || _tcpClient.Client == null || _netGameClient == null)
                {
                    return false;
                }

                return _tcpClient.Connected;
            }
        }

        public IPEndPoint RemoteIPAddress
        {
            get { return ((IPEndPoint)_tcpClient.Client.RemoteEndPoint); }
        }

        public NetGameClientConnection Netgameclient
        {
            get { return _netGameClient; }
        }
        #endregion

        public NetClient(string connectionName, OnTcpClientConnect onConnectDelegate, OnTcpClientDisconnect onDisconnectDelegate,
            OnTcpClientDataTransmit onDataTransmitDelegate, OnTcpClientDataReceived onDataReceivedDelegate)
        {
            _tcpClient = new TcpClient();
            _clientConnectionName = connectionName;

            _onTcpConnect = onConnectDelegate;
            _onTcpDisconnect = onDisconnectDelegate;
            _onTcpDataReceived = onDataReceivedDelegate;
            _onTcpDataTransmit = onDataTransmitDelegate;
        }

        public void Connect(IPAddress ipAddr, int port)
        {
            if (_tcpClient == null)
            {
                Debug.LogError("TCP Client is null, it can't be");
                return;
            }

            //Double check it's not already connected
            if (_tcpClient.Connected)
            {
                _tcpClient.Close();
                _tcpClient = null;
                _tcpClient = new TcpClient();
            }

            if (IsConnected)
            {
                Debug.Log(string.Format("Disconnecting existing connection to {0}.", _tcpClient.Client.RemoteEndPoint.ToString()));
                _tcpClient.Client.Disconnect(true);
            }

            _tcpClient.BeginConnect(ipAddr, port, ConnectionStarted, null);
        }

        public void Close()
        {
            _isActive = false;
            Debug.Log("Closing netclient");
            //_netGameClient.Close(false);

            if (_tcpClient != null && _tcpClient.Client != null)
            {
                _tcpClient.Client.Shutdown(SocketShutdown.Both);
                _tcpClient.Client.Close();
            }
        }

        private void ConnectionStarted(System.IAsyncResult iar)
        {
            if (iar == null)
            {
                return;
            }

            try
            {
                _tcpClient.EndConnect(iar);
            }
            catch (System.Exception ex)
            {
                if (_tcpClient.Client == null)
                {
                    return;
                }

                Debug.LogError(string.Format("Could not connect. Reason: {0}", ex.Message));

                if (_onTcpDisconnect != null)
                {
                    _onTcpDisconnect(null, NetworkConnectionStatus.ConnectionFailed);
                }

                return;
            }

            Debug.Log(string.Format("Netclient: Client connected to {0}", _tcpClient.Client.RemoteEndPoint.ToString()));

            if (_netGameClient != null)
            {
                //Connection already exists, close it
                _netGameClient = null;
            }

            _netGameClient = new NetGameClientConnection(_tcpClient, OnTcpClientConnect, OnTcpClientDisconnect, OnTcpDataReceived, OnTcpDataTransmitted);

            _isActive = true;
        }

        public void OnTcpClientConnect(NetGameClientConnection newConnection)
        {
            if (_onTcpConnect != null)
            {
                _onTcpConnect(newConnection);
            }
        }

        public void OnTcpClientDisconnect(IPEndPoint remoteEP, NetworkConnectionStatus connStatus)
        {
            if (_onTcpDisconnect != null)
            {
                _onTcpDisconnect(remoteEP, connStatus);
            }
        }

        public void OnTcpDataReceived(NetMessage msg)
        {
            if (_onTcpDataReceived != null)
            {
                _onTcpDataReceived(msg);
            }
        }

        public void OnTcpDataTransmitted()
        {
            if (_onTcpDataTransmit != null)
            {
                _onTcpDataTransmit();
            }
        }

        public void SendNetMessage(NetMessageType msgType, NetByteBuffer nbb, bool useCompression = false)
        {
            if (!IsConnected)
            {
                Debug.Log("Not connected! Can't send message");
                return;
            }

            _netGameClient.SendMessage(msgType, nbb, useCompression);
        }
    }
}