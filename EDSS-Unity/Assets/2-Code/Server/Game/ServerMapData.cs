//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// ServerMapData - Class for holding actual, working map data both on server and client
// Created: December 5 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 5 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;
using EveryDaySpaceStation.Json;

namespace EveryDaySpaceStation
{
    [System.Serializable]
    public class ServerMapData : MapData
    {
        public override bool LoadMap(MapDataConfig mapConfig)
        {
            return true;
        }

        public override bool LoadEntities(MapEntityDataConfig mapEntities)
        {
            return true;
        }
    }

    [System.Serializable]
    public class ServerMapTileData : MapTileData
    {
    }

    //[System.Serializable]
    //public class ServerEntityData : EntityData
    //{
    //}
}