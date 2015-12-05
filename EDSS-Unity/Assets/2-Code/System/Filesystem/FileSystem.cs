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

namespace EveryDaySpaceStation
{
    public static partial class FileSystem
    {
        #region Vars
        static bool _initDone;
        static bool _hasGameStarted;

        static string _appDataDirectory = Application.persistentDataPath; //Where all the app data outside of Unity, is located, e.g.: C:/Users/<User>/AppData/LocalLow/EveryDaySpaceStation/EveryDaySpaceStation
        static string _clientDataDirectory = string.Empty; //Where the current client data is being stored, e.g.: C:/Users/<User>/AppData/LocalLow/EveryDaySpaceStation/EveryDaySpaceStation/client
        static string _serverDataDirectory = string.Empty; //Where the current server data is being stored (if at all), e.g.: C:/Users/<User>/AppData/LocalLow/EveryDaySpaceStation/EveryDaySpaceStation/server
        static string _clientGameDataDirectory = string.Empty; //Where the current game's client data is being stored, e.g.: C:/Users/<User>/AppData/LocalLow/EveryDaySpaceStation/EveryDaySpaceStation/client/bobsserver
        static string _mapDirectory = string.Empty; //Where the maps are located (for editing or running a server), e.g.: C:/Users/<User>/AppData/LocalLow/EveryDaySpaceStation/EveryDaySpaceStation/maps

        static FileSystemConfig _fsConfig = null;
        static OptionsConfig _optionsConfig = null;
        //static ServerNetConfig _serverNetConfig = null;
        //static ClientNetConfig _clientNetConfig = null;
        #endregion

        #region Gets/Sets
        public static string AppDataDirectory { get { return _appDataDirectory; } }
        public static string ClientDataDirectory { get { return _clientDataDirectory; } }
        public static string ServerDataDirectory { get { return _serverDataDirectory; } }
        public static string ClientGameDataDirectory { get { return _clientGameDataDirectory; } }
        public static string MapDirectory { get { return _mapDirectory; } }

        public static bool InitDone { get { return _initDone; } }
        public static bool HasGameStarted 
        { 
            get { return _hasGameStarted; }
            set { _hasGameStarted = value; }
        }
        #endregion

        public static void Init()
        {
            _fsConfig = Config.Get<FileSystemConfig>("filesystemconfig");

            //if(_fsConfig 

            //LoadConfigFiles();

            //TODO Load this properly
            _mapDirectory = "maps";

            MapDataConfig testMap = LoadMap("testLevel1");

            testMap.LevelData.LevelName = "HAHAHA TEST";

            SaveMap(testMap, "fartlevel1");

            _initDone = true;
            _hasGameStarted = true;
        }

        #region Image Stuff
        static public Texture2D LoadImageFromFile(string fileName, TextureFormat imgFormat = TextureFormat.ARGB32)
        {
            string fileAndPath = string.Format("{0}{1}{2}{1}{3}{1}{4}", _appDataDirectory, System.IO.Path.DirectorySeparatorChar, _clientDataDirectory, _clientGameDataDirectory, fileName);

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

            Debug.Log(string.Format("Loaded image {0} as ({1}w {2}h and format '{3}'", fileAndPath, newTexture.width, newTexture.height, imgFormat));
            newTexture.name = fileName;

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

            JsonSerializerSettings jss = new JsonSerializerSettings();
            //jss.

            using (FileStream fs = File.Open(fileAndPath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(jsonText);
                }
            }
        }
        #endregion
    }
}