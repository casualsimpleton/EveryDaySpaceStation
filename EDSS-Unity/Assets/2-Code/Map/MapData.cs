//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// MapData - Class for holding actual, working map data both on server and client
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

public sealed class MapData
{
    #region Classes
    /// <summary>
    /// Represents the playable area
    /// </summary>
    public class MapTileData
    {
        public Vec2Int TilePosition { get; set; }
        public int TileIndex { get; set; }
        public bool BlocksLight { get; set; }
        public bool IsTransparent { get; set; }
        public GameData.GameBlockData BlockType { get; set; }
        public Color32 LightColor { get; set; }

        public uint?[] FaceSpritesUID { get; set; }
        public uint[] UnderLayerFaceSpritesUID { get; set; }

        public bool HasScaffold { get; set; }
        public uint ScaffoldingUID { get; set; }
        public uint FloorSpriteUID { get; set; }
        public uint WallSpriteUID { get; set; }

        //public override string ToString()
        //{
        //    return string.Format("Pos: {0} Index: {1} BlocksLight: {2} IsTransparent: {3} ScaffoldingUID: {4} FloorSpriteUID: {5} WallSpriteUID: {6}",
        //        TilePosition, TileIndex, BlocksLight, IsTransparent, BlockType, ScaffoldingUID, FloorSpriteUID, WallSpriteUID);
        //}
    }

    /// <summary>
    /// Represents the atmosphere data for a tile
    /// </summary>
    public class AtmosphereTileData
    {
    }

    /// <summary>
    /// Represents the large tube data for a tile
    /// </summary>
    public class LargeTubeTileData
    {
    }

    /// <summary>
    /// Represents the thin tube data for a tile
    /// </summary>
    public class ThinTubeTileData
    {
    }

    /// <summary>
    /// Represents the wire data for a tile
    /// </summary>
    public class WireTileData
    {
    }
    #endregion

    #region Vars
    public string MapName { get; set; }
    public string DisplayName { get; set; }
    public string MapVersion { get; set; }
    public int MapFormat { get; set; }
    public Vec2Int _mapSize;
    public MapTileData[] _mapTiles;
    //public AtmosphereTileData[] _atmosTiles;
    //public LargeTubeTileData[] _largeTubeTiles;
    //public ThinTubeTileData[] _thinTubeTiles;
    //public WireTileData[] _wireTubeTiles;
    #endregion

    /// <summary>
    /// Converts MapDataConfig into data for runtime usage. Returns true if things are ready to proceed
    /// </summary>
    public bool LoadMap(MapDataConfig mapConfig)
    {
        MapName = mapConfig.LevelData.LevelName;
        DisplayName = mapConfig.LevelData.DisplayName;
        MapVersion = mapConfig.LevelData.MapVersion;
        MapFormat = mapConfig.LevelData.MapFormat;

        _mapSize = mapConfig.LevelData.Size;
        _mapTiles = new MapTileData[_mapSize.x * _mapSize.y];

        int len = _mapTiles.Length;
        for (int i = 0; i < len && i < mapConfig.LevelData.TileData.Length; i++)
        {
            TileDataJson tile = mapConfig.LevelData.TileData[i];

            MapTileData mapTile = _mapTiles[i] = new MapTileData();

            mapTile.TilePosition = new Vec2Int(tile.X, tile.Y);
            mapTile.TileIndex = Helpers.IndexFromVec2Int(mapTile.TilePosition, _mapSize.x);
            mapTile.LightColor = new Color32(1, 1, 1, 255);
            
            //Look up the block type
            GameData.GameBlockData block = null;
            GameManager.Singleton.Gamedata.GetGameBlock(tile.Block, out block);

            mapTile.BlockType = block;
            mapTile.BlocksLight = block.BlocksLight;

            //Do this after the block type since the map may force an override
            //Basically the override allows you to force it to do the opposite of the block's basic definition
            mapTile.BlocksLight = (tile.OverrideBlockLight > 0 ? !mapTile.BlocksLight : mapTile.BlocksLight);

            mapTile.FaceSpritesUID = new uint?[(int)GameData.GameBlockData.BlockFaces.MAX];
            mapTile.UnderLayerFaceSpritesUID = new uint[(int)GameData.GameBlockData.UnderFaces.MAX];

            for(int j = 0; j < mapTile.FaceSpritesUID.Length && j < tile.FaceSpriteUID.Length; j++)
            {
                mapTile.FaceSpritesUID[j] = 0;
                if (tile.FaceSpriteUID[j] == null)
                {
                    continue;
                }

                mapTile.FaceSpritesUID[j] = tile.FaceSpriteUID[j];
            }

            mapTile.HasScaffold = false;
            if (tile.HasScaffold != 0)
            {
                mapTile.HasScaffold = true;
                mapTile.UnderLayerFaceSpritesUID[(int)GameData.GameBlockData.UnderFaces.BottomLayer] = 500;
            }

            mapTile.ScaffoldingUID = 0;
            mapTile.FloorSpriteUID = tile.FloorUID;
            mapTile.WallSpriteUID = tile.WallUID;
        }

        return true;
    }
}