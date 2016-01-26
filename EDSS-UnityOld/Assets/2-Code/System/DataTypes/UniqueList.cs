//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// UniqueList - A wrapper for List<T> that replicates the functionality of a HashSet but allows us to not need a foreach
// Useful for situations where we have small numbers of items, rare addition/removal but frequent iteration
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
    public class UniqueList<T>
    {
        protected List<T> _list;

        public T GetValue(int index) { return _list[index]; }
        public List<T> List { get { return _list; } }
        public int Count { get { return _list.Count; } }

        public UniqueList()
        {
            _list = new List<T>();
        }

        /// <summary>
        /// Attempts to add the newItem to the list. Returns true if item already present.
        /// </summary>
        public bool AddUnique(T newItem)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Equals(newItem))
                {
                    return true;
                }
            }

            _list.Add(newItem);
            return false;
        }

        public void Add(T newItem)
        {
            _list.Add(newItem);
        }

        /// <summary>
        /// Returns true if it removed item
        /// </summary>
        public bool Remove(T newItem)
        {
            return _list.Remove(newItem);
        }

        /// <summary>
        /// Clears the list as normal List.Clear() does
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }
    }
}