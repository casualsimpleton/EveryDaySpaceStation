//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// PoolSceneChunkRenderer - A pool structured to hold and reuse SceneChunkRenderer along with its verts, normals, meshes and gameobject
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
    public class PoolSceneChunkRenderer : PoolAbstract<SceneChunkRenderer>
    {
        public override SceneChunkRenderer CreateNewObject()
        {
#if CLIENTDEBUG
            uint uid = GetUID();
            GameObject go = new GameObject(string.Format("screnderer-{0}", uid);
#else
            GameObject go = new GameObject("screnderer");
#endif

            SceneChunkRenderer scr = go.AddComponent<SceneChunkRenderer>();
            scr.Create();
            scr.Reset();

            return scr;
        }

        public override void ReturnObject(SceneChunkRenderer scr)
        {
            scr.Reset();

            base.ReturnObject(scr);
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

            SceneChunkRenderer scr;
            //Copy as many from overflow as possible into available
            for (int i = 0; i < difference && _overflowPool.Count > 0; i++)
            {
                scr = _overflowPool.Dequeue();
                _availablePool.Enqueue(scr);
            }

            //Now destroy whatever is called for
            for (int i = 0; i < _numToDestroyPerMaintenance && _overflowPool.Count > 0; i++)
            {
                scr = _overflowPool.Dequeue();
                Destroy(scr);
            }

            _updateTimer = Time.time + _updateTimerDelta;
        }

        protected void Destroy(SceneChunkRenderer scr)
        {
            GameObject.Destroy(scr.gameObject);
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
                    while(_availablePool.Count > 0)
                    {
                        Destroy(_availablePool.Dequeue());
                    }

                    while(_overflowPool.Count > 0)
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

        ~PoolSceneChunkRenderer()
        {
            Dispose(false);
        }
        #endregion
    }
}