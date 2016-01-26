//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// NetMessage - A message structure for receiving all incoming data over tcp network. Both client and server use them
// Created: January 20 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 20 2016
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
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
    public class NetMessage : IDisposable
    {
        #region Consts
        public static int NETMSGHEADER_MSGTYPE = 0;
        public static int NETMSGHEADER_BODYSIZEINDEX = 1;
        public static int NETMSG_BODYINDEX = 5;

        public static int MSGTHRESHOLDWARNING = 30;

        public static byte GetByteFromNetMessageType(NetMessageType msgType)
        {
            return (byte)msgType;
        }

        public static NetMessageType GetNetMessageTypeFromByte(byte b)
        {
            return (NetMessageType)b;
        }
        #endregion

        #region Vars
        /// <summary>
        /// Who the message is going to
        /// </summary>
        protected IPEndPoint _senderAddr;

        /// <summary>
        /// If the header has enough data to be complete
        /// </summary>
        protected bool _isHeaderDataFinished;

        /// <summary>
        /// If the message has all the data it should to be complete
        /// </summary>
        protected bool _isMsgDataFinished;

        /// <summary>
        /// Whether or not this message is from the server (as used by the server)
        /// </summary>
        protected bool _isServerMessage;

        /// <summary>
        /// Byte count for header
        /// </summary>
        protected byte _msgHeaderLength;

        /// <summary>
        /// Number of bytes expected for message payload
        /// </summary>
        protected int _msgBodyLength;

        /// <summary>
        /// Total number of bytes expected, basically _msgBodyLength + _msgHeaderLength
        /// </summary>
        protected int _msgTotalLength;

        /// <summary>
        /// Current running length of the message, when it equals _msgTotalLength, message is complete
        /// </summary>
        protected int _curMsgTotalLength;

        /// <summary>
        /// Number of bytes currently held for message payload. When this meets _msgBodyLength, _isMsgDataFinished should be true
        /// </summary>
        protected int _curMsgBodyLength;

        /// <summary>
        /// Current index where it's to be read from
        /// </summary>
        protected int _curHeaderByteLength;

        /// <summary>
        /// Time message is receive (as Time.time)
        /// </summary>
        protected float _timeStamp;

        /// <summary>
        /// _msgCurBodyLength / _msgBodyLength
        /// </summary>
        protected float _completionAmount;

        /// <summary>
        /// Header data, should be fixed size, containing a byte for NetMessageType, and 4 bytes for MessageLength
        /// </summary>
        protected NetByteBuffer _headerData;
        protected NetByteBuffer _msgData;
        #endregion

        #region Gets/Sets
        public NetMessageType MsgType
        {
            get { return (NetMessageType)_headerData.data[NETMSGHEADER_MSGTYPE]; }
        }

        public NetByteBuffer MsgData
        {
            get { return _msgData; }
        }

        public bool IsHeaderFinished
        {
            get { return _isHeaderDataFinished; }
        }

        public bool IsMessageFinished
        {
            get { return _isMsgDataFinished; }
        }

        public bool IsServerMessage
        {
            get { return _isServerMessage; }
            set { _isServerMessage = value; }
        }

        /// <summary>
        /// Header Len (5 bytes) + Payload Len
        /// </summary>
        public int TotalMessageLength
        {
            get { return _msgTotalLength; }
        }

        /// <summary>
        /// The current message total length. When it equals TotalMessageLength, message is complete
        /// </summary>
        public int CurrentMessageTotalLength
        {
            get { return _curMsgTotalLength; }
        }

        public int MessagePayloadLength
        {
            get { return _msgBodyLength; }
        }

        /// <summary>
        /// Current length of received data (compared to expected length)
        /// </summary>
        public int CurrentMessagePayloadLength
        {
            get { return _curMsgBodyLength; }
        }

        public IPEndPoint SenderAddress
        {
            get { return _senderAddr; }
        }

        public float TimeStamp
        {
            get { return _timeStamp; }
        }

        public void GetHeaderBuffer()
        {
            if (_isServerMessage)
            {
                _headerData = NetworkPool.RequestServerIncBuffer(NETMSG_BODYINDEX);
            }
            else
            {
                _headerData = NetworkPool.RequestClientIncBuffer(NETMSG_BODYINDEX);
            }
        }

        public float CompleteionAmount
        {
            get { return _completionAmount; }
        }
        #endregion

        public NetMessage()
        {
            _headerData = null;
            _msgData = null;
            _senderAddr = null;

            _curMsgBodyLength = 0;
            _curHeaderByteLength = 0;
            _curMsgTotalLength = 0;
            _msgBodyLength = 0;
            _msgHeaderLength = 0;
            _msgTotalLength = 0;

            _isHeaderDataFinished = false;
            _isMsgDataFinished = false;
            _isServerMessage = false;

            _completionAmount = -1f;
        }

        /// <summary>
        /// Clears the message and prepares it to be used again for another purpose
        /// </summary>
        public void Clear()
        {
            //Return byte buffers to appropriate pool
            if (_isServerMessage)
            {
                NetworkPool.ReturnServerIncBuffer(_headerData);
                NetworkPool.ReturnServerIncBuffer(_msgData);
            }
            else
            {
                NetworkPool.ReturnClientIncBuffer(_headerData);
                NetworkPool.ReturnClientIncBuffer(_msgData);
            }
            _headerData = null;
            _msgData = null;

            _curMsgBodyLength = 0;
            _curHeaderByteLength = 0;
            _curMsgTotalLength = 0;
            _msgHeaderLength = 0;
            _msgBodyLength = 0;
            _msgHeaderLength = 0;
            _msgTotalLength = 0;

            _isHeaderDataFinished = false;
            _isMsgDataFinished = false;
            _isMsgDataFinished = false;

            _completionAmount = -1f;
        }

        /// <summary>
        /// Adds newly received data to message. Depending on state of the message, it might be added to 
        /// the header buffer, the message buffer, or return an integer indicating the message is complete
        /// and any remaining data should pass on to next message
        /// </summary>
        public int IncomingNewData(byte[] newData, IPEndPoint sender, int newDataIndexOffset, int totalBytesReceived)
        {
            //Lock the newData so it has to wait
            lock (newData)
            {
                _senderAddr = sender;

                if (_headerData == null)
                {
                    Debug.LogError(string.Format("Message requires header NetByteBuffer, but is null"));
                }

                int actualReceived = totalBytesReceived - newDataIndexOffset;

                #region Header Buffer
                if (!_isHeaderDataFinished)
                {
                    _completionAmount = 0f;

                    //How many bytes are needed to complete the header
                    int headerBytesNeed = NETMSG_BODYINDEX - _curHeaderByteLength;

                    //TODO Add thread safe time
                    _timeStamp = -1f;

                    //Is there enough to complete the header?
                    if (actualReceived < headerBytesNeed)
                    {
                        headerBytesNeed = actualReceived;
                    }

                    try
                    {
                        System.Buffer.BlockCopy(newData, newDataIndexOffset, _headerData.data, _curHeaderByteLength, headerBytesNeed);
                    }
                    catch (System.Exception exception)
                    {
                        Debug.LogError(string.Format("ERROR: Trying to copy new incoming byte to header buffer. Exception: {0}", exception.Message.ToString()));
                    }

                    _curHeaderByteLength += headerBytesNeed;

                    //If there is still not enough bytes to complete the header, return -1 to indicate message isn't complete
                    if (_curHeaderByteLength < NetMessage.NETMSG_BODYINDEX)
                    {
                        return -1;
                    }

                    //Enough bytes have been received to complete the header
                    //The message is now ready to fill the body
                    //NOTE: THIS LENGTH IS SENT BY OTHER SIDE. IT INCLUDES 5 HEADER BYTES 
                    //AS IT'S REQUIRED FOR PROCESSING
                    _msgTotalLength = System.BitConverter.ToInt32(_headerData.data, NetMessage.NETMSGHEADER_BODYSIZEINDEX);
                    _msgBodyLength = _msgTotalLength - _msgHeaderLength;

                    //Confirm that the length seems right
                    if (_msgBodyLength < 0 || _msgBodyLength > int.MaxValue)
                    {
                        Debug.LogError(string.Format("Problem with new message. Expected body length is out of bounds: {0}. Header Data: {1}", _msgBodyLength, _headerData.DumpToText()));
                    }

                    //Header all done, body length calculated, now get a netbytebuffer for body
                    if (_isServerMessage)
                    {
                        _msgData = NetworkPool.RequestServerIncBuffer((int)_msgTotalLength);
                    }
                    else
                    {
                        _msgData = NetworkPool.RequestClientIncBuffer((int)_msgTotalLength);
                    }

                    //Copy the header data into message data
                    System.Buffer.BlockCopy(_headerData.data, 0, _msgData.data, 0, NetMessage.NETMSG_BODYINDEX);
                    _curMsgTotalLength = NetMessage.NETMSG_BODYINDEX;

                    newDataIndexOffset += headerBytesNeed;
                    _isHeaderDataFinished = true;
                }
                #endregion

                //Check if it's a bodyless message, and might just be something signified by message type
                if (_curMsgTotalLength == _msgTotalLength)
                {
                    _isMsgDataFinished = true;
                    //Return 0 to signifiy message is done and there's not additional data
                    return 0;
                }

                //Update the number more of bytes that are available
                actualReceived = _msgTotalLength - newDataIndexOffset;

                if (actualReceived < 1)
                {
                    //Message isn't done yet
                    return -1;
                }

                //How many bytes are still needed for the body
                int msgBytesNeeded = _msgTotalLength - _curMsgTotalLength;

                //Are more bytes need than actually available?
                if (msgBytesNeeded > actualReceived)
                {
                    //Take whatever is available only
                    msgBytesNeeded = actualReceived;
                }

                if (msgBytesNeeded != 0)
                {
                    try
                    {
                        System.Buffer.BlockCopy(newData, newDataIndexOffset, _msgData.data, _curMsgTotalLength, msgBytesNeeded);
                    }
                    catch (System.Exception exception)
                    {
                        Debug.LogError(string.Format("ERROR: Trying to copy new incoming byte to message buffer. Exception: {0}", exception.Message.ToString()));
                    }
                }

                _curMsgTotalLength += msgBytesNeeded;

                //Did we get enough bytes to complete the message?
                if (_curMsgTotalLength == _msgTotalLength)
                {
                    _isMsgDataFinished = true;
                }

                //Calculate the completed amount
                _completionAmount = (float)_curMsgTotalLength / (float)_msgTotalLength * 100f;

                return actualReceived - msgBytesNeeded;
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //IDisposable overrides
        protected bool isDisposed = false;

        public virtual void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (IsServerMessage)
                    {
                        NetworkPool.ReturnServerIncBuffer(_headerData);
                        NetworkPool.ReturnServerIncBuffer(_msgData);
                    }
                    else
                    {
                        NetworkPool.ReturnClientIncBuffer(_headerData);
                        NetworkPool.ReturnClientIncBuffer(_msgData);
                    }

                    _headerData = null;
                    _msgData = null;
                }
            }

            isDisposed = true;
        }

        ~NetMessage()
        {
            Dispose(false);
        }
    }
}