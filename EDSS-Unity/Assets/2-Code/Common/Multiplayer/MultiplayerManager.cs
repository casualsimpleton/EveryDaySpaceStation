//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// MultiplayerManager - Works with the network layer and game
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

namespace EveryDaySpaceStation.Network
{
    public static class MultiplayerManager
    {
        #region Enums
        public enum MultiplayerType
        {
            Client,
            ListenServer,
            DedicatedServer
        }
        #endregion

        #region Vars
        static MultiplayerType _multiplayerGameType;
        #endregion
    }
}