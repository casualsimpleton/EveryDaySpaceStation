//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// GameManagerAbstract - Base class used for ClientGameManager and ServerGameManager
// Created: January 27 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 27 2016
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

public abstract class GameManagerAbstract : MonoBehaviour
{
    #region Enums

    #endregion

    #region Vars
    [SerializeField]
    protected GameData _gameData;
    [SerializeField]
    protected MapData _mapData;

    protected System.Diagnostics.Stopwatch _timer = new System.Diagnostics.Stopwatch();

    protected EntityBuildManager _entitybuildManager;

    #endregion

    #region Gets/Sets
    public EntityBuildManager EntityBuildManager
    {
        get { return _entitybuildManager; }
    }
    #endregion

    public virtual void Init()
    {
    }

    protected virtual void Start()
    {
        _entitybuildManager = this.gameObject.AddComponent<EntityBuildManager>();
    }
}