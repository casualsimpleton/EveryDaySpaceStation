//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// JsonHelper - Helper methods for some json serializing
// Copied from http://stackoverflow.com/questions/30175911/can-i-serialize-nested-properties-to-my-class-in-one-operation-with-json-net
// Created: December 4 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 4 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using EveryDaySpaceStation;
using EveryDaySpaceStation.Utils;

namespace EveryDaySpaceStation.Utils
{
    public static class JsonExtensions
    {
        public static void MoveTo(this JToken token, JObject newParent)
        {
            if (newParent == null)
                throw new System.ArgumentNullException();
            if (token != null)
            {
                if (token is JProperty)
                {
                    token.Remove();
                    newParent.Add(token);
                }
                else if (token.Parent is JProperty)
                {
                    token.Parent.Remove();
                    newParent.Add(token.Parent);
                }
                else
                {
                    throw new System.InvalidOperationException();
                }
            }
        }
    }

    public struct PushValue<T> : System.IDisposable
    {
        System.Action<T> setValue;
        T oldValue;

        public PushValue(T value, System.Func<T> getValue, System.Action<T> setValue)
        {
            if (getValue == null || setValue == null)
                throw new System.ArgumentNullException();
            this.setValue = setValue;
            this.oldValue = getValue();
            setValue(value);
        }

        #region IDisposable Members

        // By using a disposable struct we avoid the overhead of allocating and freeing an instance of a finalizable class.
        public void Dispose()
        {
            if (setValue != null)
                setValue(oldValue);
        }

        #endregion
    }
}