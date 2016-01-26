//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// Vec2Int - A container struct for two ints. Similar for Vector2, but helpful for some lovely tiles
// Created: December 4 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 4 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using EveryDaySpaceStation.DataTypes;

namespace EveryDaySpaceStation.DataTypes
{
    /// <summary>
    /// A int only version of Vector2, useful for dealing with 2D x/y coordinates
    /// </summary>
    [System.Serializable]
    public struct Vec2Int
    {
        public int x;
        public int y;

        public static Vec2Int Zero = new Vec2Int(0, 0);

        public Vec2Int(int newX, int newY)
        {
            x = newX;
            y = newY;
        }

        public Vec2Int(Vector2 vec2)
        {
            x = Mathf.RoundToInt(vec2.x);
            y = Mathf.RoundToInt(vec2.y);
        }

        public Vec2Int(Vector3 vec3)
        {
            x = Mathf.RoundToInt(vec3.x);
            y = Mathf.RoundToInt(vec3.y);
        }

        public static bool operator ==(Vec2Int m, Vec2Int n)
        {
            if (m.x == n.x && m.y == n.y)
                return true;

            return false;
        }

        public static bool operator !=(Vec2Int m, Vec2Int n)
        {
            if (m.x != n.x || m.y != n.y)
                return true;

            return false;
        }

        public static Vec2Int operator +(Vec2Int m, Vec2Int n)
        {
            return new Vec2Int(m.x + n.x, m.y + n.y);
        }

        public static Vec2Int operator -(Vec2Int m, Vec2Int n)
        {
            return new Vec2Int(m.x - n.x, m.y - n.y);
        }

        public static Vec2Int operator *(Vec2Int m, int n)
        {
            return new Vec2Int(m.x * n, m.y * n);
        }

        public static Vec2Int operator *(Vec2Int m, float n)
        {
            return new Vec2Int(Mathf.RoundToInt(m.x * n), Mathf.RoundToInt(m.y * n));
        }

        public static Vec2Int operator *(Vec2Int m, Vec2Int n)
        {
            return new Vec2Int(m.x * n.x, m.y * n.y);
        }

        public static Vec2Int operator /(Vec2Int m, int n)
        {
            return new Vec2Int(m.x / n, m.y / n);
        }

        public static Vec2Int operator /(Vec2Int m, float n)
        {
            return new Vec2Int(Mathf.RoundToInt(m.x / n), Mathf.RoundToInt(m.y / n));
        }

        public override string ToString()
        {
            return string.Format("x: {0}, y: {1}", x, y);
        }

        // Override the Object.Equals(object o) method:
        public override bool Equals(object o)
        {
            try
            {
                return (bool)(this == (Vec2Int)o);
            }
            catch
            {
                return false;
            }
        }

        // Override the Object.GetHashCode() method:
        public override int GetHashCode()
        {
            return x + y;
        }
    }
}
