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
        BlockEdit,
        EntityEdit
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

    List<Tuple<Color32, ushort>> _mapColorDefinitions = new List<Tuple<Color32,ushort>>();

    Vector2 _scrollPos = new Vector2();

    MapEditorCamera _editorCamera;
    VoxelWorld _vw;

    bool _hasStarted = false;

    void StartLoad()
    {
        Debug.Log(string.Format("Loading Map Editor: {0}", System.Environment.SpecialFolder.ApplicationData));

        _hasStarted = true;
    }

    void Reset()
    {
        _curMapData = null;
        _curMapRegion = null;
        _selDelAddButtons = SelectDeleteAdd.None;
        _regionCreateRenumDelete = RegionAction.None;
        _regionRenumberArray = null;
        _importRegionTextures = null;
        _mapColorDefinitions = null;
        _scrollPos = new Vector2();

        if (_editorCamera == null)
        {
            _editorCamera = FindObjectOfType<MapEditorCamera>();
        }

        _editorCamera._targetCube.SetActive(false);
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

            _mapColorDefinitions.Clear();
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
            if (GUILayout.Button("Reset To Cur Size"))
            {
                if (_importRegionTextures != null && _importRegionTextures.Count > 0)
                {
                    //Go through all textures and find largest size
                    for (int i = 0; i < _importRegionTextures.Count; i++)
                    {
                        if (_importRegionTextures[i].width > _curMapRegionSize.x)
                        {
                            _curMapRegionSize.x = _importRegionTextures[i].width;
                        }

                        if (_importRegionTextures[i].height > _curMapRegionSize.z)
                        {
                            _curMapRegionSize.z = _importRegionTextures[i].height;
                        }

                        _curMapRegionSize.y = _importRegionTextures.Count;
                    }
                }
            }
            
            showImportRegionTextures = EditorGUILayout.BeginToggleGroup("Import From Texture", showImportRegionTextures);

            GUILayout.Space(10);

            if(_importRegionTextures == null)
            {
                _importRegionTextures = new List<Texture2D>();
            }

            if (_mapColorDefinitions == null)
            {
                _mapColorDefinitions = new List<Tuple<Color32, ushort>>();
            }

            //This has to happen before drawing the loop. Otherwise it poops itself
            if (Event.current.type == EventType.Layout)
            {
                if (commandName == "ObjectSelectorUpdated")
                {
                    _assetSelectorObject = EditorGUIUtility.GetObjectPickerObject();

                    if (_assetSelectorObject is Texture2D)
                    {
                        Texture2D texture = _assetSelectorObject as Texture2D;
                        _importRegionTextures.Add(texture);

                        if (texture.width > _curMapRegionSize.x)
                        {
                            _curMapRegionSize.x = texture.width;
                        }

                        if (texture.height > _curMapRegionSize.z)
                        {
                            _curMapRegionSize.z = texture.height;
                        }

                        _curMapRegionSize.y = _importRegionTextures.Count;

                        //Check colors
                        UpdateColorBlockPairing(texture, ref _mapColorDefinitions);
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
                GUILayout.Label(string.Format("Level {0}", i));

                GUILayout.Label(_importRegionTextures[i], GUILayout.MinWidth(180), GUILayout.MinHeight(180), GUILayout.MaxWidth(_importRegionTextures[i].width), GUILayout.MaxHeight(_importRegionTextures[i].height));

                if (i == 0 && showImportRegionTextures)
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

                if (i == 0 && showImportRegionTextures)
                {
                    GUI.enabled = true;
                }

                if (i == _importRegionTextures.Count - 1 && showImportRegionTextures)
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

                if (i == _importRegionTextures.Count - 1 && showImportRegionTextures)
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

            GUILayout.Space(10);

            GUILayout.Label("Color / Block Tile Type Definitions", GUILayout.MaxHeight(18));
            Color prevColor = GUI.color;
            for (int i = 0; i < _mapColorDefinitions.Count; i++)
            {
                GUILayout.BeginHorizontal();

                GUI.color = _mapColorDefinitions[i].First;
                GUILayout.Label(string.Format("R {0} G {1} B {2}", _mapColorDefinitions[i].First.r, _mapColorDefinitions[i].First.g, _mapColorDefinitions[i].First.b), GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
                GUILayout.Label(string.Format("Block Type: {0}", _mapColorDefinitions[i].Second), GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
                _mapColorDefinitions[i].Second = (ushort)Mathf.Clamp(EditorGUILayout.IntField(_mapColorDefinitions[i].Second, GUILayout.MaxWidth(30)), 0, ushort.MaxValue);

                GUILayout.EndHorizontal();
            }
            GUI.color = prevColor;

            GUILayout.Space(10);

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

            GUILayout.Space(10);

            EditorGUILayout.EndToggleGroup();            

            if (GUILayout.Button("Create Region", GUILayout.MaxWidth(350), GUILayout.MinHeight(60)))
            {
                _regionCreateRenumDelete = RegionAction.None;
                _curMapRegion = new MapDataV2.MapRegion();
                _curMapRegion.RegionSize = new Vec3Int(_curMapRegionSize);
                _curMapRegion.RegionUID = newUID;

                if (_importRegionTextures == null || _importRegionTextures.Count < 1)
                {
                    AdjustRegionSize(ref _curMapRegion, _curMapRegion.RegionSize);
                }
                else
                {
                    PopulateRegionFromTextures(ref _curMapRegion, _curMapRegion.RegionSize, _importRegionTextures, _mapColorDefinitions);
                }

                //VoxelWorld vw = GameObject.FindObjectOfType<VoxelWorld>();
                _vw = GameObject.FindObjectOfType<VoxelWorld>();
                _vw.CreateWorld(_curMapRegion.RegionBlocks);

                _curMapData.MapRegions.Add(_curMapRegion);
                //_curMapRegion = null;
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

            if (_selDelAddButtons == SelectDeleteAdd.BlockEdit)
            {
                GUILayout.Label(string.Format("Select Anchor: {0},{1},{2}", _editorCamera._curTargetBlockPos.x, _editorCamera._curTargetBlockPos.y, _editorCamera._curTargetBlockPos.z));
            }
        }

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Block Edit", GUILayout.MaxWidth(80), GUILayout.MinHeight(30)))
        {
            _selDelAddButtons = SelectDeleteAdd.BlockEdit;

            if (_editorCamera == null)
            {
                _editorCamera = FindObjectOfType<MapEditorCamera>();
            }

            _editorCamera.MouseSelectionMode = MapEditorCamera.MouseEditMode.BlockEdit;
            _editorCamera._targetCube.SetActive(true);
        }

        if (GUILayout.Button("Entity Edit", GUILayout.MaxWidth(80), GUILayout.MinHeight(30)))
        {
            _selDelAddButtons = SelectDeleteAdd.EntityEdit;
        }

        if (_selDelAddButtons != SelectDeleteAdd.None)
        {
            if (GUILayout.Button("Clear", GUILayout.MaxWidth(60), GUILayout.MinHeight(30)))
            {
                _selDelAddButtons = SelectDeleteAdd.None;

                if (_editorCamera == null)
                {
                    _editorCamera = FindObjectOfType<MapEditorCamera>();
                }

                _editorCamera._targetCube.SetActive(false);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        //if (_selDelAddButtons == SelectDeleteAdd.Select)
        //{
        //    GUILayout.BeginHorizontal();
        //    if (GUILayout.Button("<X", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
        //    {
        //        _editorCamera.ChangeSelectionBlock(-1, 0, 0);
        //    }

        //    if (GUILayout.Button("X>", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
        //    {
        //        _editorCamera.ChangeSelectionBlock(1, 0, 0);
        //    }

        //    if (GUILayout.Button("<Y", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
        //    {
        //        _editorCamera.ChangeSelectionBlock(0, -1, 0);
        //    }

        //    if (GUILayout.Button("Y>", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
        //    {
        //        _editorCamera.ChangeSelectionBlock(0, 1, 0);
        //    }

        //    if (GUILayout.Button("<Z", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
        //    {
        //        _editorCamera.ChangeSelectionBlock(0, 0, -1);
        //    }

        //    if (GUILayout.Button("Z>", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
        //    {
        //        _editorCamera.ChangeSelectionBlock(0, 0, 1);
        //    }

        //    if (GUILayout.Button("Reset", GUILayout.MaxWidth(40), GUILayout.MinHeight(40)))
        //    {
        //        _editorCamera.ResetSelectionBlock();
        //    }
        //    GUILayout.EndHorizontal();
        //}
        GUILayout.EndScrollView();
    }

    void Update()
    {
        if (!_hasStarted)
        {
            StartLoad();
        }

        ProcessMouseCameraEvent();
    }
    
    /// <summary>
    /// Since the mouse has no reliable way to send messages to the editor window, we need to check the editor mouse to see if it has any new mouse events to process
    /// </summary>
    public void ProcessMouseCameraEvent()
    {
        if(_curMapRegion == null)
            return;

        MapEditorCamera.MouseActionEvent mouseEditEvent = _editorCamera.GetMouseAction();

        if (mouseEditEvent == null)
        {
            return;
        }

        //Check for bounds
        if (mouseEditEvent._position.x < 0 || mouseEditEvent._position.y < 0 || mouseEditEvent._position.z < 0
            || mouseEditEvent._position.x >= _curMapRegion.RegionSize.x || mouseEditEvent._position.y >= _curMapRegion.RegionSize.y || mouseEditEvent._position.z >= _curMapRegion.RegionSize.z)
        {
            return;
        }

        if (mouseEditEvent._mouseActionType == MapEditorCamera.MouseActionType.AddBlock)
        {
            MapDataV2.MapBlock newBlock = new MapDataV2.MapBlock();
            newBlock.BlockType = 1;
            _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z] = newBlock;
        }
        else if (mouseEditEvent._mouseActionType == MapEditorCamera.MouseActionType.RemoveBlock)
        {
            //It's already "empty"
            if (_curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockType == 0)
            {
                return;
            }

            MapDataV2.MapBlock newBlock = new MapDataV2.MapBlock();
            newBlock.BlockType = 0;
            _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z] = newBlock;
        }

        //VoxelWorld vw = GameObject.FindObjectOfType<VoxelWorld>();
        _vw.UpdateBlock(mouseEditEvent._position, _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z]);
    }

    public void AdjustRegionSize(ref MapDataV2.MapRegion region, Vec3Int newSize)
    {
        if (region.RegionBlocks == null)
        {
            region.RegionBlocks = new MapDataV2.MapBlock[newSize.x, newSize.y, newSize.z];

            //Have to init this to something otherwise we'll get nothing out of it later
            for (int x = 0; x < newSize.x; x++)
            {
                for (int y = 0; y < newSize.y; y++)
                {
                    for (int z = 0; z < newSize.z; z++)
                    {
                        MapDataV2.MapBlock block = new MapDataV2.MapBlock();
                        block.Init();
                        region.RegionBlocks[x, y, z] = block;
                    }
                }
            }
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

    public void PopulateRegionFromTextures(ref MapDataV2.MapRegion region, Vec3Int newSize, List<Texture2D> importTextures, List<Tuple<Color32, ushort>> colorBlockDefs)
    {
        if (region.RegionBlocks == null)
        {
            region.RegionBlocks = new MapDataV2.MapBlock[newSize.x, newSize.y, newSize.z];
        }

        //First pass, fill with empty stuff
        for (int x = 0; x < newSize.x; x++)
        {
            for (int y = 0; y < newSize.y; y++)
            {
                for (int z = 0; z < newSize.z; z++)
                {
                    MapDataV2.MapBlock block = new MapDataV2.MapBlock();
                    block.Init();
                    region.RegionBlocks[x, y, z] = block;
                }
            }
        }

        for (int y = 0; y < importTextures.Count && y < newSize.y; y++)
        {
            int width = importTextures[y].width;
            int index = 0;
            Color32[] colors = importTextures[y].GetPixels32();

            //Because of how Unity reads in the pixels, we need to swap the X and the Z in order to get the same orientation as the source image
            //So we swap X and Z, but also need to swap bounds check 
            int height = importTextures[y].height;
            for (int x = 0; x < height && x < newSize.x; x++)
            {                
                for (int z = 0; z < width && z < newSize.z; z++)
                {
                    //We're going to flip the z and x import here so it matches the same from any image it's imported from
                    region.RegionBlocks[z, y, x] = MapBlockFromColor(colors[index], colorBlockDefs);
                    index++;
                }
            }
        }
    }

    public MapDataV2.MapBlock MapBlockFromColor(Color32 color, List<Tuple<Color32, ushort>> colorBlockDefs)
    {
        for (int i = 0; i < colorBlockDefs.Count; i++)
        {
            if (color.Compare(colorBlockDefs[i].First))
            {
                MapDataV2.MapBlock newBlock = new MapDataV2.MapBlock();
                newBlock.BlockType = colorBlockDefs[i].Second;

                return newBlock;
            }
        }

        MapDataV2.MapBlock blankBlock = new MapDataV2.MapBlock();
        blankBlock.BlockType = 0;

        return blankBlock;
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

    public void UpdateColorBlockPairing(Texture2D newTexture, ref List<Tuple<Color32, ushort>> blockColorPairings)
    {
        Color32[] colors = newTexture.GetPixels32();

        for (int i = 0; i < colors.Length; i++)
        {
            bool doesContain = false;
            for (int j = 0; j < blockColorPairings.Count; j++)
            {
                if (blockColorPairings[j].First.Compare(colors[i]))
                {
                    doesContain = true;
                    break;
                }
            }

            //Color already present
            if (doesContain)
            {
                continue;
            }

            blockColorPairings.Add(Tuple.New<Color32, ushort>(colors[i], 0));
        }
    }
}
