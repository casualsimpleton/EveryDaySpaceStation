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

        public static Vec2Int Vec2IntFromIndex(int index, int width)
        {
            Vec2Int pos = new Vec2Int(0, 0);
            pos.y = (int)(index / width);
            pos.x = index - (pos.y * width);

            return pos;
        }

        public static Vec2Int Vec2IntFromIndex(uint index, Vec2Int widthHeight)
        {
            Vec2Int pos = new Vec2Int(0, 0);
            pos.y = (int)(index / widthHeight.x);
            pos.x = (int)index - (pos.y * widthHeight.x);

            return pos;
        }

        public static Vec2Int Vec2IntFromIndex(uint index, uint width)
        {
            Vec2Int pos = new Vec2Int(0, 0);
            pos.y = (int)(index / width);
            pos.x = (int)index - (pos.y * (int)width);

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

        //http://stackoverflow.com/questions/9742724/how-to-convert-a-string-to-a-bool
        /// <summary>
        /// Attempts to convert a string to a bool
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this string value)
        {
            switch (value.ToLower())
            {
                case "true":
                    return true;
                case "t":
                    return true;
                case "1":
                    return true;
                case "0":
                    return false;
                case "false":
                    return false;
                case "f":
                    return false;
                default:
                    throw new System.InvalidCastException("You can't cast a weird value to a bool!");
            }
        }
        #endregion

        #region Lights
        public static void FillLineWithLight(int startX, int endX, int y, Color32 centerColor, ref MapData mapData, int centerX, int centerY, float fallOff = 0.2f)
        {
            for (int i = startX; i < endX; i++)
            {
                //Check for bounds
                if (i < 0 || i > mapData._mapSize.x - 1 || y < 0 || y > mapData._mapSize.y - 1)
                {
                    continue;
                }

                int index = IndexFromVec2Int(i, y, mapData._mapSize.x);
                Color32 newColor = new Color32(centerColor.r, centerColor.g, centerColor.b, centerColor.a);
                Color32 curColor = mapData._mapTiles[index].LightColor;
                int xDiff = centerX - i;
                int yDiff = centerY - y;
                float dist = Mathf.Sqrt((xDiff * xDiff) + (yDiff * yDiff));
                //newColor.r = (byte)Mathf.Clamp(centerColor.r * (1f - (fallOff * dist)), 0, 255);
                //newColor.g = (byte)Mathf.Clamp(centerColor.g * (1f - (fallOff * dist)), 0, 255);
                //newColor.b = (byte)Mathf.Clamp(centerColor.b * (1f - (fallOff * dist)), 0, 255);

                newColor.r = (byte)Mathf.Clamp(centerColor.r * (1f - (fallOff * dist)), 0, 255);
                newColor.g = (byte)Mathf.Clamp(centerColor.g * (1f - (fallOff * dist)), 0, 255);
                newColor.b = (byte)Mathf.Clamp(centerColor.b * (1f - (fallOff * dist)), 0, 255);

                newColor.r = (curColor.r > newColor.r ? curColor.r : newColor.r);
                newColor.g = (curColor.g > newColor.g ? curColor.g : newColor.g);
                newColor.b = (curColor.b > newColor.b ? curColor.b : newColor.b);

                mapData._mapTiles[index].LightColor = newColor;
            }
        }

        //http://stackoverflow.com/questions/10878209/midpoint-circle-algorithm-for-filled-circles
        public static void FillCircleAreaWithLight(int centerX, int centerY, int radius, Color32 centerColor, ref MapData mapData)
        {
            radius = (int)(255f / 50f);
            int x = radius;
            int y = 0;
            int radiusError = 1 - x;

            while (x > y)
            {
                //Use symmetry to draw the two horizontal lines at this Y with a special case to draw
                // only one line at the centerY where y == 0;
                int startX = -x + centerX;
                int endX = x + centerX;
                FillLineWithLight(startX, endX, y + centerY, centerColor, ref mapData, centerX, centerY);

                if (y != 0)
                {
                    FillLineWithLight(startX, endX, -y + centerY, centerColor, ref mapData, centerX, centerY);
                }

                //move Y one line
                y++;

                //calculate or maintain new x
                if (radiusError < 0)
                {
                    radiusError += 2 * y + 1;
                }
                else
                {
                    // we're about to move x over one, this means we completed a column of X values, use
                    // symmetry to draw those complete columns as horizontal lines at the top and bottom of the circle
                    // beyond the diagonal of the main loop
                    if (x >= y)
                    {
                        startX = -y + 1 + centerX;
                        endX = y - 1 + centerX;
                        FillLineWithLight(startX, endX, x + centerY, centerColor, ref mapData, centerX, centerY);
                        FillLineWithLight(startX, endX, -x + centerY, centerColor, ref mapData, centerX, centerY);
                    }
                    x--;
                    radiusError += 2 * (y - x + 1);
                }
            }

        }
        #endregion

        #region Color Comparison
        public static bool Compare (this Color32 m, Color32 n)
        {
            if (m.r == n.r && m.g == n.g && m.b == n.b && m.a == n.a)
                return true;

            return false;
        }
        #endregion

        public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
        {
            Vector3 perp = Vector3.Cross(fwd, targetDir);
            float dir = Vector3.Dot(perp, up);

            if (dir > 0f)
            {
                return 1f;
            }
            else if (dir < 0)
            {
                return -1f;
            }

            return 0f;
        }
    }
}