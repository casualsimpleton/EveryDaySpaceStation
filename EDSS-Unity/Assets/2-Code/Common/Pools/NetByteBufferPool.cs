//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// NetByteBufferPool - A pool for network byte buffers
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

namespace EveryDaySpaceStation.DataTypes
{
    public class NetByteBufferPool : ThreadSafeBasePool<NetByteBuffer>
    {
        public static uint _uniqueID;
        public static uint GetNewUID() { return _uniqueID++; }

        private int _bufferDataLength = -1;
        private int _bytesDisposedSoFar;
        private readonly object _key = new object();

        public virtual NetByteBuffer RequestObject(int desiredLength)
        {
            lock (_key)
            {
                NetByteBuffer item = base.RequestObject();
                item.CurDataLength = desiredLength;
                return item;
            }
        }

        public override void ReturnObject(EveryDaySpaceStation.NetByteBuffer item)
        {
            lock (_key)
            {
                base.ReturnObject(item);
            }
        }

        public override NetByteBuffer CreateNewObject()
        {
            NetByteBuffer nbb = new NetByteBuffer(_bufferDataLength);
            nbb.bufferUID = NetByteBufferPool.GetNewUID();

            return nbb;
        }

        public virtual void Init(int count, float timerDelta, int bufferDataLength)
        {
            if (_pool != null)
            {
                Debug.LogError(string.Format("NetByteBuffer has already been inited. Check this!"));
                Dispose();
            }

            _bufferDataLength = bufferDataLength;

            base.Init(count, timerDelta);
        }

        public override void Maintenance()
        {
            if (_updateTimer > Time.time)
            {
                return;
            }

            NetByteBuffer item;
            float multiplier = 3f;

            lock (_key)
            {
                int max = Mathf.RoundToInt(_targetCapacity * multiplier);

                if (_pool.Count > max)
                {
                    Debug.LogWarning(string.Format("{0} is at targeted {1} multiplier. Actual count: {2}", this.GetType().ToString(), multiplier, _pool.Count));
                }

                while (_pool.Count > max)
                {
                    item = _pool.Dequeue();

                    if (item != null)
                    {
                        _bytesDisposedSoFar += item.data.Length;

                        item.Dispose();
                    }
                }

                //We've disposed enough that we want to trigger a GC.Collect
                if (_bytesDisposedSoFar > ushort.MaxValue)
                {
                    _bytesDisposedSoFar = 0;
                    System.GC.Collect();
                }
            }

            _updateTimer = Time.time + _updateTimerDelta;
        }

        //////////////////////////////////////////////////////////////////////////////
        //IDisposable overrides
        protected bool isDisposed = false;

        public override void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    while (_pool.Count > 0)
                    {
                        NetByteBuffer nbb = _pool.Dequeue();

                        if (nbb != null)
                        {
                            nbb.Dispose();
                        }
                    }

                    _pool.Clear();
                    _pool = null;
                }
            }

            isDisposed = true;
        }

        ~NetByteBufferPool()
        {
            Dispose(false);
        }
    }
}