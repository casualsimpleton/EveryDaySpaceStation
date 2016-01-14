//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// Tuple - Since Tuples are .Net 4 and above (and Unity's current .net version), but so useful, we'll create our own
//http://stackoverflow.com/questions/7120845/equivalent-of-tuple-net-4-for-net-framework-3-5
// Created: December 13 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 13 2015
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
    public class Tuple<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
        internal Tuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        internal Tuple()
        {
            First = default(T1);
            Second = default(T2);
        }

        public override string ToString()
        {
            return string.Format("First: {0} Second: {1}", First, Second);
        }
    }

    public class Tuple<T1, T2, T3>
    {
        public T1 First { get; private set; }
        public T2 Second { get; private set; }
        public T3 Third { get; private set; }
        internal Tuple(T1 first, T2 second, T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        internal Tuple()
        {
            First = default(T1);
            Second = default(T2);
            Third = default(T3);
        }

        public override string ToString()
        {
            return string.Format("First: {0} Second: {1} Third: {2}", First, Second, Third);
        }
    }

    public static class Tuple
    {
        public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            var tuple = new Tuple<T1, T2>(first, second);
            return tuple;
        }

        public static Tuple<T1, T2, T3> New<T1, T2, T3>(T1 first, T2 second, T3 third)
        {
            var tuple = new Tuple<T1, T2, T3>(first, second, third);
            return tuple;
        }
    }
}