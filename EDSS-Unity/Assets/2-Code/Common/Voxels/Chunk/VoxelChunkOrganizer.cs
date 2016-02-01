//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// VoxelChunkOrganizer - The root gameobject for the chunk
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
    public class VoxelChunkOrganizer : MonoBehaviour
    {
        public static ushort[, ,] TempMap;

        protected UniqueList<ChunkRenderer> _chunkRenderers;
        protected VoxelChunk _chunk;

        void Start()
        {
            TempMap = new ushort[4, 4, 6];
            #region Bottom Layer
            TempMap[0, 0, 0] = 1;
            TempMap[0, 0, 1] = 1;
            TempMap[0, 0, 2] = 1;
            TempMap[0, 0, 3] = 1;
            TempMap[0, 0, 4] = 1;
            TempMap[0, 0, 5] = 1;

            TempMap[1, 0, 0] = 1;
            TempMap[1, 0, 1] = 1;
            TempMap[1, 0, 2] = 1;
            TempMap[1, 0, 3] = 1;
            TempMap[1, 0, 4] = 1;
            TempMap[1, 0, 5] = 1;

            TempMap[2, 0, 0] = 1;
            TempMap[2, 0, 1] = 1;
            TempMap[2, 0, 2] = 1;
            TempMap[2, 0, 3] = 1;
            TempMap[2, 0, 4] = 1;
            TempMap[2, 0, 5] = 1;

            TempMap[3, 0, 0] = 1;
            TempMap[3, 0, 1] = 1;
            TempMap[3, 0, 2] = 1;
            TempMap[3, 0, 3] = 1;
            TempMap[3, 0, 4] = 1;
            TempMap[3, 0, 5] = 1;
            #endregion

            #region Second Layer
            TempMap[0, 1, 0] = 3;
            TempMap[0, 1, 1] = 3;
            TempMap[0, 1, 2] = 3;
            TempMap[0, 1, 3] = 3;
            TempMap[0, 1, 4] = 3;
            TempMap[0, 1, 5] = 3;

            TempMap[1, 1, 0] = 0;
            TempMap[1, 1, 1] = 0;
            TempMap[1, 1, 2] = 0;
            TempMap[1, 1, 3] = 0;
            TempMap[1, 1, 4] = 0;
            TempMap[1, 1, 5] = 0;

            TempMap[2, 1, 0] = 0;
            TempMap[2, 1, 1] = 0;
            TempMap[2, 1, 2] = 0;
            TempMap[2, 1, 3] = 0;
            TempMap[2, 1, 4] = 0;
            TempMap[2, 1, 5] = 0;

            TempMap[3, 1, 0] = 3;
            TempMap[3, 1, 1] = 3;
            TempMap[3, 1, 2] = 3;
            TempMap[3, 1, 3] = 3;
            TempMap[3, 1, 4] = 3;
            TempMap[3, 1, 5] = 3;
            #endregion

            #region Third Layer
            TempMap[0, 2, 0] = 3;
            TempMap[0, 2, 1] = 3;
            TempMap[0, 2, 2] = 3;
            TempMap[0, 2, 3] = 3;
            TempMap[0, 2, 4] = 3;
            TempMap[0, 2, 5] = 3;

            TempMap[1, 2, 0] = 0;
            TempMap[1, 2, 1] = 0;
            TempMap[1, 2, 2] = 0;
            TempMap[1, 2, 3] = 0;
            TempMap[1, 2, 4] = 0;
            TempMap[1, 2, 5] = 0;

            TempMap[2, 2, 0] = 0;
            TempMap[2, 2, 1] = 0;
            TempMap[2, 2, 2] = 0;
            TempMap[2, 2, 3] = 0;
            TempMap[2, 2, 4] = 0;
            TempMap[2, 2, 5] = 0;

            TempMap[3, 2, 0] = 3;
            TempMap[3, 2, 1] = 3;
            TempMap[3, 2, 2] = 3;
            TempMap[3, 2, 3] = 3;
            TempMap[3, 2, 4] = 3;
            TempMap[3, 2, 5] = 3;
            #endregion

            #region Top Layer
            TempMap[0, 3, 0] = 1;
            TempMap[0, 3, 1] = 1;
            TempMap[0, 3, 2] = 1;
            TempMap[0, 3, 3] = 1;
            TempMap[0, 3, 4] = 1;
            TempMap[0, 3, 5] = 1;

            TempMap[1, 3, 0] = 1;
            TempMap[1, 3, 1] = 1;
            TempMap[1, 3, 2] = 1;
            TempMap[1, 3, 3] = 1;
            TempMap[1, 3, 4] = 1;
            TempMap[1, 3, 5] = 1;

            TempMap[2, 3, 0] = 1;
            TempMap[2, 3, 1] = 1;
            TempMap[2, 3, 2] = 1;
            TempMap[2, 3, 3] = 1;
            TempMap[2, 3, 4] = 1;
            TempMap[2, 3, 5] = 1;

            TempMap[3, 3, 0] = 1;
            TempMap[3, 3, 1] = 1;
            TempMap[3, 3, 2] = 1;
            TempMap[3, 3, 3] = 1;
            TempMap[3, 3, 4] = 1;
            TempMap[3, 3, 5] = 1;
            #endregion


            _chunk = new VoxelChunk(TempMap.GetLength(0), TempMap.GetLength(1), TempMap.GetLength(2));

            _chunk.SetChunkGameObject(this);
            //_chunk.TestRandomChunkData();
            _chunk.LoadChunkData(TempMap);
        }

        void Update()
        {
            _chunk.ChunkUpdate();
        }

        void LateUpdate()
        {
            _chunk.ChunkLateUpdate();
        }
    }
}