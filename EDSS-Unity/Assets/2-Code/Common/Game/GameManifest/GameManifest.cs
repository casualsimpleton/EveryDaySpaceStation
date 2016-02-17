//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// GameManifestV2 - Class for holding game template data. Second version
// Created: Febuary 12 2016
// CasualSimpleton <casualsimpleton@gmail.com>
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
    public class GameManifestV2
    {
        public static GameManifestV2 Singleton = new GameManifestV2();

        #region Classes
        public class BlockDataTemplate
        {
            public enum ShowFaceDirection : byte
            {
                FaceZPlus,
                FaceXPlus,
                FaceYPlus,
                FaceZMinus,
                FaceXMinus,
                FaceYMinus
            }


            public ushort BlockUID { get; private set; }
            public string BlockName { get; private set; }
            public int BlockStrength { get; private set; }
            public byte[] BlockFaces { get; private set; }
            public uint[] BlockDefaultFaceUIDs { get; private set; }
            public string[] BlockFlags { get; private set; }
            public uint BlockRequirement { get; private set; }

            public BlockDataTemplate(ushort blockUID, string blockName, int blockStength, byte[] blockFaces,
                string[] blockFlags, uint[] blockDefaultFaceUIDs, uint blockRequirement)
            {
                BlockUID = blockUID;
                BlockName = blockName;
                BlockStrength = blockStength;
                BlockFaces = blockFaces;
                BlockFlags = blockFlags;
                BlockRequirement = blockRequirement;
                BlockDefaultFaceUIDs = blockDefaultFaceUIDs;
            }

        }

        public class SpriteDataTemplate
        {
            public uint SpriteUID { get; private set; }
            public string SpriteName { get; private set; }
            public Vec2Int TopLeft { get; private set; }
            public Vec2Int WidthHeight { get; private set; }
            public SpriteSheetDataTemplate SpriteSheetTemplate { get; private set; }
            public string[] SpriteFlags { get; private set; }
            /// <summary>
            /// This should be 25% of one pixel of the sprite sheet. This is so when anti alias is turned on, we aren't right on the seam between sprites
            /// </summary>
            public Vector2 SpriteUVOffset { get; private set; }

            private Vector4 _uvCoords = Vector4.zero;
            public Vector4 GetUVCoords()
            {
                if (_uvCoords == Vector4.zero)
                {
                    float xPos = (float)TopLeft.x / (float)SpriteSheetTemplate.Texture.width;
                    //float yPos = (float)(TopLeft.y + WidthHeight.y) / (float)SpriteSheetTemplate.Texture.height;
                    float yPos = (float)(TopLeft.y) / (float)SpriteSheetTemplate.Texture.height;
                    float width = (float)(WidthHeight.x) / (float)SpriteSheetTemplate.Texture.width;
                    float height = (float)WidthHeight.y / (float)SpriteSheetTemplate.Texture.height;

                    _uvCoords = new Vector4(xPos, yPos, width, height);
                }

                return _uvCoords;
            }

            public void CreateSprite(uint spriteUID, Vec2Int topLeft, Vec2Int widthHeigh, SpriteSheetDataTemplate spriteSheet, string spriteName, string[] flags)
            {
                SpriteUID = spriteUID;
                SpriteName = spriteName;
                TopLeft = topLeft;
                WidthHeight = widthHeigh;
                SpriteSheetTemplate = spriteSheet;
                SpriteFlags = flags;

                SpriteUVOffset = new Vector2(1f / SpriteSheetTemplate.Texture.width * 1.0f, 1f / SpriteSheetTemplate.Texture.height * 0.1f);
            }
        }

        public class SpriteSheetDataTemplate
        {
            public enum ShaderType
            {
                World,
                Billboard,
                TwoSided
            }

            public uint SpriteSheetUID { get; private set; }
            public uint MaterialUID { get; private set; }
            public Texture2D Texture { get; private set; }
            public List<SpriteDataTemplate> Sprites { get; private set; }
            public Material Material { get; private set; }
            public ShaderType MaterialType { get; private set; }

            public void CreateSpriteSheetTemplate(uint uid, uint materialUID, Texture2D art, Material mat, List<SpriteDataTemplate> existingSprites = null)
            {
                SpriteSheetUID = uid;
                MaterialUID = materialUID;
                Texture = art;
                Material = mat;
                Sprites = existingSprites;
            }

            public void AddSprite(SpriteDataTemplate sprite)
            {
                Sprites.Add(sprite);
            }

            public void RemoveSprite(SpriteDataTemplate sprite)
            {
                Sprites.Remove(sprite);
            }

            public SpriteDataTemplate CreateSpriteTemplate(uint spriteUID, Vec2Int xyPos, Vec2Int widthHeight, string spriteName, string[] spriteFlags)
            {
                SpriteDataTemplate newSprite = new SpriteDataTemplate();
                newSprite.CreateSprite(spriteUID, xyPos, widthHeight, this, spriteName, spriteFlags);

                if (Sprites == null)
                {
                    Sprites = new List<SpriteDataTemplate>();
                }

                Sprites.Add(newSprite);

                return newSprite;
            }
        }
        #endregion

        #region Basic Manifest Content
        public string ManifestFileName { get; private set; }
        public string ManifestFilePath { get; private set; }
        public string ManifestName { get; private set; }
        public ushort ManifestVersion { get; private set; }
        #endregion

        #region Class Variables
        public static SpriteDataTemplate DefaultSprite;
        private uint _spriteSheetUID = 0;
        private uint _materialUID = 0;
        #endregion

        #region Gets/Sets
        public uint GetNewSpriteSheetUID() { return _spriteSheetUID++; }
        public uint GetNewMaterialUID() { return _materialUID++; }
        #endregion

        #region Block Data
        Dictionary<ushort, BlockDataTemplate> _blockDataTemplates;

        public void AddBlockTemplate(BlockDataTemplate newBlock)
        {
            _blockDataTemplates.Add(newBlock.BlockUID, newBlock);
        }

        public bool GetBlockTemplate(ushort uid, out BlockDataTemplate blockTemplate)
        {
            return _blockDataTemplates.TryGetValue(uid, out blockTemplate);
        }

        public Dictionary<ushort, BlockDataTemplate> GetAllBlockTemplates()
        {
            return _blockDataTemplates;
        }
        #endregion

        #region Art
        Dictionary<string, Texture2D> _textures;
        Dictionary<uint, Material> _materials;

        public void AddTexture(string name, Texture2D texture)
        {
            _textures.Add(name, texture);
        }

        public bool GetTexture(string name, out Texture2D texture)
        {
            //return _textures.TryGetValue(name, out texture);
            bool exists = _textures.TryGetValue(name, out texture);

            //Can't find texture, so return default
            if (!exists)
            {
                texture = _textures[DefaultSprite.SpriteSheetTemplate.Texture.name];
            }

            return exists;
        }

        public void AddMaterial(uint uid, Material material)
        {
            _materials.Add(uid, material);
        }

        public bool GetMaterial(uint uid, out Material material)
        {
            return _materials.TryGetValue(uid, out material);
        }
        #endregion

        #region Sprite Data
        Dictionary<uint, SpriteDataTemplate> _spritesTemplates;
        Dictionary<uint, SpriteSheetDataTemplate> _spriteSheetsTemplates;

        public void AddSprite(uint uid, SpriteDataTemplate sprite)
        {
            _spritesTemplates.Add(uid, sprite);
        }

        public bool GetSprite(uint uid, out SpriteDataTemplate sprite)
        {
            bool exists = _spritesTemplates.TryGetValue(uid, out sprite);

            //Return default sprite if sprite not found
            if (!exists)
            {
                sprite = GameManifestV2.DefaultSprite;
            }

            return exists;
        }

        public void AddSpriteSheet(uint uid, SpriteSheetDataTemplate spriteSheet)
        {
            _spriteSheetsTemplates.Add(uid, spriteSheet);
        }

        public bool GetSpriteSheet(uint uid, out SpriteSheetDataTemplate spriteSheet)
        {
            return _spriteSheetsTemplates.TryGetValue(uid, out spriteSheet);
        }
        
        /// <summary>
        /// Look for a SpriteSheetDataTemplate by texture name, since we might not have the UID yet. NOTE - Going to be slower that searching by UID
        /// </summary>
        public bool GetSpriteSheet(string name, out SpriteSheetDataTemplate spriteSheet)
        {
            spriteSheet = null;
            foreach (KeyValuePair<uint, SpriteSheetDataTemplate> sheet in _spriteSheetsTemplates)
            {
                if (sheet.Value.Material.name.CompareTo(name) == 0)
                {
                    spriteSheet = sheet.Value;
                }
            }

            return false;
        }
        #endregion

        #region Entity Data
        #endregion

        public GameManifestV2()
        {
            _spriteSheetUID = 0;
            _materialUID = 0;
        }

        public void PrepareManifest(string manifestFileName, string manifestFilePath, string manifestName, ushort manifestVersion)
        {
            ManifestFileName = manifestFileName;
            ManifestFilePath = manifestFilePath;
            ManifestName = manifestName;
            ManifestVersion = manifestVersion;

            _blockDataTemplates = new Dictionary<ushort, BlockDataTemplate>();
            _spritesTemplates = new Dictionary<uint, SpriteDataTemplate>();
            _spriteSheetsTemplates = new Dictionary<uint, SpriteSheetDataTemplate>();
            _textures = new Dictionary<string, Texture2D>();
            _materials = new Dictionary<uint, Material>();
        }

        public string DumpToLog()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();

            str.AppendLine("Game Manifest Dump ------------------");
            str.AppendFormat("Manifest File Name: {0}", ManifestFileName);
            str.AppendLine();
            str.AppendFormat("Manifest File Path: {0}", ManifestFilePath);
            str.AppendLine();
            str.AppendFormat("Manifest Name: {0}", ManifestName);
            str.AppendLine();
            str.AppendFormat("Manifest Version: {0}", ManifestVersion);
            str.AppendLine();
            str.AppendLine();
            str.AppendLine("Block Templates++++++++++++++++++++++++++");
            foreach (KeyValuePair<ushort, BlockDataTemplate> blockTemplate in _blockDataTemplates)
            {
                str.AppendFormat("--BlockUID: {0}\t\tBlockName: {1}\t\tBlockStrength: {2}", blockTemplate.Value.BlockUID,
                    blockTemplate.Value.BlockName, blockTemplate.Value.BlockStrength);
                str.AppendLine();
                str.AppendFormat("--{0} {1} {2} {3} {4} {5}",
                    blockTemplate.Value.BlockFaces[(int)BlockDataTemplate.ShowFaceDirection.FaceZPlus],
                    blockTemplate.Value.BlockFaces[(int)BlockDataTemplate.ShowFaceDirection.FaceXPlus],
                    blockTemplate.Value.BlockFaces[(int)BlockDataTemplate.ShowFaceDirection.FaceYPlus],
                    blockTemplate.Value.BlockFaces[(int)BlockDataTemplate.ShowFaceDirection.FaceZMinus],
                    blockTemplate.Value.BlockFaces[(int)BlockDataTemplate.ShowFaceDirection.FaceXMinus],
                    blockTemplate.Value.BlockFaces[(int)BlockDataTemplate.ShowFaceDirection.FaceYMinus]);
                str.AppendLine();
                str.AppendLine("-- Flags:");
                for (int i = 0; i < blockTemplate.Value.BlockFlags.Length; i++)
                {
                    str.AppendFormat("[{0}]", blockTemplate.Value.BlockFlags[i]);
                    str.AppendLine();
                }
                str.AppendFormat("Block Requirement: {0}", blockTemplate.Value.BlockRequirement);
                str.AppendLine();
                str.AppendLine();
                str.AppendLine();
            }
            str.AppendLine();
            str.AppendLine();
            str.AppendLine("Sprite Templates++++++++++++++++++++++++++");
            foreach (KeyValuePair<uint, SpriteDataTemplate> spriteTemplate in _spritesTemplates)
            {
                str.AppendFormat("--Sprite UID: {0}\tSpriteName: {1}", spriteTemplate.Value.SpriteUID, spriteTemplate.Value.SpriteName);
                str.AppendLine();
                str.AppendFormat("--X: {0}, Y: {1} W: {2} H: {3}", spriteTemplate.Value.TopLeft.x, spriteTemplate.Value.TopLeft.y, spriteTemplate.Value.WidthHeight.x, spriteTemplate.Value.WidthHeight.y);
                str.AppendLine();
                Vector4 uvs = spriteTemplate.Value.GetUVCoords();
                str.AppendFormat("--UV X: {0}, Y: {1} W: {2} H: {3}", uvs.x, uvs.y, uvs.w, uvs.z);
                str.AppendLine();
                str.AppendFormat("-- Flags:");
                if (spriteTemplate.Value.SpriteFlags != null)
                {
                    for (int i = 0; i < spriteTemplate.Value.SpriteFlags.Length; i++)
                    {
                        str.AppendFormat("[{0}]", spriteTemplate.Value.SpriteFlags[i]);
                        str.AppendLine();
                    }
                }
                str.AppendLine();
                str.AppendLine();
                str.AppendLine();
            }
            str.AppendLine();
            str.AppendLine();

            return str.ToString();
        }
    }
}