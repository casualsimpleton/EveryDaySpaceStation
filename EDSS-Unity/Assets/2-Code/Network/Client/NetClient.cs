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
        protected OnTcpClientDisconnect _onTcpDataDisconnect;
        #endregion

        public NetClient(string connectionName, OnTcpClientConnect onConnectDelegate, OnTcpClientDisconnect onDisconnectDelegate,
            OnTcpClientDataTransmit onDataTransmitDelegate, OnTcpClientDataReceived onDataReceivedDelegate)
        {
            _tcpClient = new TcpClient();
            _clientConnectionName = connectionName;

            _onTcpConnect = onConnectDelegate;
            _onTcpDataDisconnect = onDisconnectDelegate;
            _onTcpDataReceived = onDataReceivedDelegate;
            _onTcpDataTransmit = onDataTransmitDelegate;
        }


    }
}