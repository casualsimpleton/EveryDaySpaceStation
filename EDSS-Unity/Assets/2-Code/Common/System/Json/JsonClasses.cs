//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// This file contains the json compatible data classes
//////////////////////////////////////////////////////////////////////////////////////////
// Created: December 3 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 16 2015
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

        [JsonProperty("defaultambient")]
        public Color32 AmbientLightColor { get; set; }

        [JsonProperty("tiledata")]
        public TileDataJson[] TileData { get; set; }
    }

    public class TileDataJson
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("facespritesuid")]
        public uint?[] FaceSpriteUID { get; set; }

        [JsonProperty("hasscaffold")]
        public byte HasScaffold { get; set; }

        [JsonProperty("floor")]
        public uint FloorUID { get; set; }

        [JsonProperty("wall")]
        public uint WallUID { get; set; }

        [JsonProperty("block")]
        public uint Block { get; set; }

        [JsonProperty("overrideblocklight")]
        public int OverrideBlockLight { get; set; }
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

    #region Map Entity Data
    [JsonConverter(typeof(MapEntityDataJsonConverter))]
    public class MapEntityDataConfig
    {
        [JsonProperty("mapdataentities")]
        public MapEntityJson[] EntityInstanceData { get; set; }
    }

    public class MapEntityJson
    {
        /// <summary>
        /// This should be unique to the map and this particular entity configuration file
        /// </summary>
        [JsonProperty("mapentityuid")]
        public uint MapEntityInstanceUID { get; set; }

        /// <summary>
        /// This should match the UID of one of the entities listed in manifest.json, probably won't be unique
        /// </summary>
        [JsonProperty("entitytemplateuid")]
        public uint EntityTemplateUID { get; set; }

        /// <summary>
        /// Name of entity as overriden by map's entity entry. Otherwise it'll use template based name
        /// </summary>
        [JsonProperty("overridename")]
        public string EntityOverrideName { get; set; }

        /// <summary>
        /// The tile that this entity will start associated with
        /// </summary>
        [JsonProperty("tilepos")]
        public Vec2Int EntityTilePos { get; set; }

        /// <summary>
        /// What wall (if any) this entity is anchored to
        /// </summary>
        [JsonProperty("anchor")]
        public string AnchorType { get; set; }

        /// <summary>
        /// Rotation in world space. Some entities might ignore this
        /// </summary>
        [JsonProperty("rotation")]
        public Vector3 Rotation { get; set; }
    }

    public class MapEntityDataJsonConverter : JsonConverter
    {
        bool CannotWrite { get; set; }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanConvert(System.Type objectType)
        {
            return typeof(MapEntityDataConfig).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            using (reader = obj.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                existingValue = (existingValue ?? new MapEntityDataConfig());
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

    #region Game Data Manifest
    [JsonConverter(typeof(GameManifestJsonConverter))]
    public class GameManifestJson
    {
        [JsonProperty("manifestname")]
        public string ManifestName { get; set; }

        [JsonProperty("manifestversion")]
        public ushort ManifestVersion { get; set; }

        [JsonProperty("art")]
        public string[] ArtFileNames { get; set; }

        [JsonProperty("spritedata")]
        public string[] SpriteDataFileNames { get; set; }

        [JsonProperty("blockdata")]
        public string[] BlockDataFileNames { get; set; }

        [JsonProperty("entitydata")]
        public string[] EntityDataFileNames { get; set; }
    }

    public class GameManifestJsonConverter : JsonConverter
    {
        bool CannotWrite { get; set; }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanConvert(System.Type objectType)
        {
            return typeof(GameManifestJson).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            using (reader = obj.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                existingValue = (existingValue ?? new GameManifestJson());
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
                //                JObject details = new JObject();

                obj.WriteTo(writer);
            }
        }
    }
    #endregion

    #region Block Data
    [JsonConverter(typeof(GameBlockDataJsonConverter))]
    public class BlockTemplateCollectionJson
    {
        [JsonProperty("blockdata")]
        public BlockTemplateJson[] BlockData { get; set; }
    }

    public class BlockTemplateJson
    {
        [JsonProperty("blockuid")]
        public ushort BlockUID { get; set; }

        [JsonProperty("blockname")]
        public string BlockName { get; set; }

        [JsonProperty("blockstrength")]
        public int BlockDefaultStrength { get; set; }

        [JsonProperty("facedraw")]
        public byte[] FaceDraw { get; set; }

        [JsonProperty("facedefaultspriteuids")]
        public ushort[] FaceDefaultSpriteUIDs { get; set; }

        [JsonProperty("blockflags")]
        public string[] Flags { get; set; }

        [JsonProperty("blockrequirement")]
        public ushort Requirement { get; set; }
    }

    public class GameBlockDataJsonConverter : JsonConverter
    {
        bool CannotWrite { get; set; }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanConvert(System.Type objectType)
        {
            return typeof(BlockTemplateCollectionJson).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            using (reader = obj.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                existingValue = (existingValue ?? new BlockTemplateCollectionJson());
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

    #region Sprite Data
    [JsonConverter(typeof(SpriteDataJsonConverter))]
    public class SpriteTemplateCollectionJson
    {
        [JsonProperty("spritedata")]
        public SpriteTemplateJson[] SpriteData { get; set; }
    }

    public class SpriteTemplateJson
    {
        [JsonProperty("spriteuid")]
        public ushort UID { get; set; }

        [JsonProperty("spritename")]
        public string SpriteName { get; set; }

        [JsonProperty("spritesheet")]
        public string SpriteSheetFileName { get; set; }

        /// <summary>
        /// Position (in pixels) of the top left corner of the sprite from top left corner of the sprite sheet
        /// </summary>
        [JsonProperty("spritepos")]
        public Vec2Int SpritePosition { get; set; }

        /// <summary>
        /// Width and height of sprite (in pixels)
        /// </summary>
        [JsonProperty("spritewidthheight")]
        public Vec2Int SpriteWidthHeight { get; set; }

        [JsonProperty("flags")]
        public string[] Flags { get; set; }

        public override string ToString()
        {
            return string.Format("Sprite: {0}, '{1}'. SheetName: '{2}' Pos: {3} Width: {4}", UID, SpriteName, SpriteSheetFileName, SpritePosition, SpriteWidthHeight);
        }
    }

    public class SpriteDataJsonConverter : JsonConverter
    {
        bool CannotWrite { get; set; }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanConvert(System.Type objectType)
        {
            return typeof(SpriteTemplateCollectionJson).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            //obj.SelectToken("spritedata.uid").MoveTo(obj);
            //obj.SelectToken("spritedata.name").MoveTo(obj);
            //obj.SelectToken("spritedata.spritesheet").MoveTo(obj);
            //obj.SelectToken("spritedata.spritepos").MoveTo(obj);
            //obj.SelectToken("spritedata.spritewidthheight").MoveTo(obj);
            //obj.SelectToken("spritedata.flags").MoveTo(obj);
            using (reader = obj.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                existingValue = (existingValue ?? new SpriteTemplateCollectionJson());
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

    #region Entity Data
    [JsonConverter(typeof(EntityDataJsonConverter))]
    public class EntityDataConfig
    {
        [JsonProperty("entitydata")]
        public EntityDataJson[] EntityData { get; set; }
    }

    public class EntityDataJson
    {
        [JsonProperty("entitytemplateuid")]
        public uint UID { get; set; }

        [JsonProperty("entitytemplatename")]
        public string EntityName { get; set; }

        [JsonProperty("states")]
        public EntityStateDataJson[] EntityStates { get; set; }

        [JsonProperty("typeflags")]
        public string[] EntityTypeFlags { get; set; }

        [JsonProperty("lightstates")]
        public EntityLightStateJson[] EntityLightStates { get; set; }

        [JsonProperty("fixedstates")]
        public EntityFixedStateJson[] EntityFixedStates { get; set; }

        [JsonProperty("poweredstates")]
        public EntityPoweredStateJson[] EntityPoweredStates { get; set; }

        [JsonProperty("devicestates")]
        public EntityDeviceStateJson[] EntityDeviceStates { get; set; }

        [JsonProperty("craftstates")]
        public EntityCraftStateJson[] EntityCraftStates { get; set; }

        [JsonProperty("multianglestates")]
        public EntityMultiAngleJson[] EntityMultiAngleStates { get; set; }

        [JsonProperty("doorstates")]
        public EntityDoorJson EntityDoorState { get; set; }

        [JsonProperty("containerstates")]
        public EntityContainerStateJson EntityContainerState { get; set; }


        public override string ToString()
        {
            return string.Format("Entity UID: {0} Name: {1} State#: {2}", UID, EntityName, EntityStates.Length);
        }
    }

    #region Entity Components
    public class EntityStateDataJson
    {
        [JsonProperty("stateuid")]
        public ushort StateUID { get; set; }

        [JsonProperty("statename")]
        public string StateName { get; set; }

        [JsonProperty("spriteuid")]
        public ushort SpriteUID { get; set; }

        /// <summary>
        /// Position offset for display plane
        /// </summary>
        [JsonProperty("positionoffset")]
        public Vector3 PositionOffset { get; set; }

        /// <summary>
        /// Size of the display plane in world size (1,1,1 is the same size as a world block
        /// </summary>
        [JsonProperty("graphicsize")]
        public Vector3 DisplaySize { get; set; }

        [JsonProperty("collidersize")]
        public Vector3 ColliderSize { get; set; }
    }

    /// <summary>
    /// Abstract base class entity components
    /// </summary>
    public abstract class EntityTypeComponentBaseJson
    {
        public class EntityTransitions
        {
            [JsonProperty("transitionname")]
            public string TransitionName { get; set; }

            /// <summary>
            /// Condition UID if all requirements are fulfilled
            /// </summary>
            [JsonProperty("nextconditionuid")]
            public ushort TransitionNextConditionUID { get; set; }

            [JsonProperty("requirements")]
            public TransitionRequirements[] Transitionrequirements { get; set; }

            public class TransitionRequirements
            {
                [JsonProperty("variable")]
                public string TransitionRequirementVariable { get; set; }

                [JsonProperty("value")]
                public string TransitionRequirementValue { get; set; }
            }
        }

        public string EntityTypeComponentName { get; set; }

        public EntityTypeComponentBaseJson(string componentType)
        {
            EntityTypeComponentName = componentType;
        }
    }

    public class EntityLightStateJson : EntityTypeComponentBaseJson
    {
        public EntityLightStateJson()
            : base("lightstates")
        {
        }

        /// <summary>
        /// Range of the light, color and strength determined by the light bulb
        /// </summary>
        [JsonProperty("light")]
        public int EntityLightValue { get; set; }
    }

    public class EntityFixedStateJson : EntityTypeComponentBaseJson
    {
        public EntityFixedStateJson()
            : base("fixedstates")
        {
        }

        /// <summary>
        /// UID for type of tool necessary to toggle whether the entity is anchored (bolted to ground) or free
        /// </summary>
        [JsonProperty("toggletooltype")]
        public ushort[] EntityFixedToggleToolType { get; set; }

        /// <summary>
        /// UID for type of tool necessary to break the entity free from its anchoring
        /// </summary>
        [JsonProperty("breakabletooltype")]
        public ushort[] EntityFixedBreakbleToolType { get; set; }

        /// <summary>
        /// UID for type of tool necessary to repair the anchoring if it's been broken
        /// </summary>
        [JsonProperty("repairtooltype")]
        public ushort[] EntityFixedRepairToolType { get; set; }
    }

    public class EntityPoweredStateJson : EntityTypeComponentBaseJson
    {
        public EntityPoweredStateJson()
            : base("poweredstates")
        {
        }

        [JsonProperty("resourcename")]
        public string EntityPowerResourceName { get; set; }

        /// <summary>
        /// How much the resource costs, can be negative (and thus generate that resource)
        /// </summary>
        [JsonProperty("cost")]
        public int EntityPowerCost { get; set; }
    }

    public class EntityDeviceStateJson : EntityTypeComponentBaseJson
    {
        public EntityDeviceStateJson()
            : base("devicestates")
        {
        }

        [JsonProperty("acceptedinputtype")]
        public string[] EntityAcceptedInputTypeNames { get; set; }

        [JsonProperty("maxcount")]
        public int EntityAcceptedInputCount { get; set; }
    }

    public class EntityCraftStateJson : EntityTypeComponentBaseJson
    {
        public EntityCraftStateJson()
            : base("craftstates")
        {
        }

        [JsonProperty("materials")]
        public MaterialsJson[] EntityCraftMaterials { get; set; }

        public class MaterialsJson
        {
            [JsonProperty("name")]
            public string MaterialCraftingName { get; set; }

            [JsonProperty("count")]
            public int MaterialCraftingCount { get; set; }
        }
    }

    public class EntityMultiAngleJson : EntityTypeComponentBaseJson
    {
        public EntityMultiAngleJson()
            : base("multianglestates")
        { }

        [JsonProperty("stateuid")]
        public ushort AngleStateUID { get; set; }

        [JsonProperty("anglename")]
        public string AngleStateName { get; set; }

        [JsonProperty("anglemin")]
        public float AngleStateMinAngle { get; set; }

        [JsonProperty("anglemax")]
        public float AngleStateMaxAngle { get; set; }

        [JsonProperty("spriteuid")]
        public uint AngleSpriteUID { get; set; }

    }

    public class EntityDoorJson : EntityTypeComponentBaseJson
    {
        public EntityDoorJson()
            : base("doorstates")
        { }

        [JsonProperty("doubledoors")]
        public bool DoorDoubleDoors { get; set; }

        [JsonProperty("horizontal")]
        public bool DoorHorizontal { get; set; }

        [JsonProperty("initialcondition")]
        public ushort InitialConditionUID { get; set; }

        [JsonProperty("doorconditions")]
        public EntityDoorConditionsJson[] DoorConditions { get; set; }

        public class EntityDoorConditionsJson
        {
            [JsonProperty("conditionuid")]
            public ushort EntityDoorConditionUID { get; set; }

            [JsonProperty("conditionname")]
            public string EntityDoorConditionName { get; set; }

            [JsonProperty("states")]
            public ushort[] EntityDoorConditionStates { get; set; }

            [JsonProperty("defspeed")]
            public float EntityDoorConditionAnimDelta { get; set; }

            [JsonProperty("translations")]
            public Vector3[] EntityDoorConditionTranslations { get; set; }

            [JsonProperty("rotations")]
            public Vector3[] EntityDoorConditionRotations { get; set; }

            [JsonProperty("hascolliders")]
            public bool[] EntityDoorConditionHasColliders { get; set; }

            [JsonProperty("transitions")]
            public EntityTransitions[] EntityDoorTransitions { get; set; }

            [JsonProperty("duration")]
            public float EntityDoorTransitionDuration { get; set; }
        }
    }

    public class EntityContainerStateJson : EntityTypeComponentBaseJson
    {
        public EntityContainerStateJson()
            : base("containerstates")
        {
        }

        /// <summary>
        /// Maximum volume that the container can hold
        /// </summary>
        [JsonProperty("maxvolume")]
        public float ContainerMaxVolume { get; set; }

        /// <summary>
        /// Whether or not container has a lid
        /// </summary>
        [JsonProperty("haslid")]
        public bool ContainerHasLid { get; set; }

        /// <summary>
        /// Sprite UID when viewing lid from front (closed) position
        /// </summary>
        [JsonProperty("frontlidspriteuid")]
        public uint ContainerLidFrontSprite { get; set; }

        /// <summary>
        /// Sprite UID when viewing lid from back/inside
        /// </summary>
        [JsonProperty("backlidspriteuid")]
        public uint ContainerLidBackSprite { get; set; }

        /// <summary>
        /// Size of the graphic mesh
        /// </summary>
        [JsonProperty("graphicsize")]
        public Vector3 ContainerLidGraphicSize { get; set; }

        /// <summary>
        /// Local position offset for container lid
        /// </summary>
        [JsonProperty("positionoffset")]
        public Vector3 ContainerLidPositionOffset { get; set; }

        /// <summary>
        /// Local rotation of lid when closed
        /// </summary>
        [JsonProperty("rotationaclosed")]
        public Vector3 ContainerLidRotationClosed { get; set; }

        /// <summary>
        /// Local rotation of lid when opened
        /// </summary>
        [JsonProperty("rotationopened")]
        public Vector3 ContainerLidRotationOpened { get; set; }

        /// <summary>
        /// The container states where the front of the lid is visible
        /// </summary>
        [JsonProperty("containerfrontlidstates")]
        public ushort[] ContainerLidFrontStates { get; set; }

        /// <summary>
        /// The container states where the back of the lid is visible
        /// </summary>
        [JsonProperty("containerbacklidstates")]
        public ushort[] ContainerLidBackStates { get; set; }

    }
    #endregion

    public class EntityDataJsonConverter : JsonConverter
    {
        bool CannotWrite { get; set; }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanConvert(System.Type objectType)
        {
            return typeof(EntityDataConfig).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            //obj.SelectToken("entitydata.lightstates").MoveTo(obj);
            //obj.SelectToken("entitydata.fixedstates").MoveTo(obj);
            //obj.SelectToken("entitydata.poweredstates").MoveTo(obj);
            //obj.SelectToken("entitydata.devicestates").MoveTo(obj);
            //obj.SelectToken("entitydata.craftstates").MoveTo(obj);
            //obj.SelectToken("entitydata.craftstates.materials.name").MoveTo(obj);
            //obj.SelectToken("entitydata.craftstates.materials.count").MoveTo(obj);
            using (reader = obj.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                existingValue = (existingValue ?? new EntityDataConfig());
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
        [JsonProperty("serverconfig")]
        public string ServerConfigFile { get; set; }

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
            obj.SelectToken("filesystemconfig.serverconfig").MoveTo(obj);
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
                obj["serverconfig"].MoveTo(details);
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

    #region Server Config
    [JsonConverter(typeof(ServerConfigJsonConverter))]
    public class ServerConfig
    {
        [JsonProperty("servername")]
        public string ServerName { get; set; }

        [JsonProperty("module")]
        public string ModuleName { get; set; }

        [JsonProperty("maps")]
        public ServerConfigMapNameJson[] MapChoices { get; set; }

        [JsonProperty("defaultmaxroundtime")]
        public int MaxRoundTime { get; set; }
    }

    public class ServerConfigMapNameJson
    {
        [JsonProperty("map")]
        public string MapName { get; set; }

        [JsonProperty("entities")]
        public string EntityName { get; set; }
    }

    public class ServerConfigJsonConverter : JsonConverter
    {
        bool CannotWrite { get; set; }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanConvert(System.Type objectType)
        {
            return typeof(ServerConfig).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            obj.SelectToken("serverconfig.servername").MoveTo(obj);
            obj.SelectToken("serverconfig.module").MoveTo(obj);
            obj.SelectToken("serverconfig.maps").MoveTo(obj);
            obj.SelectToken("serverconfig.defaultmaxroundtime").MoveTo(obj);
            using (reader = obj.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                existingValue = (existingValue ?? new ServerConfig());
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

                obj.Add("serverconfig", details);
                obj["servername"].MoveTo(details);
                obj["module"].MoveTo(details);
                obj["maps"].MoveTo(details);
                obj["defaultmaxroundtime"].MoveTo(details);
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