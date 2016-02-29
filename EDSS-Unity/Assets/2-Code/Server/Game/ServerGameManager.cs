//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// ServerGameManager - Handles server considerations for the game
// Created: January 26 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 26 2016
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;
using EveryDaySpaceStation.Network;

//public class ServerGameManager : GameManagerAbstract
//{
//    #region Singleton
//    protected static ServerGameManager m_singleton = null;
//    public static ServerGameManager Singleton
//    {
//        get
//        {
//            return m_singleton;
//        }
//    }

//    void Awake()
//    {
//        m_singleton = this;
//    }
//    #endregion

//    #region Enums

//    #endregion

//    #region Vars
//    protected float _timeSinceGameStarted;

//    public static int ServerWorldLayer;
//    public static int ServerEntityLayer;
//    public static int ServerTriggerLayer;

//    protected new ServerMapData _mapData;
//    #endregion

//    protected override void Start()
//    {
//        base.Start();

//        ServerWorldLayer = LayerMask.NameToLayer("ServerWorld");
//        ServerEntityLayer = LayerMask.NameToLayer("ServerEntity");
//        ServerTriggerLayer = LayerMask.NameToLayer("ServerTrigger");

//        _timer.Start();
//        _gameData = new GameData();
//        FileSystem.Init();
//        PoolManager.Singleton.Init();
//        //SceneLevelManager.Singleton.Init();
//        PoolManager.Singleton.LateInit();
//        //SceneLevelManager.Singleton.PopulateMapTiles(_mapData);
//        _timer.Stop();

//        Debug.Log(string.Format("Load dur: {0}ms", _timer.ElapsedMilliseconds));

//        _timeSinceGameStarted = 0f;
//    }

//    public void ProcessMap(EveryDaySpaceStation.Json.MapDataConfig mapConfig)
//    {
//        //TODO Can valid things like map version and map format
//        //mapConfig.LevelData.TileData
//        _mapData = new ServerMapData();
//        _mapData.LoadMap(mapConfig);
//    }

//    public void ProcessMapEntities(EveryDaySpaceStation.Json.MapEntityDataConfig mapEntityConfig)
//    {
//        if (_mapData == null)
//        {
//            Debug.LogError("Can't load map entity data before map data");
//            return;
//        }

//        _mapData.LoadEntities(mapEntityConfig);
//    }
//}