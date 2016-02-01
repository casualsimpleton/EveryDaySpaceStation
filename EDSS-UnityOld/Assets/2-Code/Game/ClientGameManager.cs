//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// ClientGameManager - Unity Monobehaviour that is one of the high level organizers for the client game
// Created: December 4 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 25 2016
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class ClientGameManager : MonoBehaviour
{
    #region Singleton
    protected static ClientGameManager m_singleton = null;
    public static ClientGameManager Singleton
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

    #region Classes/Structs
    
    #endregion

    #region Vars
    [SerializeField]
    protected GameData _gameData;
    [SerializeField]
    protected MapData _mapData;
    
    protected float _timeSinceGameStarted;

    public EDSSFirstPersonControls playerControl;
    public EDSSFirstPersonCamera playerCamera;
    public static int ClientWorldLayer;
    public static int ClientEntityLayer;
    public static int ClientTriggerLayer;
    public static Color32 HighlightColor = new Color32(180, 255, 180, 255);
    System.Diagnostics.Stopwatch _timer = new System.Diagnostics.Stopwatch();
    #endregion

    #region Gets/Sets
    public GameData Gamedata { get { return _gameData; } }
    public MapData Mapdata { get { return _mapData; } }
    #endregion

    public void Start()
    {
        ClientWorldLayer = LayerMask.NameToLayer("ClientWorld");
        ClientEntityLayer = LayerMask.NameToLayer("ClientEntity");
        ClientTriggerLayer = LayerMask.NameToLayer("ClientTrigger");

        _timer.Start();
        _gameData = new GameData();
        FileSystem.Init();
        PoolManager.Singleton.Init();
        SceneLevelManager.Singleton.Init();
        PoolManager.Singleton.LateInit();
        SceneLevelManager.Singleton.PopulateMapTiles(_mapData);
        _timer.Stop();

        Debug.Log(string.Format("Load dur: {0}ms", _timer.ElapsedMilliseconds));

        _timeSinceGameStarted = 0f;
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        _mapData.Cleanup();
        _gameData.Cleanup();
    }

    public void ProcessMap(EveryDaySpaceStation.Json.MapDataConfig mapConfig)
    {
        //TODO Can valid things like map version and map format
        //mapConfig.LevelData.TileData
        _mapData = new MapData();
        _mapData.LoadMap(mapConfig);
    }

    public void ProcessMapEntities(EveryDaySpaceStation.Json.MapEntityDataConfig mapEntityConfig)
    {
        if (_mapData == null)
        {
            Debug.LogError("Can't load map entity data before map data");
            return;
        }

        _mapData.LoadEntities(mapEntityConfig);
    }
    
    public void Client_AttemptInput(EntitySpriteGameObject esgo)
    {
        if (esgo != null)
        {
            esgo.Activate();
        }
    }

    public void Client_AttemptAltInput(EntitySpriteGameObject esgo)
    {
        if (esgo != null)
        {
            esgo.TEMP_AltActivate();
        }
    }

    public void Client_AttemptTertiaryInput(EntitySpriteGameObject esgo)
    {
        if (esgo != null)
        {
            esgo.TEMP_TertiaryActivate();
        }
    }

    public void Client_AttempLockInput(EntitySpriteGameObject esgo)
    {
        if (esgo != null)
        {
            esgo.TEMP_ToggleLockState();
        }
    }

    public void Client_AttemptPowerInput(EntitySpriteGameObject esgo)
    {
        if (esgo != null)
        {
            esgo.TEMP_PowerActivate();
        }
    }
}