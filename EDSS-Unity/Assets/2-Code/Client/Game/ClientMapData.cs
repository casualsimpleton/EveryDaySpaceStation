//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// ClientMapData - Class for holding actual, working map data both on server and client
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
    public class ClientMapData : MapData
    {
        /// <summary>
        /// Converts MapDataConfig into data for runtime usage. Returns true if things are ready to proceed
        /// </summary>
        public override bool LoadMap(MapDataConfig mapConfig)
        {
            MapName = mapConfig.LevelData.LevelName;
            DisplayName = mapConfig.LevelData.DisplayName;
            MapVersion = mapConfig.LevelData.MapVersion;
            MapFormat = mapConfig.LevelData.MapFormat;

            AmbientLightColor = mapConfig.LevelData.AmbientLightColor;
            _mapSize = mapConfig.LevelData.Size;
            _mapTiles = new MapTileData[_mapSize.x * _mapSize.y];

            int len = _mapTiles.Length;
            //for (int i = 0; i < len && i < mapConfig.LevelData.TileData.Length; i++)
            //{
            //    TileDataJson tile = mapConfig.LevelData.TileData[i];

            //    MapTileData mapTile = _mapTiles[i] = new MapTileData();

            //    mapTile.TilePosition = new Vec2Int(tile.X, tile.Y);
            //    mapTile.TileIndex = Helpers.IndexFromVec2Int(mapTile.TilePosition, _mapSize.x);
            //    mapTile.LightColor = AmbientLightColor;//new Color32(1, 1, 1, 255);

            //    //Look up the block type
            //    GameData.BlockTemplate block = null;
            //    //ClientGameManager.Singleton.Gamedata.GetGameBlock(tile.Block, out block);

            //    mapTile.BlockType = block;
            //    mapTile.BlocksLight = block.BlocksLight;

            //    //Do this after the block type since the map may force an override
            //    //Basically the override allows you to force it to do the opposite of the block's basic definition
            //    mapTile.BlocksLight = (tile.OverrideBlockLight > 0 ? !mapTile.BlocksLight : mapTile.BlocksLight);

            //    mapTile.FaceSpritesUID = new uint?[(int)GameData.BlockTemplate.BlockFaces.MAX];
            //    mapTile.UnderLayerFaceSpritesUID = new uint[(int)GameData.BlockTemplate.UnderFaces.MAX];

            //    for (int j = 0; j < mapTile.FaceSpritesUID.Length && j < tile.FaceSpriteUID.Length; j++)
            //    {
            //        mapTile.FaceSpritesUID[j] = 0;
            //        if (tile.FaceSpriteUID[j] == null)
            //        {
            //            continue;
            //        }

            //        mapTile.FaceSpritesUID[j] = tile.FaceSpriteUID[j];
            //    }

            //    mapTile.HasScaffold = false;
            //    if (tile.HasScaffold != 0)
            //    {
            //        mapTile.HasScaffold = true;

            //        //TODO Unhardcode this
            //        mapTile.UnderLayerFaceSpritesUID[(int)GameData.BlockTemplate.UnderFaces.BottomLayer] = 100;
            //    }

            //    mapTile.ScaffoldingUID = 0;
            //    mapTile.FloorSpriteUID = tile.FloorUID;
            //    mapTile.WallSpriteUID = tile.WallUID;
            //}

            return true;
        }

        public override bool LoadEntities(MapEntityDataConfig mapEntities)
        {
            _entitiesInMap = new Dictionary<uint, EntityData>();
            uint entitiesAdded = 0;
            for (int i = 0; i < mapEntities.EntityInstanceData.Length; i++)
            {
                MapEntityJson curEntry = mapEntities.EntityInstanceData[i];

                //Check bounds, and print a warning if outside bounds, but we'll skip it for now
                if (curEntry.EntityTilePos.x < 0 || curEntry.EntityTilePos.x > _mapSize.x - 1 ||
                    curEntry.EntityTilePos.y < 0 || curEntry.EntityTilePos.y > _mapSize.y - 1)
                {
                    Debug.LogWarning(string.Format("Entity entry {0} has a tile pos of {1} which is out of bounds for current map (mapsize: {2})", (i + 1), curEntry.EntityTilePos, _mapSize));
                    continue;
                }

                //ClientEntityData newEntity = new ClientEntityData(curEntry.MapEntityInstanceUID, curEntry.EntityTemplateUID, curEntry.EntityOverrideName, curEntry.Rotation, curEntry.AnchorType);

                ////Keep track of the 
                //if (newEntity.EntityUID > _currentEntityUID)
                //{
                //    _currentEntityUID = newEntity.EntityUID + 1;
                //}

                //_entitiesInMap.Add(newEntity.EntityUID, newEntity);

                ////Map tile Stuff
                //int index = Helpers.IndexFromVec2Int(curEntry.EntityTilePos, _mapSize.x);
                ////Tell map tile about the new entity
                //_mapTiles[index].AddEntityToTile(newEntity);

                //entitiesAdded++;
            }

            Debug.Log(string.Format("Added {0} entities to map", entitiesAdded));

            return true;
        }
    }

    [System.Serializable]
    public class ClientMapTileData : MapTileData
    {
    }

    //[System.Serializable]
    //public class ClientEntityData : EntityData
    //{
    //}
}