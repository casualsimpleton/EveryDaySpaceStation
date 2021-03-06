﻿//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// FileSystem - Handles all the file i/o as well as knowing various pathing and directories
// Created: December 3 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 3 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using EveryDaySpaceStation;
using EveryDaySpaceStation.Utils;
using EveryDaySpaceStation.Json;
using EveryDaySpaceStation.DataTypes;

namespace EveryDaySpaceStation
{
    public static partial class FileSystem
    {
        #region Vars
        static bool _initDone;
        static bool _hasGameStarted;

        static string _appDataDirectory = Application.persistentDataPath; //Where all the app data outside of Unity, is located, e.g.: "C:/Users/<User>/AppData/LocalLow/EveryDaySpaceStation/EveryDaySpaceStation"
        static string _clientDataDirectory = string.Empty; //Where the current client data is being stored, e.g.: C:/Users/<User>/AppData/LocalLow/EveryDaySpaceStation/EveryDaySpaceStation/"client"
        static string _clientGameDataDirectory = string.Empty; //Where the current game's client data is being stored, e.g.: C:/Users/<User>/AppData/LocalLow/EveryDaySpaceStation/EveryDaySpaceStation/client/"bobsserver"
        static string _serverDataDirectory = string.Empty; //Where the current server data is being stored (if at all), e.g.: C:/Users/<User>/AppData/LocalLow/EveryDaySpaceStation/EveryDaySpaceStation/"server"
        static string _serverGameDataDirectory = string.Empty; //Where the current server gamemode is being stored (if at all), e.g.: C:/Users/<User>/AppData/LocalLow/EveryDaySpaceStation/EveryDaySpaceStation/server/"default"
        static string _mapDirectory = string.Empty; //Where the maps are located (for editing or running a server), e.g.: C:/Users/<User>/AppData/LocalLow/EveryDaySpaceStation/EveryDaySpaceStation/"maps"

        static FileSystemConfig _fsConfig = null;
        static OptionsConfig _optionsConfig = null;
        static ServerConfig _serverConfig = null;
        static ServerNetConfig _serverNetConfig = null;
        static ClientNetConfig _clientNetConfig = null;
        //static GameManfiest _gameManifestConfig = null;
        #endregion

        #region Gets/Sets
        public static string AppDataDirectory { get { return _appDataDirectory; } }
        public static string ClientDataDirectory { get { return _clientDataDirectory; } }
        public static string ServerDataDirectory { get { return _serverDataDirectory; } }
        public static string ClientGameDataDirectory { get { return _clientGameDataDirectory; } }
        public static string MapDirectory { get { return _mapDirectory; } }

        public static string Compiled_ServerModuleDirectory
        {
            get
            {
                return string.Format("{0}{1}{2}{1}{3}", AppDataDirectory, System.IO.Path.DirectorySeparatorChar, _serverDataDirectory, _serverGameDataDirectory);
            }
        }

        public static bool InitDone { get { return _initDone; } }
        public static bool HasGameStarted
        {
            get { return _hasGameStarted; }
            set { _hasGameStarted = value; }
        }

        public static FileSystemConfig FileSystemconfig { get { return _fsConfig; } }
        public static OptionsConfig Optionsconfig { get { return _optionsConfig; } }
        public static ServerNetConfig ServerNetconfig { get { return _serverNetConfig; } }
        public static ClientNetConfig ClientNetconfig { get { return _clientNetConfig; } }
        #endregion

        public static void Init()
        {
            _fsConfig = Config.GetConfig<FileSystemConfig>();

            if (_fsConfig == null)
            {
                Debug.LogError("Unable to load config.json.");
                return;
            }

            LoadConfigFiles();

            ////TODO Load this properly
            //_mapDirectory = "maps";

            //if (_serverConfig.MapChoices.Length < 1)
            //{
            //    Debug.LogError(string.Format("No maps present in rotation. Unable to proceed!"));
            //    return;
            //}

            //MapDataConfig testMap = LoadMap(_serverConfig.MapChoices[0].MapName);  //"testLevel1");
            //MapEntityDataConfig testEntities = LoadMapEnities(_serverConfig.MapChoices[0].EntityName);

            //ServerGameManager.Singleton.ProcessMap(testMap);
            //ServerGameManager.Singleton.ProcessMapEntities(testEntities);
            ////ClientGameManager.Singleton.ProcessMap(testMap);
            ////ClientGameManager.Singleton.ProcessMapEntities(testEntities);

            ////Trigger a collection now so it doesn't kick in later as we just produced a good amount of trash
            //System.GC.Collect();

            _initDone = true;
            //_hasGameStarted = true;
        }

        #region Config stuff
        private static void LoadConfigFiles()
        {
            _optionsConfig = Config.GetConfig<OptionsConfig>();
            _serverNetConfig = Config.GetConfig<ServerNetConfig>();
            _clientNetConfig = Config.GetConfig<ClientNetConfig>();

            //LoadServerConfig();
            //ProcessGameManifest();
        }

        public static void LoadServerConfig(string manifestPath = "")
        {
            _serverConfig = Config.GetConfig<ServerConfig>();

            _serverDataDirectory = _fsConfig.ServerDir;
            _serverGameDataDirectory = _serverConfig.ModuleName;

            //Check to make sure that directory exists
            //string path = string.Format("{0}{1}{2}{1}{3}", AppDataDirectory, System.IO.Path.DirectorySeparatorChar, _serverDataDirectory, _serverGameDataDirectory);
            string path = Compiled_ServerModuleDirectory;

            if (!Directory.Exists(path))
            {
                Debug.LogError(string.Format("Can't find game module path '{0}'. Check configs!", path));
                return;
            }

            if (string.IsNullOrEmpty(manifestPath))
            {
                manifestPath = Path.Combine(path, "manifest.json");
            }

            if (!File.Exists(manifestPath))
            {
                Debug.LogError(string.Format("Can't find manifest.json at '{0}'. Can't proceed!", manifestPath));
            }

            string rawJson = File.ReadAllText(manifestPath);
            string fileName = FileSystem.GetFileNameWithExtension(manifestPath);

            //_gameManifestConfig = JsonConvert.DeserializeObject<GameManfiest>(rawJson);
            ProcessGameManifest(path, fileName, rawJson);
        }

        public static void ProcessGameManifest(string manifestPath, string manifestFileName, string rawManifestJson)
        {
            if (string.IsNullOrEmpty(rawManifestJson))
            {
                Debug.LogError(string.Format("Game Manifest string is null. Can't proceed."));
                return;
            }

            GameManifestJson jsonData = JsonConvert.DeserializeObject<GameManifestJson>(rawManifestJson);

            GameManifestV2.Singleton.PrepareManifest(manifestFileName, manifestPath, jsonData.ManifestName, jsonData.ManifestVersion);

            //Add default texture to texture dictionary
            AddDefaultTexture();

            #region Textures and Sprite Sheets
            int artFilesProcessed = 0;
            for (int i = 0; i < jsonData.ArtFileNames.Length; i++)
            {
                string fileAndPath = Path.Combine(Compiled_ServerModuleDirectory, jsonData.ArtFileNames[i]);

                if (!File.Exists(fileAndPath))
                {
                    Debug.LogError(string.Format("Unable to load art file '{0}'. Check the manifest for accuracy.", fileAndPath));
                    continue;
                }

                artFilesProcessed++;
                Texture2D tex = LoadImageFromFileAndPath(fileAndPath);
                tex.name = jsonData.ArtFileNames[i];

                GameManifestV2.Singleton.AddTexture(jsonData.ArtFileNames[i], tex);
            }

            //Create the default sprite sheet and sprite
            CreateDefaultSpriteSheetAndSprite();
            #endregion

            #region Sprite Data
            int spriteFileProcessed = 0;
            int spriteDataTemplatesProcessed = 0;
            for (int i = 0; i < jsonData.SpriteDataFileNames.Length; i++)
            {
                spriteFileProcessed++;
                List<GameManifestV2.SpriteDataTemplate> newSprites = ProcessSpriteTemplates(jsonData.SpriteDataFileNames[i], ref spriteDataTemplatesProcessed);

                if (newSprites == null)
                {
                    Debug.LogWarning(string.Format("No sprite templates processed from '{0}'. You might want to check that.", jsonData.SpriteDataFileNames[i]));
                    continue;
                }

                for (int j = 0; j < newSprites.Count; j++)
                {
                    GameManifestV2.Singleton.AddSprite(newSprites[j].SpriteUID, newSprites[j]);
                }
            }
            #endregion

            #region Block Data
            int blockDataFilesProcessed = 0;
            int blockDataTemplatesProcessed = 0;
            for (int i = 0; i < jsonData.BlockDataFileNames.Length; i++)
            {
                blockDataFilesProcessed++;
                List<GameManifestV2.BlockDataTemplate> newBlocks = ProcessBlockTemplates(jsonData.BlockDataFileNames[i], ref blockDataTemplatesProcessed);

                if (newBlocks == null)
                {
                    Debug.LogWarning(string.Format("No block templates processed from '{0}'. You might want to check that.", jsonData.BlockDataFileNames[i]));
                    continue;
                }

                for (int j = 0; j < newBlocks.Count; j++)
                {
                    GameManifestV2.Singleton.AddBlockTemplate(newBlocks[j]);
                }
            }
            #endregion

            GameManifestV2.Singleton.DoneLoaded();
        }

        private static List<GameManifestV2.BlockDataTemplate> ProcessBlockTemplates(string blockDataFileName, ref int numberOfBlockTemplatesProcessed)
        {
            List<GameManifestV2.BlockDataTemplate> newBlockTemplates = new List<GameManifestV2.BlockDataTemplate>();

            string fileAndPath = Path.Combine(Compiled_ServerModuleDirectory, blockDataFileName);

            if (!File.Exists(fileAndPath))
            {
                Debug.LogError(string.Format("Unable to load block template data file '{0}'. Check the manifest json for accuracy", fileAndPath));
                return null;
            }

            string rawJson = File.ReadAllText(fileAndPath);

            BlockTemplateCollectionJson groupBlockDataJson = JsonConvert.DeserializeObject<BlockTemplateCollectionJson>(rawJson);

            if (groupBlockDataJson == null)
            {
                Debug.LogError(string.Format("Problem loading block template data json for '{0}'. Please double check it.", fileAndPath));
                return null;
            }

            for (int i = 0; i < groupBlockDataJson.BlockData.Length; i++)
            {
                BlockTemplateJson blockJson = groupBlockDataJson.BlockData[i];

                byte[] blockDrawingFaces = new byte[System.Enum.GetValues(typeof(GameManifestV2.BlockDataTemplate.ShowFaceDirection)).Length];

                for (int j = 0; j < blockDrawingFaces.Length && j < blockJson.FaceDraw.Length; j++)
                {
                    blockDrawingFaces[j] = blockJson.FaceDraw[j];
                }

                ushort[] blockDefaultFaceSpriteUIDs = new ushort[System.Enum.GetValues(typeof(GameManifestV2.BlockDataTemplate.ShowFaceDirection)).Length];

                for (int j = 0; j < blockDefaultFaceSpriteUIDs.Length && j < blockJson.FaceDefaultSpriteUIDs.Length; j++)
                {
                    blockDefaultFaceSpriteUIDs[j] = blockJson.FaceDefaultSpriteUIDs[j];
                }

                GameManifestV2.BlockDataTemplate newBlock = new GameManifestV2.BlockDataTemplate(blockJson.BlockUID, blockJson.BlockName,
                    blockJson.BlockDefaultStrength, blockDrawingFaces, blockJson.Flags, blockDefaultFaceSpriteUIDs, blockJson.Requirement);

                numberOfBlockTemplatesProcessed++;

                newBlockTemplates.Add(newBlock);
            }

            return newBlockTemplates;
        }

        private static List<GameManifestV2.SpriteDataTemplate> ProcessSpriteTemplates(string spriteDataFileName, ref int numberOfSpriteTemplatesProcessed)
        {
            List<GameManifestV2.SpriteDataTemplate> newSpriteTemplates = new List<GameManifestV2.SpriteDataTemplate>();

            string fileAndPath = Path.Combine(Compiled_ServerModuleDirectory, spriteDataFileName);

            if (!File.Exists(fileAndPath))
            {
                Debug.LogError(string.Format("Unable to load sprite template data file '{0}'. Check the manifest json for accuracy.", fileAndPath));

                return null;
            }

            string rawJson = File.ReadAllText(fileAndPath);

            SpriteTemplateCollectionJson groupSpriteDataJson = JsonConvert.DeserializeObject<SpriteTemplateCollectionJson>(rawJson);

            if (groupSpriteDataJson == null)
            {
                Debug.LogError(string.Format("Problem loading sprite template data json for '{0}'. Please double check it.", fileAndPath));
                return null;
            }

            for (int i = 0; i < groupSpriteDataJson.SpriteData.Length; i++)
            {
                SpriteTemplateJson spriteJson = groupSpriteDataJson.SpriteData[i];

                GameManifestV2.SpriteDataTemplate newSprite = CreateSpriteTemplate(spriteJson);

                if (newSprite == null)
                    continue;

                newSpriteTemplates.Add(newSprite);
                numberOfSpriteTemplatesProcessed++;
            }

            return newSpriteTemplates;
        }

        public static GameManifestV2.SpriteDataTemplate CreateSpriteTemplate(SpriteTemplateJson spriteJsonData)
        {
            string sheetTypeName = "world";
            GameManifestV2.SpriteSheetDataTemplate.ShaderType shaderType = GameManifestV2.SpriteSheetDataTemplate.ShaderType.World;
            //We've got to process the flags here as this will determine what kind of material. Hopefully they're well clustered so we don't create excess amounts of materials
            if (spriteJsonData.Flags != null)
            {
                for (int k = 0; k < spriteJsonData.Flags.Length; k++)
                {
                    string flag = spriteJsonData.Flags[k].ToLower();

                    switch (flag)
                    {
                        case "billboard":
                            sheetTypeName = "billboard";
                            shaderType = GameManifestV2.SpriteSheetDataTemplate.ShaderType.Billboard;
                            break;

                        case "twosided":
                            sheetTypeName = "twosided";
                            shaderType = GameManifestV2.SpriteSheetDataTemplate.ShaderType.TwoSided;
                            break;
                    }
                }
            }

            //First look if there is a SpriteSheet
            GameManifestV2.SpriteSheetDataTemplate sheet = null;
            string sheetName = string.Format("{0}-{1}", spriteJsonData.SpriteSheetFileName, sheetTypeName);
            bool found = GameManifestV2.Singleton.GetSpriteSheet(sheetName, out sheet);

            //Couldn't find the sheet, so we need to create it
            if (!found)
            {
                //Attempt to create it
                found = AttemptCreateSpriteSheet(spriteJsonData, sheetName, shaderType, out sheet);
            }

            //It's still not found, so we need to throw an error and move on
            if (!found)
            {
                Debug.LogError(string.Format("Could not locate a spritesheet for sprite '{0}'. Giving up.", spriteJsonData));
                return null;
            }

            //We've got a sheet somehow, so we're ready to make the sprite
            GameManifestV2.SpriteDataTemplate newSprite = sheet.CreateSpriteTemplate(spriteJsonData.UID, spriteJsonData.SpritePosition, spriteJsonData.SpriteWidthHeight, spriteJsonData.SpriteName, spriteJsonData.Flags);

            return newSprite;
        }

        /// <summary>
        /// Not the most elengant process, but it copies the default texture into memory, so it can be inserted into the runtime texture dictionary
        /// </summary>
        private static void AddDefaultTexture()
        {
            Texture2D defaultCopy = new Texture2D(DefaultFiles.defaultTexture.width, DefaultFiles.defaultTexture.height, DefaultFiles.defaultTexture.format, true);
            Color[] defColors = DefaultFiles.defaultTexture.GetPixels();
            defaultCopy.SetPixels(defColors);
            defaultCopy.Apply();
            defaultCopy.name = DefaultFiles.defaultTexture.name;
            defaultCopy.filterMode = DefaultFiles.defaultTexture.filterMode;
            defaultCopy.wrapMode = DefaultFiles.defaultTexture.wrapMode;
            GameManifestV2.Singleton.AddTexture(defaultCopy.name, defaultCopy);

        }

        /// <summary>
        /// Create the default sprite sheet and sprites for use
        /// </summary>
        private static void CreateDefaultSpriteSheetAndSprite()
        {
            GameManifestV2.SpriteSheetDataTemplate sheet = new GameManifestV2.SpriteSheetDataTemplate();
            ushort uid = GameManifestV2.Singleton.GetNewSpriteSheetUID();
            ushort matUID = GameManifestV2.Singleton.GetNewMaterialUID();

            Material newMat = new Material(DefaultFiles.defaultShader);

            Texture2D texture = null;
            //We'll use the same name since we copied that
            GameManifestV2.Singleton.GetTexture(DefaultFiles.defaultTexture.name, out texture);

            newMat.name = string.Format("{0}", texture);
            newMat.SetTexture("_MainTex", texture);
            GameManifestV2.Singleton.AddMaterial(matUID, newMat);

            sheet.CreateSpriteSheetTemplate(uid, matUID, texture, newMat, null);

            GameManifestV2.Singleton.AddSpriteSheet(uid, sheet);

            //Create the default sprite
            SpriteTemplateJson defaultSpriteJson = new SpriteTemplateJson();
            defaultSpriteJson.UID = 0;
            defaultSpriteJson.SpriteName = "Default";
            defaultSpriteJson.SpriteSheetFileName = texture.name;
            defaultSpriteJson.SpritePosition = new Vec2Int(0, 0);
            defaultSpriteJson.SpriteWidthHeight = new Vec2Int(texture.width, texture.height);

            GameManifestV2.SpriteDataTemplate newSprite = CreateSpriteTemplate(defaultSpriteJson);
            GameManifestV2.DefaultSprite = newSprite;
            GameManifestV2.Singleton.AddSprite(newSprite.SpriteUID, newSprite);
        }

        public static bool AttemptCreateSpriteSheet(SpriteTemplateJson spriteDataJson, string sheetName, GameManifestV2.SpriteSheetDataTemplate.ShaderType desiredType, out GameManifestV2.SpriteSheetDataTemplate sheet)
        {
            sheet = null;

            //If we arrived here, then we probably couldn't find the sheet by texture name. We're going to assume the textures have all successfull been loaded by now
            //So we'll first look at the textures to see if we can make a sheet from it
            Texture2D texture = null;
            bool found = GameManifestV2.Singleton.GetTexture(spriteDataJson.SpriteSheetFileName, out texture);

            if (!found)
            {
                Debug.LogError(string.Format("Attempted to locate texture '{0}' for sprite '{1}' but failed.", spriteDataJson.SpriteSheetFileName, spriteDataJson));
                return false;
            }

            sheet = new GameManifestV2.SpriteSheetDataTemplate();
            ushort uid = GameManifestV2.Singleton.GetNewSpriteSheetUID();
            ushort matUID = GameManifestV2.Singleton.GetNewMaterialUID();

            //Create a new material and assign the texture
            Material newMat;

            if (desiredType == GameManifestV2.SpriteSheetDataTemplate.ShaderType.Billboard)
            {
                newMat = new Material(DefaultFiles.billboardShader);
            }
            else if (desiredType == GameManifestV2.SpriteSheetDataTemplate.ShaderType.TwoSided)
            {
                newMat = new Material(DefaultFiles.twoSidedSpriteShader);
            }
            else
            {
                newMat = new Material(DefaultFiles.defaultShader);
            }

            newMat.name = string.Format("{0}", sheetName);
            newMat.SetTexture("_MainTex", texture);
            GameManifestV2.Singleton.AddMaterial(matUID, newMat);

            sheet.CreateSpriteSheetTemplate(uid, matUID, texture, newMat, null);

            GameManifestV2.Singleton.AddSpriteSheet(uid, sheet);

            return true;
        }
        #endregion

        #region Map Data
        //Map Format:
        //4 Byte Char - EDSS 
        //2 Byte Ushort - Map Format Version
        //\0 Terminated String - Map Name
        //2 Byte Ushort - Region ID
        //2 Byte Ushort - Region Block Count X
        //2 Byte Ushort - Region Block Count Y
        //2 Byte Ushort - Region Block Count Z

        //Each Block
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

        public static void WriteMap(MapDataV2 mapData, string filePath)
        {
            System.Diagnostics.Stopwatch writeTimer = new System.Diagnostics.Stopwatch();
            writeTimer.Start();
            using (Stream s = File.Open(filePath, FileMode.Create))
            {
                using (BinaryWriter bWriter = new BinaryWriter(s))
                {
                    //Preface file with EDSS
                    bWriter.Write('E');
                    bWriter.Write('D');
                    bWriter.Write('S');
                    bWriter.Write('S');

                    //Map Format Version
                    bWriter.Write(mapData.MapVersion);

                    //Map Name
                    bWriter.Write(mapData.MapName);

                    for (int i = 0; i < mapData.MapRegions.Count; i++)
                    {
                        MapDataV2.MapRegion region = mapData.MapRegions[i];

                        //Region UID
                        bWriter.Write(region.RegionUID);

                        //Region Block Size X
                        bWriter.Write(region.RegionSize.x);
                        //Region Block Size Y
                        bWriter.Write(region.RegionSize.y);
                        //Region Block Size Z
                        bWriter.Write(region.RegionSize.z);

                        int xlen = region.RegionBlocks.GetLength(0);
                        int ylen = region.RegionBlocks.GetLength(1);
                        int zlen = region.RegionBlocks.GetLength(2);

                        if (xlen != region.RegionSize.x || ylen != region.RegionSize.y || zlen != region.RegionSize.z)
                        {
                            Debug.LogWarning(string.Format("Region size {2} mismatches with actual block size {3},{4},{5} for map {0}, region {1}", mapData.MapName, region.RegionUID, region.RegionSize, xlen, ylen, zlen));
                        }

                        for(int z = 0; z < zlen; z++)
                        {
                            for(int x = 0; x < xlen; x++)
                            {
                                for(int y = 0; y < ylen; y++)
                                {
                                    MapDataV2.MapBlock block = region.RegionBlocks[x, y, z];
                                    
                                    //Block Type
                                    bWriter.Write(block.BlockType);
                                    
                                    //Faces UID - Top, Bottom, Forward, Back, Right, Left
                                    if (block.BlockFacesSpriteUIDs == null)
                                    {
                                        //If faces don't exist, write all 0s
                                        for (int f = 0; f < (int)MapDataV2.MapBlock.BlockFace.MAX; f++)
                                        {
                                            bWriter.Write((ushort)0);
                                        }
                                    }
                                    else
                                    {
                                        for (int f = 0; f < block.BlockFacesSpriteUIDs.Length; f++)
                                        {
                                            bWriter.Write(block.BlockFacesSpriteUIDs[f]);
                                        }
                                    }

                                    //Block Light Info
                                    bWriter.Write(block.DoesBlockLight);

                                    //Block Pipe Info
                                    bWriter.Write(block.BlockPipe);

                                    //Debug.Log(string.Format("{0},{1},{2} len: {3}", x, y, z, bWriter.BaseStream.Position));
                                }
                            }
                        }
                    }

                    Debug.Log(string.Format("Wrote {0} Regions. Writer Position: {1} File length: {2}", mapData.MapRegions.Count, bWriter.BaseStream.Position, bWriter.BaseStream.Length));
                }
            }

            writeTimer.Stop();
            Debug.Log(string.Format("Finished writing {0} to file. Dur: {1}s", mapData.MapName, (writeTimer.ElapsedMilliseconds / 1000f)));
        }

        public static MapDataV2 LoadMapData(string fileAndPath)
        {
            if (!File.Exists(fileAndPath))
            {
                Debug.LogWarning(string.Format("Can't find a file at: {0}", fileAndPath));
                return null;
            }

            using (Stream s = File.Open(fileAndPath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader b = new BinaryReader(s))
                {
                    byte[] fourBytes = b.ReadBytes(4);

                    //Check that it starts with EDSS...
                    //Lazy and fast way to do it
                    if (fourBytes[0] != 'E' && fourBytes[1] != 'D' && fourBytes[2] != 'S' && fourBytes[3] != 'S')
                    {
                        Debug.LogWarning(string.Format("Data loaded from '{0}' doesn't appear to be a map file", fileAndPath));
                        return null;
                    }

                    MapDataV2 loadedMap = new MapDataV2();

                    //Map Format Version
                    loadedMap.MapVersion = b.ReadUInt16();

                    //Map Name
                    loadedMap.MapName = b.ReadString();

                    while (b.BaseStream.Position != b.BaseStream.Length)
                    {
                        //Now for regions - Expect at least 1
                        MapDataV2.MapRegion newRegion = new MapDataV2.MapRegion();

                        //Region UID
                        newRegion.RegionUID = b.ReadUInt16();

                        //Region Block Sizes
                        Vec3Int size = new Vec3Int(0, 0, 0);

                        size.x = b.ReadInt32();
                        size.y = b.ReadInt32();
                        size.z = b.ReadInt32();

                        newRegion.RegionSize = size;

                        newRegion.RegionBlocks = new MapDataV2.MapBlock[size.x, size.y, size.z];

                        for (int z = 0; z < size.z; z++)
                        {
                            for (int x = 0; x < size.x; x++)
                            {
                                for (int y = 0; y < size.y; y++)
                                {
                                    MapDataV2.MapBlock block = new MapDataV2.MapBlock();

                                    //Block Type
                                    block.BlockType = b.ReadUInt16();

                                    //Block Faces
                                    block.BlockFacesSpriteUIDs = new ushort[(int)MapDataV2.MapBlock.BlockFace.MAX];

                                    for (int f = 0; f < block.BlockFacesSpriteUIDs.Length; f++)
                                    {
                                        block.BlockFacesSpriteUIDs[f] = b.ReadUInt16();
                                    }

                                    //Block Light Info
                                    block.DoesBlockLight = b.ReadByte();

                                    //Block Pipe Info
                                    block.BlockPipe = b.ReadByte();

                                    newRegion.RegionBlocks[x, y, z] = block;
                                }
                            }
                        }

                        loadedMap.MapRegions.Add(newRegion);
                    }

                    return loadedMap;
                }
            }
        }

        public static string GetFileNameWithoutExtension(string fullPathAndFile)
        {
            return Path.GetFileNameWithoutExtension(fullPathAndFile);
        }

        public static string GetFileNameWithExtension(string fullPathAndFile)
        {
            return Path.GetFileName(fullPathAndFile);
        }
        #endregion
    }
}