//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// PairedList - A wrapper for two List<T> that replicates the functionality of a dictionary, but removes the need for using foreach to iterate
// Useful for situations where we have small numbers of items, rare addition/removal but frequent iteration
// IT ASSUMES ALL ADDS/REMOVES ARE PAIRED. IF YOU DON'T MAINTAIN PAIRING THEN IT'S GOING TO BREAK AND BE WORTHLESS!
// PairListUint - A preset class where the key list is automatically a uint. Prevents having to do .Equals() on the T generics. Super fast on small sets and no garbage allocation
// Created: December 13 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 13 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EveryDaySpaceStation.DataTypes;

namespace EveryDaySpaceStation.DataTypes
{
    public class PairedList<T1, T2>
    {
        protected List<T1> _keyList;
        protected List<T2> _valueList;

        public List<T1> Keys { get { return _keyList; } }
        public List<T2> Values { get { return _valueList; } }

        public T2 GetValue(int index) { return _valueList[index]; }

        public int Count { get { return _keyList.Count; } }

        public PairedList()
        {
            _keyList = new List<T1>();
            _valueList = new List<T2>();
        }

        /// <summary>
        /// Attempts to add the newItem to the list. Returns true if item already present.
        /// </summary>
        public bool Add(T1 key, T2 value)
        {
            for (int i = 0; i < _keyList.Count; i++)
            {
                if (_keyList[i].Equals(key))
                {
                    return true;
                }
            }

            _keyList.Add(key);
            _valueList.Add(value);
            return false;
        }

        /// <summary>
        /// Returns true if it removed item
        /// </summary>
        public bool Remove(T1 key)
        {
            for (int i = 0; i < _keyList.Count; i++)
            {
                if (_keyList[i].Equals(key))
                {
                    _keyList.RemoveAt(i);
                    _valueList.RemoveAt(i); 
                    return true;
                }
            }

            return false;
        }

        public bool TryGetValue(T1 key, out T2 value)
        {
            Profiler.BeginSample("TryGetValue");
            for (int i = 0; i < _keyList.Count; i++)
            {
                if (_keyList[i].Equals(key))
                {
                    value = _valueList[i];
                    return true;
                }
            }

            value = default(T2);
            Profiler.EndSample();
            return false;
        }

        /// <summary>
        /// Clears the list as normal List.Clear() does
        /// </summary>
        public void Clear()
        {
            _keyList.Clear();
            _valueList.Clear();
        }
    }

    public class PairedListUint<T>
    {
        protected List<uint> _keyList;
        protected List<T> _valueList;

        public List<uint> Keys { get { return _keyList; } }
        public List<T> Values { get { return _valueList; } }

        public T GetValue(int index) { return _valueList[index]; }

        public int Count { get { return _keyList.Count; } }

        public PairedListUint()
        {
            _keyList = new List<uint>();
            _valueList = new List<T>();
        }

        /// <summary>
        /// Attempts to add the newItem to the list. Returns true if item already present.
        /// </summary>
        public bool Add(uint key, T value)
        {
            for (int i = 0; i < _keyList.Count; i++)
            {
                if (_keyList[i] == key)
                {
                    return true;
                }
            }

            _keyList.Add(key);
            _valueList.Add(value);
            return false;
        }

        /// <summary>
        /// Returns true if it removed item
        /// </summary>
        public bool Remove(uint key)
        {
            for (int i = 0; i < _keyList.Count; i++)
            {
                if (_keyList[i] == key)
                {
                    _keyList.RemoveAt(i);
                    _valueList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool TryGetValue(uint key, out T value)
        {
            //Profiler.BeginSample("TryGetValue");
            for (int i = 0; i < _keyList.Count; i++)
            {
                if (_keyList[i] == key)
                {
                    value = _valueList[i];
                    return true;
                }
            }

            value = default(T);
            //Profiler.EndSample();
            return false;
        }

        /// <summary>
        /// Clears the list as normal List.Clear() does
        /// </summary>
        public void Clear()
        {
            _keyList.Clear();
            _valueList.Clear();
        }
    }
}