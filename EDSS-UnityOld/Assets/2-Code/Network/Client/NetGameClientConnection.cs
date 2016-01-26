//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// NetGameClientConnection - Class of some higher level game related logic associated with NetClient's lower level tcpclient, as used by the client
// Created: January 20 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 20 2016
// CasualSimpleton
//
// NetGameBaseConnection - Base abstract class for Server and Client specific versions
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
    public abstract class NetGameBaseConnection
    {
        #region Enums/Structs
        public struct NetTransmissionStats
        {
            public int incBytesPerSec;
            public int outByesPerSec;
            public long totalIncBytes;
            public long totalOutBytes;
        }
        #endregion

        #region Vars
        public static int ReadByteSize = 2048;

        /// <summary>
        /// Current message waiting for incoming data
        /// </summary>
        protected NetMessage _currentIncomingMsg;

        /// <summary>
        /// NetByteBuffers waiting to be sent
        /// </summary>
        protected ThreadSafeQueue<NetByteBuffer> _outGoingByteQueue = new ThreadSafeQueue<NetByteBuffer>();

        /// <summary>
        /// NetByteBuffers that have been sent, and are awaiting return to the pool
        /// </summary>
        protected ThreadSafeQueue<NetByteBuffer> _spentByteBuffersQueue = new ThreadSafeQueue<NetByteBuffer>();

        /// <summary>
        /// How many bytes per second are coming and going, and total in and out
        /// </summary>
        protected NetTransmissionStats _netStats = new NetTransmissionStats();

        /// <summary>
        /// True if currently writing to out going stream
        /// </summary>
        protected bool _isSending;

        /// <summary>
        /// TcpClient for easy access
        /// </summary>
        protected TcpClient _tcpClient = new TcpClient();

        /// <summary>
        /// Raw socket for connection
        /// </summary>
        protected Socket _socket;

        /// <summary>
        /// The consistently reused byte array that all incoming data will be
        /// saved into, before being copied to the buffer array in the NetMessage
        /// </summary>
        protected byte[] _incReadBuffer;

        /// <summary>
        /// If connected
        /// </summary>
        protected bool _isConnected;

        /// <summary>
        /// Whether or not to force connection closed via indirect method since we're using async ops
        /// </summary>
        protected bool _doCloseConnection;
        #endregion

        #region Gets/Sets
        public NetMessage CurrentIncMessage
        {
            get { return _currentIncomingMsg; }
        }

        public bool IsSending
        {
            get { return _isSending; }
        }

        public NetTransmissionStats NetStats
        {
            get { return _netStats; }
        }

        public float IncomingCompletion
        {
            get
            {
                if (_currentIncomingMsg != null)
                {
                    return _currentIncomingMsg.CompleteionAmount;
                }

                return -1f;
            }
        }

        public TcpClient tcpClient
        {
            get { return _tcpClient; }
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return ((IPEndPoint)_tcpClient.Client.RemoteEndPoint); }
        }

        public IPAddress RemoteIPAddr
        {
            get { return RemoteEndPoint.Address; }
        }

        public int RemotePort
        {
            get { return RemoteEndPoint.Port; }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        public bool DoCloseConnection
        {
            get { return _doCloseConnection; }
            set { _doCloseConnection = value; }
        }

        public int NumOfPendingOutMsgs
        {
            get { return _outGoingByteQueue.Count; }
        }
        #endregion

        protected abstract void IncomingDataReceived(int bytesReceived, int offset);
        protected abstract void AdvanceOutgoingQueue();
        protected abstract void StartOutgoingWrite(NetByteBuffer outgoingData);
        public abstract void SendMessage(NetMessageType msgType, NetByteBuffer data, bool useCompression = false);
        protected abstract void EndAsyncWrite(IAsyncResult iar);
        public abstract void ClearFinishedOutgoingBuffers();
    }

    public class NetGameClientConnection : NetGameBaseConnection
    {
        #region Delegates
        private OnTcpClientConnect _onClientConnect;
        private OnTcpClientDisconnect _onClientDisconnect;
        private OnTcpClientDataReceived _onClientDataReceived;
        private OnTcpClientDataTransmit _onClientDataTransmitted;
        #endregion

        public NetGameClientConnection(TcpClient newClient,
            OnTcpClientConnect connectDelegate,
            OnTcpClientDisconnect disconnectDelegate,
            OnTcpClientDataReceived dataReceivedDelegate,
            OnTcpClientDataTransmit dataTransmittedDelegate)
        {
            _onClientConnect = connectDelegate;
            _onClientDisconnect = disconnectDelegate;
            _onClientDataReceived = dataReceivedDelegate;
            _onClientDataTransmitted = dataTransmittedDelegate;

            _tcpClient = newClient;
            //Make sure to turn off Nagle's
            _tcpClient.NoDelay = true;
            _socket = _tcpClient.Client;

            _incReadBuffer = new byte[NetGameBaseConnection.ReadByteSize];

            _currentIncomingMsg = NetworkPool.RequestClientNetMessage();

            _isConnected = true;

            ClientConnected();

            StartDataReceive();
        }

        #region Delegate Bodies
        private void ClientConnected()
        {
            if (_onClientConnect != null)
            {
                _onClientConnect(this);
            }
        }

        /// <summary>
        /// Tells the socket to begin receiving as async
        /// </summary>
        private void StartDataReceive()
        {
            _socket.BeginReceive(_incReadBuffer, 0, _incReadBuffer.Length, SocketFlags.None, ClientAsyncDataReceive, null);
        }

        /// <summary>
        /// ASync callback for when data is received
        /// </summary>
        /// <param name="iar"></param>
        public void ClientAsyncDataReceive(IAsyncResult iar)
        {
            int numBytesRead = 0;

            try
            {
                numBytesRead = _socket.EndReceive(iar);

                //Did we finish reading but nothing came? Connection has probably been closed
                if (numBytesRead == 0)
                {
                    if (_onClientDisconnect != null)
                    {
                        _onClientDisconnect(null, NetworkConnectionStatus.UnexpectedConnectionClose);
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
                    Debug.LogError(string.Format("ERROR: Exception ClientAsyncDataReceive(): '{0}'", exception.Message.ToString()));
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
                        if(_onClientDataReceived != null)
                        {
                            _onClientDataReceived(_currentIncomingMsg);
                        }

                        //Get a new message from cache
                        _currentIncomingMsg = NetworkPool.RequestClientNetMessage();
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
                        Debug.LogWarning(string.Format("Outgoing client message queue above warning threshold: {0}. Something might be wrong.", _outGoingByteQueue.Count));
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
                Debug.LogError(string.Format("Write Socket Exception in StartOutgoingWrite(). Dropping connection. Message: '{0}'.", sException.Message.ToString()));
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
                Debug.LogError(string.Format("Write Socket Exception in EndAsyncWrite(). Dropping connection. Message: '{0}'.", sException.Message.ToString()));
                Close();
            }

            if (_onClientDataTransmitted != null)
            {
                _onClientDataTransmitted();
            }

            //Get the NetByteBuffer from Async State and add it to the spent queue
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

            if (_onClientDisconnect != null)
            {
                _onClientDisconnect(null, NetworkConnectionStatus.ClientDisconnect);
            }
        }

        public override void ClearFinishedOutgoingBuffers()
        {
            lock (_spentByteBuffersQueue)
            {
                while (_spentByteBuffersQueue.Count > 0)
                {
                    NetByteBuffer nbb = _spentByteBuffersQueue.Dequeue();

                    NetworkPool.ReturnClientOutBuffer(nbb);
                }
            }
        }
    }
}