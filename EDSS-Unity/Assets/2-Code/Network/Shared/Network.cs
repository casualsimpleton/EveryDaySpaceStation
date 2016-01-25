//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// Network - Various classes used by both server and client network
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
    #region Enums
    [System.Flags]
    public enum NetworkConnectionStatus
    {
        Unconnected, //When unconnected
        ConnectionFailed, //Connection was attempted but failed for some reason
        Connected, //Is connected
        ClientDisconnect, //Was connected, but client ended connection
        ServerDisconnect, //Was connected, but server ended connection
        UnexpectedConnectionClose, //Was connected, but connection closed without message
        UnknownError //Unknown problem, like a timeout or network failure
    }

    public enum NetMessageType : byte
    {
        RESERVED = 0,

        CONNECTIONSTART = 1,
        CONNECTIONESTABLISHED = 2,
        DISCONNECT = 3,
        PING = 4,
        PONG = 5
    }
    #endregion

    #region Delegates
    public delegate void OnTcpClientConnect(NetGameClientConnection newConnection);
    public delegate void OnTcpClientDataReceived(NetMessage message);
    public delegate void OnTcpClientDataTransmit();
    public delegate void OnTcpClientDisconnect(IPEndPoint remoteEP, NetworkConnectionStatus connectionStatus);

    public delegate void OnTcpServerClientConnect(NetGameServerConnection newConnection);
    public delegate void OnTcpServerDataReceived(NetMessage message);
    public delegate void OnTcpServerDataTransmit(NetGameServerConnection clientDestination);
    public delegate void OnTcpServerClientDisconnect(IPEndPoint remoteEP, NetworkConnectionStatus connectionStatus);
    public delegate void OnTcpServerClientAccepted(NetGameServerConnection newConnection, uint uniqueID, string playerName);
    #endregion

    #region Utility
    //Inspired greatly by Lidgren's UDP Library
    //https://github.com/lidgren/lidgren-network-gen3/blob/master/Lidgren.Network/NetUtility.cs
    public static class NetUtility
    {
        /// <summary>
        /// Resolve IP address callback
        /// </summary>
        /// <param name="ipAddr"></param>
        public delegate void ResolveAddressCallback(IPAddress ipAddr);

        public static void ResolveAsync(string ipOrHost, ResolveAddressCallback ipAddrCallback)
        {
            if(string.IsNullOrEmpty(ipOrHost))
            {
                Debug.LogException(new System.ArgumentException("String cannot be empty", "ipOrHost"));
            }

            ipOrHost = ipOrHost.Trim();

            IPAddress ipAddr = null;

            //IP
            if (IPAddress.TryParse(ipOrHost, out ipAddr))
            {
                if (ipAddr.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddrCallback(ipAddr);
                    return;
                }

                Debug.LogException(new System.Exception(string.Format("Cannot handle '{0}'. Use IPv4 only.", ipAddr.AddressFamily.ToString())));
            }

            //Hostname
            IPHostEntry entry;
            try
            {
                Dns.BeginGetHostEntry(ipOrHost, delegate(System.IAsyncResult result)
                {
                    try
                    {
                        entry = Dns.EndGetHostEntry(result);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.HostNotFound)
                        {
                            //LogWrite(string.Format(CultureInfo.InvariantCulture, "Failed to resolve host '{0}'.", ipOrHost));
                            ipAddrCallback(null);
                            return;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    if (entry == null)
                    {
                        ipAddrCallback(null);
                        return;
                    }

                    // check each entry for a valid IP address
                    foreach (var ipCurrent in entry.AddressList)
                    {
                        if (ipCurrent.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddrCallback(ipCurrent);
                            return;
                        }
                    }

                    ipAddrCallback(null);
                }, null);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.HostNotFound)
                {
                    //LogWrite(string.Format(CultureInfo.InvariantCulture, "Failed to resolve host '{0}'.", ipOrHost));
                    ipAddrCallback(null);
                }
                else
                {
                    throw;
                }
            }
        }
    }
    #endregion

    public static class Network
    {
        #region Vars
        private static System.Diagnostics.Stopwatch _syncTimer;
        private static bool _allowConnections;
        #endregion

        #region Gets/Sets
        static public float NetworkTime
        {
            get { return _syncTimer.ElapsedMilliseconds * 0.001f; }
        }

        static public bool AllowConnections
        {
            get { return _allowConnections; }
            set { _allowConnections = value; }
        }
        #endregion

        public static void Init()
        {
            _syncTimer = new System.Diagnostics.Stopwatch();
            _syncTimer.Start();
        }

        public static void Shutdown()
        {
            if (_syncTimer != null)
            {
                _syncTimer.Stop();
            }
        }
    }
    
    public static class NetworkPool
    {
        static NetByteBufferPool _msgServerHeaderPool;
        static NetByteBufferPool _msgClientHeaderPool;

        static public void Init()
        {
            _msgServerHeaderPool = new NetByteBufferPool();
            _msgClientHeaderPool = new NetByteBufferPool();
        }

        #region Incoming Buffers
        static public NetByteBuffer RequestServerIncBuffer(int desiredLength)
        {
            return _msgServerHeaderPool.RequestObject(desiredLength);
        }

        static public NetByteBuffer RequestClientIncBuffer(int desiredLength)
        {
            return _msgClientHeaderPool.RequestObject(desiredLength);
        }

        static public void ReturnServerIncBuffer(NetByteBuffer item)
        {
            _msgServerHeaderPool.ReturnObject(item);
        }

        static public void ReturnClientIncBuffer(NetByteBuffer item)
        {
            _msgClientHeaderPool.ReturnObject(item);
        }
        #endregion

        #region Outgoing Buffers
        static public NetByteBuffer RequestServerOutBuffer(int desiredLength)
        {
            return new NetByteBuffer(desiredLength);
        }

        static public NetByteBuffer RequestClientOutBuffer(int desiredLength)
        {
            return new NetByteBuffer(desiredLength);
        }

        static public void ReturnServerOutBuffer(NetByteBuffer nbb)
        {
        }

        static public void ReturnClientOutBuffer(NetByteBuffer nbb)
        {
        }
        #endregion

        //TODO - FIX THIS
        static public NetMessage RequestClientNetMessage()
        {
            return new NetMessage();
        }

        static public NetMessage RequestServerNetMessage()
        {
            return new NetMessage();
        }
    }
}