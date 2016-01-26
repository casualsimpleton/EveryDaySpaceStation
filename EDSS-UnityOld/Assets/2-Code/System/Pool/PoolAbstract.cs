//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// PoolAbstract - The base abstract class for a pool implementation suitable for use in Unity. The goal is to provide reusability while keeping it easy to maintain and understand
// It is not thread-safe and thus should only be used within Unity's main thread.
// Created: December 7 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 7 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

namespace EveryDaySpaceStation.DataTypes
{
    public abstract class PoolAbstract<T> : System.IDisposable
    {
        #region Vars
        protected Queue<T> _availablePool; //The pool in which normal objects sit and wait
        protected Queue<T> _overflowPool; //An overflow area where objects sit and wait before either being returned to _availablePool or destroyed

        protected int _targetCapacity; //Target capacity for _availablePool
        protected bool _isInit; //Has this pool be initialized (whatever that means)
        protected float _updateTimer; //Used for keeping track of maintenance periods
        protected float _updateTimerDelta; //How often the pool is updated
        protected int _numToDestroyPerMaintenance; //Number of objects to destroy if necessary per cycle

#if CLIENTDEBUG
        protected uint _uniqueID; //Gives out unique ids to all generated pool objects to help accounting if needed
#endif
        #endregion

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

        /// <summary>
        /// How many should be destroyed from overflow per maintenance cycle
        /// </summary>
        public int OverflowDestroyCount
        {
            get { return _numToDestroyPerMaintenance; }
            set
            {
                if (value < 0)
                {
                    Debug.LogError(string.Format("{0} is an invalid overflow destroy number. Value can't be less than 0.", value));
                    value = 0;
                }

                _numToDestroyPerMaintenance = value;
            }
        }

        public bool IsInit { get { return _isInit; } }

        public int PoolCount { get { return _availablePool.Count; } }
        public int OverflowCount { get { return _overflowPool.Count; } }

#if CLIENTDEBUG
        public uint GetUID() { return _uniqueID++; }
#endif
        #endregion

        public abstract void Maintenance();
        public abstract T CreateNewObject();

        /// <summary>
        /// Initializes the pool.
        /// </summary>
        /// <param name="capacity">Targe siz of the pool</param>
        /// <param name="timerDelta">How often it should perform maintenance. Suggested between 0.1s -> 1s</param>
        /// <param name="overflowDestroyGoal">How many objects to destroy per update from overflow if necessary</param>
        /// <param name="overflowRelativeSize">How large can overflow expand relative to available size, before being emptied, 0 makes it immediate</param>
        public virtual void Init(int capacity, float timerDelta, int overflowDestroyGoal, float overflowRelativeSize = 0.1f)
        {
            _targetCapacity = capacity;
            _numToDestroyPerMaintenance = overflowDestroyGoal;

            _availablePool = new Queue<T>(_targetCapacity);
            _overflowPool = new Queue<T>(Mathf.RoundToInt(_targetCapacity * overflowRelativeSize));
            
            //Create those many objects to populate initial pool
            for (int i = 0; i < _targetCapacity; i++)
            {
                T newObj = CreateNewObject();
                _availablePool.Enqueue(newObj);
            }

            _updateTimerDelta = timerDelta;
            _isInit = true;
        }

        public virtual T RequestObject()
        {
            T obj;

            //Get one from pool if possible
            if (_availablePool.Count > 0)
            {
                obj = _availablePool.Dequeue();
            }
            else
            {
                //Can't find one in availablePool, so check overflow, and if that's also empty, then create one
                if (_overflowPool.Count > 0)
                {
                    obj = _overflowPool.Dequeue();
                }
                else
                {
                    obj = CreateNewObject();
                }
            }

#if CLIENTDEBUG
            if(obj == null)
            {
                Debug.LogError("Object To Return Was Null");
            }
#endif

            return obj;
        }

        public virtual void ReturnObject(T obj)
        {
#if CLIENTDEBUG
            if(obj == null)
            {
                Debug.LogError("Trying to return a null obj to pool".);
            }
#endif

            //If we're already at capacity, put it into the overflow queue for eventual deletion
            if (_availablePool.Count > _targetCapacity)
            {
                _overflowPool.Enqueue(obj);
            }
            else
            {
                _availablePool.Enqueue(obj);
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("Pool Name: {0}, TargetCap: {1} Pool#: {2}, Overflow#: {3}, MaintenaceDelta: {4}", this.GetType().FullName.ToString(), _targetCapacity, _availablePool.Count, _overflowPool.Count, _updateTimerDelta);

            return sb.ToString();
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

        protected abstract void Dispose(bool disposing);

        ~PoolAbstract()
        {
            Dispose(false);
        }
        #endregion
    }
}