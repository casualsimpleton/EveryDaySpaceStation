//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// Vec3Int - A container struct for three ints. Similar for Vector3, but helpful for some lovely blocks
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
    public struct Vec3Int
    {
        public int x;
        public int y;
        public int z;

        public static Vec3Int Zero = new Vec3Int(0, 0, 0);
        public static Vec3Int Up = new Vec3Int(0, 1, 0);
        public static Vec3Int Forward = new Vec3Int(0, 0, 1);
        public static Vec3Int Right = new Vec3Int(1, 0, 0);

        public Vec3Int(int newX, int newY, int newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        public Vec3Int(Vector3 vec3)
        {
            x = Mathf.RoundToInt(vec3.x);
            y = Mathf.RoundToInt(vec3.y);
            z = Mathf.RoundToInt(vec3.z);
        }

        public Vec3Int(Vec3Int vec3)
        {
            x = vec3.x;
            y = vec3.y;
            z = vec3.z;
        }
        
        public static bool operator ==(Vec3Int m, Vec3Int n)
        {
            if (m.x == n.x && m.y == n.y)
                return true;

            return false;
        }

        public static bool operator !=(Vec3Int m, Vec3Int n)
        {
            if (m.x != n.x || m.y != n.y)
                return true;

            return false;
        }

        public static Vec3Int operator +(Vec3Int m, Vec3Int n)
        {
            return new Vec3Int(m.x + n.x, m.y + n.y, m.z + n.z);
        }

        public static Vec3Int operator -(Vec3Int m, Vec3Int n)
        {
            return new Vec3Int(m.x - n.x, m.y - n.y, m.z - n.z);
        }

        public static Vec3Int operator *(Vec3Int m, int n)
        {
            return new Vec3Int(m.x * n, m.y * n, m.z * n);
        }

        public static Vec3Int operator *(Vec3Int m, float n)
        {
            return new Vec3Int(Mathf.RoundToInt(m.x * n), Mathf.RoundToInt(m.y * n), Mathf.RoundToInt(m.z * n));
        }

        public static Vec3Int operator *(Vec3Int m, Vec3Int n)
        {
            return new Vec3Int(m.x * n.x, m.y * n.y, m.z * n.z);
        }

        public static Vec3Int operator /(Vec3Int m, int n)
        {
            return new Vec3Int(m.x / n, m.y / n, m.z / n);
        }

        public static Vec3Int operator /(Vec3Int m, float n)
        {
            return new Vec3Int(Mathf.RoundToInt(m.x / n), Mathf.RoundToInt(m.y / n), Mathf.RoundToInt(m.z / n));
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", x, y, z);
        }

        // Override the Object.Equals(object o) method:
        public override bool Equals(object o)
        {
            try
            {
                return (bool)(this == (Vec3Int)o);
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
