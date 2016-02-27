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

    string _fileName;
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
        EntityEdit,
        FaceEdit
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

    string _blockDataDirectory = "";

    MapDataV2 _curMapData = null;
    MapDataV2.MapRegion _curMapRegion = null;
    Vec3Int _curMapRegionSize = Vec3Int.Zero;

    bool showImportRegionTextures = false;
    List<Texture2D> _importRegionTextures;
    Object _assetSelectorObject;

    List<Tuple<Color32, ushort>> _mapColorDefinitions = new List<Tuple<Color32,ushort>>();

    Vector2 _scrollPos = new Vector2();
    Vector2 _textureScrollPos = new Vector2();
    int _textureSelIndex = 0;

    MapEditorCamera _editorCamera;
    VoxelWorld _vw;
    ushort _selectedBlockType = 1;

    GameManifestV2.BlockDataTemplate[] _blockDataTemplates;
    Tuple<ushort, Texture2D, string>[] _textureTemplates;

    bool _hasStarted = false;

    void StartLoad()
    {
        //Debug.Log(string.Format("Loading Map Editor: {0}{1}server", Application.persistentDataPath, System.IO.Path.DirectorySeparatorChar));
        _blockDataDirectory = string.Format("{0}{1}server", Application.persistentDataPath, System.IO.Path.DirectorySeparatorChar);
        isDirty = false;
        _hasStarted = true;
    }

    void SaveMap()
    {
        FileSystem.WriteMap(_curMapData, _fileName);
        Debug.Log(string.Format("Saving map to {0}", _fileName));
    }

    void LoadMap()
    {
        _fileName = EditorUtility.OpenFilePanel("Load Map", _blockDataDirectory, "edss");
        _curMapData = FileSystem.LoadMapData(_fileName);
        _curMapRegion = _curMapData.MapRegions[0];
        Debug.Log(string.Format("Loaded map {0} from {1}", _curMapData.MapName, _fileName));

        _vw = GameObject.FindObjectOfType<VoxelWorld>();
        _vw.CreateWorld(_curMapRegion.RegionBlocks, false);
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
        isDirty = false;

        if (_editorCamera == null)
        {
            _editorCamera = FindObjectOfType<MapEditorCamera>();
        }

        _editorCamera._targetCube.SetActive(false);
        _editorCamera._targetPlane.SetActive(false);

        _hasStarted = false;
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
        if (GameManifestV2.Singleton == null || !GameManifestV2.Singleton.IsLoaded)
        {
            if(!string.IsNullOrEmpty(_blockDataDirectory))
            {
                StartLoad();

                FileSystem.Init();

                _blockDataDirectory = EditorUtility.OpenFilePanel("Select Game Data Manifest", _blockDataDirectory, "json");
                //string fileName = FileSystem.GetFileNameWithoutExtension(_blockDataDirectory);

                //string rawJson = System.IO.File.ReadAllText(_blockDataDirectory);

                //FileSystem.ProcessGameManifest(_blockDataDirectory, fileName, rawJson);
                FileSystem.LoadServerConfig(_blockDataDirectory);

                string text = GameManifestV2.Singleton.DumpToLog();

                Debug.Log(text);
            }
            else
            {

                if (GUILayout.Button("Set Game Data", GUILayout.MaxWidth(160), GUILayout.MinHeight(40)))
                {
                    if (string.IsNullOrEmpty(_blockDataDirectory))
                    {
                        StartLoad();
                    }

                    FileSystem.Init();

                    _blockDataDirectory = EditorUtility.OpenFilePanel("Select Game Data Manifest", _blockDataDirectory, "json");
                    //string fileName = FileSystem.GetFileNameWithoutExtension(_blockDataDirectory);

                    //string rawJson = System.IO.File.ReadAllText(_blockDataDirectory);

                    //FileSystem.ProcessGameManifest(_blockDataDirectory, fileName, rawJson);
                    FileSystem.LoadServerConfig(_blockDataDirectory);

                    string text = GameManifestV2.Singleton.DumpToLog();

                    Debug.Log(text);
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New Map", GUILayout.MaxWidth(80), GUILayout.MinHeight(40)))
        {
            //EditorWindow.GetWindow<MapEditorNewMap>().Show();
            _fileName = EditorUtility.SaveFilePanel("New Map", string.Format("{0}{1}maps", Application.persistentDataPath, System.IO.Path.DirectorySeparatorChar), "newmap", "edss");
            Debug.Log("Filename " + _fileName);

            if (!EditorApplication.isPlaying)
            {
                Reset();
                EditorApplication.isPlaying = true;
            }

            _curMapData = new MapDataV2();
            _curMapData.MapName = FileSystem.GetFileNameWithoutExtension(_fileName);
        }

        if (GUILayout.Button("Load Map", GUILayout.MaxWidth(80), GUILayout.MinHeight(40)))
        {
            LoadMap();
        }

        if (isDirty)
        {
            if (GUILayout.Button("Save Map", GUILayout.MaxWidth(80), GUILayout.MinHeight(40)))
            {
                SaveMap();
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

        GUILayout.Space(15);

        GUILayout.Label(string.Format("Block Definitions: {0}", blockDefinition), GUILayout.MaxWidth(250));
        DrawBlocks();
        GUILayout.Space(15);

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
            isDirty = true;
        }

        if (GUILayout.Button("Renum. Region", GUILayout.MaxWidth(100), GUILayout.MinHeight(30)))
        {
            _regionCreateRenumDelete = RegionAction.Renumber;
            _regionRenumberArray = new ushort[_curMapData.MapRegions.Count];

            for (int i = 0; i < _regionRenumberArray.Length; i++)
            {
                _regionRenumberArray[i] = _curMapData.MapRegions[i].RegionUID;
            }
            isDirty = true;
        }

        if (GUILayout.Button("Del Region", GUILayout.MaxWidth(100), GUILayout.MinHeight(30)))
        {
            _regionCreateRenumDelete = RegionAction.Delete;
            isDirty = true;
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

            if (GUILayout.Button("Add New Texture"))
            {
                EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, "", -1);
            }

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
                isDirty = true;
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
                _vw.CreateWorld(_curMapRegion.RegionBlocks, true);

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
                _selectedBlockType = (ushort)EditorGUILayout.IntField("Block Type:", (int)_selectedBlockType);
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
            _editorCamera._targetPlane.SetActive(false);
        }

        if (GUILayout.Button("Entity Edit", GUILayout.MaxWidth(80), GUILayout.MinHeight(30)))
        {
            _selDelAddButtons = SelectDeleteAdd.EntityEdit;
        }

        if (GUILayout.Button("Paint Face", GUILayout.MaxWidth(80), GUILayout.MinHeight(30)))
        {
            _selDelAddButtons = SelectDeleteAdd.FaceEdit;

            if (_editorCamera == null)
            {
                _editorCamera = FindObjectOfType<MapEditorCamera>();
            }

            _editorCamera.MouseSelectionMode = MapEditorCamera.MouseEditMode.FaceEdit;
            _editorCamera._targetPlane.SetActive(true);
            _editorCamera._targetCube.SetActive(false);
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
                _editorCamera._targetPlane.SetActive(false);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (_selDelAddButtons == SelectDeleteAdd.FaceEdit)
        {
            GUILayout.Label("--- AVAILABLE TEXTURES ---");
            GUILayout.BeginHorizontal();

            DrawTextures();
            GUILayout.EndHorizontal();
        }

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

    void DrawBlocks()
    {
        //May need to build them
        if(_blockDataTemplates == null)
        {
            Dictionary<ushort, GameManifestV2.BlockDataTemplate> allBlocks = GameManifestV2.Singleton.GetAllBlockTemplates();

            if(allBlocks != null)
            {
                _blockDataTemplates = new GameManifestV2.BlockDataTemplate[allBlocks.Count];

                int index = 0;
                foreach(KeyValuePair<ushort, GameManifestV2.BlockDataTemplate> block in allBlocks)
                {
                    _blockDataTemplates[index] = block.Value;
                    index++;
                }
            }
        }

        if (_blockDataTemplates != null)
        {
            for (int i = 0; i < _blockDataTemplates.Length; i++)
            {
                if (_blockDataTemplates[i] == null)
                    continue;

                GUILayout.BeginHorizontal();

                GUILayout.Label(string.Format("{0}", _blockDataTemplates[i].BlockName), GUILayout.MaxWidth(120));
                GUILayout.Label(string.Format("UID: {0}", _blockDataTemplates[i].BlockUID), GUILayout.MaxWidth(60));
                GUILayout.Label(string.Format("Req: {0}", _blockDataTemplates[i].BlockRequirement), GUILayout.MaxWidth(60));
                GUILayout.Label(string.Format("Str: {0}", _blockDataTemplates[i].BlockStrength), GUILayout.MaxWidth(60));

                GUILayout.EndHorizontal();
            }
        }
    }

    void DrawTextures()
    {
        if(_textureTemplates == null)
        {
            Dictionary<ushort, GameManifestV2.SpriteDataTemplate> allSprites = GameManifestV2.Singleton.GetAllSprites();

            List<Tuple<ushort, Texture2D, string>> allTextures = new List<Tuple<ushort, Texture2D, string>>();

            if (allSprites == null)
            {
                return;
            }

            foreach (KeyValuePair<ushort, GameManifestV2.SpriteDataTemplate> s in allSprites)
            {
                //Hard code, shouldn't be able to paint "empty"
                if (s.Key == 1)
                {
                    continue;
                }

                Texture2D newTex = new Texture2D(s.Value.WidthHeight.x, s.Value.WidthHeight.y, s.Value.SpriteSheetTemplate.Texture.format, false);

                Color[] c = s.Value.SpriteSheetTemplate.Texture.GetPixels(s.Value.TopLeft.x, s.Value.TopLeft.y, newTex.width, newTex.height);

                newTex.SetPixels(c);
                newTex.filterMode = s.Value.SpriteSheetTemplate.Texture.filterMode;
                newTex.wrapMode = s.Value.SpriteSheetTemplate.Texture.wrapMode;
                newTex.Apply();

                allTextures.Add(Tuple.New<ushort, Texture2D, string>(s.Key, newTex, s.Value.SpriteName));
            }

            _textureTemplates = allTextures.ToArray();
        }

        _textureScrollPos = GUILayout.BeginScrollView(_textureScrollPos);
        for (int i = 0; i < _textureTemplates.Length; i++)
        {
            if (_textureSelIndex == i)
            {
                GUILayout.Label(string.Format("----> Name: {0}", _textureTemplates[i].Third));
                GUILayout.BeginHorizontal();
                GUILayout.Label("----> ", GUILayout.MaxWidth(36));
                if (GUILayout.Button(_textureTemplates[i].Second, GUILayout.MaxWidth(128), GUILayout.MaxHeight(128)))
                {
                    _textureSelIndex = i;
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label(string.Format("Name: {0}", _textureTemplates[i].Third));
                if (GUILayout.Button(_textureTemplates[i].Second, GUILayout.MaxWidth(128), GUILayout.MaxHeight(128)))
                {
                    _textureSelIndex = i;
                }
            }
            GUILayout.Space(10);
        }
        GUILayout.EndScrollView();
    }
    
    /// <summary>
    /// Since the mouse has no reliable way to send messages to the editor window, we need to check the editor mouse to see if it has any new mouse events to process
    /// </summary>
    public void ProcessMouseCameraEvent()
    {
        if(_curMapRegion == null)
            return;

        if (_editorCamera == null)
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

        ushort prevBlockType = 0;
        if (mouseEditEvent._mouseActionType == MapEditorCamera.MouseActionType.AddBlock)
        {
            isDirty = true;

            //If current block is empty
            if (_curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockType == 0)
            {
                MapDataV2.MapBlock newBlock = new MapDataV2.MapBlock();
                newBlock.Init();
                newBlock.BlockType = _selectedBlockType;
                prevBlockType = _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockType;
                _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z] = newBlock;

                _vw.UpdateBlock(mouseEditEvent._position, _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z], prevBlockType);
            }
            //Block isn't empty
            else
            {
                prevBlockType = _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockType;
                _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockType = _selectedBlockType;

                _vw.UpdateBlock(mouseEditEvent._position, _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z], prevBlockType);
            }
        }
        else if (mouseEditEvent._mouseActionType == MapEditorCamera.MouseActionType.RemoveBlock)
        {
            //It's already "empty"
            if (_curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockType == 0)
            {
                return;
            }

            isDirty = true;
            MapDataV2.MapBlock newBlock = new MapDataV2.MapBlock();
            newBlock.Init();
            newBlock.BlockType = 0;
            prevBlockType = _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockType;
            _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z] = newBlock;

            _vw.UpdateBlock(mouseEditEvent._position, _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z], prevBlockType);
        }
        else if (mouseEditEvent._mouseActionType == MapEditorCamera.MouseActionType.PaintFace)
        {
            //If block is empty, don't allow face paint
            if (_curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockType == 0)
            {
                return;
            }

            isDirty = true;
            _vw.UpdateBlockFace(mouseEditEvent._position, mouseEditEvent._face, _textureTemplates[_textureSelIndex].First);
        }
        else if (mouseEditEvent._mouseActionType == MapEditorCamera.MouseActionType.ClearFace)
        {
            //If block is empty, don't allow face paint
            if (_curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockType == 0)
            {
                return;
            }

            GameManifestV2.BlockDataTemplate blockTemplate;

            bool canFind = GameManifestV2.Singleton.GetBlockTemplate(_curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockType,
                out blockTemplate);

            if (canFind)
            {
                isDirty = true;
                _vw.UpdateBlockFace(mouseEditEvent._position, mouseEditEvent._face, blockTemplate.BlockDefaultFaceUIDs[(int)mouseEditEvent._face]);
            }
        }
        else if (mouseEditEvent._mouseActionType == MapEditorCamera.MouseActionType.SampleFace)
        {
            //If block is empty, don't allow face paint
            if (_curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockType == 0)
            {
                return;
            }

            ushort faceSpriteUID = _curMapRegion.RegionBlocks[mouseEditEvent._position.x, mouseEditEvent._position.y, mouseEditEvent._position.z].BlockFacesSpriteUIDs[(int)mouseEditEvent._face];

            //Try to find the actual index based on the sprite UID
            if (_textureTemplates != null)
            {
                for (int i = 0; i < _textureTemplates.Length; i++)
                {
                    if (_textureTemplates[i].First == faceSpriteUID)
                    {
                        _textureSelIndex = i;
                    }
                }
            }
        }
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
            //int width = importTextures[y].width;
            int index = 0;
            Color32[] colors = importTextures[y].GetPixels32();

            for (int z = 0; z < newSize.z; z++)
            {
                for (int x = 0; x < newSize.x; x++)
                {
                    region.RegionBlocks[x, y, z] = MapBlockFromColor(colors[index], colorBlockDefs);
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
                newBlock.Init();
                newBlock.BlockType = colorBlockDefs[i].Second;

                return newBlock;
            }
        }

        MapDataV2.MapBlock blankBlock = new MapDataV2.MapBlock();
        blankBlock.Init();
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
