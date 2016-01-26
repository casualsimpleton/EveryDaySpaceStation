//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// PoolMeshQuad - A pool structure for the entity sprites (Unity's quad)
// It is not thread-safe and thus should only be used within Unity's main thread.
// Created: December 14 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 14 2015
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
    public class PoolMeshQuad : PoolAbstract<MeshQuad>
    {
        public override void Init(int capacity, float timerDelta, int overflowDestroyGoal, float overflowRelativeSize = 0.1f)
        {
            base.Init(capacity, timerDelta, overflowDestroyGoal, overflowRelativeSize);
        }

        public override MeshQuad CreateNewObject()
        {
            GameObject go = new GameObject("MeshQuad");

            MeshQuad mq = go.AddComponent<MeshQuad>();

            mq.Create();
            mq.Reset();

            return mq;
        }

        public override void ReturnObject(MeshQuad mq)
        {
            mq.Reset();

            base.ReturnObject(mq);
        }

        /// <summary>
        /// Performs maintenance like emptying out overflow pool
        /// </summary>
        public override void Maintenance()
        {
            if (_updateTimer > Time.time)
            {
                return;
            }

            //How much are we from being full?
            int difference = _targetCapacity - _availablePool.Count;

            MeshQuad mq;
            //Copy as many from overflow as possible into available
            for (int i = 0; i < difference && _overflowPool.Count > 0; i++)
            {
                mq = _overflowPool.Dequeue();
                _availablePool.Enqueue(mq);
            }

            //Now destroy whatever is called for
            for (int i = 0; i < _numToDestroyPerMaintenance && _overflowPool.Count > 0; i++)
            {
                mq = _overflowPool.Dequeue();
                Destroy(mq);
            }

            _updateTimer = Time.time + _updateTimerDelta;
        }

        protected void Destroy(MeshQuad mq)
        {
            GameObject.Destroy(mq.gameObject);
        }

        #region Dispose
        ///////////
        //IDisposable Overrides
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //Dispose here
                    while (_availablePool.Count > 0)
                    {
                        Destroy(_availablePool.Dequeue());
                    }

                    while (_overflowPool.Count > 0)
                    {
                        Destroy(_overflowPool.Dequeue());
                    }

                    _availablePool.Clear();
                    _overflowPool.Clear();

                    _availablePool = null;
                    _overflowPool = null;
                }
            }

            _isDisposed = true;
        }

        ~PoolMeshQuad()
        {
            Dispose(false);
        }
        #endregion
    }
}