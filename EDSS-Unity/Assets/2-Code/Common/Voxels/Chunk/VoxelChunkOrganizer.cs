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
        protected UniqueList<ChunkRenderer> _chunkRenderers;
        protected VoxelChunk _chunk;

        void Start()
        {
            //_chunk = new VoxelChunk(TempMap.GetLength(0), TempMap.GetLength(1), TempMap.GetLength(2));

            //_chunk.SetChunkGameObject(this);
            ////_chunk.TestRandomChunkData();
            //_chunk.LoadChunkData(TempMap);
        }

        public void Init(int xwidth, int yheight, int zlength)
        {
            _chunk = new VoxelChunk(xwidth, yheight, zlength);
            _chunk.SetChunkGameObject(this);
        }

        public void LoadChunkData()
        {

        }

        public void LoadChunkDataPreSet(Vec3Int chunkSize)
        {
            _chunk.LoadChunkDataPreSet(chunkSize);
        }

        public void LoadChunkDataPiecemeal(MapDataV2.MapBlock block, Vec3Int xyz, bool useBlockDefaults = true)
        {
            _chunk.LoadChunkDataPiecemeal(block, xyz, useBlockDefaults);
        }

        public void LoadChunkDataPostSet()
        {
            _chunk.LoadChunkDataPostSet();
        }

        public void ChangeDataBlock(MapDataV2.MapBlock block, ushort prevBlockType, Vec3Int xyz)
        {
            _chunk.ChangeDataBlock(block, prevBlockType, xyz);
        }

        public void ChangeDataFace(Vec3Int localPos, GameManifestV2.BlockDataTemplate.ShowFaceDirection faceSide, ushort newBlockFaceUID)
        {
            _chunk.ChangeDataFace(localPos, faceSide, newBlockFaceUID);
        }

        void Update()
        {
            _chunk.ChunkUpdate();
        }

        void LateUpdate()
        {
            _chunk.ChunkLateUpdate();
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;

            Gizmos.DrawWireCube(
                new Vector3(_chunk.ChunkSize.x * 0.5f * VoxelBlock.DefaultBlockSize.x, _chunk.ChunkSize.y * 0.5f * VoxelBlock.DefaultBlockSize.x, _chunk.ChunkSize.z * 0.5f * VoxelBlock.DefaultBlockSize.x)
                 + transform.position, new Vector3(_chunk.ChunkSize.x * VoxelBlock.DefaultBlockSize.x, _chunk.ChunkSize.y * VoxelBlock.DefaultBlockSize.x, _chunk.ChunkSize.z * VoxelBlock.DefaultBlockSize.x));
        }
    }
}