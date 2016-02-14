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

            public uint BlockUID { get; private set; }
            public string BlockName { get; private set; }
            public int BlockStrength { get; private set; }
            public byte[] BlockFaces { get; private set; }
            public string[] BlockFlags { get; private set; }
            public uint BlockRequirement { get; private set; }

            public BlockDataTemplate(uint blockUID, string blockName, int blockStength, byte[] blockFaces,
                string[] blockFlags, uint blockRequirement)
            {
                BlockUID = blockUID;
                BlockName = blockName;
                BlockStrength = blockStength;
                BlockFaces = blockFaces;
                BlockFlags = blockFlags;
                BlockRequirement = blockRequirement;
            }

        }

        public class SpriteDataTemplate
        {
        }
        #endregion

        #region Basic Manifest Content
        public string ManifestFileName { get; private set; }
        public string ManifestFilePath { get; private set; }
        public string ManifestName { get; private set; }
        public ushort ManifestVersion { get; private set; }
        #endregion

        #region Block Data
        Dictionary<uint, BlockDataTemplate> _blockDataTemplates;

        public void AddBlockTemplate(BlockDataTemplate newBlock)
        {
            _blockDataTemplates.Add(newBlock.BlockUID, newBlock);
        }

        public bool GetBlockTemplate(uint uid, out BlockDataTemplate blockTemplate)
        {
            return _blockDataTemplates.TryGetValue(uid, out blockTemplate);
        }
        #endregion

        #region Art
        #endregion

        #region Sprite Data
        #endregion

        #region Entity Data
        #endregion

        public GameManifestV2()
        {
        }

        public void PrepareManifest(string manifestFileName, string manifestFilePath, string manifestName, ushort manifestVersion)
        {
            ManifestFileName = manifestFileName;
            ManifestFilePath = manifestFilePath;
            ManifestName = manifestName;
            ManifestVersion = manifestVersion;

            _blockDataTemplates = new Dictionary<uint, BlockDataTemplate>();
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
            foreach (KeyValuePair<uint, BlockDataTemplate> blockTemplate in _blockDataTemplates)
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

            return str.ToString();
        }
    }
}