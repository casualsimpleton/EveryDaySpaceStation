//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// MapEditor - Map Editor Class for easy voxel editing
// Created: Febuary 3 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: Febuary 3 2016
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class MapEditor : EditorWindow {

    [MenuItem("EveryDaySpaceStation/Map Editor")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow<MapEditor>().Show();
    }

    string fileName;
    string path;
    bool isDirty = false;

    string mapName = "";
    string blockDefinition = "";

    ushort mapVersion = 1;
    ushort _curRegion = 0;

    public enum SelectDeleteAdd
    {
        None,
        Select,
        Delete,
        Add
    }

    public enum RegionAction
    {
        None,
        CreateNew,
        Renumber,
        Delete
    }

    SelectDeleteAdd _selDelAddButtons = SelectDeleteAdd.None;
    RegionAction _regionCreateRenumDelete = RegionAction.None;
    ushort[] _regionRenumberArray;

    MapDataV2 _curMapData = null;
    MapDataV2.MapRegion _curMapRegion = null;
    Vec3Int _curMapRegionSize = Vec3Int.Zero;

    bool showImportRegionTextures = false;
    List<Texture2D> _importRegionTextures;
    Object _assetSelectorObject;

    Vector2 _scrollPos = new Vector2();

    void Reset()
    {
        _curMapData = null;
        _curMapRegion = null;
        _selDelAddButtons = SelectDeleteAdd.None;
        _regionCreateRenumDelete = RegionAction.None;
        _regionRenumberArray = null;
        _importRegionTextures = null;
        _scrollPos = new Vector2();
    }

    void OnGUI()
    {
        if (EditorApplication.isCompiling)
        {
            EditorApplication.isPlaying = false;

            Reset();
        }

        _scrollPos = GUILayout.BeginScrollView(_scrollPos);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New Map", GUILayout.MaxWidth(80), GUILayout.MinHeight(40)))
        {
            //EditorWindow.GetWindow<MapEditorNewMap>().Show();
            fileName = EditorUtility.SaveFilePanel("New Map", string.Format("{0}{1}maps", Application.persistentDataPath, System.IO.Path.DirectorySeparatorChar), "newmap", "edss");
            Debug.Log("Filename " + fileName);

            if (!EditorApplication.isPlaying)
            {
                Reset();
                EditorApplication.isPlaying = true;
            }

            _curMapData = new MapDataV2();
            _curMapData.MapName = FileSystem.GetFileNameWithoutExtension(fileName);
        }

        if (GUILayout.Button("Load Map", GUILayout.MaxWidth(80), GUILayout.MinHeight(40)))
        {
        }

        if (isDirty)
        {
            if (GUILayout.Button("Save Map", GUILayout.MaxWidth(80), GUILayout.MinHeight(40)))
            {
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Map Name:", GUILayout.MaxWidth(250));
        if (_curMapData != null && !string.IsNullOrEmpty(_curMapData.MapName))
        {
            mapName = GUILayout.TextField(_curMapData.MapName, 128, GUILayout.MaxWidth(250)); //EditorGUILayout.TextField("Map Name:", mapName);//, );
        }

        GUILayout.Label("Map Version:", GUILayout.MaxWidth(250));
        if (_curMapData != null)
        {
            mapVersion = (ushort)EditorGUILayout.IntField(_curMapData.MapVersion, GUILayout.MaxWidth(250));
        }

        if (EditorGUI.EndChangeCheck())
        {
            _curMapData.MapName = mapName;
            _curMapData.MapVersion = mapVersion;
        }

        GUILayout.Space(5);

        GUILayout.Label(string.Format("Block Definitions: {0}", blockDefinition), GUILayout.MaxWidth(250), GUILayout.MinHeight(40));
        
        GUILayout.Space(5);

        GUILayout.Label(string.Format("Current Region: {0}", _curRegion), GUILayout.MaxWidth(250));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New Region", GUILayout.MaxWidth(100), GUILayout.MinHeight(30)))
        {
            _regionCreateRenumDelete = RegionAction.CreateNew;
            //_curMapRegion = new MapDataV2.MapRegion();

            _importRegionTextures.Clear();
            showImportRegionTextures = false;
            _assetSelectorObject = null;
        }

        if (GUILayout.Button("Renum. Region", GUILayout.MaxWidth(100), GUILayout.MinHeight(30)))
        {
            _regionCreateRenumDelete = RegionAction.Renumber;
            _regionRenumberArray = new ushort[_curMapData.MapRegions.Count];

            for (int i = 0; i < _regionRenumberArray.Length; i++)
            {
                _regionRenumberArray[i] = _curMapData.MapRegions[i].RegionUID;
            }
        }

        if (GUILayout.Button("Del Region", GUILayout.MaxWidth(100), GUILayout.MinHeight(30)))
        {
            _regionCreateRenumDelete = RegionAction.Delete;
        }
        GUILayout.EndHorizontal();

        if (_regionCreateRenumDelete == RegionAction.CreateNew)
        {
            string commandName = Event.current.commandName;
            ushort newUID = FindFirstUnusedRegionUID(_curMapData);
            
            GUILayout.Label(string.Format("New UID: {0}", newUID));
            
            GUILayout.Label("New Region Size:");
            _curMapRegionSize.x = Mathf.Clamp(EditorGUILayout.IntField("Width (X):", _curMapRegionSize.x), 0, 256);
            _curMapRegionSize.y = Mathf.Clamp(EditorGUILayout.IntField("Height (Y):", _curMapRegionSize.y), 0, 256);
            _curMapRegionSize.z = Mathf.Clamp(EditorGUILayout.IntField("Length (Z):", _curMapRegionSize.z), 0, 256);
            
            //showImportRegionTextures = EditorGUILayout.BeginToggleGroup("Import From Texture", showImportRegionTextures);

            if(_importRegionTextures == null)
            {
                _importRegionTextures = new List<Texture2D>();
            }

            //This has to happen before drawing the loop. Otherwise it poops itself
            if (Event.current.type == EventType.Layout)
            {
                if (commandName == "ObjectSelectorUpdated")
                {
                    _assetSelectorObject = EditorGUIUtility.GetObjectPickerObject();

                    if (_assetSelectorObject is Texture2D)
                    {
                        _importRegionTextures.Add(_assetSelectorObject as Texture2D);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Invalid file", "Only supported textures can be used.", "OK");
                    }
                }
            }

            for (int i = 0; i < _importRegionTextures.Count; i++)
            {
                GUILayout.BeginHorizontal();
                
                GUILayout.Label(_importRegionTextures[i], GUILayout.MaxWidth(180), GUILayout.MaxHeight(180));

                if (i == 0)
                {
                    GUI.enabled = false;
                }

                //Move item up
                if (GUILayout.Button("/\\"))
                {
                    Texture2D tex = _importRegionTextures[i];
                    _importRegionTextures.RemoveAt(i);
                    _importRegionTextures.Insert(i - 1, tex);
                }

                if (i == 0)
                {
                    GUI.enabled = true;
                }

                if (i == _importRegionTextures.Count - 1)
                {
                    GUI.enabled = false;
                }

                //Move item down
                if (GUILayout.Button("\\/"))
                {
                    Texture2D tex = _importRegionTextures[i];
                    _importRegionTextures.RemoveAt(i);
                    _importRegionTextures.Insert(i + 1, tex);
                }

                if (i == _importRegionTextures.Count - 1)
                {
                    GUI.enabled = true;
                }

                if (GUILayout.Button("R"))
                {
                    _importRegionTextures.RemoveAt(i);
                    break;
                }
                GUILayout.EndHorizontal();
            }
            
            if (GUILayout.Button("Add New Texture"))
            {
                EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, "", -1);
            }

            //else if (commandName == "ObjectSelectorClosed")
            //if (commandName == "ObjectSelectorClosed")
            //{
            //    //_assetSelectorObject = EditorGUIUtility.GetObjectPickerObject();
            //    //if (_assetSelectorObject is Texture2D)
            //    //{
            //    //    if (Event.current.type == EventType.Layout)
            //    //    {
            //    //        _importRegionTextures.Add(_assetSelectorObject as Texture2D);
            //    //        return;
            //    //    }
            //    //}
            //    //else
            //    //{
            //    //    EditorUtility.DisplayDialog("Invalid file", "Only supported textures can be used.", "OK");
            //    //}
            //}

            GUILayout.Box("", GUILayout.MaxWidth(350), GUILayout.Height(0));

            //EditorGUILayout.EndToggleGroup();            

            if (GUILayout.Button("Create Region", GUILayout.MaxWidth(350), GUILayout.MinHeight(60)))
            {
                _regionCreateRenumDelete = RegionAction.None;
                _curMapRegion = new MapDataV2.MapRegion();
                _curMapRegion.RegionSize = new Vec3Int(_curMapRegionSize);
                _curMapRegion.RegionUID = newUID;

                AdjustRegionSize(ref _curMapRegion, _curMapRegion.RegionSize);

                _curMapData.MapRegions.Add(_curMapRegion);
                _curMapRegion = null;
            }
        }
        else if (_regionCreateRenumDelete == RegionAction.Renumber)
        {
            GUILayout.Label("Renumber Region:");
            for (int i = 0; i < _curMapData.MapRegions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("Reg Index: {0}, Cur UID: {1}", i, _curMapData.MapRegions[i].RegionUID));
                _regionRenumberArray[i] = (ushort)EditorGUILayout.IntField(_regionRenumberArray[i], GUILayout.MaxWidth(18));

                if (GUILayout.Button("Set"))
                {
                    //Make sure that UID isn't already in use
                    bool inUse = false;

                    for (int j = 0; j < _curMapData.MapRegions.Count; j++)
                    {
                        if (_curMapData.MapRegions[j].RegionUID == _regionRenumberArray[i])
                        {
                            inUse = true;
                            break;
                        }
                    }

                    //No other in use, so set it
                    if (!inUse)
                    {
                        _curMapData.MapRegions[i].RegionUID = _regionRenumberArray[i];
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("UID Already In Use", string.Format("Can't set Region UID to {0} as it is already in use. It must be unique.", _regionRenumberArray[i]), "OK");
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        else if (_regionCreateRenumDelete == RegionAction.Delete)
        {
            for (int i = 0; i < _curMapData.MapRegions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("Reg Index: {0}, UID: {1} BlockSize: {2},{3},{4}", i, _curMapData.MapRegions[i].RegionUID, _curMapData.MapRegions[i].RegionSize.x, _curMapData.MapRegions[i].RegionSize.y, _curMapData.MapRegions[i].RegionSize.z));
                if (GUILayout.Button("Delete"))
                {
                    bool delConfirm = EditorUtility.DisplayDialog("ARE YOU SURE?", "Deleting a region is a one way operation. There is no undo. Are you sure?", "Yes, Delete", "No. Cancel");

                    if (delConfirm)
                    {
                        _curMapData.MapRegions.RemoveAt(i);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        
        if (_regionCreateRenumDelete != RegionAction.None)
        {
            if (GUILayout.Button("Cancel/Close", GUILayout.MaxWidth(350), GUILayout.MinHeight(30)))
            {
                _regionCreateRenumDelete = RegionAction.None;
                _regionRenumberArray = null;
            }
        }

        //GUILayout.Space(5);
        GUILayout.Box("", GUILayout.MaxWidth(350), GUILayout.Height(0));

        //if (_selDelAddButtons != SelectDeleteAdd.None)
        {
            GUILayout.Label(string.Format("Action: {0}", _selDelAddButtons));
        }

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select", GUILayout.MaxWidth(60), GUILayout.MinHeight(30)))
        {
            _selDelAddButtons = SelectDeleteAdd.Select;
        }

        if (GUILayout.Button("Delete", GUILayout.MaxWidth(60), GUILayout.MinHeight(30)))
        {
            _selDelAddButtons = SelectDeleteAdd.Delete;
        }

        if (GUILayout.Button("Add", GUILayout.MaxWidth(60), GUILayout.MinHeight(30)))
        {
            _selDelAddButtons = SelectDeleteAdd.Add;
        }

        if (_selDelAddButtons != SelectDeleteAdd.None)
        {
            if (GUILayout.Button("Clear", GUILayout.MaxWidth(60), GUILayout.MinHeight(30)))
            {
                _selDelAddButtons = SelectDeleteAdd.None;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        

        if (_selDelAddButtons == SelectDeleteAdd.Select)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<X", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
            {
            }

            if (GUILayout.Button("X>", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
            {
            }

            if (GUILayout.Button("<Y", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
            {
            }

            if (GUILayout.Button("Y>", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
            {
            }

            if (GUILayout.Button("<Z", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
            {
            }

            if (GUILayout.Button("Z>", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
            {
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    public void AdjustRegionSize(ref MapDataV2.MapRegion region, Vec3Int newSize)
    {
        if (region.RegionBlocks == null)
        {
            region.RegionBlocks = new MapDataV2.MapBlock[newSize.x, newSize.y, newSize.z];
        }
        else
        {
            int curX = region.RegionBlocks.GetLength(0);
            int curY = region.RegionBlocks.GetLength(1);
            int curZ = region.RegionBlocks.GetLength(2);

            MapDataV2.MapBlock[, ,] newBlocks = new MapDataV2.MapBlock[newSize.x, newSize.y, newSize.z];

            //First pass, fill it all with empty (but valid) blocks
            //Probably more efficient ways to do this
            for (int x = 0; x < newSize.x; x++)
            {
                for (int y = 0; y < newSize.y; y++)
                {
                    for (int z = 0; z < newSize.z; z++)
                    {
                        MapDataV2.MapBlock block = new MapDataV2.MapBlock();
                        block.Init();
                        newBlocks[x, y, z] = block;
                    }
                }
            }

            //Now copy whatever blocks from old one
            for (int x = 0; x < newSize.x && x < curX; x++)
            {
                for (int y = 0; y < newSize.y && y < curY; y++)
                {
                    for (int z = 0; z < newSize.z && z < curZ; z++)
                    {
                        newBlocks[x, y, z] = region.RegionBlocks[x, y, z];
                    }
                }
            }

            region.RegionBlocks = null;
            region.RegionBlocks = newBlocks;
        }
    }

    public ushort FindFirstUnusedRegionUID(MapDataV2 mapData)
    {
        ushort uid = 0;

        bool searching = true;

        while (searching)
        {
            searching = false;
            for (int i = 0; i < mapData.MapRegions.Count; i++)
            {
                if (mapData.MapRegions[i].RegionUID == uid)
                {
                    searching = true;
                    uid++;
                    break;
                }
            }

            if (!searching)
                break;
        }

        return uid;
    }
}
