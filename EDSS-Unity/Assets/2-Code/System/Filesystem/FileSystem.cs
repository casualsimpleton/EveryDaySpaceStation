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

            MapDataConfig testMap = LoadMap(_serverConfig.MapChoices[0]);  //"testLevel1");

            GameManager.Singleton.ProcessMap(testMap);

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

        #region Image Stuff
        static public Texture2D LoadImageFromFileName(string fileName, TextureFormat imgFormat = TextureFormat.ARGB32)
        {
            string fileAndPath = string.Format("{0}{1}{2}{1}{3}{1}{4}", _appDataDirectory, System.IO.Path.DirectorySeparatorChar, _clientDataDirectory, _clientGameDataDirectory, fileName);

            return LoadImageFromFileAndPath(fileAndPath, imgFormat);
        }

        static public Texture2D LoadImageFromFileAndPath(string fileAndPath, TextureFormat imgFormat = TextureFormat.ARGB32)
        {           
            if (!File.Exists(fileAndPath))
            {
                Debug.LogWarning(string.Format("Could not find art piece '{0}'.", fileAndPath));
                return null;
            }

            Texture2D newTexture = new Texture2D(4, 4, imgFormat, false, false);

            byte[] byteData = null;
            using (FileStream fs = File.OpenRead(fileAndPath))
            {
                byteData = new byte[fs.Length];
                ReadWholeDataStreamArray(fs, ref byteData);
            }

            newTexture.LoadImage(byteData);

            Debug.Log(string.Format("Loaded image {0} as ({1}w {2}h) and format '{3}'", fileAndPath, newTexture.width, newTexture.height, imgFormat));

            newTexture.filterMode = FilterMode.Point;

            return newTexture;
        }

        /// <summary>
        /// Reads data into a complete array, throwing an EndOfStreamException
        /// if the stream runs out of data first, or if an IOException
        /// naturally occurs.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="data">The array to read bytes into. The array
        /// will be completely filled from the stream, so an appropriate
        /// size must be given.</param>
        static public void ReadWholeDataStreamArray(Stream stream, ref byte[] data)
        {
            int offset = 0;
            int remaining = data.Length;
            while (remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                {
                    throw new EndOfStreamException(string.Format("End of stream reached with {0} bytes left to read", remaining));
                }

                remaining -= read;
                offset += read;
            }
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
        private static void ProcessGameManifest()
        {
            if (_gameManifestConfig == null)
            {
                Debug.LogError(string.Format("Game Manifest is null. Can't proceed."));
            }

            //Load all the texture data based on the 
            int artProcessed = 0;
            for (int i = 0; i < _gameManifestConfig.ArtFileNames.Length; i++)
            {
                string fileAndPath = Path.Combine(Compiled_ServerModuleDirectory, _gameManifestConfig.ArtFileNames[i]);

                if (!File.Exists(fileAndPath))
                {
                    Debug.LogError(string.Format("Unable to load art file '{0}'. Check manifest.json for accuracy"));
                    continue;
                }

                Texture2D tex = LoadImageFromFileAndPath(fileAndPath);
                tex.name = _gameManifestConfig.ArtFileNames[i];

                GameManager.Singleton.Gamedata.AddTexture(_gameManifestConfig.ArtFileNames[i], tex);
                artProcessed++;
            }

            //Load Sprite Data and store it into Gamedata
            int spriteFileProcessed = 0;
            int spritesProcessed = ProcessSprites(_gameManifestConfig.SpriteDataFileNames, out spriteFileProcessed);

            //Block Data
            int blockDataProcessed = 0;
            int blockFilesProcessed = ProcessBlockData(_gameManifestConfig.BlockDataFileNames, out blockDataProcessed);

            string results = string.Format("Game Manifest Done! Processed: {0} Art Files, {1} Sprite Files and {2} Sprites, {3} Game Block Files and {4} Game Blocks", artProcessed, spriteFileProcessed, spritesProcessed, blockFilesProcessed, blockDataProcessed);
            Debug.Log(results);
        }

        public static bool AttemptCreateSpriteSheet(SpriteDataJson spriteData, out EDSSSpriteSheet sheet)
        {
            sheet = null;

            //If we arrived here, then we probably couldn't find the sheet by texture name. We're going to assume the textures have all successfully been loaded by now
            //So we'll first look at the textures to see if we can make a sheet from it
            Texture2D texture = null;
            bool found = GameManager.Singleton.Gamedata.GetTexture(spriteData.SpriteSheetFileName, out texture);

            if (!found)
            {
                Debug.LogError(string.Format("Attempted to locate texture '{0}' for sprite '{1}' but failed.", spriteData.SpriteSheetFileName, spriteData));
                return false;
            }

            sheet = new EDSSSpriteSheet();
            uint uid = GameManager.Singleton.Gamedata.GetNewSpriteSheetUID();
            sheet.CreateSpriteSheet(uid, texture, null);

            GameManager.Singleton.Gamedata.AddSpriteSheet(uid, sheet);

            return true;
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

                for (int j = 0; j < blockConfig.BlockData.Length; j++)
                {
                    BlockDataJson blockData = blockConfig.BlockData[j];

                    GameData.GameBlockData newBlock = new GameData.GameBlockData(blockData.UID, blockData.Name, blockData.DefaultStrength, blockData.Flags, blockData.Requirement);

                    GameManager.Singleton.Gamedata.AddGameBlock(newBlock.UID, newBlock);

                    blockDataProcessed++;
                }

                blockFilesProcessed++;
            }

            return blockDataProcessed;
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

                    //First look if there is a EDSSSpriteSheet
                    EDSSSpriteSheet sheet = null;
                    bool found = GameManager.Singleton.Gamedata.GetSpriteSheet(spriteData.SpriteSheetFileName, out sheet);

                    //Couldn't find the sheet already created
                    if (!found)
                    {
                        //So attempt to create
                        found = AttemptCreateSpriteSheet(spriteData, out sheet);
                    }

                    //It's still not found, so we need to throw an error and move on
                    if (!found)
                    {
                        Debug.LogError(string.Format("Could not locate a spritesheet for sprite '{0}'. Giving up.", spriteData));
                        continue;
                    }

                    //We've got a sheet somehow, so we're ready to make the sprite
                    EDSSSprite newSprite = sheet.CreateSprite(spriteData.UID, spriteData.SpritePosition, spriteData.SpriteWidthHeight, spriteData.SpriteName);

                    GameManager.Singleton.Gamedata.AddSprite(newSprite.UID, newSprite);
                    spriteProcessed++;
                }

                spriteFileProcess++;
            }

            return spriteProcessed;
        }
        #endregion
    }
}