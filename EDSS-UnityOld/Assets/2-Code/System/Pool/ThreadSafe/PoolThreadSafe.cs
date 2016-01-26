//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// PoolThreadSafe - A collection of thread safe pools. Unlike the other pools, we don't use an overflow
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
    public abstract class ThreadSafeBasePool<T> : IDisposable
    {
        protected ThreadSafeQueue<T> _pool;

        protected int _targetCapacity; //Target capacity for _availablePool
        protected bool _isInit; //Has this pool be initialized (whatever that means)
        protected float _updateTimer; //Used for keeping track of maintenance periods
        protected float _updateTimerDelta; //How often the pool is updated

        #region Gets/Sets
        public int Capacity
        {
            get { return _targetCapacity; }
            set
            {
                if (value < 1)
                {
                    Debug.LogError(string.Format("{0} is an invalid capacity. Value can't be less than 1.", value));
                    value = 1;
                }

                _targetCapacity = value;
            }
        }

        public float TimerDelta
        {
            get { return _updateTimerDelta; }
            set
            {
                if (value < 0.0001f)
                {
                    Debug.LogError(string.Format("{0} is an invalid timer delta. Value can't be less than 0.0001f.", value));
                    value = 0.0001f;
                }

                _updateTimerDelta = value;
            }
        }

        public bool IsInit { get { return _isInit; } }
        #endregion

        public abstract void Maintenance();
        public abstract T CreateNewObject();

        public virtual void Init(int capacity, float timerDelta)
        {
            _targetCapacity = capacity;

            _pool = new ThreadSafeQueue<T>();

            //Create those many objects to populate initial pool
            for (int i = 0; i < _targetCapacity; i++)
            {
                T newObj = CreateNewObject();
                _pool.Enqueue(newObj);
            }

            _updateTimerDelta = timerDelta;
            _isInit = true;
        }

        public virtual T RequestObject()
        {
            T item;

            //Can't find one in availablePool, so check overflow, and if that's also empty, then create one
            if (_pool.Count > 0)
            {
                item = _pool.Dequeue();
            }
            else
            {
                item = CreateNewObject();
            }
            
            return item;
        }

        public virtual void ReturnObject(T item)
        {
            _pool.Enqueue(item);
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("Pool Name: {0}, TargetCap: {1} Pool#: {2}", this.GetType().FullName.ToString(), _targetCapacity, _updateTimerDelta);

            return sb.ToString();
        }

        //////////////////////////////////////////////////////////////////////////////
        //IDisposable overrides
        protected bool isDisposed = false;

        public virtual void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        ~ThreadSafeBasePool()
        {
            Dispose(false);
        }
    }
}