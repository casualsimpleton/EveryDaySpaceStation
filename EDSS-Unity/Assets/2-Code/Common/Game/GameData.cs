//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// GameData - Class for containing all the various data imported from the jsons pertaining to game data, like blocks and sprites
// Created: December 5 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 5 2015
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

namespace EveryDaySpaceStation
{
    [System.Serializable]
    public sealed class GameData
    {
        #region Classes/Structs/Enums
        [System.Flags]
        public enum EntityFlags
        {
            None,
            Light,
            Fixed,
            Powered,
            Device,
            Craftable,
            Multiangle,
            Container
        }

        public class EntityStateTemplate
        {
            public ushort StateUID { get; private set; }
            public string StateName { get; private set; }
            public ushort SpriteUID { get; private set; }
            public Vector3 StateGraphicsSize { get; private set; }
            public Vector3 StateColliderSize { get; private set; }
            public Vector3 StatePositionOffset { get; private set; }

            public EntityStateTemplate(ushort uid, string name, ushort spriteUID, Vector3 graphicsSize, Vector3 colliderSize, Vector3 offset)
            {
                StateUID = uid;
                StateName = name;
                SpriteUID = spriteUID;

                if (graphicsSize == Vector3.zero)
                {
                    graphicsSize = Vector3.one;
                }

                StateGraphicsSize = graphicsSize;

                if (colliderSize == Vector3.zero)
                {
                    colliderSize = Vector3.one;
                }

                StateColliderSize = colliderSize;
                StatePositionOffset = offset;
            }
        }

        public class LightStateTemplate
        {
        }

        public class EntityDataTemplate
        {
            #region Vars
            public uint UID { get; set; }
            public string Name { get; set; }
            public EntityFlags[] EntityFlags { get; private set; }

            public Dictionary<ushort, EntityStateTemplate> EntityStateTemplates { get; private set; }
            public Dictionary<ushort, LightStateTemplate> LightStateTemplates { get; private set; }
            #endregion

            #region Accessors
            public bool GetEntityStateTemplate(ushort stateUID, out EntityStateTemplate template)
            {
                template = null;
                return EntityStateTemplates.TryGetValue(stateUID, out template);
            }

            public bool GetLightStateTemplate(ushort stateUID, out LightStateTemplate template)
            {
                template = null;
                return LightStateTemplates.TryGetValue(stateUID, out template);
            }
            #endregion

            #region Constructor
            public EntityDataTemplate(uint entityTemplateUID, string name, string[] entityFlags, EveryDaySpaceStation.Json.EntityStateDataJson[] stateJsons)
            {
                UID = entityTemplateUID;
                Name = name;
                //EntityFlags = entityFlags;

                EntityStateTemplates = new Dictionary<ushort, EntityStateTemplate>();

                //Process these first as this determines a bunch of other things as the number of other states should not exceed this count
                for(int i = 0; i < stateJsons.Length; i++)
                {
                    EveryDaySpaceStation.Json.EntityStateDataJson curState = stateJsons[i];
                    EntityStateTemplate newState = new EntityStateTemplate(curState.StateUID, curState.StateName, curState.SpriteUID, curState.DisplaySize, curState.ColliderSize, curState.PositionOffset);

                    EntityStateTemplates.Add(newState.StateUID, newState);
                }

                if (entityFlags == null)
                {
                    Debug.LogError("No type flags provided. What to do?");
                    return;
                }

                EntityFlags = new EntityFlags[entityFlags.Length];

                for (int i = 0; i < EntityFlags.Length; i++)
                {
                    string curType = entityFlags[i].ToLower();

                    EntityFlags curFlag = (EntityFlags)Enum.Parse(typeof(EntityFlags), curType);

                    EntityFlags[i] = curFlag;

                    //TODO Keep this update
                    switch (curFlag)
                    {
                        case GameData.EntityFlags.Light:
                            LightStateTemplates = new Dictionary<ushort, LightStateTemplate>();
                            break;

                        default:
                            Debug.LogWarning(string.Format("Unsupport EntityFlag type: {0}", curFlag.ToString()));
                            break;
                    }
                }
            }
            #endregion

            #region Cleanup
            public void Cleanup()
            {
                if (EntityStateTemplates != null)
                {
                    EntityStateTemplates.Clear();
                }
                EntityStateTemplates = null;

                //if (LightStates != null)
                //{
                //    LightStates.Clear();
                //}
                //LightStates = null;

                //if (FixedStates != null)
                //{
                //    foreach (KeyValuePair<ushort, FixedStateTemplate> t in FixedStates)
                //    {
                //        t.Value.Clear();
                //    }

                //    FixedStates.Clear();
                //}
                //FixedStates = null;

                //if (PoweredStates != null)
                //{
                //    PoweredStates.Clear();
                //}
                //PoweredStates = null;

                //if (DeviceStates != null)
                //{
                //    DeviceStates.Clear();
                //}
                //DeviceStates = null;

                //if (CraftStates != null)
                //{
                //    foreach (KeyValuePair<ushort, CraftStateTemplate> t in CraftStates)
                //    {
                //        t.Value.Clear();
                //    }
                //    CraftStates.Clear();
                //}
                //CraftStates = null;

                //if (DoorState != null)
                //{
                //    DoorState.Cleanup();
                //}
                //DoorState = null;

                //if (ContainerState != null)
                //{
                //    ContainerState.Cleanup();
                //}
                //ContainerState = null;
            }
            #endregion

            public override string ToString()
            {
                return string.Format("Entity Data Template: {0} UID: {1}", Name, UID);
            }
        }

        public class BlockTemplate
        {
            #region Classes/Enums/Structs
            public struct FaceInfo
            {
                public enum FaceDirection : byte
                {
                    Forward,
                    Inverted
                }

                public FaceDirection _faceDir; //Whether the face is point out or in
                public bool _visible;

                public override string ToString()
                {
                    return string.Format("{0} {1}", _faceDir, _visible);
                }
            }

            public enum BlockFaces : byte
            {
                FaceZForward,
                FaceXForward,
                FaceZBack,
                FaceXBack,
                FaceTop,
                FaceBottom,

                //..
                MAX
            }

            public enum UnderFaces : byte
            {
                BottomLayer,
                LargePipeLayer,
                ThinPipeLayer,
                WireLayer,
                //..
                MAX
            }

            [System.Flags]
            public enum BlockFlags
            {
                Empty,
                Vacuum,
                BlocksLight,
                Door,
                Porous,
                Transparent
            }
            #endregion

            #region Vars
            public ushort UID { get; private set; }
            public string Name { get; private set; }
            public int DefaultBlockStrength { get; private set; }
            public List<BlockFlags> Flags { get; private set; }
            public bool BlocksLight { get; private set; }
            public bool IsVacuum { get; private set; }
            public bool IsPorous { get; private set; }
            public bool IsEmpty { get; private set; }
            /// <summary>
            /// The UID for the type of block that must be present in order for this block to be placed. Not required for mapping, 
            /// but will be used during run-time for dynamic building
            /// </summary>
            public ushort RequirementUID { get; private set; }

            public FaceInfo[] Faceinfo;
            #endregion

            #region Constructor
            public BlockTemplate(ushort uid, string name, int defaultStrength, string[] flags, ushort requirementUID)
            {
                UID = uid;
                Name = name;
                DefaultBlockStrength = defaultStrength;
                RequirementUID = requirementUID;
                Flags = new List<BlockFlags>(flags.Length);

                ParseFlags(flags);
            }

            public void SetFaceParameters(params int[] FaceValues)
            {
                int goalCount = (int)BlockFaces.MAX;
                if (Faceinfo == null)
                {
                    Faceinfo = new FaceInfo[goalCount];
                }

                if (FaceValues == null)
                {
                    Debug.LogError(string.Format("Face parameters shouldn't be null. GameBlockData: {0}", ToString()));
                    return;
                }

                if (FaceValues.Length != goalCount)
                {
                    Debug.LogWarning(string.Format("Number of inputted face parameters ({0}) does not match the desired amount {1} for GameBlockData: {2}", FaceValues.Length, goalCount, ToString()));
                }

                for (int i = 0; i < goalCount && i < FaceValues.Length; i++)
                {
                    FaceInfo curFace = new FaceInfo();

                    //Not visible
                    if (FaceValues[i] == 0)
                    {
                        curFace._faceDir = FaceInfo.FaceDirection.Forward;
                        curFace._visible = false;
                    }
                    //Face present, but inverted
                    else if (FaceValues[i] == 2)
                    {
                        curFace._faceDir = FaceInfo.FaceDirection.Inverted;
                        curFace._visible = true;
                    }
                    //Catch all for now
                    else
                    {
                        curFace._faceDir = FaceInfo.FaceDirection.Forward;
                        curFace._visible = true;
                    }

                    Faceinfo[i] = curFace;
                }
            }

            private void ParseFlags(string[] flags)
            {
                for (int i = 0; i < flags.Length; i++)
                {
                    BlockFlags flag = (BlockFlags)Enum.Parse(typeof(BlockFlags), flags[i]);

                    Flags.Add(flag);

                    switch (flag)
                    {
                        case BlockFlags.Empty:
                            IsEmpty = true;
                            continue;

                        case BlockFlags.Vacuum:
                            IsVacuum = true;
                            continue;

                        case BlockFlags.BlocksLight:
                            BlocksLight = true;
                            continue;

                        case BlockFlags.Door:
                            //TODO
                            continue;

                        case BlockFlags.Porous:
                            //TODO
                            continue;

                        case BlockFlags.Transparent:
                            BlocksLight = false;
                            continue;
                    }
                }
            }
            #endregion

            public override string ToString()
            {
                //Not the most efficient, but it works
                string flagsTxt = "";

                for (int i = 0; i < flagsTxt.Length; i++)
                {
                    flagsTxt += string.Format("'{0}|'", flagsTxt[i]);
                }

                return string.Format("UID: {0} Name: {1} DefaultBlockStrength: {2} RequirementUID: {3} Flags: {4}", UID, Name, DefaultBlockStrength, RequirementUID, flagsTxt);
            }
        }
        #endregion

        #region Vars
        //Dictionary<uint, EDSSSprite> _edssSprites;
        //Dictionary<uint, EDSSSpriteSheet> _edssSpriteSheets;
        //Dictionary<uint, BlockTemplate> _blockTemplates;
        //Dictionary<string, Texture2D> _textures;
        //Dictionary<uint, Material> _materials;
        //Dictionary<uint, EntityDataTemplate> _entityDataTemplates;

        //public static EDSSSprite DefaultSprite;

        private ushort _spriteSheetUID = 1;
        private ushort _materialUID = 1;
        #endregion

        #region Gets/Sets
        //public uint GetNewSpriteSheetUID() { return _spriteSheetUID++; }
        //public uint GetNewMaterilUID() { return _materialUID++; }

        //public void AddSprite(uint uid, EDSSSprite sprite)
        //{
        //    _edssSprites.Add(uid, sprite);
        //}

        //public void AddSpriteSheet(uint uid, EDSSSpriteSheet spriteSheet)
        //{
        //    _edssSpriteSheets.Add(uid, spriteSheet);
        //}

        //public void AddGameBlock(uint uid, BlockTemplate blockData)
        //{
        //    _blockTemplates.Add(uid, blockData);
        //}

        //public void AddEntityTemplate(uint uid, EntityDataTemplate entityData)
        //{
        //    _entityDataTemplates.Add(uid, entityData);
        //}

        //public void AddTexture(string name, Texture2D texture)
        //{
        //    _textures.Add(name, texture);
        //}

        //public void AddMaterial(uint uid, Material material)
        //{
        //    _materials.Add(uid, material);
        //}

        //public bool GetSprite(uint uid, out EDSSSprite sprite)
        //{
        //    bool exists = _edssSprites.TryGetValue(uid, out sprite);

        //    //Return the default sprite
        //    if (!exists)
        //    {
        //        sprite = GameData.DefaultSprite;
        //    }

        //    return exists;
        //}

        //public bool GetSpriteSheet(uint uid, out EDSSSpriteSheet spriteSheet)
        //{
        //    bool exists = _edssSpriteSheets.TryGetValue(uid, out spriteSheet);

        //    return exists;
        //}

        ///// <summary>
        ///// Look for a EDSSSpriteSheet by texture name, since we might not have the UID yet. NOTE - Going to be slower that searching by UID
        ///// </summary>
        //public bool GetSpriteSheet(string name, out EDSSSpriteSheet spriteSheet)
        //{
        //    spriteSheet = null;
        //    foreach (KeyValuePair<uint, EDSSSpriteSheet> sheet in _edssSpriteSheets)
        //    {
        //        if (sheet.Value.Material.name.CompareTo(name) == 0)
        //        {
        //            spriteSheet = sheet.Value;
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //public bool GetGameBlock(uint uid, out BlockTemplate blockData)
        //{
        //    bool exists = _blockTemplates.TryGetValue(uid, out blockData);

        //    return exists;
        //}

        //public bool GetEntityTemplate(uint uid, out EntityDataTemplate template)
        //{
        //    bool exists = _entityDataTemplates.TryGetValue(uid, out template);
        //    return exists;
        //}

        //public bool GetTexture(string name, out Texture2D texture)
        //{
        //    bool exists = _textures.TryGetValue(name, out texture);

        //    //Can't find texture, so return default
        //    if (!exists)
        //    {
        //        texture = _textures[DefaultSprite.SpriteSheet.Texture.name];
        //    }

        //    return exists;
        //}

        //public bool GetMaterial(uint uid, out Material material)
        //{
        //    bool exists = _materials.TryGetValue(uid, out material);

        //    return exists;
        //}
        #endregion

        public void Cleanup()
        {
            //TODO - CasualSimpleton - Removed the disposal portion to keep things simplified for now
            //foreach (KeyValuePair<uint, EDSSSprite> sprite in _edssSprites)
            //{
            //    sprite.Value.Dispose();
            //}

            //foreach (KeyValuePair<uint, EDSSSpriteSheet> sheet in _edssSpriteSheets)
            //{
            //    sheet.Value.Dispose();
            //}

            //foreach (KeyValuePair<uint, BlockTemplate> block in _blockTemplates)
            //{
            //    block.Value.Dispose();
            //}

            //foreach (KeyValuePair<string, Texture2D> texture in _textures)
            //{
            //    GameObject.Destroy(texture.Value);
            //}

            //foreach (KeyValuePair<uint, Material> material in _materials)
            //{
            //    GameObject.Destroy(material.Value);
            //}

            //foreach (KeyValuePair<uint, EntityDataTemplate> template in _entityDataTemplates)
            //{
            //    template.Value.Cleanup();
            //}

            //_edssSprites.Clear();
            //_edssSpriteSheets.Clear();
            //_blockTemplates.Clear();
            //_textures.Clear();
            //_materials.Clear();
            //_entityDataTemplates.Clear();

            //_edssSprites = null;
            //_edssSpriteSheets = null;
            //_blockTemplates = null;
            //_textures = null;
            //_materials = null;
            //_entityDataTemplates = null;
        }
    }
}