//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// ThreadSafeQueue - Since we don't have access to .Net 4 Concurrenct Collections, we need to create our own
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
    public class ThreadSafeQueue<T>
    {
        #region Vars
        private readonly object _key = new object();
        private Queue<T> _queue = new Queue<T>();
        #endregion

        #region Get/Set
        public int Count
        {
            get
            {
                lock (_key)
                {
                    return _queue.Count;
                }
            }
        }

        public T[] ToArray()
        {
            lock (_key)
            {
                return _queue.ToArray();
            }
        }
        #endregion

        public void Enqueue(T item)
        {
            lock (_key)
            {
                _queue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            lock (_key)
            {
                if (_queue.Count < 1)
                {
                    return default(T);
                }

                return _queue.Dequeue();
            }
        }

        public T Peek()
        {
            lock (_key)
            {
                return _queue.Peek();
            }
        }

        public void Clear()
        {
            lock (_key)
            {
                _queue.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_key)
            {
                return _queue.Contains(item);
            }
        }
    }
}