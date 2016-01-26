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
    protected List<LightComponent> _visibleLights;
    protected float _lightUpdateTimer;
    protected float _lightUpdateDelta = 1f / 10f;
    protected SquareBounds _previousVisibleLightBounds;
    protected SquareBounds _visibleLightBounds;

    public void AddLight(LightComponent light)
    {
        _visibleLights.Add(light);
    }

    public void RemoveLight(LightComponent light)
    {
        _visibleLights.Remove(light);
    }

#endregion

    #region Gets/Sets
    public Vec2Int ChunkDim { get { return _chunkDim; } }
    #endregion

    public void Init()
    {
        EntityBuildManager.Singleton.Init(5, 5);
        BlocksPerChuck = FileSystem.Optionsconfig.SceneChunkSize;
        _visibleLights = new List<LightComponent>();
        _visibleLightBounds = new SquareBounds(new Vec2Int(0, 0), ClientGameManager.Singleton.Mapdata._mapSize);
        _previousVisibleLightBounds = _visibleLightBounds;

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

        ClientGameManager.Singleton.playerControl.SpawnTileLight();
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
            newChunk.transform.parent = _worldRootTransform;
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

        MapData mapData = ClientGameManager.Singleton.Mapdata;
        int mapWidth = ClientGameManager.Singleton.Mapdata._mapSize.x;

#if DEBUGCLIENT
        Profiler.BeginSample("Reset Visibility");
#endif
        //If the previousLightBounds is different than the current, we need to mark all of those tiles as not visible
        if (_previousVisibleLightBounds != _visibleLightBounds)
        {
            for (int x = _previousVisibleLightBounds.MinPoint.x; x < _previousVisibleLightBounds.MaxPoint.x; x++)
            {
                for (int y = _previousVisibleLightBounds.MinPoint.y; y < _previousVisibleLightBounds.MaxPoint.y; y++)
                {
                    int index = Helpers.IndexFromVec2Int(x, y, mapWidth);
                    mapData._mapTiles[index].SetVisible(false);
                }
            }
        }
        Profiler.EndSample();

        //Reset all lights in visible area
#if DEBUGCLIENT
        Profiler.BeginSample("Reset Lighting");
#endif
        for (int x = _visibleLightBounds.MinPoint.x; x < _visibleLightBounds.MaxPoint.x; x++)
        {
            for (int y = _visibleLightBounds.MinPoint.y; y < _visibleLightBounds.MaxPoint.y; y++)
            {
                int index = Helpers.IndexFromVec2Int(x, y, mapWidth);
                mapData._mapTiles[index].LightColor = ClientGameManager.Singleton.Mapdata.AmbientLightColor; //new Color32(10, 10, 10, 255);
                mapData._mapTiles[index].SetVisible(true);
            }
        }
#if DEBUGCLIENT
        Profiler.EndSample();
#endif

        _previousVisibleLightBounds = _visibleLightBounds;

        //Now go through each light, and find the tile it's on
        int count = _visibleLights.Count;
        for (int i = 0; i < count; i++)
        {
            LightComponent light = _visibleLights[i];

            //If the light is mobile, we need to update its position
            if (light.IsMobile)
            {
                light.UpdatePosition();
            }

            if (light.EntitySpriteObject == null || light.LightRadius < 1)//  .TileIndex < 0 || light.TileIndex > mapData._mapTiles.Length - 1)
            {
                //If the light's entity is nul,, then something funky is going on, and we should remove the light component
                _visibleLights.RemoveAt(i);
                i--;
                continue;
            }
            
            //Helpers.FillCircleAreaWithLight(light.TilePos.x, light.TilePos.y, light.LightRange, light.LightColor, ref mapData);

            if (!light.EntitySpriteObject.EntityData.MapTile.BlocksLight)
            {
                light.EntitySpriteObject.EntityData.MapTile.LightColor = light.LightColor;
            }

            Vec2Int tilePos = light.EntitySpriteObject.EntityData.MapTile.TilePosition;

            SceneLighting.LightFloodFillForward(tilePos.x, tilePos.y, light.LightRadius, light.LightColor, ref mapData, SceneLighting.LightFloodFillQueueItem.FillDirection.Up);
            SceneLighting.LightFloodFillForward(tilePos.x, tilePos.y, light.LightRadius, light.LightColor, ref mapData, SceneLighting.LightFloodFillQueueItem.FillDirection.Right);
            SceneLighting.LightFloodFillForward(tilePos.x, tilePos.y, light.LightRadius, light.LightColor, ref mapData, SceneLighting.LightFloodFillQueueItem.FillDirection.Down);
            SceneLighting.LightFloodFillForward(tilePos.x, tilePos.y, light.LightRadius, light.LightColor, ref mapData, SceneLighting.LightFloodFillQueueItem.FillDirection.Left);
        }

#if DEBUGCLIENT
        Profiler.BeginSample("Update Neighbors");
#endif
        ////Now update the colors on the meshes
        count = mapData._mapTiles.Length;
        for (int i = 0; i < count; i++)
        {
            int localTileIndex;
            int chunkIndex = GetChunkIndex(mapData._mapTiles[i].TilePosition, ClientGameManager.Singleton.Mapdata._mapSize.x, out localTileIndex);
            _sceneChunks[chunkIndex].UpdateBlockLightWithNeighborsWalls(mapData._mapTiles[i], localTileIndex);
        }
#if DEBUGCLIENT
        Profiler.EndSample();
#endif

#if DEBUGCLIENT
        Profiler.BeginSample("Update Mesh Colors");
#endif
        for (int i = 0; i < _sceneChunks.Length; i++)
        {
            _sceneChunks[i].UpdateAllMeshColors();
        }
#if DEBUGCLIENT
        Profiler.EndSample();
#endif

        _lightUpdateTimer = Time.time + _lightUpdateDelta;
    }

    public void UpdateNeighborChunkWallLight(int neighborWorldTileIndex, Vec2Int neighborWorldTilePos, GameData.GameBlockData.BlockFaces face, Color32 color)
    {
        if (neighborWorldTileIndex < 0 || neighborWorldTileIndex > ClientGameManager.Singleton.Mapdata._mapTiles.Length - 1)
        {
            //Debug.LogWarning(string.Format("Trying to set neighbor tile index on something OOB: {0},{1}", neighborWorldTilePos.x, neighborWorldTilePos.y));
            return;
        }

        int localTileIndex;
        int chunkIndex = GetChunkIndex(ClientGameManager.Singleton.Mapdata._mapTiles[neighborWorldTileIndex].TilePosition, ClientGameManager.Singleton.Mapdata._mapSize.x, out localTileIndex);

        _sceneChunks[chunkIndex].UpdateBlockWithLightFromNeighborChunk(localTileIndex, face, color);
    }
}