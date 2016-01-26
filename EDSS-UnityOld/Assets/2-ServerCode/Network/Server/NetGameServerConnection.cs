//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// NetGameServerConnection - Class of some higher level game related logic associated with NetServer's lower level tcpclient, as used by the server
// Created: January 23 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 23 2016
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using System;
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
    public class NetGameServerConnection : NetGameBaseConnection
    {
        #region Enums
        public enum ConnectionHandshakeStatus
        {
            NewConnection,
            ConnectionApproved
        }
        #endregion

        #region Vars
        protected NetServer _hostServer;

        /// <summary>
        /// Timestamp when last pinged
        /// </summary>
        protected float _lastPingTime;
        
        /// <summary>
        /// Timestamp when a message was last received
        /// </summary>
        protected float _lastReceivedMsgTime;
        
        protected ConnectionHandshakeStatus _handshakeStatus;
        #endregion

        #region Delegates
        private OnTcpServerClientConnect _onServerConnect;
        private OnTcpServerClientDisconnect _onServerDisconnect;
        private OnTcpServerDataReceived _onServerDataReceived;
        private OnTcpServerDataTransmit _onServerDataTransmitted;
        #endregion

        #region Gets/Sets
        public NetServer netServer
        {
            get { return _hostServer; }
        }

        public float LastPingTime
        {
            get { return _lastPingTime; }
        }

        public float LastReceivedMessageTime
        {
            get { return _lastReceivedMsgTime; }
        }

        public ConnectionHandshakeStatus HandshakeStatus
        {
            get { return _handshakeStatus; }
            set { _handshakeStatus = value; }
        }
        #endregion

        public NetGameServerConnection(TcpClient newClient, NetServer newServer,
            OnTcpServerClientConnect connectDelegate,
            OnTcpServerClientDisconnect disconnectDelegate,
            OnTcpServerDataReceived dataReceivedDelegate,
            OnTcpServerDataTransmit dataTransmittedDelegate)
        {
            _onServerConnect = connectDelegate;
            _onServerDisconnect = disconnectDelegate;
            _onServerDataReceived = dataReceivedDelegate;
            _onServerDataTransmitted = dataTransmittedDelegate;

            _tcpClient = newClient;
            _tcpClient.NoDelay = true;
            _socket = _tcpClient.Client;

            _hostServer = newServer;

            _incReadBuffer = new byte[NetGameBaseConnection.ReadByteSize];

            _currentIncomingMsg = NetworkPool.RequestServerNetMessage();

            _isConnected = true;

            _handshakeStatus = ConnectionHandshakeStatus.NewConnection;

            _lastReceivedMsgTime = Network.NetworkTime;

            Connected();

            StartDataReceive();
        }

        #region Delegate Bodies
        private void Connected()
        {
            if (_onServerConnect != null)
            {
                _onServerConnect(this);
            }

            _isConnected = true;
        }

        /// <summary>
        /// Tells the socket to begin receiving as async
        /// </summary>
        private void StartDataReceive()
        {
            _socket.BeginReceive(_incReadBuffer, 0, _incReadBuffer.Length, SocketFlags.None, ServerAsyncDataReceive, null);
        }

        /// <summary>
        /// Async callback for when data is received
        /// </summary>
        /// <param name="iar"></param>
        public void ServerAsyncDataReceive(IAsyncResult iar)
        {
            int numBytesRead = 0;

            try
            {
                numBytesRead = _socket.EndReceive(iar);

                //Did we finish reading but nothing came? Connection has probably been closed
                if (numBytesRead == 0)
                {
                    if (_onServerDisconnect != null)
                    {
                        _onServerDisconnect(null, NetworkConnectionStatus.UnexpectedConnectionClose);
                    }

                    return;
                }

                IncomingDataReceived(numBytesRead, 0);

                StartDataReceive();
            }
            catch (Exception exception)
            {
                //We got an exception, except we wanted to close the connection anyways, so... just roll with it
                if (DoCloseConnection)
                {
                    return;
                }

                //Poll the connection, see if it fails and if the socket is not available, we've got ourselves a legit problem
                if (_socket.Poll(10, SelectMode.SelectRead) && _socket.Available == 0)
                {
                    //Do nothing
                }
                else
                {
                    Debug.LogError(string.Format("ERROR: Exception ServerAsyncDataReceive(): '{0}'", exception.Message.ToString()));
                }
            }
        }

        /// <summary>
        /// Data is received via async operation. Figures out how to handle data into useable form
        /// </summary>
        /// <param name="bytesReceived"></param>
        /// <param name="offset"></param>
        protected override void IncomingDataReceived(int bytesReceived, int offset)
        {
            lock (_incReadBuffer)
            {
                IPEndPoint ipEP = RemoteEndPoint;

                int remainingUnreadBytes = bytesReceived;

                _netStats.incBytesPerSec += bytesReceived;

                while (remainingUnreadBytes > 0 && _isConnected)
                {
                    offset = bytesReceived - remainingUnreadBytes;
                    remainingUnreadBytes = _currentIncomingMsg.IncomingNewData(_incReadBuffer, ipEP, offset, bytesReceived);

                    //Check if the message has used up all the bytes, or if we need a new message
                    if (remainingUnreadBytes < 0)
                    {
                        //Less than 0 means message isn't full
                        return;
                    }

                    if (_currentIncomingMsg.IsMessageFinished)
                    {
                        //Pass the message up the chain, and get another message
                        if (_onServerDataReceived != null)
                        {
                            _onServerDataReceived(_currentIncomingMsg);
                        }

                        //Get a new message from cache
                        _currentIncomingMsg = NetworkPool.RequestServerNetMessage();
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }
        }
        #endregion

        public override void SendMessage(NetMessageType msgType, NetByteBuffer nbb, bool useCompression = false)
        {
            nbb.data[NetMessage.NETMSGHEADER_MSGTYPE] = NetMessage.GetByteFromNetMessageType(msgType);

            //Convert length to byte array
            byte[] msgLengthArray = System.BitConverter.GetBytes(nbb.CurDataLength);

            //Copy byte array into array
            System.Buffer.BlockCopy(msgLengthArray, 0, nbb.data, NetMessage.NETMSGHEADER_BODYSIZEINDEX, msgLengthArray.Length);

            lock (nbb)
            {
                //Check if this message should go out immediately, or wait in queue
                if (!_isSending && _outGoingByteQueue.Count < 1)
                {
                    StartOutgoingWrite(nbb);
                }
                else
                {
                    //Not sending, but queue isn't empty, so advance queue before adding this
                    if (!_isSending)
                    {
                        AdvanceOutgoingQueue();
                    }

                    _outGoingByteQueue.Enqueue(nbb);

                    if (_outGoingByteQueue.Count > NetMessage.MSGTHRESHOLDWARNING)
                    {
                        Debug.LogWarning(string.Format("Outgoing client message queue above warning threshold: {0} for {1}. Something might be wrong.", _outGoingByteQueue.Count, RemoteIPAddr));
                    }
                }
            }
        }

        protected override void StartOutgoingWrite(NetByteBuffer outgoingData)
        {
            try
            {
                _netStats.outByesPerSec += outgoingData.CurDataLength;

                _isSending = true;
                _socket.BeginSend(outgoingData.data, 0, outgoingData.CurDataLength, SocketFlags.None, EndAsyncWrite, outgoingData);
            }
            catch (SocketException sException)
            {
                Debug.LogError(string.Format("Write Socket Exception in StartOutgoingWrite(). Dropping connection for {0}. Message: '{1}'.", RemoteIPAddr, sException.Message.ToString()));
                Close();
            }
        }

        protected override void EndAsyncWrite(IAsyncResult iar)
        {
            _isSending = false;

            try
            {
                _socket.EndSend(iar);
            }
            catch (SocketException sException)
            {
                Debug.LogError(string.Format("Write Socket Exception in EndAsyncWrite(). Dropping connection to {0}. Message: '{1}'.", RemoteIPAddr, sException.Message.ToString()));
                Close();
            }

            if (_onServerDataTransmitted != null)
            {
                _onServerDataTransmitted(this);
            }

            //Get the NetByteBuffer from Async state and add it to the spent queue
            NetByteBuffer nbb = (iar.AsyncState as NetByteBuffer);

            lock (_spentByteBuffersQueue)
            {
                _spentByteBuffersQueue.Enqueue(nbb);
            }

            //Close the connection?
            if (DoCloseConnection)
            {
                Close();
                return;
            }

            //Advance the out going queue
            AdvanceOutgoingQueue();
        }

        protected override void AdvanceOutgoingQueue()
        {
            lock (_outGoingByteQueue)
            {
                if (_outGoingByteQueue.Count > 0 && !_isSending)
                {
                    NetByteBuffer nbb = _outGoingByteQueue.Dequeue();
                    StartOutgoingWrite(nbb);
                }
            }
        }

        public void Close()
        {
            try
            {
                _tcpClient.Client.Close();
                _tcpClient.Close();
            }
            catch (Exception exception)
            {
#if DEBUGCLIENT
                Debug.LogError(string.Format("ERROR: Problem closing tcpClient. {0}", exception.Message.ToString()));
#endif
            }

            if (_onServerDisconnect != null)
            {
                _onServerDisconnect(null, NetworkConnectionStatus.ServerDisconnect);
            }
        }

        public override void ClearFinishedOutgoingBuffers()
        {
            lock (_spentByteBuffersQueue)
            {
                while (_spentByteBuffersQueue.Count > 0)
                {
                    NetByteBuffer nbb = _spentByteBuffersQueue.Dequeue();

                    NetworkPool.ReturnServerOutBuffer(nbb);
                }
            }
        }
    }
}