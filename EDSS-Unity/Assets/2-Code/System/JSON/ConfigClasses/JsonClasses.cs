//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// This file contains the json compatible data classes
//////////////////////////////////////////////////////////////////////////////////////////
// FileSystemConfig - Class that contains path data to be used by JSON
// Created: December 3 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 4 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////
// ServerNetConfig - Class that contains the network configuration data for a server
// Created: December 4 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 4 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////
// ClientNetConfig - Class that contains the network configuration data for a client
// Created: December 4 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 4 2015
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

namespace EveryDaySpaceStation.Json
{
    #region Map Data
    [JsonConverter(typeof(MapDataJsonConverter))]
    public class MapDataConfig
    {
        [JsonProperty("mapdata")]
        public MapDataJson LevelData { get; set; }
    }

    public class MapDataJson
    {
        [JsonProperty("mapname")]
        public string LevelName { get; set; }

        [JsonProperty("displayname")]
        public string DisplayName { get; set; }

        [JsonProperty("mapformat")]
        public int MapFormat { get; set; }

        [JsonProperty("mapversion")]
        public string MapVersion { get; set; }

        [JsonProperty("createtime")]
        public int CreateTime { get; set; }

        [JsonProperty("lastmodifiedtime")]
        public int LastModifiedTime { get; set; }

        [JsonProperty("anchorpoint")]
        public Vec2Int AnchorPoint { get; set; }

        [JsonProperty("size")]
        public Vec2Int Size { get; set; }

        [JsonProperty("tiledata")]
        public TileDataJson[] TileData { get; set; }
    }

    public class TileDataJson
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("scaffold")]
        public int Scaffold { get; set; }

        [JsonProperty("floor")]
        public int Floor { get; set; }

        [JsonProperty("block")]
        public int Block { get; set; }

        [JsonProperty("blocklight")]
        public int BlockLight { get; set; }
    }

    public class MapDataJsonConverter : JsonConverter
    {
        bool CannotWrite { get; set; }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanConvert(System.Type objectType)
        {
            return typeof(MapDataConfig).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            
            using (reader = obj.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                existingValue = (existingValue ?? new MapDataConfig());
                serializer.Populate(reader, existingValue);
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            (writer as JsonTextWriter).IndentChar = '\t';
            (writer as JsonTextWriter).Indentation = 1;
            // Disabling writing prevents infinite recursion.
            using (new PushValue<bool>(true, () => CannotWrite, val => CannotWrite = val))
            {
                JObject obj = JObject.FromObject(value, serializer);
                //JObject details = new JObject();
                                
                obj.WriteTo(writer);
            }
        }
    }
    #endregion

    #region FileSystem
    //http://stackoverflow.com/questions/30175911/can-i-serialize-nested-properties-to-my-class-in-one-operation-with-json-net
    [JsonConverter(typeof(FileSystemConfigConverter))]
    public class FileSystemConfig
    {
        [JsonProperty("optionsfile")]
        public string OptionsFile { get; set; }
        [JsonProperty("mapdir")]
        public string MapDir { get; set; }
        [JsonProperty("clientdir")]
        public string ClientDir { get; set; }
        [JsonProperty("serverdir")]
        public string ServerDir { get; set; }

        [JsonProperty("clientnetconfigfile")]
        public string ClientNetConfigFile { get; set; }
        [JsonProperty("servernetconfigfile")]
        public string ServerNetConfigFile { get; set; }
    }

    public class FileSystemConfigConverter : JsonConverter
    {
        bool CannotWrite { get; set; }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanConvert(System.Type objectType)
        {
            return typeof(FileSystemConfig).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            obj.SelectToken("filesystemconfig.optionsfile").MoveTo(obj);
            obj.SelectToken("filesystemconfig.mapdir").MoveTo(obj);
            obj.SelectToken("filesystemconfig.clientdir").MoveTo(obj);
            obj.SelectToken("filesystemconfig.serverdir").MoveTo(obj);
            obj.SelectToken("filesystemconfig.clientnetconfigfile").MoveTo(obj);
            obj.SelectToken("filesystemconfig.servernetconfigfile").MoveTo(obj);
            using (reader = obj.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                existingValue = (existingValue ?? new FileSystemConfig());
                serializer.Populate(reader, existingValue);
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            (writer as JsonTextWriter).IndentChar = '\t';
            (writer as JsonTextWriter).Indentation = 1;

            // Disabling writing prevents infinite recursion.
            using (new PushValue<bool>(true, () => CannotWrite, val => CannotWrite = val))
            {
                JObject obj = JObject.FromObject(value, serializer);
                JObject details = new JObject();

                obj.Add("filesystemconfig", details);
                obj["optionsfile"].MoveTo(details);
                obj["mapdir"].MoveTo(details);
                obj["clientdir"].MoveTo(details);
                obj["serverdir"].MoveTo(details);
                obj["clientnetconfigfile"].MoveTo(details);
                obj["servernetconfigfile"].MoveTo(details);

                obj.WriteTo(writer);
            }
        }
    }
    #endregion

    #region Options
    [JsonConverter(typeof(OptionsConfigConverter))]
    public class OptionsConfig
    {
        [JsonProperty("chunksize")]
        public int SceneChunkSize { get; set; }
    }

    public class OptionsConfigConverter : JsonConverter
    {
        bool CannotWrite { get; set; }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanConvert(System.Type objectType)
        {
            return typeof(OptionsConfig).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            obj.SelectToken("options.chunksize").MoveTo(obj);
            using (reader = obj.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                existingValue = (existingValue ?? new OptionsConfig());
                serializer.Populate(reader, existingValue);
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            (writer as JsonTextWriter).IndentChar = '\t';
            (writer as JsonTextWriter).Indentation = 1;

            // Disabling writing prevents infinite recursion.
            using (new PushValue<bool>(true, () => CannotWrite, val => CannotWrite = val))
            {
                JObject obj = JObject.FromObject(value, serializer);
                JObject details = new JObject();

                obj.Add("options", details);
                obj["chunksize"].MoveTo(details);
                obj.WriteTo(writer);
            }
        }
    }
    #endregion

    #region Server Network
    [JsonConverter(typeof(ServerNetConfigConverter))]
    public class ServerNetConfig
    {

    }

    public class ServerNetConfigConverter : JsonConverter
    {
        bool CannotWrite { get; set; }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanConvert(System.Type objectType)
        {
            return typeof(FileSystemConfig).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            //obj.SelectToken("filesystemconfig.mapdir").MoveTo(obj);
            //obj.SelectToken("filesystemconfig.clientdir").MoveTo(obj);
            //obj.SelectToken("filesystemconfig.serverdir").MoveTo(obj);
            //obj.SelectToken("filesystemconfig.clientnetconfigfile").MoveTo(obj);
            //obj.SelectToken("filesystemconfig.servernetconfigfile").MoveTo(obj);
            using (reader = obj.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                existingValue = (existingValue ?? new FileSystemConfig());
                serializer.Populate(reader, existingValue);
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Disabling writing prevents infinite recursion.
            using (new PushValue<bool>(true, () => CannotWrite, val => CannotWrite = val))
            {
                JObject obj = JObject.FromObject(value, serializer);
                //JObject details = new JObject();

                //obj.Add("filesystemconfig", details);
                //obj["mapdir"].MoveTo(details);
                //obj["clientdir"].MoveTo(details);
                //obj["serverdir"].MoveTo(details);
                //obj["clientnetconfigfile"].MoveTo(details);
                //obj["servernetconfigfile"].MoveTo(details);

                obj.WriteTo(writer);
            }
        }
    }
    #endregion

    #region Client Network
    [JsonConverter(typeof(ClientNetConfigConverter))]
    public class ClientNetConfig
    {

    }

    public class ClientNetConfigConverter : JsonConverter
    {
        bool CannotWrite { get; set; }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanConvert(System.Type objectType)
        {
            return typeof(FileSystemConfig).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            //obj.SelectToken("filesystemconfig.mapdir").MoveTo(obj);
            //obj.SelectToken("filesystemconfig.clientdir").MoveTo(obj);
            //obj.SelectToken("filesystemconfig.serverdir").MoveTo(obj);
            //obj.SelectToken("filesystemconfig.clientnetconfigfile").MoveTo(obj);
            //obj.SelectToken("filesystemconfig.servernetconfigfile").MoveTo(obj);
            using (reader = obj.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                existingValue = (existingValue ?? new FileSystemConfig());
                serializer.Populate(reader, existingValue);
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Disabling writing prevents infinite recursion.
            using (new PushValue<bool>(true, () => CannotWrite, val => CannotWrite = val))
            {
                JObject obj = JObject.FromObject(value, serializer);
                //JObject details = new JObject();

                //obj.Add("filesystemconfig", details);
                //obj["mapdir"].MoveTo(details);
                //obj["clientdir"].MoveTo(details);
                //obj["serverdir"].MoveTo(details);
                //obj["clientnetconfigfile"].MoveTo(details);
                //obj["servernetconfigfile"].MoveTo(details);

                obj.WriteTo(writer);
            }
        }
    }
    #endregion
}