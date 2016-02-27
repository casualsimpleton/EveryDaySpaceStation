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

namespace EveryDaySpaceStation
{
    [System.Serializable]
    public class MapDataV2
    {
        #region Structs/Classes
        public struct MapBlock
        {
            public enum BlockFace
            {
                TopFace,
                BottomFace,
                ForwardFace,
                BackFace,
                RightFace,
                LeftFace,
                MAX
            }

            public enum BlockPipeFlags : byte
            {
                O2 = 0,
                N2 = 1,
                Air = 2,
                CO2 = 3,
                N2O = 4,
                Plasma = 5,
            }
            //2 Byte Ushort - Block Type UID
            //2 Byte Ushort - Top Face UID
            //2 Byte Ushort - Bottom Face UID
            //2 Byte Ushort - Forward Face UID
            //2 Byte Ushort - Back Face UID
            //2 Byte Ushort - Right Face UID
            //2 Byte Ushort - Left Face UID
            //1 Byte - Light Info
            //1 Byte - Pipe Data
                //Bit 0 - O2
                //Bit 1 - N2
                //Bit 2 - Air
                //Bit 3 - CO2
                //Bit 4 - N2O
                //Bit 5 - Plasma
                //Bit 6 - TBD
                //Bit 7 - TBD

            public ushort BlockType { get; set; }
            public ushort[] BlockFacesSpriteUIDs { get; set; }
            public byte BlockLight { get; set; }
            public byte BlockPipe { get; set; }

            //public MapBlock()
            //{
            //    Init();
            //}

            public void Init()
            {
                BlockFacesSpriteUIDs = new ushort[(int)BlockFace.MAX];
            }

            public bool HasPipe(BlockPipeFlags testFlag)
            {
                return ((BlockPipe & (1 << (int)testFlag)) != 0);
            }

            public void SetPipe(BlockPipeFlags newFlag)
            {
                BlockPipe |= (byte)(1 << (int)newFlag);
            }

            public void RemovePipe(BlockPipeFlags flag)
            {
                BlockPipe = (byte)(BlockPipe & ~(byte)flag);
            }

            public MapBlock(MapBlock blockToCopy)
            {
                BlockType = blockToCopy.BlockType;
                BlockFacesSpriteUIDs = (ushort[])blockToCopy.BlockFacesSpriteUIDs.Clone();
                BlockLight = blockToCopy.BlockLight;
                BlockPipe = blockToCopy.BlockPipe;
            }

            public void SetBlockFaceSpriteUIDs(ushort[] blockFaceUIDs)
            {
                if (BlockFacesSpriteUIDs == null)
                {
                    BlockFacesSpriteUIDs = new ushort[blockFaceUIDs.Length];
                }

                for (int i = 0; i < BlockFacesSpriteUIDs.Length; i++)
                {
                    BlockFacesSpriteUIDs[i] = blockFaceUIDs[i];
                }
            }
        }

        public class MapRegion
        {
            public ushort RegionUID { get; set; }
            /// <summary>
            /// Region size in blocks
            /// </summary>
            public Vec3Int RegionSize { get; set; }

            public MapBlock[, ,] RegionBlocks { get; set; }
        }

        #endregion

        #region Vars
        public string MapName { get; set; }
        public ushort MapVersion { get; set; }
        public List<MapRegion> MapRegions { get; set; }
        #endregion

        public MapDataV2()
        {
            MapRegions = new List<MapRegion>();
        }
    }

    [System.Serializable]
    public abstract class MapData
    {
        #region Classes        

        #endregion

        #region Vars
        public string MapName { get; set; }
        public string DisplayName { get; set; }
        public string MapVersion { get; set; }
        public int MapFormat { get; set; }
        protected Color32 _ambientLight = new Color32(10, 10, 10, 255);
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

        protected Dictionary<uint, EntityData> _entitiesInMap;
        protected uint _currentEntityUID;

        public uint GetNewEntityUID()
        {
            return _currentEntityUID++;
        }
        #endregion

        public abstract bool LoadMap(MapDataConfig mapConfig);
        public abstract bool LoadEntities(MapEntityDataConfig mapEntities);
        

        

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

    /// <summary>
    /// Represents the playable area
    /// </summary>
    [System.Serializable]
    public abstract class MapTileData
    {
        public Vec2Int TilePosition { get; set; }
        public int TileIndex { get; set; }
        public bool BlocksLight { get; set; }
        public bool IsTransparent { get; set; }
        public GameData.BlockTemplate BlockType { get; set; }
        public Color32 LightColor { get; set; }

        public uint?[] FaceSpritesUID { get; set; }
        public ushort[] UnderLayerFaceSpritesUID { get; set; }

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
                    //EntityBuildManager.Singleton.AddEntityToBuildQueue(_presentEntities[i]);
                }
            }
            else
            {
                for (int i = 0; i < _presentEntities.Count; i++)
                {
                    //EntityBuildManager.Singleton.AddEntityToDeconstruction(_presentEntities[i]);
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
            return Vector3.zero;
            //return new Vector3(TilePosition.x * SceneChunk.blockSize, 0, TilePosition.y * SceneChunk.blockSize);
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
    public abstract class EntityData
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
            protected GameData.EntityStateTemplate _entityState;

            protected float _timeAtStateSwitch;

            protected EntityData _associatedEntity;

            public GameData.EntityStateTemplate StateTemplate { get { return _entityState; } }
            public float TimeAtStateSwitch { get { return _timeAtStateSwitch; } }
            public EntityData Entity { get { return _associatedEntity; } }

            public EntityState(EntityData entity, ushort entityStateUID)
            {
                //GameData.EntityDataTemplate template = null;
                //bool foundTemplate = ClientGameManager.Singleton.Gamedata.GetEntityTemplate(entity.TemplateUID, out template);

                //if (!foundTemplate)
                //{
                //    Debug.LogWarning(string.Format("Unable to find entity template '{0}' for EntityData.EntityState()", entity.TemplateUID));
                //    return;
                //}

                //bool foundStateTemplate = template.GetEntityStateTemplate(entityStateUID, out _entityState);

                //if (!foundStateTemplate)
                //{
                //    Debug.LogWarning(string.Format("Unable to find entity state template '{0}' for EntityData.EntityState()", entityStateUID));
                //    return;
                //}

                //ChangeState(_entityState);
            }

            public void ChangeState(GameData.EntityStateTemplate newState)
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
            //EntityUID = UID;
            //TemplateUID = templateUID;
            //_rotation = rotation;
            //_template = null;
            //ClientGameManager.Singleton.Gamedata.GetEntityTemplate(templateUID, out _template);
            //if (string.IsNullOrEmpty(entityName))
            //{
            //    EntityName = _template.Name;
            //}
            //else
            //{
            //    EntityName = entityName;
            //}

            ////CS - 12/13/2015 - I know this isn't ideal, but it prevents having to store temporary variables and stuff
            //BuildState = new EntityBuildManager.BuildConstructionQueueHelper();

            //anchorTypeStr = anchorTypeStr.ToLower();
            //_anchorType = EntityWallAnchor.None;
            //switch (anchorTypeStr)
            //{
            //    case "top":
            //    case "ceiling":
            //        _anchorType = EntityWallAnchor.Ceiling;
            //        break;

            //    case "bottom":
            //    case "floor":
            //        _anchorType = EntityWallAnchor.Floor;
            //        break;

            //    case "front":
            //    case "forward":
            //    case "z+":
            //    case "zplus":
            //        _anchorType = EntityWallAnchor.Front;
            //        break;

            //    case "back":
            //    case "rear":
            //    case "z-":
            //    case "zminus":
            //        _anchorType = EntityWallAnchor.Back;
            //        break;

            //    case "right":
            //    case "x+":
            //    case "xplus":
            //        _anchorType = EntityWallAnchor.Right;
            //        break;

            //    case "left":
            //    case "x-":
            //    case "xminus":
            //        _anchorType = EntityWallAnchor.Left;
            //        break;

            //    default:
            //        _anchorType = EntityWallAnchor.None;
            //        break;
            //}

            //_currentEntityState = new EntityState(this, 0);
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
}