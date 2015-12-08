//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// SceneLevelManager - Unity Monobehaviour that sits on a gameobject and helps to control the rendering of the world within the game scene
// Created: December 4 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 4 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class SceneLevelManager : MonoBehaviour
{
    #region Singleton
    protected static SceneLevelManager m_singleton = null;
    public static SceneLevelManager Singleton
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

#region Vars
    public int BlocksPerChuck { get; set; }
    [SerializeField]
    protected Transform _worldRootTransform;
    [SerializeField]
    protected SceneChunk[] _sceneChunks;
    [SerializeField]
    protected Vec2Int _chunkDim;
#endregion

    public void Init()
    {
        BlocksPerChuck = FileSystem.Optionsconfig.SceneChunkSize;

        CreateMap();
    }

    public void CreateMap()
    {
        GameObject rootGO = new GameObject("WorldRoot");
        _worldRootTransform = rootGO.transform;

        _worldRootTransform.position = Vector3.zero;

        //_sceneChunks = new SceneChunk[1];

        //for (int i = 0; i < _sceneChunks.Length; i++)
        //{
        //    GameObject newChunk = new GameObject();
        //    newChunk.transform.parent = this.transform;
        //    newChunk.transform.localPosition = Vector3.zero;

        //    _sceneChunks[i] = newChunk.AddComponent<SceneChunk>();

        //    Vector3 worldPos = new Vector3((i * SceneChunk.blockSize * BlocksPerChuck), 0f, 0f);

        //    _sceneChunks[i].CreateChunk(worldPos, new Vec2Int(i, 0));
        //}
    }

    public void PopulateMapTiles(MapData mapData)
    {
        _chunkDim = new Vec2Int();
        _chunkDim.x = Mathf.CeilToInt((float)mapData._mapSize.x / (float)BlocksPerChuck);
        _chunkDim.y = Mathf.CeilToInt((float)mapData._mapSize.y / (float)BlocksPerChuck);

        _sceneChunks = new SceneChunk[_chunkDim.x * _chunkDim.y];

        int len = _sceneChunks.Length;
        for (int i = 0; i < len; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.transform.parent = this.transform;
            newChunk.transform.localPosition = Vector3.zero;

            _sceneChunks[i] = newChunk.AddComponent<SceneChunk>();

            Vector3 worldPos = new Vector3((i * SceneChunk.blockSize * BlocksPerChuck), 0f, 0f);

            int x, z;
            z = (int)((float)i / (float)_chunkDim.x);
            x = i - (z * _chunkDim.x);

            _sceneChunks[i].CreateChunk(worldPos, new Vec2Int(x, z));
        }

        len = mapData._mapTiles.Length;
        for (int i = 0; i < len; i++)
        {
            int chunkIndex = GetChunkIndex(mapData._mapTiles[i].TilePosition, mapData._mapSize.x);
            //_sceneChunks[chunkIndex].UpdateBlock(mapData._mapTiles[i]);
            _sceneChunks[chunkIndex].FirstUpdateBlock(mapData._mapTiles[i]);
        }

        for (int i = 0; i < _sceneChunks.Length; i++)
        {
            _sceneChunks[i].UpdateAllMeshColliders();
        }
    }

    protected int GetChunkIndex(Vec2Int tilePos, int width)
    {
        int chunkIndexX = Mathf.FloorToInt((float)tilePos.x / (float)BlocksPerChuck);
        int chunkIndexY = Mathf.FloorToInt((float)tilePos.y / (float)BlocksPerChuck);

        int chunkIndex = chunkIndexY * _chunkDim.x + chunkIndexX;
        return chunkIndex;
    }
}