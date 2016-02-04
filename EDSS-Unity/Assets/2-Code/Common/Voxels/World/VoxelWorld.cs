//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// VoxelWorld - Handles voxel related world functionality. Organizes the chunks
// Created: Febuary 1 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: Febuary 1 2016
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
    public class VoxelWorld : MonoBehaviour
    {
        #region Singleton
        protected static VoxelWorld m_singleton = null;
        public static VoxelWorld Singleton
        {
            get
            {
                return m_singleton;
            }
        }

        void Awake()
        {
            m_singleton = this;
        }
        #endregion

        protected VoxelChunkOrganizer[,,] _voxelChunks;
        public static ushort[, ,] TempMap;
        public Transform myTransform { get; private set; }
        System.Diagnostics.Stopwatch _timer;

        public static Vec3Int ChunkSize = new Vec3Int(16, 16, 16);

        public static VoxelChunkOrganizer GetVoxelChunk(int x, int z)
        {
            GameObject go = new GameObject(string.Format("VoxelChunk-{0}-{1}", x, z));

            VoxelChunkOrganizer vco = go.AddComponent<VoxelChunkOrganizer>();

            return vco;
        }

        public void Start()
        {
            myTransform = this.transform;

            _timer = new System.Diagnostics.Stopwatch();
            _timer.Start();

            TempMap = VoxelTempWorldData.GetMapFromFile();

            int xChunkNum = Mathf.CeilToInt((float)TempMap.GetLength(0) / (float)ChunkSize.x);
            int zChunkNum = Mathf.CeilToInt((float)TempMap.GetLength(2) / (float)ChunkSize.z);

            _voxelChunks = new VoxelChunkOrganizer[xChunkNum, 1, zChunkNum];

            //Prepare chunks
            for (int x = 0; x < xChunkNum; x++)
            {
                for (int z = 0; z < zChunkNum; z++)
                {
                    VoxelChunkOrganizer vco = GetVoxelChunk(x, z);
                    _voxelChunks[x, 0, z] = vco;
                    vco.Init(ChunkSize.x, ChunkSize.y, ChunkSize.z);
                    vco.transform.parent = myTransform;
                    vco.transform.localPosition = new Vector3(x * VoxelBlock.DefaultBlockSize.x * ChunkSize.x, 0, z * VoxelBlock.DefaultBlockSize.z * ChunkSize.z);

                    vco.LoadChunkDataPreSet(ChunkSize);
                }
            }

            //Read through the map data, and pass it to the proper chunk
            int chunkX, chunkY, chunkZ;
            int maxX = TempMap.GetLength(0);
            int maxY = TempMap.GetLength(1);
            int maxZ = TempMap.GetLength(2);
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                    {
                        chunkX = x / ChunkSize.x;
                        chunkY = y / ChunkSize.y;
                        chunkZ = z / ChunkSize.z;

                        Vec3Int localPos = new Vec3Int(x % ChunkSize.x, y % ChunkSize.y, z % ChunkSize.z);

                        _voxelChunks[chunkX, chunkY, chunkZ].LoadChunkDataPiecemeal(TempMap[x, y, z], localPos);
                    }
                }
            }

            for (int x = 0; x < xChunkNum; x++)
            {
                for (int z = 0; z < zChunkNum; z++)
                {
                    _voxelChunks[x, 0, z].LoadChunkDataPostSet();
                }
            }

            _timer.Stop();
            Debug.Log(string.Format("World Created in {0}ms for {1},{2},{3}", _timer.ElapsedMilliseconds, TempMap.GetLength(0), TempMap.GetLength(1), TempMap.GetLength(2)));
        }
    }
}