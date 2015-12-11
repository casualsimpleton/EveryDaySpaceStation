//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// PoolSceneChunkScaffoldRenderer - A pool structured to hold and reuse SceneChunkScaffoldRenderer along with its verts, normals, meshes and gameobject
// It is not thread-safe and thus should only be used within Unity's main thread.
// Created: December 10 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 10 2015
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
    public class PoolSceneChunkScaffoldRenderer : PoolAbstract<SceneChunkScaffoldRenderer>
    {
        public override SceneChunkScaffoldRenderer CreateNewObject()
        {
#if CLIENTDEBUG
            uint uid = GetUID();
            GameObject go = new GameObject(string.Format("sscrenderer-{0}", uid);
#else
            GameObject go = new GameObject("sscrenderer");
#endif

            SceneChunkScaffoldRenderer sscr = go.AddComponent<SceneChunkScaffoldRenderer>();
            sscr.Create();
            sscr.Reset();

            return sscr;
        }

        public override void ReturnObject(SceneChunkScaffoldRenderer sscr)
        {
            sscr.Reset();

            base.ReturnObject(sscr);
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

            SceneChunkScaffoldRenderer sscr;
            //Copy as many from overflow as possible into available
            for (int i = 0; i < difference && _overflowPool.Count > 0; i++)
            {
                sscr = _overflowPool.Dequeue();
                _availablePool.Enqueue(sscr);
            }

            //Now destroy whatever is called for
            for (int i = 0; i < _numToDestroyPerMaintenance && _overflowPool.Count > 0; i++)
            {
                sscr = _overflowPool.Dequeue();
                Destroy(sscr);
            }

            _updateTimer = Time.time + _updateTimerDelta;
        }

        protected void Destroy(SceneChunkScaffoldRenderer sscr)
        {
            GameObject.Destroy(sscr.gameObject);
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

        ~PoolSceneChunkScaffoldRenderer()
        {
            Dispose(false);
        }
        #endregion
    }
}