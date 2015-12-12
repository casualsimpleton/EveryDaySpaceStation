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
using System.Collections;
using System.Collections.Generic;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;


namespace EveryDaySpaceStation
{
    [System.Serializable]
    public sealed class GameData
    {
        #region Classes/Structs
        public class EntityData
        {
            public uint UID { get; private set; }
            public string Name { get; private set; }

            public EntityData(uint uid, string name)
            {
                UID = uid;
                Name = name;
            }
        }

        public class GameBlockData : System.IDisposable
        {
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

            public uint UID { get; private set; }
            public string Name { get; private set; }
            public int DefaultStrength { get; private set; }
            public List<string> Flags { get; private set; }
            public bool BlocksLight { get; private set; }
            public bool IsVacuum { get; private set; }
            public bool IsPorous { get; private set; }
            public bool IsEmpty { get; private set; }
            /// <summary>
            /// The UID for the type of block that must be present in order for this block to be placed. Not required for mapping, 
            /// but will be used during run-time for dynamic building
            /// </summary>
            public uint RequirementUID { get; private set; }

            public FaceInfo[] Faceinfo;

            public GameBlockData(uint uid, string name, int defaultStrength, string[] flags, uint requirementUID)
            {
                UID = uid;
                Name = name;
                DefaultStrength = defaultStrength;
                Flags = new List<string>(flags);
                RequirementUID = requirementUID;

                ParseFlags();
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

            private void ParseFlags()
            {
                if (Flags == null)
                    return;

                for (int i = 0; i < Flags.Count; i++)
                {
                    switch (Flags[i].ToLower())
                    {
                        case "emtpty":
                            IsEmpty = true;
                            continue;
                        case "vacuum":
                            IsVacuum = true;
                            continue;
                        case "blockslight":
                            BlocksLight = true;
                            continue;
                        case "door":
                            //TODO
                            continue;
                        case "porous":
                            continue;
                        case "transparent":
                            BlocksLight = false;
                            continue;
                    }
                }
            }

            public override string ToString()
            {
                //Not the most efficient, but it works
                string flagsTxt = "";

                for (int i = 0; i < flagsTxt.Length; i++)
                {
                    flagsTxt += string.Format("'{0}|'", flagsTxt[i]);
                }

                return string.Format("UID: {0} Name: {1} DefaultStrength: {2} RequirementUID: {3} Flags: {4}", UID, Name, DefaultStrength, RequirementUID, flagsTxt);
            }

            #region Dispose
            ///////////
            //IDisposable Overrides
            protected bool _isDisposed = false;

            public virtual void Dispose()
            {
                Dispose(true);
                System.GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_isDisposed)
                {
                    if (disposing)
                    {
                        //Dispose here
                        Flags.Clear();
                        Flags = null;
                    }
                }
            }

            ~GameBlockData()
            {
                Dispose(false);
            }
            #endregion
        }
        #endregion

        #region Vars
        Dictionary<uint, EDSSSprite> _sprites;
        Dictionary<uint, EDSSSpriteSheet> _spriteSheets;
        Dictionary<uint, GameBlockData> _gameBlockData;
        Dictionary<string, Texture2D> _textures;
        Dictionary<uint, Material> _materials;
        Dictionary<uint, EntityData> _entityData;        

        private uint _spriteSheetUID = 1;
        private uint _materialUID = 1;
        #endregion

        #region Gets/Sets
        public uint GetNewSpriteSheetUID() { return _spriteSheetUID++; }
        public uint GetNewMaterialUID() { return _materialUID++; }
        #endregion

        #region Constructors
        public GameData()
        {
            _sprites = new Dictionary<uint, EDSSSprite>();
            _spriteSheets = new Dictionary<uint, EDSSSpriteSheet>();
            _gameBlockData = new Dictionary<uint, GameBlockData>();
            _textures = new Dictionary<string, Texture2D>();
            _materials = new Dictionary<uint, Material>();
            _entityData = new Dictionary<uint, EntityData>();
        }
        #endregion

        public void AddSprite(uint uid, EDSSSprite sprite)
        {
            _sprites.Add(uid, sprite);
        }

        public void AddSpriteSheet(uint uid, EDSSSpriteSheet spriteSheet)
        {
            _spriteSheets.Add(uid, spriteSheet);
        }

        public void AddGameBlock(uint uid, GameBlockData blockData)
        {
            _gameBlockData.Add(uid, blockData);
        }

        public void AddEntity(uint uid, EntityData entityData)
        {
            _entityData.Add(uid, entityData);
        }

        public void AddTexture(string name, Texture2D texture)
        {
            _textures.Add(name, texture);
        }

        public void AddMaterial(uint uid, Material material)
        {
            _materials.Add(uid, material);
        }

        public bool GetSprite(uint uid, out EDSSSprite sprite)
        {
            bool exists = _sprites.TryGetValue(uid, out sprite);

            return exists;
        }

        public bool GetSpriteSheet(uint uid, out EDSSSpriteSheet spriteSheet)
        {
            bool exists = _spriteSheets.TryGetValue(uid, out spriteSheet);

            return exists;
        }

        /// <summary>
        /// Look for a EDSSSpriteSheet by texture name, since we might not have the UID yet. NOTE - Going to be slower that searching by UID
        /// </summary>
        public bool GetSpriteSheet(string name, out EDSSSpriteSheet spriteSheet)
        {
            spriteSheet = null;
            foreach (KeyValuePair<uint, EDSSSpriteSheet> sheet in _spriteSheets)
            {
                if (sheet.Value.Material.name.CompareTo(name) == 0)
                {
                    spriteSheet = sheet.Value;
                    return true;
                }
            }

            return false;
        }

        public bool GetGameBlock(uint uid, out GameBlockData blockData)
        {
            bool exists = _gameBlockData.TryGetValue(uid, out blockData);

            return exists;
        }

        public bool GetTexture(string name, out Texture2D texture)
        {
            bool exists = _textures.TryGetValue(name, out texture);

            return exists;
        }

        public bool GetMaterial(uint uid, out Material material)
        {
            bool exists = _materials.TryGetValue(uid, out material);

            return exists;
        }

        public void Cleanup()
        {
            foreach (KeyValuePair<uint, EDSSSprite> sprite in _sprites)
            {
                sprite.Value.Dispose();
            }

            foreach (KeyValuePair<uint, EDSSSpriteSheet> sheet in _spriteSheets)
            {
                sheet.Value.Dispose();
            }

            foreach (KeyValuePair<uint, GameBlockData> block in _gameBlockData)
            {
                block.Value.Dispose();
            }

            foreach (KeyValuePair<string, Texture2D> texture in _textures)
            {
                GameObject.Destroy(texture.Value);
            }

            foreach (KeyValuePair<uint, Material> material in _materials)
            {
                GameObject.Destroy(material.Value);
            }

            _sprites.Clear();
            _spriteSheets.Clear();
            _gameBlockData.Clear();
            _textures.Clear();
            _materials.Clear();

            _sprites = null;
            _spriteSheets = null;
            _gameBlockData = null;
            _textures = null;
            _materials = null;
        }
    }
}