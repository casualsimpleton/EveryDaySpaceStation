//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// NetByteBuffer - A pooled array type designed to be used and recycled by the network
// Created: January 22 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 22 2016
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

namespace EveryDaySpaceStation
{
    public class NetByteBuffer : IDisposable
    {
        #region Vars
        /// <summary>
        /// Byte buffer UID for tracking
        /// </summary>
        public uint bufferUID;

        /// <summary>
        /// Premade buffer array
        /// </summary>
        public byte[] data;

        /// <summary>
        /// Current length of data in buffer. For instance, data might be 256 bytes in length, but the message contained will be 190 bytes
        /// </summary>
        public int CurDataLength { get; set; }
        #endregion

        public NetByteBuffer(int definedLength)
        {
            data = new byte[definedLength];
        }

        /// <summary>
        /// Copy from srcBuffer to this buffer
        /// </summary>
        /// <param name="srcBuffer"></param>
        public void Copy(NetByteBuffer srcBuffer)
        {
            CurDataLength = srcBuffer.CurDataLength;
            System.Buffer.BlockCopy(srcBuffer.data, 0, data, 0, CurDataLength);
        }

        public override string ToString()
        {
            return string.Format("NetByteBuffer. UID: {0} Array Size: {1} CurDataLength: {2}", bufferUID, data.Length, CurDataLength);
        }

        public string DumpToText()
        {
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

            strBuilder.AppendLine(this.ToString());

            for (int i = 0; i < CurDataLength; i++)
            {
                strBuilder.AppendFormat("{0} ", data[i]);
            }

            return strBuilder.ToString();
        }

        #region Dispose
        ///////////
        //IDisposable Overrides
        protected bool _isDisposed = false;

        public virtual void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //Dispose here
                    data = null;
                    CurDataLength = -1;
                }
            }
        }

        ~NetByteBuffer()
        {
            Dispose(false);
        }
        #endregion
    }
}