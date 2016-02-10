//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// VoxelBlock - Basic Voxel Block
// Created: January 31 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 31 2016
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
using EveryDaySpaceStation.Json;
using EveryDaySpaceStation.DataTypes;

namespace EveryDaySpaceStation
{
    [System.Serializable]
    public struct VoxelBlock
    {
        public static Vector3 DefaultBlockSize = new Vector3(1f, 1f, 1f);

        public ushort BlockType { get; private set; }
        public byte LightValue { get; private set; }
        public byte IsModified { get; private set; }
        //public bool IsDirty { get; private set; }

        public VoxelChunk ParentChunk { get; private set; }

        public VoxelBlock(ushort blockType, VoxelChunk parent)
        {
            BlockType = blockType;
            ParentChunk = parent;
        }

        public void SetLight(byte newLight)
        {
            LightValue = newLight;
        }

        public void SetModified(bool isChanged)
        {
            IsModified = (byte)(isChanged == true ? 1 : 0);
        }

        //public void SetDirty(bool isDirty)
        //{
        //    IsDirty = IsDirty;
        //}
    }
}