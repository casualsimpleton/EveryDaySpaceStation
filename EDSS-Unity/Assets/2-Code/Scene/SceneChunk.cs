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

    Dictionary<uint, SceneChunkScaffoldRenderer> _sceneChunkScaffoldRenders;
    SceneChunkScaffoldRenderer _lastUsedSSCR;

    public void CreateChunk(Vector3 worldPosition, Vec2Int chunkPosition)
    {
        _sceneChunkRenders = new Dictionary<uint, SceneChunkRenderer>();
        _sceneChunkScaffoldRenders = new Dictionary<uint, SceneChunkScaffoldRenderer>();
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
        int underLayerVertIndex = 0;
        for (int i = 0; i < len; i++)
        {
            z = i / SceneLevelManager.Singleton.BlocksPerChuck;
            x = i - (z * SceneLevelManager.Singleton.BlocksPerChuck);

            //TODO this currently assumes only one total chunk
            Vec2Int pos = new Vec2Int(x, z);
            int index = Helpers.IndexFromVec2Int(pos, SceneLevelManager.Singleton.BlocksPerChuck);
            int[] vertFirstIndex = new int[SceneBlock.FacesPerBlock];
            int[] underlayerVertFirstIndex = new int[SceneBlock.UnderLayerFaces];

            #region Verts, Norms, UVs and Triangles
            //Forward Face - Z+ - Clockwise LOOKING AT face - normal point Z+
            //Vert 0
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceZForward] = vertIndex;
            underlayerVertFirstIndex[(int)GameData.GameBlockData.UnderFaces.BottomLayer] = underLayerVertIndex;
            vertIndex += 4;
            underLayerVertIndex += 4;

            //Right Face - X+ - Clockwise LOOKING AT face - normal point X+
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceXForward] = vertIndex;
            underlayerVertFirstIndex[(int)GameData.GameBlockData.UnderFaces.LargePipeLayer] = underLayerVertIndex;
            vertIndex += 4;
            underLayerVertIndex += 4;

            //Back Face - Z- - Clockwise LOOKING at face - normal pointing Z-
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceZBack] = vertIndex;
            underlayerVertFirstIndex[(int)GameData.GameBlockData.UnderFaces.ThinPipeLayer] = underLayerVertIndex;
            vertIndex += 4;
            underLayerVertIndex += 4;

            //Left Face - X- - Clockwise LOOKING at face - normal pointing X-
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceXBack] = vertIndex;
            underlayerVertFirstIndex[(int)GameData.GameBlockData.UnderFaces.WireLayer] = underLayerVertIndex;
            vertIndex += 4;
            underLayerVertIndex += 4;

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

            startPoint = i * SceneBlock.UnderLayerFaces * 6;
            triLen = startPoint + (SceneBlock.UnderLayerFaces * 6);
            vertCount = 0 + (i * SceneBlock.UnderLayerFaces * 4);
            int[] underlayerFaceFirstIndex = new int[SceneBlock.UnderLayerFaces];
            faceIndex = 0;
            for (int j = startPoint; j < triLen; j += 6)
            {
                underlayerFaceFirstIndex[faceIndex] = j;
                vertCount += 4;
                faceIndex++;
            }
            #endregion

            _chunkBlocks[i] = new SceneBlock();
            Vec2Int worldPos = new Vec2Int(pos.x + (SceneLevelManager.Singleton.BlocksPerChuck * _chunkPos.x), pos.y + (SceneLevelManager.Singleton.BlocksPerChuck * _chunkPos.y));
            int worldIndex = Helpers.IndexFromVec2Int(worldPos, GameManager.Singleton.Mapdata._mapSize.x);
            _chunkBlocks[i].Create(worldPos, pos, worldIndex, index, faceFirstTri, vertFirstIndex, underlayerFaceFirstIndex, underlayerVertFirstIndex, this);
        }
    }

    public void UpdateBlockLight(MapData.MapTileData blockData, int localTileIndex)
    {
        SceneBlock block = null;
        block = _chunkBlocks[localTileIndex];

        if (block == null)
        {
            Debug.LogWarning(string.Format("Can't find a sceneblock for maptile: {0}", blockData.TilePosition));
            return;
        }

        block.UpdateLights(blockData);
    }

    public void UpdateBlockLightWithNeighborsWalls(MapData.MapTileData blockData, int localTileIndex)
    {
        SceneBlock block = null;
        block = _chunkBlocks[localTileIndex];

        if (block == null)
        {
            Debug.LogWarning(string.Format("Can't find a sceneblock for maptile: {0}", blockData.TilePosition));
            return;
        }

        if (blockData.BlocksLight)
        {
            return;
        }

        block.UpdateLights(blockData);

        #region Neighbors
        Vec2Int blockPos = block.WorldPos;

        //Now check neighbors
        //Top neighbor, we'd be altering their "Z-" or "down face" when looking from above
        Vec2Int topNeighborPos = new Vec2Int(blockPos.x, blockPos.y + 1);

        //This neighbor is actually in another chunk
        if (topNeighborPos.x < 0 || topNeighborPos.x > SceneLevelManager.Singleton.BlocksPerChuck - 1 || topNeighborPos.y < 0 || topNeighborPos.y > SceneLevelManager.Singleton.BlocksPerChuck - 1)
        {
            SceneLevelManager.Singleton.UpdateNeighborChunkWallLight(block.WorldPosIndex + GameManager.Singleton.Mapdata._mapSize.x, new Vec2Int(block.WorldPos.x, block.WorldPos.y + 1), GameData.GameBlockData.BlockFaces.FaceZBack, blockData.LightColor);
        }
        else
        {
            int topNeighborIndex = Helpers.IndexFromVec2Int(topNeighborPos, SceneLevelManager.Singleton.BlocksPerChuck);
            SceneBlock topBlock = _chunkBlocks[topNeighborIndex];
            topBlock.UpdateSpecificFaceLights(GameData.GameBlockData.BlockFaces.FaceZBack, blockData.LightColor);
        }

        //Right neighbor, we'd be altering their "X-" or "left face" when looking from above
        Vec2Int rightNeighborPos = new Vec2Int(blockPos.x + 1, blockPos.y);

        //This neighbor is actually in another chunk
        if (rightNeighborPos.x < 0 || rightNeighborPos.x > SceneLevelManager.Singleton.BlocksPerChuck - 1 || rightNeighborPos.y < 0 || rightNeighborPos.y > SceneLevelManager.Singleton.BlocksPerChuck - 1)
        {
            SceneLevelManager.Singleton.UpdateNeighborChunkWallLight(block.WorldPosIndex + 1, new Vec2Int(block.WorldPos.x + 1, block.WorldPos.y), GameData.GameBlockData.BlockFaces.FaceXBack, blockData.LightColor);
        }
        else
        {

            int rightNeighborIndex = Helpers.IndexFromVec2Int(rightNeighborPos, SceneLevelManager.Singleton.BlocksPerChuck);

            SceneBlock rightBlock = _chunkBlocks[rightNeighborIndex];
            rightBlock.UpdateSpecificFaceLights(GameData.GameBlockData.BlockFaces.FaceXBack, blockData.LightColor);
        }

        //Down neighbor, we'd be altering their "Z+" or "up face" when looking from above
        Vec2Int downNeighborPos = new Vec2Int(blockPos.x, blockPos.y - 1);

        //This neighbor is actually in another chunk
        if (downNeighborPos.x < 0 || downNeighborPos.x > SceneLevelManager.Singleton.BlocksPerChuck - 1 || downNeighborPos.y < 0 || downNeighborPos.y > SceneLevelManager.Singleton.BlocksPerChuck - 1)
        {
            SceneLevelManager.Singleton.UpdateNeighborChunkWallLight(block.WorldPosIndex - GameManager.Singleton.Mapdata._mapSize.x, new Vec2Int(block.WorldPos.x, block.WorldPos.y - 1), GameData.GameBlockData.BlockFaces.FaceZForward, blockData.LightColor);
        }
        else
        {

            int downNeighborIndex = Helpers.IndexFromVec2Int(downNeighborPos, SceneLevelManager.Singleton.BlocksPerChuck);

            SceneBlock downBlock = _chunkBlocks[downNeighborIndex];
            downBlock.UpdateSpecificFaceLights(GameData.GameBlockData.BlockFaces.FaceZForward, blockData.LightColor);
        }

        //Left neighbor, we'd be altering their "X+" or "right face" when looking from above
        Vec2Int leftNeighborPos = new Vec2Int(blockPos.x - 1, blockPos.y);

        //This neighbor is actually in another chunk
        if (leftNeighborPos.x < 0 || leftNeighborPos.x > SceneLevelManager.Singleton.BlocksPerChuck - 1 || leftNeighborPos.y < 0 || leftNeighborPos.y > SceneLevelManager.Singleton.BlocksPerChuck - 1)
        {
            SceneLevelManager.Singleton.UpdateNeighborChunkWallLight(block.WorldPosIndex - 1, new Vec2Int(block.WorldPos.x + 1, block.WorldPos.y), GameData.GameBlockData.BlockFaces.FaceXForward, blockData.LightColor);
        }
        else
        {

            int leftNeighborIndex = Helpers.IndexFromVec2Int(leftNeighborPos, SceneLevelManager.Singleton.BlocksPerChuck);

            SceneBlock leftBlock = _chunkBlocks[leftNeighborIndex];
            leftBlock.UpdateSpecificFaceLights(GameData.GameBlockData.BlockFaces.FaceXForward, blockData.LightColor);
        }
        #endregion
    }

    public void UpdateBlockWithLightFromNeighborChunk(int localTileIndex, GameData.GameBlockData.BlockFaces face, Color32 color)
    {
        SceneBlock block = null;
        block = _chunkBlocks[localTileIndex];

        block.UpdateSpecificFaceLights(face, color);
    }


    /// <summary>
    /// Call whenever updating a block with new info (like a window has been smashed or a wall built)
    /// </summary>
    /// <param name="blockData"></param>
    public void UpdateBlock(MapData.MapTileData blockData, int localTileIndex)
    {
        SceneBlock block = null;

        //TODO This should be calculatable and not required for a search. But too tired to figure it out at the moment
        //-CasualSimpleton
        //for (int i = 0; i < _chunkBlocks.Length; i++)
        //{
        //    if (_chunkBlocks[i].WorldPos == blockData.TilePosition)
        //    {
        //        block = _chunkBlocks[i];
        //        break;
        //    }
        //}
        block = _chunkBlocks[localTileIndex];

        if (block == null)
        {
            Debug.LogWarning(string.Format("Can't find a sceneblock for maptile: {0}", blockData.TilePosition));
            return;
        }

        block.UpdateFaces(blockData, false);
        block.UpdateUnderLayerFaces(blockData, false);
    }

    /// <summary>
    /// Call when starting up the map, this streamlines a few things, like ensuring the meshcollider is only refreshed once at the end
    /// </summary>
    /// <param name="blockData"></param>
    public void FirstUpdateBlock(MapData.MapTileData blockData, int localTileIndex)
    {
        SceneBlock block = null;

        ////TODO This should be calculatable and not required for a search. But too tired to figure it out at the moment
        ////-CasualSimpleton
        //for (int i = 0; i < _chunkBlocks.Length; i++)
        //{
        //    if (_chunkBlocks[i].WorldPos == blockData.TilePosition)
        //    {
        //        block = _chunkBlocks[i];
        //        break;
        //    }
        //}
        block = _chunkBlocks[localTileIndex];

        if (block == null)
        {
            Debug.LogWarning(string.Format("Can't find a sceneblock for maptile: {0}", blockData.TilePosition));
            return;
        }

        block.UpdateFaces(blockData, true);
        block.UpdateUnderLayerFaces(blockData, true);
    }

    public void UpdateAllMeshColliders()
    {
        foreach (KeyValuePair<uint, SceneChunkRenderer> scr in _sceneChunkRenders)
        {
            scr.Value.UpdateMesh(false);
        }
    }

    public void UpdateAllMeshColors()
    {
        foreach (KeyValuePair<uint, SceneChunkRenderer> scr in _sceneChunkRenders)
        {
            scr.Value.UpdateColors();
        }
    }

    public void UpdateAllUnderLayerColliders()
    {
        //We'll use a large box collider here since players won't be directly walking on them
    }

    public void UpdateAllUnderLayerMeshColors()
    {
        foreach (KeyValuePair<uint, SceneChunkScaffoldRenderer> sscr in _sceneChunkScaffoldRenders)
        {
            sscr.Value.UpdateColors();
        }
    }

    #region Scene Chunk Renderer Wrangling
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

            scr.AssignToChunk(this);
            scr.gameObject.SetActive(true);

            scr.UpdateMaterial(mat, scrUID);

            _sceneChunkRenders.Add(scrUID, scr);
        }

        _lastUsedSCR = scr;

        return scr;
    }

    private SceneChunkScaffoldRenderer GetSceneChunkScaffoldRenderer(uint sscrUID)
    {
        //Since we often use the same one in a row, we'll check if it's the same as the last one, so we can prevent looking it up
        if (_lastUsedSSCR != null && sscrUID == _lastUsedSSCR._MaterialUID)
        {
            return _lastUsedSSCR;
        }

        SceneChunkScaffoldRenderer sscr = null;
        bool exists = _sceneChunkScaffoldRenders.TryGetValue(sscrUID, out sscr);

        //No SSCR exists for that, so we need to get one
        if (!exists)
        {
            Material mat = null;
            GameManager.Singleton.Gamedata.GetMaterial(sscrUID, out mat);

            sscr = PoolManager.Singleton.RequestSceneChunkScaffolderRenderer();
            sscr.UpdateMaterial(mat, sscrUID);

            sscr.AssignToChunk(this);
            sscr.gameObject.SetActive(true);

            sscr.UpdateMaterial(mat, sscrUID);

            _sceneChunkScaffoldRenders.Add(sscrUID, sscr);
        }

        _lastUsedSSCR = sscr;

        return sscr;
    }
    #endregion

    #region Updates/Modifications
    public void ModifyTriangles(uint rendererUID, int triIndex, int vertOneIndex, int vertTwoIndex, int vertThreeIndex, int vertFourIndex)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);

        scr.ModifyTriangles(triIndex, vertOneIndex, vertTwoIndex, vertThreeIndex, vertFourIndex);
        scr.UpdateMesh(true);
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

    public void ModifyUVNoUpdate(uint rendererUID, int uvIndex, Vector4 uv, Vector2 uvOffset)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);

        scr.ModifyUVNoUpdate(uvIndex, uv, uvOffset);
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

    public void UpdateMesh(uint rendererUID, bool isFirstTime = false)
    {
        SceneChunkRenderer scr = GetSceneChunkRenderer(rendererUID);

        scr.UpdateMesh(isFirstTime);
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

    #region Underlayers Updates/Modifications
    public void ModifyUnderlayerTriangles(uint rendererUID, int triIndex, int vertOneIndex, int vertTwoIndex, int vertThreeIndex, int vertFourIndex)
    {
        SceneChunkScaffoldRenderer sscr = GetSceneChunkScaffoldRenderer(rendererUID);

        sscr.ModifyTriangles(triIndex, vertOneIndex, vertTwoIndex, vertThreeIndex, vertFourIndex);
        sscr.UpdateMesh(true);
    }

    public void ModifyUnderlayerTrianglesNoUpdate(uint rendererUID, int triIndex, int vertOneIndex, int vertTwoIndex, int vertThreeIndex, int vertFourIndex)
    {
        SceneChunkScaffoldRenderer sscr = GetSceneChunkScaffoldRenderer(rendererUID);

        sscr.ModifyTrianglesNoUpdate(triIndex, vertOneIndex, vertTwoIndex, vertThreeIndex, vertFourIndex);
    }

    public void ModifyUnderlayerUV(uint rendererUID, int uvIndex, Vector4 uv)
    {
        SceneChunkScaffoldRenderer sscr = GetSceneChunkScaffoldRenderer(rendererUID);

        sscr.ModifyUV(uvIndex, uv);

        sscr.UpdateUV();
    }

    public void ModifyUnderlayerUVNoUpdate(uint rendererUID, int uvIndex, Vector4 uv, Vector2 uvOffset)
    {
        SceneChunkScaffoldRenderer sscr = GetSceneChunkScaffoldRenderer(rendererUID);

        sscr.ModifyUVNoUpdate(uvIndex, uv, uvOffset);
    }

    public void ModifyUnderlayerColor(uint rendererUID, int colorIndex, Color32 newColor)
    {
        SceneChunkScaffoldRenderer sscr = GetSceneChunkScaffoldRenderer(rendererUID);

        sscr.ModifyColorNoUpdate(colorIndex, newColor);

        sscr.UpdateColors();
    }

    public void ModifyUnderlayerColorNoUpdate(uint rendererUID, int colorIndex, Color32 newColor)
    {
        SceneChunkScaffoldRenderer sscr = GetSceneChunkScaffoldRenderer(rendererUID);

        sscr.ModifyColorNoUpdate(colorIndex, newColor);
    }

    public void UpdateUnderlayerMesh(uint rendererUID, bool isFirstTime = false)
    {
        SceneChunkScaffoldRenderer sscr = GetSceneChunkScaffoldRenderer(rendererUID);

        sscr.UpdateMesh(isFirstTime);
    }

    public void UpdateUnderlayerUV(uint rendererUID)
    {
        SceneChunkScaffoldRenderer sscr = GetSceneChunkScaffoldRenderer(rendererUID);

        sscr.UpdateUV();
    }

    public void UpdateUnderlayerColors(uint rendererUID)
    {
        SceneChunkScaffoldRenderer sscr = GetSceneChunkScaffoldRenderer(rendererUID);

        sscr.UpdateColors();
    }
    #endregion
}