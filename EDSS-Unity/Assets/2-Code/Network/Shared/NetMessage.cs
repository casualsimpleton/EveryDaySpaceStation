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
        public static int NETMSGHEADER_BODYINDEX = 1;
        public static int NETMSG_BODYINDEX = 5;
        #endregion

        #region Vars
        /// <summary>
        /// Who the message is going to
        /// </summary>
        protected IPEndPoint _destinationAddr;

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
        protected uint _msgBodyLength;
        
        /// <summary>
        /// Number of bytes currently held for message payload. When this meets _msgBodyLength, _isMsgDataFinished should be true
        /// </summary>
        protected uint _curMsgBodyLength;

        /// <summary>
        /// Current index where it's to be read from
        /// </summary>
        protected int _curReadIndex;

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
        public uint TotalMessageLength
        {
            get { return _msgHeaderLength + _msgBodyLength; }
        }

        public uint MessagePayloadLength
        {
            get { return _msgBodyLength; }
        }

        /// <summary>
        /// Current length of received data (compared to expected length)
        /// </summary>
        public uint CurrentMessagePayloadLength
        {
            get { return _curMsgBodyLength; }
        }

        public IPEndPoint DestinationAddress
        {
            get { return _destinationAddr; }
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
        #endregion

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
                        NetworkPool.ReturnServerBuffer(_headerData);
                        NetworkPool.ReturnServerBuffer(_msgData);
                    }
                    else
                    {
                        NetworkPool.ReturnClientBuffer(_headerData);
                        NetworkPool.ReturnClientBuffer(_msgData);
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