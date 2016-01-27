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

    #region Enums
        
    #endregion

    #region Vars
        
    #endregion
}