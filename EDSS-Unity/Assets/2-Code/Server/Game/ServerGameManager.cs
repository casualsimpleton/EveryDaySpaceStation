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

public class ServerGameManager : MonoBehaviour
{
    #region Singleton
    protected static ServerGameManager m_singleton = null;
    public static ServerGameManager Singleton
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