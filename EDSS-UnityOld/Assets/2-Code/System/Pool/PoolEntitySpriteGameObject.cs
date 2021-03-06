﻿//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// PoolEntitySpriteGameObject - A pool structure for the entity's gameobject
// It is not thread-safe and thus should only be used within Unity's main thread.
// Created: December 13 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 13 2015
// CasualSimpleton
// PoolEntitySpriteGraphics - A pool structure for the entity's sprite. Not thread-safe and should only be access from Unity's main thread
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
    public class PoolEntitySpriteGameObject : PoolAbstract<EntitySpriteGameObject>
    {
        public override void Init(int capacity, float timerDelta, int overflowDestroyGoal, float overflowRelativeSize = 0.1f)
        {
            base.Init(capacity, timerDelta, overflowDestroyGoal, overflowRelativeSize);
        }

        public override EntitySpriteGameObject CreateNewObject()
        {
#if CLIENTDEBUG
            uint uid = GetUID();
            GameObject go = new GameObject(string.Format("spriteRootGO-{0}", uid));
#else
            GameObject go = new GameObject(string.Format("spriteRootGO"));
#endif

            EntitySpriteGameObject cc = go.AddComponent<EntitySpriteGameObject>();

            cc.Create(null);
            cc.Reset();

            return cc;
        }

        public override void ReturnObject(EntitySpriteGameObject es)
        {
            es.Reset();

#if CLIENTDEBUG
            //es.gameObject.name = string.Format("spriteGO-{0}", uid);
            //Feeling lazy, but no way of keeping track of previous UID from pool
            es.gameObject.name = string.Format("spriteGO");
#else
            es.gameObject.name = string.Format("spriteGO");
#endif

            base.ReturnObject(es);
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

            EntitySpriteGameObject es;
            //Copy as many from overflow as possible into available
            for (int i = 0; i < difference && _overflowPool.Count > 0; i++)
            {
                es = _overflowPool.Dequeue();
                _availablePool.Enqueue(es);
            }

            //Now destroy whatever is called for
            for (int i = 0; i < _numToDestroyPerMaintenance && _overflowPool.Count > 0; i++)
            {
                es = _overflowPool.Dequeue();
                Destroy(es);
            }

            _updateTimer = Time.time + _updateTimerDelta;
        }

        protected void Destroy(EntitySpriteGameObject es)
        {
            GameObject.Destroy(es.gameObject);
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

        ~PoolEntitySpriteGameObject()
        {
            Dispose(false);
        }
        #endregion
    }

    public class PoolEntitySpriteGraphics : PoolAbstract<EntitySpriteGraphics>
    {
        public override void Init(int capacity, float timerDelta, int overflowDestroyGoal, float overflowRelativeSize = 0.1f)
        {
            base.Init(capacity, timerDelta, overflowDestroyGoal, overflowRelativeSize);
        }

        public override EntitySpriteGraphics CreateNewObject()
        {
#if CLIENTDEBUG
            uint uid = GetUID();
            GameObject go = new GameObject(string.Format("spriteGO-{0}", uid));
#else
            GameObject go = new GameObject(string.Format("spriteGO"));
#endif

            EntitySpriteGraphics cc = go.AddComponent<EntitySpriteGraphics>();

            cc.Create(null);
            cc.Reset();

            return cc;
        }

        public override void ReturnObject(EntitySpriteGraphics es)
        {
            es.Reset();

#if CLIENTDEBUG
            //es.gameObject.name = string.Format("spriteGO-{0}", uid);
            //Feeling lazy, but no way of keeping track of previous UID from pool
            es.gameObject.name = string.Format("spriteGO");
#else
            es.gameObject.name = string.Format("spriteGO");
#endif

            base.ReturnObject(es);
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

            EntitySpriteGraphics es;
            //Copy as many from overflow as possible into available
            for (int i = 0; i < difference && _overflowPool.Count > 0; i++)
            {
                es = _overflowPool.Dequeue();
                _availablePool.Enqueue(es);
            }

            //Now destroy whatever is called for
            for (int i = 0; i < _numToDestroyPerMaintenance && _overflowPool.Count > 0; i++)
            {
                es = _overflowPool.Dequeue();
                Destroy(es);
            }

            _updateTimer = Time.time + _updateTimerDelta;
        }

        protected void Destroy(EntitySpriteGraphics es)
        {
            GameObject.Destroy(es.gameObject);
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

        ~PoolEntitySpriteGraphics()
        {
            Dispose(false);
        }
        #endregion
    }
}