//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// GameManager - Unity Monobehaviour that is one of the high level organizers for the game
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

public class GameManager : MonoBehaviour
{
    #region Singleton
    protected static GameManager m_singleton = null;
    public static GameManager Singleton
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
    protected GameData _gameData;
    protected float _timeSinceGameStarted;
    protected MapData _mapData;
    #endregion

    #region Gets/Sets
    public GameData Gamedata { get { return _gameData; } }
    public MapData Mapdata { get { return _mapData; } }
    #endregion

    public void Start()
    {
        _gameData = new GameData();
        FileSystem.Init();
        SceneLevelManager.Singleton.Init();
        SceneLevelManager.Singleton.PopulateMapTiles(_mapData);

        _timeSinceGameStarted = 0f;
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        _gameData.Cleanup();
    }

    public void ProcessMap(EveryDaySpaceStation.Json.MapDataConfig mapConfig)
    {
        //TODO Can valid things like map version and map format
        //mapConfig.LevelData.TileData
        _mapData = new MapData();
        _mapData.LoadMap(mapConfig);
    }
}