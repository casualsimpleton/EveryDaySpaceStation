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
        //Less than ideal since we're constructing this here, but can't do a runtime value length
        public static GameManifestV2.BlockDataTemplate DefaultBlockData =
            new GameManifestV2.BlockDataTemplate(1, "DefaultEditorBlock", 1000,
                new byte[] { 1, 1, 1, 1, 1, 1 },
                null, new uint[] { 1, 1, 1, 1, 1, 1 }, 0);

        public static Vector3 DefaultBlockSize = new Vector3(1f, 1f, 1f);

        public ushort BlockType { get; private set; }
        public byte LightValue { get; private set; }
        public byte IsModified { get; private set; }
        public GameManifestV2.BlockDataTemplate BlockData { get; private set; }
        //public bool IsDirty { get; private set; }

        public VoxelChunk ParentChunk { get; private set; }

        public VoxelBlock(ushort blockType, VoxelChunk parent)
        {
            BlockType = blockType;
            ParentChunk = parent;

            GameManifestV2.BlockDataTemplate blockTemplate = null;
            GameManifestV2.Singleton.GetBlockTemplate(blockType, out blockTemplate);
            if (blockTemplate == null)
            {
                BlockData = VoxelBlock.DefaultBlockData;
            }
            else
            {
                BlockData = blockTemplate;
            }
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