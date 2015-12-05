//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// Helpers - A static class containing a collection of misc helper methods.
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
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

namespace EveryDaySpaceStation.Utils
{
    public static class Helpers
    {
        #region Unix Time
        /// <summary>
        /// Get the current time, convert to unix epoch timestamp and return
        /// </summary>
        /// <returns>Integer expression of current time</returns>
        public static int GetCurrentUnixTime()
        {
            System.DateTime rightNow = System.DateTime.Now;
            return rightNow.ToUnixTime();
        }

        /// <summary>
        /// Converts a DateTime to Unix Epoch Time
        /// </summary>
        /// <returns>An int32 of time</returns>
        public static int ToUnixTime(this System.DateTime date)
        {
            //Use UTC time so everyone is on the same page
            System.DateTime epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return System.Convert.ToInt32((date - epoch).TotalSeconds);
        }

        public static System.DateTime FromUnixTime(int timeStamp)
        {
            //Unix timestamp is seconds past epoch
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            dateTime = dateTime.AddSeconds(timeStamp).ToLocalTime();
            return dateTime;
        }
        #endregion

        #region Indices
        public static int IndexFromVec2Int(Vec2Int pos, Vec2Int widthHeight)
        {
            return (widthHeight.x * pos.y) + pos.x;
        }

        public static int IndexFromVec2Int(Vec2Int pos, int width)
        {
            return (width * pos.y) + pos.x;
        }

        public static int IndexFromVec2Int(int x, int y, Vec2Int widthHeight)
        {
            return (widthHeight.x * y) + x;
        }

        public static int IndexFromVec2Int(int x, int y, int width)
        {
            return (width * y) + x;
        }

        public static Vec2Int Vec2IntFromIndex(int index, Vec2Int widthHeight)
        {
            Vec2Int pos = new Vec2Int(0, 0);
            pos.y = (int)(index / widthHeight.x);
            pos.x = index - (pos.y * widthHeight.x);

            return pos;
        }

        public static Vec2Int Vec2IntFromIndex(uint index, Vec2Int widthHeight)
        {
            Vec2Int pos = new Vec2Int(0, 0);
            pos.y = (int)(index / widthHeight.x);
            pos.x = (int)index - (pos.y * widthHeight.x);

            return pos;
        }
        #endregion

        #region Text and Strings
        public static string StringLowerAndWhiteSpaceTrimmed(string input)
        {
            input = input.ToLower();
            //Trim spaces
            input = input.Trim((char)0x20);

            return input;
        }
        #endregion
    }
}