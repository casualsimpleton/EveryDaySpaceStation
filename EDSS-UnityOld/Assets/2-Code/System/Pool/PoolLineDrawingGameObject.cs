//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// PoolLineDrawingGameObject - A pool structure for the line drawing gameobjects
// It is not thread-safe and thus should only be used within Unity's main thread.
// Created: December 15 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 15 2015
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
    public class PoolBoundsDrawing : PoolAbstract<BoundsDrawing>
    {
        public override void Init(int capacity, float timerDelta, int overflowDestroyGoal, float overflowRelativeSize = 0.1f)
        {
            base.Init(capacity, timerDelta, overflowDestroyGoal, overflowRelativeSize);
        }

        public override BoundsDrawing CreateNewObject()
        {
            GameObject go = new GameObject("lineboundsdrawer");
            BoundsDrawing bd = go.AddComponent<BoundsDrawing>();

            bd.Create();
            bd.Reset();

            return bd;
        }

        public override void ReturnObject(BoundsDrawing bd)
        {
            bd.Reset();

            base.ReturnObject(bd);
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

            BoundsDrawing bd;
            //Copy as many from overflow as possible into available
            for (int i = 0; i < difference && _overflowPool.Count > 0; i++)
            {
                bd = _overflowPool.Dequeue();
                _availablePool.Enqueue(bd);
            }

            //Now destroy whatever is called for
            for (int i = 0; i < _numToDestroyPerMaintenance && _overflowPool.Count > 0; i++)
            {
                bd = _overflowPool.Dequeue();
                Destroy(bd);
            }

            _updateTimer = Time.time + _updateTimerDelta;
        }

        protected void Destroy(BoundsDrawing bd)
        {
            GameObject.Destroy(bd.gameObject);
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

        ~PoolBoundsDrawing()
        {
            Dispose(false);
        }
        #endregion
    }
}