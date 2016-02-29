//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// ClientGameManager - Handles client considerations for the game
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

//public class ClientGameManager : GameManagerAbstract
//{
//    #region Singleton
//    protected static ClientGameManager m_singleton = null;
//    public static ClientGameManager Singleton
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
//    //public EDSSFirstPersonControls playerControl;
//    //public EDSSFirstPersonCamera playerCamera;
//    public static int ClientWorldLayer;
//    public static int ClientEntityLayer;
//    public static int ClientTriggerLayer;
//    public static Color32 HighlightColor = new Color32(180, 255, 180, 255);
//    #endregion

//    #region Gets/Sets

//    #endregion

//    public override void Init()
//    {
//        base.Init();
//    }

//    protected override void Start()
//    {
//        base.Start();

//        ClientWorldLayer = LayerMask.NameToLayer("ClientWorld");
//        ClientEntityLayer = LayerMask.NameToLayer("ClientEntity");
//        ClientTriggerLayer = LayerMask.NameToLayer("ClientTrigger");

//        _timer.Start();
//        //_gameData = new GameData();
//        //FileSystem.Init();
//        //PoolManager.Singleton.Init();
//        ////SceneLevelManager.Singleton.Init();
//        //PoolManager.Singleton.LateInit();
//        ////SceneLevelManager.Singleton.PopulateMapTiles(_mapData);
//        _timer.Stop();

//        Debug.Log(string.Format("Load dur: {0}ms", _timer.ElapsedMilliseconds));

        
//    }
//}