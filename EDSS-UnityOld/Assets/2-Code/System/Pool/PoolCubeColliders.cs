//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// PoolCubeColliders - A pool structure for all the various cube colliders we're going to need
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
    public class PoolCubeCollider : PoolAbstract<CubeCollider>
    {
        Vector3 _cubeSize;

        public virtual void Init(int capacity, Vector3 cubeSize, float timerDelta, int overflowDestroyGoal, float overflowRelativeSize = 0.1f)
        {
            _cubeSize = cubeSize;
            base.Init(capacity, timerDelta, overflowDestroyGoal, overflowRelativeSize);
        }

        public override CubeCollider CreateNewObject()
        {
#if CLIENTDEBUG
            uint uid = GetUID();
            GameObject go = new GameObject(string.Format("cubecollider-{0}-{1}", _cubeSize.y, uid));
#else
            GameObject go = new GameObject(string.Format("cubecollider-{0}", _cubeSize.y));
#endif
            CubeCollider cc = go.AddComponent<CubeCollider>();

            cc.Create(_cubeSize);
            cc.Reset();
            
            return cc;
        }

        public override void ReturnObject(CubeCollider cc)
        {
            cc.Reset();

            base.ReturnObject(cc);
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

            CubeCollider cc;
            //Copy as many from overflow as possible into available
            for (int i = 0; i < difference && _overflowPool.Count > 0; i++)
            {
                cc = _overflowPool.Dequeue();
                _availablePool.Enqueue(cc);
            }

            //Now destroy whatever is called for
            for (int i = 0; i < _numToDestroyPerMaintenance && _overflowPool.Count > 0; i++)
            {
                cc = _overflowPool.Dequeue();
                Destroy(cc);
            }

            _updateTimer = Time.time + _updateTimerDelta;
        }

        protected void Destroy(CubeCollider cc)
        {
            GameObject.Destroy(cc.gameObject);
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

        ~PoolCubeCollider()
        {
            Dispose(false);
        }
        #endregion
    }
}