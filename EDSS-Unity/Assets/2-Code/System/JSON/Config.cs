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

namespace EveryDaySpaceStation.Utils
{
    public sealed class Config
    {
        #region Vars
        private static string _configFile = "config.json";
        private static bool _loaded = false;
        private static object _lockObject = new object();
        private static JObject _jsonObject = null;

        private static Dictionary<System.Type, object> _loadedConfigs = new Dictionary<System.Type,object>();
        #endregion

        #region Public
        //public static void Load

        public static void Load(string config = "")
        {
            if (!string.IsNullOrEmpty(config))
            {
                _configFile = config;
            }

            if (!File.Exists(_configFile))
            {
                string basePath = FileSystem.AppDataDirectory;
                _configFile = Path.Combine(basePath, _configFile);

                //Can't find the file
                if (!File.Exists(_configFile))
                {
                    _jsonObject = new JObject();
                    return;
                }
            }

            //Read all text from file and pass it to the Json object to parse
            string rawJson = File.ReadAllText(_configFile);
            _jsonObject = JObject.Parse(rawJson);

            if (_jsonObject == null)
            {
                throw new System.Exception("bad config data");
            }
        }

        public static string GetString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new System.ArgumentNullException();
            }

            EnsureLoaded();

            JToken token = null;
            bool tokenFound = _jsonObject.TryGetValue(key, out token);
            if (!tokenFound)
            {
                return string.Empty;
            }

            return token.ToString();
        }

        public static int GetInt(string key)
        {
            string result = GetString(key);
            int intResult;
            bool intSuc = int.TryParse(result, out intResult);

            if (!intSuc)
            {
                return 0;
            }

            return intResult;
        }

        //public static T Get<T>(string key)
        //{
        //    string json = GetString(key);
        //    T result = JsonConvert.DeserializeObject<T>(json);
        //    return result;
        //}
        public static T Get<T>(string fileNameWithExt)
        {
            string fileAndPath = Path.Combine(FileSystem.AppDataDirectory, fileNameWithExt);

            if (!File.Exists(fileAndPath))
            {
                Debug.LogError(string.Format("Can't find config file '{0}'.", fileAndPath));
                return default(T);
            }

            string rawJson = File.ReadAllText(fileAndPath);
            T jsonObject = JsonConvert.DeserializeObject<T>(rawJson);

            return jsonObject;
        }

        public static void SaveConfig<T>(T configToSave)
        {
            if(!File.Exists(_configFile))
            {
                string basePath = FileSystem.AppDataDirectory;
                _configFile = Path.Combine(basePath, _configFile);
            }

            using (FileStream fs = File.Open(_configFile, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    using (JsonWriter jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = Formatting.Indented;
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(jw, configToSave);
                    }
                }
            }
        }
        
        #endregion

        private static void EnsureLoaded()
        {
            if (!_loaded) //quick optimization around locking once loaded
            {
                lock (_lockObject)
                {
                    if (!_loaded) //check for race condition since first check was outside lock
                    {
                        _loaded = true;
                        Load();
                    }
                }
            }
        }
    }
}