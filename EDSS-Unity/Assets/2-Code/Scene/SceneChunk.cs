//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// SceneChunk - Represents a chunk of the map. Usuaully going to be 16x16 blocks. Mainly should interact with SceneLevelManager and child SceneBlocks
// Is responsible for rendering blocks to allow batching and minimize meshes in an orderly, predictable fashion
// Created: December 4 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 4 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class SceneChunk : MonoBehaviour
{
    public static float blockSize = 1f;

    public Transform _transform { get; private set; }

    [SerializeField]
    SceneBlock[] _chunkBlocks;

    [SerializeField]
    Vec2Int _chunkPos;

    [SerializeField]
    Vector3 _worldPosition;

    Dictionary<uint, SceneChunkRenderer> _sceneChunkRenders;
    SceneChunkRenderer _lastUsedSCR;

    public void CreateChunk(Vector3 worldPosition, Vec2Int chunkPosition)
    {
        _sceneChunkRenders = new Dictionary<uint, SceneChunkRenderer>();
        _worldPosition = worldPosition;
        _chunkPos = chunkPosition;

        _transform = this.gameObject.transform;
        
        this.gameObject.name = string.Format("Chunk-{0}", _chunkPos);

        _transform.position = _worldPosition;

        _chunkBlocks = new SceneBlock[SceneLevelManager.Singleton.BlocksPerChuck * SceneLevelManager.Singleton.BlocksPerChuck];

        PrepareSceneBlocks();
    }

    protected void PrepareSceneBlocks()
    {
        int x, z;

        int len = SceneLevelManager.Singleton.BlocksPerChuck * SceneLevelManager.Singleton.BlocksPerChuck;
        int vertIndex = 0;
        for (int i = 0; i < len; i++)
        {
            z = i / SceneLevelManager.Singleton.BlocksPerChuck;
            x = i - (z * SceneLevelManager.Singleton.BlocksPerChuck);

            //TODO this currently assumes only one total chunk
            Vec2Int pos = new Vec2Int(x, z);
            int index = Helpers.IndexFromVec2Int(pos, SceneLevelManager.Singleton.BlocksPerChuck);
            int[] vertFirstIndex = new int[SceneBlock.FacesPerBlock];

            #region Verts, Norms, UVs and Triangles
            //Forward Face - Z+ - Clockwise LOOKING AT face - normal point Z+
            //Vert 0
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceZForward] = vertIndex;
            vertIndex += 4;

            //Right Face - X+ - Clockwise LOOKING AT face - normal point X+
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceXForward] = vertIndex;
            vertIndex += 4;

            //Back Face - Z- - Clockwise LOOKING at face - normal pointing Z-
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceZBack] = vertIndex;
            vertIndex += 4;

            //Left Face - X- - Clockwise LOOKING at face - normal pointing X-
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceXBack] = vertIndex;
            vertIndex += 4;

            //Top Face - Y+ - since this will be seen from a top down view (for the time being)
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceTop] = vertIndex;
            vertIndex += 4;

            //Bottom Face - Y+ - Since this is the floor, it needs to point up as well
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceBottom] = vertIndex;
            vertIndex += 4;
            #endregion

            #region Triangle Order
            //Start point is determined since we're only going to be doing that one cube
            int startPoint = i * SceneBlock.FacesPerBlock * 6;
            int triLen = startPoint + (SceneBlock.FacesPerBlock * 6); //Only going to do 6 faces
            int vertCount = 0 + (i * SceneBlock.FacesPerBlock * 4);
            int[] faceFirstTri = new int[SceneBlock.FacesPerBlock];
            int faceIndex = 0;
            for (int j = startPoint; j < triLen; j += 6)
            {
                faceFirstTri[faceIndex] = j;
                
                vertCount += 4;
                faceIndex++;
            }
            #endregion

            _chunkBlocks[i] = new SceneBlock();
            _chunkBlocks[i].Create(pos, pos, index, index, faceFirstTri, vertFirstIndex, this);
        }
    }

    public void UpdateBlock(MapData.MapTileData blockData)
    {
        SceneBlock block = null;

        //TODO This should be calculatable and not required for a search. But too tired to figure it out at the moment
        //-CasualSimpleton
        for (int i = 0; i < _chunkBlocks.Length; i++)
        {
            if (_chunkBlocks[i].WorldPos == blockData.TilePosition)
            {
                block = _chunkBlocks[i];
                break;
            }
        }

        if (block == null)
        {
            Debug.LogWarning(string.Format("Can't find a sceneblock for maptile: {0}", blockData.TilePosition));
            return;
        }

        block.UpdateFaces(blockData);
    }

    #region SceneChunkRenderer Wrangling
    private SceneChunkRenderer GetSceneChunkRenderer(uint scrUID)
    {
        //Since we often use the same one in a row, we'll check if it's the same as the last one, so we can prevent looking it up
        if (_lastUsedSCR != null && scrUID == _lastUsedSCR._MaterialUID)
        {
            return _lastUsedSCR;
        }

        SceneChunkRenderer scr = null;
        bool exists = _sceneChunkRenders.TryGetValue(scrUID, out scr);

        //No SCR exists for that, so we need to get one
        if (!exists)
        {
            Material mat = null;
            GameManager.Singleton.Gamedata.GetMaterial(scrUID, out mat);

            scr = PoolManager.Singleton.RequestSceneChunkRenderer();
            scr.UpdateMaterial(mat, scrUID);

            scr._transform.parent = _transform;
            scr.gameObject.SetActive(true);

            scr.UpdateMaterial(mat, scrUID);

            _sceneChunkRenders.Add(scrUID, scr);
        }

        _lastUsedSCR = scr;

        return scr;
    }
    #endregion

    #region Updates/Modifications
    public void ModifyTriangles(uint rendererUID, int triIndex, int vertOneIndex, int vertTwoIndex, int vertThreeIndex, int vertFourIndex)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);

        scr.ModifyTriangles(triIndex, vertOneIndex, vertTwoIndex, vertThreeIndex, vertFourIndex);
        scr.UpdateMesh();
    }

    public void ModifyTrianglesNoUpdate(uint rendererUID, int triIndex, int vertOneIndex, int vertTwoIndex, int vertThreeIndex, int vertFourIndex)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);

        scr.ModifyTrianglesNoUpdate(triIndex, vertOneIndex, vertTwoIndex, vertThreeIndex, vertFourIndex);
    }

    public void ModifyUV(uint rendererUID, int uvIndex, Vector4 uv)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);
        
        scr.ModifyUV(uvIndex, uv);

        scr.UpdateUV();
    }

    public void ModifyUVNoUpdate(uint rendererUID, int uvIndex, Vector4 uv)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);

        scr.ModifyUVNoUpdate(uvIndex, uv);
    }

    public void ModifyColor(uint rendererUID, int colorIndex, Color32 newColor)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);

        scr.ModifyColorNoUpdate(colorIndex, newColor);

        scr.UpdateColors();
    }

    public void ModifyColorNoUpdate(uint rendererUID, int colorIndex, Color32 newColor)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);

        scr.ModifyColorNoUpdate(colorIndex, newColor);
    }

    public void UpdateMesh(uint rendererUID)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);

        scr.UpdateMesh();
    }

    public void UpdateUV(uint rendererUID)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);

        scr.UpdateUV();
    }

    public void UpdateColors(uint rendererUID)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);

        scr.UpdateColors();
    }
    #endregion
}