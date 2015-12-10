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



    [SerializeField]
    protected bool _enableLights = true;
    protected List<TileLight> _visibleLights;
    protected float _lightUpdateTimer;
    protected float _lightUpdateDelta = 1f / 10f;
    protected SquareBounds _visibleLightBounds;

    public void AddLight(TileLight light)
    {
        _visibleLights.Add(light);
    }

    public void RemoveLight(TileLight light)
    {
        _visibleLights.Remove(light);
    }

#endregion

    #region Gets/Sets
    public Vec2Int ChunkDim { get { return _chunkDim; } }
    #endregion

    public void Init()
    {
        BlocksPerChuck = FileSystem.Optionsconfig.SceneChunkSize;
        _visibleLights = new List<TileLight>();
        _visibleLightBounds = new SquareBounds(new Vec2Int(0, 0), GameManager.Singleton.Mapdata._mapSize);

        CreateMap();

        //TEMP
        //TileLight light = new TileLight(new Color32(255, 255, 255, 255), 2);
        //light.TilePos = new Vec2Int(3, 3);
        //light.TileIndex = Helpers.IndexFromVec2Int(light.TilePos, GameManager.Singleton.Mapdata._mapSize.x);
        //_visibleLights.Add(light);

        //TileLight light2 = new TileLight(new Color32(255, 255, 255, 255), 2);
        //light2.TilePos = new Vec2Int(3, 12);
        //light2.TileIndex = Helpers.IndexFromVec2Int(light2.TilePos, GameManager.Singleton.Mapdata._mapSize.x);
        //_visibleLights.Add(light2);

        //TileLight light3 = new TileLight(new Color32(255, 255, 255, 255), 5);
        //light3.TilePos = new Vec2Int(6, 8);
        //light3.TileIndex = Helpers.IndexFromVec2Int(light3.TilePos, GameManager.Singleton.Mapdata._mapSize.x);
        //_visibleLights.Add(light3);

        //TileLight light4 = new TileLight(new Color32(255, 255, 255, 255), 2);
        //light4.TilePos = new Vec2Int(10, 5);
        //light4.TileIndex = Helpers.IndexFromVec2Int(light4.TilePos, GameManager.Singleton.Mapdata._mapSize.x);
        //_visibleLights.Add(light4);

        //TileLight light5 = new TileLight(new Color32(255, 255, 255, 255), 2);
        //light5.TilePos = new Vec2Int(13, 12);
        //light5.TileIndex = Helpers.IndexFromVec2Int(light5.TilePos, GameManager.Singleton.Mapdata._mapSize.x);
        //_visibleLights.Add(light5);

        GameManager.Singleton.playerControl.SpawnTileLight();
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

            int x, z;
            z = (int)((float)i / (float)_chunkDim.x);
            x = i - (z * _chunkDim.x);

            Vector3 worldPos = new Vector3((x * SceneChunk.blockSize * BlocksPerChuck), 0f, (z * SceneChunk.blockSize * BlocksPerChuck));

            _sceneChunks[i].CreateChunk(worldPos, new Vec2Int(x, z));
        }

        len = mapData._mapTiles.Length;
        for (int i = 0; i < len; i++)
        {
            int localTileIndex;
            int chunkIndex = GetChunkIndex(mapData._mapTiles[i].TilePosition, mapData._mapSize.x, out localTileIndex);
            //_sceneChunks[chunkIndex].UpdateBlock(mapData._mapTiles[i]);
            _sceneChunks[chunkIndex].FirstUpdateBlock(mapData._mapTiles[i], localTileIndex);
        }

        for (int i = 0; i < _sceneChunks.Length; i++)
        {
            _sceneChunks[i].UpdateAllMeshColliders();
        }
    }

    protected int GetChunkIndex(Vec2Int tilePos, int width, out int chuckLocalTileIndex)
    {
        int chunkIndexX = Mathf.FloorToInt((float)tilePos.x / (float)BlocksPerChuck);
        int chunkIndexY = Mathf.FloorToInt((float)tilePos.y / (float)BlocksPerChuck);

        int tX = tilePos.x - chunkIndexX * BlocksPerChuck;
        int tY = tilePos.y - chunkIndexY * BlocksPerChuck;

        chuckLocalTileIndex = tY * BlocksPerChuck + tX;

        int chunkIndex = chunkIndexY * _chunkDim.x + chunkIndexX;
        return chunkIndex;
    }

    public void Update()
    {
        if (_enableLights)
        {
            UpdateTileLights();
        }
    }

    void UpdateTileLights()
    {
        if (_lightUpdateTimer > Time.time)
        {
            return;
        }

        MapData mapData = GameManager.Singleton.Mapdata;

        //First reset all lights in visible area
        for (int x = 0; x < _visibleLightBounds.MaxPoint.x; x++)
        {
            for (int y = 0; y < _visibleLightBounds.MaxPoint.y; y++)
            {
                int index = Helpers.IndexFromVec2Int(x, y, _visibleLightBounds.MaxPoint.x);
                mapData._mapTiles[index].LightColor = new Color32(10, 10, 10, 255);
            }
        }

        //Now go through each light, and find the tile it's on
        int count = _visibleLights.Count;
        for (int i = 0; i < count; i++)
        {
            TileLight light = _visibleLights[i];

            //If the light is mobile, we need to update its position
            if (light.IsMobile)
            {
                light.UpdatePosition();
            }

            if (light.TileIndex < 0 || light.TileIndex > mapData._mapTiles.Length - 1)
                continue;

            //Helpers.FillCircleAreaWithLight(light.TilePos.x, light.TilePos.y, light.LightRange, light.LightColor, ref mapData);

            if (!mapData._mapTiles[light.TileIndex].BlocksLight)
            {
                mapData._mapTiles[light.TileIndex].LightColor = light.LightColor;
            }

            SceneLighting.LightFloodFillForward(light.TilePos.x, light.TilePos.y, light.LightRange, light.LightColor, ref mapData, SceneLighting.LightFloodFillQueueItem.FillDirection.Up);
            SceneLighting.LightFloodFillForward(light.TilePos.x, light.TilePos.y, light.LightRange, light.LightColor, ref mapData, SceneLighting.LightFloodFillQueueItem.FillDirection.Right);
            SceneLighting.LightFloodFillForward(light.TilePos.x, light.TilePos.y, light.LightRange, light.LightColor, ref mapData, SceneLighting.LightFloodFillQueueItem.FillDirection.Down);
            SceneLighting.LightFloodFillForward(light.TilePos.x, light.TilePos.y, light.LightRange, light.LightColor, ref mapData, SceneLighting.LightFloodFillQueueItem.FillDirection.Left);
        }

        //Now update the colors on the meshes
        count = mapData._mapTiles.Length;
        for (int i = 0; i < count; i++)
        {
            int localTileIndex;
            int chunkIndex = GetChunkIndex(mapData._mapTiles[i].TilePosition, GameManager.Singleton.Mapdata._mapSize.x, out localTileIndex);
            _sceneChunks[chunkIndex].UpdateBlockLightWithNeighborsWalls(mapData._mapTiles[i], localTileIndex);
        }

        for (int i = 0; i < _sceneChunks.Length; i++)
        {
            _sceneChunks[i].UpdateAllMeshColors();
        }

        _lightUpdateTimer = Time.time + _lightUpdateDelta;
    }

    public void UpdateNeighborChunkWallLight(int neighborWorldTileIndex, Vec2Int neighborWorldTilePos, GameData.GameBlockData.BlockFaces face, Color32 color)
    {
        if (neighborWorldTileIndex < 0 || neighborWorldTileIndex > GameManager.Singleton.Mapdata._mapTiles.Length - 1)
        {
            //Debug.LogWarning(string.Format("Trying to set neighbor tile index on something OOB: {0},{1}", neighborWorldTilePos.x, neighborWorldTilePos.y));
            return;
        }

        int localTileIndex;
        int chunkIndex = GetChunkIndex(GameManager.Singleton.Mapdata._mapTiles[neighborWorldTileIndex].TilePosition, GameManager.Singleton.Mapdata._mapSize.x, out localTileIndex);

        _sceneChunks[chunkIndex].UpdateBlockWithLightFromNeighborChunk(localTileIndex, face, color);
    }
}