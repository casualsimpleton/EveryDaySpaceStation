//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// Config - A class for reading/writing and processing data from JSON files
// Based off https://github.com/enBask/Asgard/blob/515c3ed8e26d974b7242526d291274b9c6857aef/Asgard.Core/Utils/Config.cs
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

namespace EveryDaySpaceStation.Utils
{
    public sealed class Config
    {
        #region Vars
        private static Dictionary<string, object> _loadedConfigs = new Dictionary<string, object>();
        #endregion

        #region Public
        public static T GetConfig<T>()
        {
            object loadedConfig = null;
            string key = typeof(T).Name.ToString().ToLower();
            bool alreadyLoaded = _loadedConfigs.TryGetValue(key, out loadedConfig);

            //That config is already loaded for that type, so return it
            if (alreadyLoaded)
            {
                return (T)loadedConfig;
            }

            //Else we have to load it
            //If it's the main config, we need to handle that specially
            if (typeof(T) == typeof(FileSystemConfig))
            {
                loadedConfig = LoadConfig<FileSystemConfig>("config.json");                
            }
            else
            {
                //It's another config, so we first have to make sure the main config FileSystemConfig "config.json" is loaded
                FileSystemConfig fsc = GetConfig<FileSystemConfig>();
                string fileName = string.Empty;

                switch (key)
                {
                    case "optionsconfig":
                        fileName = fsc.OptionsFile;
                        loadedConfig = LoadConfig<OptionsConfig>(fileName);
                        break;

                    case "servernetconfig":
                        fileName = fsc.ServerNetConfigFile;
                        loadedConfig = LoadConfig<ServerNetConfig>(fileName);
                        break;

                    case "clientnetconfig":
                        fileName = fsc.ClientNetConfigFile;
                        loadedConfig = LoadConfig<ClientNetConfig>(fileName);
                        break;
                }
            }

            _loadedConfigs.Add(key, loadedConfig);

            return (T)loadedConfig;
        }

        //Clear the dictionary so that the next time a GetConfig<T>() is called, it'll reload it all
        public static void ForceReload()
        {
            _loadedConfigs.Clear();
        }

        private static T LoadConfig<T>(string fileNameWithExt)
        {
            string fileAndPath = Path.Combine(FileSystem.AppDataDirectory, fileNameWithExt);

            if (!File.Exists(fileAndPath))
            {
                Debug.LogError(string.Format("Can't find config file '{0}'.", fileAndPath));
                return default(T);
            }

            string rawJson = File.ReadAllText(fileAndPath);
            T classInstance = JsonConvert.DeserializeObject<T>(rawJson);

            return classInstance;
        }

        //TODO FIX THIS
        //public static void SaveConfig<T>(T configToSave)
        //{
        //    if(!File.Exists(_configFile))
        //    {
        //        string basePath = FileSystem.AppDataDirectory;
        //        _configFile = Path.Combine(basePath, _configFile);
        //    }

        //    using (FileStream fs = File.Open(_configFile, FileMode.OpenOrCreate))
        //    {
        //        using (StreamWriter sw = new StreamWriter(fs))
        //        {
        //            using (JsonWriter jw = new JsonTextWriter(sw))
        //            {
        //                jw.Formatting = Formatting.Indented;
        //                JsonSerializer serializer = new JsonSerializer();
        //                serializer.Serialize(jw, configToSave);
        //            }
        //        }
        //    }
        //}
        
        #endregion
    }
}