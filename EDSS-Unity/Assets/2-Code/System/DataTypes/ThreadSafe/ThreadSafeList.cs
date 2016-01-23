//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// ThreadSafeList - Since we don't have access to .Net 4 Concurrenct Collections, we need to create our own
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
    public class ThreadSafeList<T>
    {
        #region Vars
        private readonly object _key = new object();
        private List<T> _list = new List<T>();
        #endregion

        #region Get/Set
        public int Count
        {
            get
            {
                lock (_key)
                {
                    return _list.Count;
                }
            }
        }

        public int Capacity
        {
            get
            {
                lock (_key)
                {
                    return _list.Capacity;
                }
            }
        }

        public T[] ToArray()
        {
            lock (_key)
            {
                return _list.ToArray();
            }
        }
        #endregion

        public void Add(T item)
        {
            lock (_key)
            {
                _list.Add(item);
            }
        }

        public void Remove(T item)
        {
            lock (_key)
            {
                _list.Remove(item);
            }
        }

        public void Clear()
        {
            lock (_key)
            {
                _list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_key)
            {
                return _list.Contains(item);
            }
        }

        public T GetFirst()
        {
            lock (_key)
            {
                T item = _list[0];
                _list.RemoveAt(0);

                return item;
            }
        }

        public T GetLast()
        {
            lock (_key)
            {
                T item = _list[_list.Count - 1];
                _list.RemoveAt(_list.Count - 1);

                return item;
            }
        }
    }
}