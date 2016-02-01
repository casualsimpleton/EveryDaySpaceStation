//////////////////////////////////////////////////////////////////////////////////////////
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
        static GameManfiest _gameManifestConfig = null;
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

            //TODO Load this properly
            _mapDirectory = "maps";

            if (_serverConfig.MapChoices.Length < 1)
            {
                Debug.LogError(string.Format("No maps present in rotation. Unable to proceed!"));
                return;
            }

            MapDataConfig testMap = LoadMap(_serverConfig.MapChoices[0].MapName);  //"testLevel1");
            MapEntityDataConfig testEntities = LoadMapEnities(_serverConfig.MapChoices[0].EntityName);

            ServerGameManager.Singleton.ProcessMap(testMap);
            ServerGameManager.Singleton.ProcessMapEntities(testEntities);
            //ClientGameManager.Singleton.ProcessMap(testMap);
            //ClientGameManager.Singleton.ProcessMapEntities(testEntities);

            //Trigger a collection now so it doesn't kick in later as we just produced a good amount of trash
            System.GC.Collect();

            _initDone = true;
            _hasGameStarted = true;
        }

        #region Config stuff
        private static void LoadConfigFiles()
        {
            _optionsConfig = Config.GetConfig<OptionsConfig>();
            _serverNetConfig = Config.GetConfig<ServerNetConfig>();
            _clientNetConfig = Config.GetConfig<ClientNetConfig>();

            LoadServerConfig();
            ProcessGameManifest();
        }

        private static void LoadServerConfig()
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

            string manifestPath = Path.Combine(path, "manifest.json");

            if (!File.Exists(manifestPath))
            {
                Debug.LogError(string.Format("Can't find manifest.json at '{0}'. Can't proceed!", manifestPath));
            }

            string rawJson = File.ReadAllText(manifestPath);

            _gameManifestConfig = JsonConvert.DeserializeObject<GameManfiest>(rawJson);
        }
        #endregion

        #region Map Data
        public static MapDataConfig LoadMap(string mapName)
        {
            string fileAndPath = string.Format("{0}{1}{2}{1}{3}.json", _appDataDirectory, System.IO.Path.DirectorySeparatorChar, _mapDirectory, mapName);

            if (!File.Exists(fileAndPath))
            {
                Debug.LogWarning(string.Format("Could not find map '{0}'.", fileAndPath));
                return null;
            }

            string rawJson = File.ReadAllText(fileAndPath);

            return JsonConvert.DeserializeObject<MapDataConfig>(rawJson);
        }

        public static MapEntityDataConfig LoadMapEnities(string entityFileName)
        {
            string fileAndPath = string.Format("{0}{1}{2}{1}{3}.json", _appDataDirectory, System.IO.Path.DirectorySeparatorChar, _mapDirectory, entityFileName);

            if (!File.Exists(fileAndPath))
            {
                Debug.LogWarning(string.Format("Could not find entity file: '{0}'.", fileAndPath));
                return null;
            }

            string rawJson = File.ReadAllText(fileAndPath);

            return JsonConvert.DeserializeObject<MapEntityDataConfig>(rawJson);
        }

        public static void SaveMap(MapDataConfig mapData, string mapName)
        {
            string fileAndPath = string.Format("{0}{1}{2}{1}{3}.json", _appDataDirectory, System.IO.Path.DirectorySeparatorChar, _mapDirectory, mapName);

            string jsonText = JsonConvert.SerializeObject(mapData, Formatting.Indented);

            using (FileStream fs = File.Open(fileAndPath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(jsonText);
                }
            }
        }
        #endregion

        #region Server Loading
        /// <summary>
        /// Not the most elegant process, but it copies the default texture to something in memory, so it can be inserted into the runtime texture diction
        /// and cleaned up just like the rest at the end of the game
        /// </summary>
        private static void PrepareDefaultTextures()
        {
            Texture2D defaultCopy = new Texture2D(DefaultFiles.Singleton.defaultTexture.width, DefaultFiles.Singleton.defaultTexture.height, DefaultFiles.Singleton.defaultTexture.format, true);
            Color[] defColors = DefaultFiles.Singleton.defaultTexture.GetPixels();
            defaultCopy.SetPixels(defColors);
            defaultCopy.Apply();
            defaultCopy.name = DefaultFiles.Singleton.defaultTexture.name;
            defaultCopy.filterMode = DefaultFiles.Singleton.defaultTexture.filterMode;
            defaultCopy.wrapMode = DefaultFiles.Singleton.defaultTexture.wrapMode;
            //ClientGameManager.Singleton.Gamedata.AddTexture(defaultCopy.name, defaultCopy);
        }

        private static void ProcessGameManifest()
        {
            if (_gameManifestConfig == null)
            {
                Debug.LogError(string.Format("Game Manifest is null. Can't proceed."));
            }

            //Adding the default texture to the texture dictionary
            PrepareDefaultTextures();

            //Load all the texture data based on the 
            int artProcessed = 0;
            for (int i = 0; i < _gameManifestConfig.ArtFileNames.Length; i++)
            {
                string fileAndPath = Path.Combine(Compiled_ServerModuleDirectory, _gameManifestConfig.ArtFileNames[i]);

                if (!File.Exists(fileAndPath))
                {
                    Debug.LogError(string.Format("Unable to load art file '{0}'. Check manifest.json for accuracy", fileAndPath));
                    continue;
                }

                Texture2D tex = LoadImageFromFileAndPath(fileAndPath);
                tex.name = _gameManifestConfig.ArtFileNames[i];

                //ClientGameManager.Singleton.Gamedata.AddTexture(_gameManifestConfig.ArtFileNames[i], tex);
                artProcessed++;
            }

            //First create the default Sprite Sheet and Sprite
            CreateDefaultSpriteSheetAndSprite();

            //Load Sprite Data and store it into Gamedata
            int spriteFileProcessed = 0;
            int spritesProcessed = ProcessSprites(_gameManifestConfig.SpriteDataFileNames, out spriteFileProcessed);

            //Block Data
            int blockDataProcessed = 0;
            int blockFilesProcessed = ProcessBlockData(_gameManifestConfig.BlockDataFileNames, out blockDataProcessed);

            //Entity Data
            int entityDataProcessed = 0;
            int entityFilesProcessed = ProcessEntityData(_gameManifestConfig.EntityDataFileNames, out entityDataProcessed);

            string results = string.Format("Game Manifest Done! Processed: {0} Art Files, {1} Sprite Files and {2} Sprites, {3} Game Block Files and {4} Game Blocks, {5} Entity Data Files and {6} Entities", artProcessed, spriteFileProcessed, spritesProcessed, blockFilesProcessed, blockDataProcessed, entityFilesProcessed, entityDataProcessed);
            Debug.Log(results);
        }

        public static bool AttemptCreateSpriteSheet(SpriteDataJson spriteData, string sheetName, EDSSSpriteSheet.ShaderType desiredType, out EDSSSpriteSheet sheet)
        {
            sheet = null;

            ////If we arrived here, then we probably couldn't find the sheet by texture name. We're going to assume the textures have all successfully been loaded by now
            ////So we'll first look at the textures to see if we can make a sheet from it
            //Texture2D texture = null;
            //bool found = ClientGameManager.Singleton.Gamedata.GetTexture(spriteData.SpriteSheetFileName, out texture);

            //if (!found)
            //{
            //    Debug.LogError(string.Format("Attempted to locate texture '{0}' for sprite '{1}' but failed.", spriteData.SpriteSheetFileName, spriteData));
            //    return false;
            //}

            //sheet = new EDSSSpriteSheet();
            //uint uid = ClientGameManager.Singleton.Gamedata.GetNewSpriteSheetUID();
            //uint matUID = ClientGameManager.Singleton.Gamedata.GetNewMaterialUID();

            ////Create a new material and assign the texture
            //Material newMat;

            //if (desiredType == EDSSSpriteSheet.ShaderType.Billboard)
            //{
            //    newMat = new Material(DefaultFiles.Singleton.billboardShader);
            //}
            //else if (desiredType == EDSSSpriteSheet.ShaderType.TwoSidedSprite)
            //{
            //    newMat = new Material(DefaultFiles.Singleton.twoSidedSpriteShader);
            //}
            //else
            //{
            //    newMat = new Material(DefaultFiles.Singleton.defaultShader);
            //}

            //newMat.name = string.Format("{0}", sheetName);
            //newMat.SetTexture("_MainTex", texture);
            //ClientGameManager.Singleton.Gamedata.AddMaterial(matUID, newMat);

            //sheet.CreateSpriteSheet(uid, matUID, texture, newMat, null);

            //ClientGameManager.Singleton.Gamedata.AddSpriteSheet(uid, sheet);

            return true;
        }

        public static void CreateDefaultSpriteSheetAndSprite()
        {
            //EDSSSpriteSheet sheet = new EDSSSpriteSheet();
            //uint uid = ClientGameManager.Singleton.Gamedata.GetNewSpriteSheetUID();
            //uint matUID = ClientGameManager.Singleton.Gamedata.GetNewMaterialUID();

            //Material newMat = new Material(DefaultFiles.Singleton.defaultShader);

            //Texture2D texture = null;
            ////We'll use the same name since we copied that
            //ClientGameManager.Singleton.Gamedata.GetTexture(DefaultFiles.Singleton.defaultTexture.name, out texture);

            //newMat.name = string.Format("{0}", texture);
            //newMat.SetTexture("_MainTex", texture);
            //ClientGameManager.Singleton.Gamedata.AddMaterial(matUID, newMat);

            //sheet.CreateSpriteSheet(uid, matUID, texture, newMat, null);

            //ClientGameManager.Singleton.Gamedata.AddSpriteSheet(uid, sheet);

            ////Create the default sprite
            //SpriteDataJson defaultSpriteJson = new SpriteDataJson();
            //defaultSpriteJson.UID = 0;
            //defaultSpriteJson.SpriteName = "Default";
            //defaultSpriteJson.SpriteSheetFileName = texture.name;
            //defaultSpriteJson.SpritePosition = new Vec2Int(0, 0);
            //defaultSpriteJson.SpritePosition = new Vec2Int(texture.width, texture.height);

            //EDSSSprite newSprite = CreateSprite(defaultSpriteJson);
            //EveryDaySpaceStation.GameData.DefaultSprite = newSprite;
            //ClientGameManager.Singleton.Gamedata.AddSprite(newSprite.UID, newSprite);
        }
        #endregion

        #region Game Block Processing
        private static int ProcessBlockData(string[] blockDataFileNames, out int blockFilesProcessed)
        {
            int blockDataProcessed = 0;
            blockFilesProcessed = 0;
            for (int i = 0; i < blockDataFileNames.Length; i++)
            {
                string fileAndPath = Path.Combine(Compiled_ServerModuleDirectory, blockDataFileNames[i]);

                if (!File.Exists(fileAndPath))
                {
                    Debug.LogError(string.Format("Unable to load block data file '{0}'. Check manifest.json for accuracy", fileAndPath));
                    continue;
                }

                string rawJson = File.ReadAllText(fileAndPath);

                GameBlockDataConfig blockConfig = JsonConvert.DeserializeObject<GameBlockDataConfig>(rawJson);

                if (blockConfig == null)
                {
                    Debug.LogError(string.Format("Problem loading block data config for '{0}'. Please double check it.", fileAndPath));
                    continue;
                }

                //for (int j = 0; j < blockConfig.BlockData.Length; j++)
                //{
                //    BlockDataJson blockData = blockConfig.BlockData[j];

                //    GameData.GameBlockData newBlock = new GameData.GameBlockData(blockData.UID, blockData.Name, blockData.DefaultStrength, blockData.Flags, blockData.Requirement);
                //    newBlock.SetFaceParameters(blockData.FaceZForward, blockData.FaceXForward, blockData.FaceZBack, blockData.FaceXBack, blockData.FaceTop, blockData.FaceBottom);

                //    //ClientGameManager.Singleton.Gamedata.AddGameBlock(newBlock.UID, newBlock);

                //    blockDataProcessed++;
                //}

                blockFilesProcessed++;
            }

            return blockDataProcessed;
        }
        #endregion

        #region Entity Data Processing
        private static int ProcessEntityData(string[] entityDataFilesNames, out int entityFilesProcessed)
        {
            int entityDataProcessed = 0;
            entityFilesProcessed = 0;
            for (int i = 0; i < entityDataFilesNames.Length; i++)
            {
                string fileAndPath = Path.Combine(Compiled_ServerModuleDirectory, entityDataFilesNames[i]);

                if (!File.Exists(fileAndPath))
                {
                    Debug.LogError(string.Format("Unable to load entity data file '{0}'. Check manifest.json for accuracy", fileAndPath));
                    continue;
                }

                string rawJson = File.ReadAllText(fileAndPath);

                EntityDataConfig entityConfig = JsonConvert.DeserializeObject<EntityDataConfig>(rawJson);

                if (entityConfig == null)
                {
                    Debug.LogError(string.Format("Problem loading entity data config for '{0}'. Please double check it.", fileAndPath));
                    continue;
                }

                for (int j = 0; j < entityConfig.EntityData.Length; j++)
                {
                    EntityDataJson entityData = entityConfig.EntityData[j];

                    //GameData.EntityDataTemplate newEntity = new GameData.EntityDataTemplate(entityData.UID, entityData.EntityName, entityData.EntityTypeFlags, entityData.EntityStates);
                    //newEntity.ParseLightStates(entityData.EntityLightStates);
                    //newEntity.ParseFixedStates(entityData.EntityFixedStates);
                    //newEntity.ParsePoweredStates(entityData.EntityPoweredStates);
                    //newEntity.ParseDeviceStates(entityData.EntityDeviceStates);
                    //newEntity.ParseCraftStates(entityData.EntityCraftStates);
                    //newEntity.ParseMultiAngleStates(entityData.EntityMultiAngleStates);
                    //newEntity.ParseDoorStates(entityData.EntityDoorState);
                    //newEntity.ParseContainerStates(entityData.EntityContainerState);

                    //ClientGameManager.Singleton.Gamedata.AddEntityTemplate(newEntity.UID, newEntity);

                    entityDataProcessed++;
                }

                entityFilesProcessed++;
            }

            return entityDataProcessed;
        }
        #endregion

        #region Sprite Data Processing
        private static int ProcessSprites(string[] spriteDatafileNames, out int spriteFileProcess)
        {
            int spriteProcessed = 0;
            spriteFileProcess = 0;
            for (int i = 0; i < spriteDatafileNames.Length; i++)
            {
                string fileAndPath = Path.Combine(Compiled_ServerModuleDirectory, spriteDatafileNames[i]);

                if (!File.Exists(fileAndPath))
                {
                    Debug.LogError(string.Format("Unable to load spritedata file '{0}'. Check manifest.json for accuracy", fileAndPath));
                    continue;
                }

                string rawJson = File.ReadAllText(fileAndPath);

                SpriteDataConfig spriteConfig = JsonConvert.DeserializeObject<SpriteDataConfig>(rawJson);

                if (spriteConfig == null)
                {
                    Debug.LogError(string.Format("Problem loading sprite data config for '{0}'. Please double check it.", fileAndPath));
                    continue;
                }

                //Now that we have the sprite data file loaded and it appears legit, we need to go through each sprite and try to turn it into a EDSSSprite
                for (int j = 0; j < spriteConfig.SpriteData.Length; j++)
                {
                    SpriteDataJson spriteData = spriteConfig.SpriteData[j];

                    EDSSSprite newSprite = CreateSprite(spriteData);

                    if (newSprite == null)
                        continue;

                    //ClientGameManager.Singleton.Gamedata.AddSprite(newSprite.UID, newSprite);
                    spriteProcessed++;
                }

                spriteFileProcess++;
            }

            return spriteProcessed;
        }

        public static EDSSSprite CreateSprite(SpriteDataJson spriteData)
        {
            //string sheetTypeName = "world";
            //EDSSSpriteSheet.ShaderType shaderType = EDSSSpriteSheet.ShaderType.World;
            ////We've got to process the flags here as this will determine what kind of material. Hopefully they're well clustered so we don't create excess amounts of materials
            //if (spriteData.Flags != null)
            //{
            //    for (int k = 0; k < spriteData.Flags.Length; k++)
            //    {
            //        string flag = spriteData.Flags[k].ToLower();

            //        switch (flag)
            //        {
            //            case "billboard":
            //                sheetTypeName = "billboard";
            //                shaderType = EDSSSpriteSheet.ShaderType.Billboard;
            //                break;

            //            case "twosided":
            //                sheetTypeName = "twosided";
            //                shaderType = EDSSSpriteSheet.ShaderType.TwoSidedSprite;
            //                break;
            //        }
            //    }
            //}

            ////First look if there is a EDSSSpriteSheet
            //EDSSSpriteSheet sheet = null;
            //string sheetname = string.Format("{0}-{1}", spriteData.SpriteSheetFileName, sheetTypeName);
            //bool found = ClientGameManager.Singleton.Gamedata.GetSpriteSheet(sheetname, out sheet);

            ////Couldn't find the sheet already created
            //if (!found)
            //{
            //    //So attempt to create
            //    found = AttemptCreateSpriteSheet(spriteData, sheetname, shaderType, out sheet);
            //}

            ////It's still not found, so we need to throw an error and move on
            //if (!found)
            //{
            //    Debug.LogError(string.Format("Could not locate a spritesheet for sprite '{0}'. Giving up.", spriteData));
            //    return null;
            //}

            ////We've got a sheet somehow, so we're ready to make the sprite
            //EDSSSprite newSprite = sheet.CreateSprite(spriteData.UID, spriteData.SpritePosition, spriteData.SpriteWidthHeight, spriteData.SpriteName, spriteData.Flags);

            //return newSprite;
            return null;
        }
        #endregion
    }
}