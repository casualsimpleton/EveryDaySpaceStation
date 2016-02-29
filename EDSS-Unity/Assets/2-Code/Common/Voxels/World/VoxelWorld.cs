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
        protected Queue<MapDataV2.MapBlock> _blocksWithModifiedLights;
        public static MapDataV2.MapBlock[, ,] TempMap;
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

            if (_blocksWithModifiedLights == null)
            {
                _blocksWithModifiedLights = new Queue<MapDataV2.MapBlock>();
            }
        }

        public void CreateWorld(MapDataV2.MapBlock[, ,] blockData, bool useBlockDefaults = true)
        {
            if (myTransform == null)
            {
                Start();
            }

            _timer = new System.Diagnostics.Stopwatch();
            _timer.Start();

            if (_blocksWithModifiedLights == null)
            {
                _blocksWithModifiedLights = new Queue<MapDataV2.MapBlock>();
            }

            //TempMap = VoxelTempWorldData.GetMapFromFile();
            TempMap = blockData;

            int xChunkNum = Mathf.CeilToInt((float)TempMap.GetLength(0) / (float)ChunkSize.x);
            int zChunkNum = Mathf.CeilToInt((float)TempMap.GetLength(2) / (float)ChunkSize.z);
            int yChunkNum = Mathf.CeilToInt((float)TempMap.GetLength(1) / (float)ChunkSize.y);

            _voxelChunks = new VoxelChunkOrganizer[xChunkNum, yChunkNum, zChunkNum];

            //Prepare chunks
            for (int y = 0; y < yChunkNum; y++)
            {
                for (int x = 0; x < xChunkNum; x++)
                {
                    for (int z = 0; z < zChunkNum; z++)
                    {
                        int xChunkSize = x * ChunkSize.x;
                        int zChunkSize = z * ChunkSize.z;
                        int yChunkSize = y * ChunkSize.y;
                        xChunkSize = Mathf.Min(TempMap.GetLength(0) - xChunkSize, ChunkSize.x);
                        yChunkSize = Mathf.Min(TempMap.GetLength(1) - yChunkSize, ChunkSize.y);
                        zChunkSize = Mathf.Min(TempMap.GetLength(2) - zChunkSize, ChunkSize.z);
                        VoxelChunkOrganizer vco = GetVoxelChunk(x, z);
                        _voxelChunks[x, y, z] = vco;
                        vco.Init(xChunkSize, yChunkSize, zChunkSize);
                        vco.transform.parent = myTransform;
                        vco.transform.localPosition = new Vector3(x * VoxelBlock.DefaultBlockSize.x * ChunkSize.x, 0, z * VoxelBlock.DefaultBlockSize.z * ChunkSize.z);

                        vco.LoadChunkDataPreSet(new Vec3Int(xChunkSize, yChunkSize, zChunkSize));
                    }
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

                        _voxelChunks[chunkX, chunkY, chunkZ].LoadChunkDataPiecemeal(TempMap[x, y, z], localPos, useBlockDefaults);
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

        public void UpdateBlock(Vec3Int position, MapDataV2.MapBlock newBlock, ushort prevBlockType)
        {
            int chunkX = position.x / ChunkSize.x;
            int chunkY = position.y / ChunkSize.y;
            int chunkZ = position.z / ChunkSize.z;

            Vec3Int localPos = new Vec3Int(position.x % ChunkSize.x, position.y % ChunkSize.y, position.z % ChunkSize.z);

            _voxelChunks[chunkX, chunkY, chunkZ].ChangeDataBlock(newBlock, prevBlockType, localPos);
        }

        public void UpdateBlockFace(Vec3Int position, GameManifestV2.BlockDataTemplate.ShowFaceDirection faceSide, ushort newBlockFaceUID)
        {
            int chunkX = position.x / ChunkSize.x;
            int chunkY = position.y / ChunkSize.y;
            int chunkZ = position.z / ChunkSize.z;

            Vec3Int localPos = new Vec3Int(position.x % ChunkSize.x, position.y % ChunkSize.y, position.z % ChunkSize.z);

            _voxelChunks[chunkX, chunkY, chunkZ].ChangeDataFace(localPos, faceSide, newBlockFaceUID);
        }

        float m_lightTimer = 0f;
        float m_lightTimerDelta = 1f / 4f;
        void Update()
        {
            if (m_lightTimer < Time.time)
            {
                //for (int y = 0; y < _voxelChunks.GetLength(1); y++)
                //{
                //    for (int x = 0; x < _voxelChunks.GetLength(0); x++)
                //    {
                //        for (int z = 0; z < _voxelChunks.GetLength(2); z++)
                //        {
                //            _voxelChunks[x, y, z].UpdateLights();
                //        }
                //    }
                //}

                //Use light around camera
                Vector3 cameraPos = Camera.main.transform.position;
                
                VoxelLight cameraLight = new VoxelLight();
                cameraLight.ColorLight = new Color32(128, 128, 128, 255);
                cameraLight.BlockPosition = new Vec3Int(cameraPos);

                if (cameraLight.BlockPosition.x < 0 || cameraLight.BlockPosition.y < 0 || cameraLight.BlockPosition.z < 0 ||
                    cameraLight.BlockPosition.x > TempMap.GetLength(0) - 1 || cameraLight.BlockPosition.y > TempMap.GetLength(1) - 1 || cameraLight.BlockPosition.z > TempMap.GetLength(2) - 1)
                {
                    m_lightTimer = Time.time + m_lightTimerDelta;
                    return;
                }

                VoxelLighting.CalculateMapLights(ref TempMap, ref _blocksWithModifiedLights, cameraLight);

                Debug.Log("Number of modified blocks " + _blocksWithModifiedLights.Count);

                while (_blocksWithModifiedLights.Count > 0)
                {
                    MapDataV2.MapBlock block = _blocksWithModifiedLights.Dequeue();

                    block.BlockLight.CalculateColor();
                }

                m_lightTimer = Time.time + m_lightTimerDelta;
            }
        }
    }
}