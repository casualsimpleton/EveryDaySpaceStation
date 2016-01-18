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

[System.Serializable]
public sealed class MapData
{
    #region Classes
    /// <summary>
    /// Represents the playable area
    /// </summary>
    [System.Serializable]
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

        [SerializeField]
        protected bool _isVisible;
        public bool IsVisible { get { return _isVisible; } }
        public void SetVisible(bool isVisible)
        {
            _isVisible = isVisible;

            if (isVisible)
            {
                for (int i = 0; i < _presentEntities.Count; i++)
                {
                    EntityBuildManager.Singleton.AddEntityToBuildQueue(_presentEntities[i]);
                }
            }
            else
            {
                for(int i = 0; i < _presentEntities.Count; i++)
                {
                    EntityBuildManager.Singleton.AddEntityToDeconstruction(_presentEntities[i]);
                }
            }
        }

        [SerializeField]
        private List<EntityData> _presentEntities;

        public MapTileData()
        {
            _presentEntities = new List<EntityData>();
        }

        public void AddEntityToTile(EntityData entity)
        {
            _presentEntities.Add(entity);

            //Tell the entity it belongs to this tile now
            entity.ChangeTile(this);
        }

        public void RemoveEntityToTile(EntityData entity)
        {
            _presentEntities.Remove(entity);
        }

        /// <summary>
        /// Attempt to remove an entity from tile by UID incase that's necessary
        /// </summary>
        /// <param name="entityUID"></param>
        public void RemoveEntityToTile(uint entityUID)
        {
            for (int i = 0; i < _presentEntities.Count; i++)
            {
                if (entityUID == _presentEntities[i].EntityUID)
                {
                    _presentEntities.RemoveAt(i);
                    return;
                }
            }
        }

        public Vector3 GetWorldPosition()
        {
            return new Vector3(TilePosition.x * SceneChunk.blockSize, 0, TilePosition.y * SceneChunk.blockSize);
        }

        //public override string ToString()
        //{
        //    return string.Format("Pos: {0} Index: {1} BlocksLight: {2} IsTransparent: {3} ScaffoldingUID: {4} FloorSpriteUID: {5} WallSpriteUID: {6}",
        //        TilePosition, TileIndex, BlocksLight, IsTransparent, BlockType, ScaffoldingUID, FloorSpriteUID, WallSpriteUID);
        //}
    }

    /// <summary>
    /// Basic class for EntityData as used by both server and client for doing all the various entity logic. 
    /// Contains links to entity templates, sprites and other necessary things
    /// </summary>
    [System.Serializable]
    public class EntityData
    {
        #region Enums
        public enum EntityWallAnchor
        {
            None,
            Front, //Z+
            Right, //X+
            Back, //Z-
            Left, //X-
            Ceiling,
            Floor
        }
        #endregion
        #region Classes
        public class EntityState
        {
            public delegate void OnStateSwitch(EntityState entityState, EntityData entity);

            /// <summary>
            /// A reference to the template data. We don't need an instance of this since it'll be the same no matter what
            /// This will also change 
            /// </summary>
            protected GameData.EntityDataTemplate.StateTemplate _entityState;

            protected float _timeAtStateSwitch;

            protected EntityData _associatedEntity;

            public GameData.EntityDataTemplate.StateTemplate StateTemplate { get { return _entityState; } }
            public float TimeAtStateSwitch { get { return _timeAtStateSwitch; } }
            public EntityData Entity { get { return _associatedEntity; } }

            public EntityState(EntityData entity, ushort entityStateUID)
            {
                GameData.EntityDataTemplate template = null;
                bool foundTemplate = GameManager.Singleton.Gamedata.GetEntityTemplate(entity.TemplateUID, out template);

                if (!foundTemplate)
                {
                    Debug.LogWarning(string.Format("Unable to find entity template '{0}' for EntityData.EntityState()", entity.TemplateUID));
                    return;
                }

                bool foundStateTemplate = template.GetEntityStateTemplate(entityStateUID, out _entityState);

                if (!foundStateTemplate)
                {
                    Debug.LogWarning(string.Format("Unable to find entity state template '{0}' for EntityData.EntityState()", entityStateUID));
                    return;
                }

                ChangeState(_entityState);
            }

            public void ChangeState(GameData.EntityDataTemplate.StateTemplate newState)
            {
                _timeAtStateSwitch = Time.time;

                _entityState = newState;
            }
        }
        #endregion
        [SerializeField]
        protected GameData.EntityDataTemplate _template;
        [SerializeField]
        protected uint _entityUID;
        [SerializeField]
        protected uint _templateUID;
        [SerializeField]
        protected string _entityName;
        [SerializeField]
        protected Vector3 _rotation;
        [SerializeField]
        protected ushort _curStateUID;
        [SerializeField]
        protected EntityState _currentEntityState;
        [SerializeField]
        protected EntitySpriteGameObject _entitySprite;
        [System.NonSerialized] 
        protected MapTileData _associatedMapTile;
        public EntityBuildManager.BuildConstructionQueueHelper BuildState;
        [SerializeField]
        protected EntityWallAnchor _anchorType;

        public GameData.EntityDataTemplate Template
        {
            get { return _template; }
            private set { _template = value; }
        }
        public uint EntityUID
        {
            get { return _entityUID; }
            private set { _entityUID = value; }
        }
        public uint TemplateUID
        {
            get { return _templateUID; }
            private set { _templateUID = value; }
        }
        public string EntityName
        {
            get { return _entityName; }
            private set { _entityName = value; }
        }
        public Vector3 Rotation
        {
            get { return _rotation; }
            private set { _rotation = value; }
        }
        public ushort CurrentStateUID
        {
            get { return _curStateUID; }
            private set { _curStateUID = value; }
        }

        public EntitySpriteGameObject Sprite
        {
            get { return _entitySprite; }
        }

        public bool IsVisible
        {
            get { return _associatedMapTile.IsVisible; }
        }

        public EntityState CurrentEntityState { get { return _currentEntityState; } }

        public MapTileData MapTile { get { return _associatedMapTile; } }

        public EntityWallAnchor WallAnchor { get { return _anchorType; } }

        public EntityData(uint UID, uint templateUID, string entityName, Vector3 rotation, string anchorTypeStr)
        {
            EntityUID = UID;
            TemplateUID = templateUID;
            _rotation = rotation;
            _template = null;
            GameManager.Singleton.Gamedata.GetEntityTemplate(templateUID, out _template);
            if (string.IsNullOrEmpty(entityName))
            {
                EntityName = _template.Name;
            }
            else
            {
                EntityName = entityName;
            }

            //CS - 12/13/2015 - I know this isn't ideal, but it prevents having to store temporary variables and stuff
            BuildState = new EntityBuildManager.BuildConstructionQueueHelper();

            anchorTypeStr = anchorTypeStr.ToLower();
            _anchorType = EntityWallAnchor.None;
            switch(anchorTypeStr)
            {
                case "top":
                case "ceiling":
                    _anchorType = EntityWallAnchor.Ceiling;
                    break;

                case "bottom":
                case "floor":
                    _anchorType = EntityWallAnchor.Floor;
                    break;

                case "front":
                case "forward":
                case "z+":
                case "zplus":
                    _anchorType = EntityWallAnchor.Front;
                    break;

                case "back":
                case "rear":
                case "z-":
                case "zminus":
                    _anchorType = EntityWallAnchor.Back;
                    break;

                case "right":
                case "x+":
                case "xplus":
                    _anchorType = EntityWallAnchor.Right;
                    break;

                case "left":
                case "x-":
                case "xminus":
                    _anchorType = EntityWallAnchor.Left;
                    break;

                default:
                    _anchorType = EntityWallAnchor.None;
                    break;
            }

            _currentEntityState = new EntityState(this, 0);
        }

        public void ChangeState(ushort newStateUID)
        {
            CurrentStateUID = newStateUID;
        }

        public void ChangeTile(MapTileData mapTile)
        {
            _associatedMapTile = mapTile;
        }

        public void AssignEntityGameObject(EntitySpriteGameObject sprite)
        {
            _entitySprite = sprite;
        }
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
    private Color32 _ambientLight = new Color32(10,10,10,255);
    public Color32 AmbientLightColor 
    { 
        get { return _ambientLight; }
        set { _ambientLight = value; }
    }
    public Vec2Int _mapSize;
    public MapTileData[] _mapTiles;
    //public AtmosphereTileData[] _atmosTiles;
    //public LargeTubeTileData[] _largeTubeTiles;
    //public ThinTubeTileData[] _thinTubeTiles;
    //public WireTileData[] _wireTubeTiles;

    private Dictionary<uint, EntityData> _entitiesInMap;
    private uint _currentEntityUID;

    public uint GetNewEntityUID()
    {
        return _currentEntityUID++;
    }
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

        AmbientLightColor = mapConfig.LevelData.AmbientLightColor;
        _mapSize = mapConfig.LevelData.Size;
        _mapTiles = new MapTileData[_mapSize.x * _mapSize.y];

        int len = _mapTiles.Length;
        for (int i = 0; i < len && i < mapConfig.LevelData.TileData.Length; i++)
        {
            TileDataJson tile = mapConfig.LevelData.TileData[i];

            MapTileData mapTile = _mapTiles[i] = new MapTileData();

            mapTile.TilePosition = new Vec2Int(tile.X, tile.Y);
            mapTile.TileIndex = Helpers.IndexFromVec2Int(mapTile.TilePosition, _mapSize.x);
            mapTile.LightColor = AmbientLightColor;//new Color32(1, 1, 1, 255);
            
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

                //TODO Unhardcode this
                mapTile.UnderLayerFaceSpritesUID[(int)GameData.GameBlockData.UnderFaces.BottomLayer] = 100;
            }

            mapTile.ScaffoldingUID = 0;
            mapTile.FloorSpriteUID = tile.FloorUID;
            mapTile.WallSpriteUID = tile.WallUID;
        }

        return true;
    }

    public bool LoadEntities(MapEntityDataConfig mapEntities)
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

            EntityData newEntity = new EntityData(curEntry.MapEntityInstanceUID, curEntry.EntityTemplateUID, curEntry.EntityOverrideName, curEntry.Rotation, curEntry.AnchorType);

            //Keep track of the 
            if (newEntity.EntityUID > _currentEntityUID)
            {
                _currentEntityUID = newEntity.EntityUID + 1;
            }

            _entitiesInMap.Add(newEntity.EntityUID, newEntity);

            //Map tile Stuff
            int index = Helpers.IndexFromVec2Int(curEntry.EntityTilePos, _mapSize.x);
            //Tell map tile about the new entity
            _mapTiles[index].AddEntityToTile(newEntity);

            entitiesAdded++;
        }

        Debug.Log(string.Format("Added {0} entities to map", entitiesAdded));

        return true;
    }

    public void Cleanup()
    {
        //foreach (KeyValuePair<uint, EntityData> entity in _entitiesInMap)
        //{
        //    //TODO Depending on how EntityData evolves
        //}

        _entitiesInMap.Clear();
        _entitiesInMap = null;
    }
}