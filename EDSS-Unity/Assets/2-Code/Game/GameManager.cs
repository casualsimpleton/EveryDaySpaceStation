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

    #region Vars
    #endregion

    public void Start()
    {
        FileSystem.Init();
        SceneLevelManager.Singleton.Init();
    }
}