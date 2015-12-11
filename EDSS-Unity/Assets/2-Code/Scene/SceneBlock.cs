//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// SceneBlock - Represents a single block (or tile) of the game map. That could be space, or a floor, or a wall or door
// Mainly interacts with SceneChunk to update its portion of the chunk's mesh
// IT IS NOT a monobehaviour since it's not really necessary. The chunk will query or push logic checks as necessary
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

[System.Serializable]
public class SceneBlock
{
    #region Vars
    public static int FacesPerBlock = 6; //Number of faces per block. Probably will go up as additional default faces are added
    public static int UnderLayerFaces = 4; //Number of faces in the under layer (layer 0 is scaffold or plating, layer 1 is large pipe, layer 2 is thin pipe, layer 3 is wire)

    /// <summary>
    /// World Position of tile
    /// </summary>
    [SerializeField]
    Vec2Int _worldPos;

    /// <summary>
    /// Int index version of world position for easy access
    /// </summary>
    [SerializeField]
    int _worldPosIndex;

    /// <summary>
    /// Position within the chunk
    /// </summary>
    [SerializeField]
    Vec2Int _chunkPos;

    /// <summary>
    /// Int index version of chunk position
    /// </summary>
    [SerializeField]
    int _chunkPosIndex;

    /// <summary>
    /// Each block has a set of corresponding of faces with SceneChunk (a cube has 6 faces). We keep the first index for each face for easy look up
    /// </summary>
    [SerializeField]
    int[] _facesFirstTriangleIndex;

    /// <summary>
    /// First indices for each of the underlayer faces
    /// </summary>
    [SerializeField]
    int[] _underlayerFacesFirstTriangleIndex;

    /// <summary>
    /// Keep a copy of the index of each vertex for the face. That way we don't have to re-compute it each time
    /// Might not be necessary if blocks don't change too often and we find we're using too much memory
    /// </summary>
    [SerializeField]
    int[] _vertFirstIndex;

    /// <summary>
    /// Keep a copy of the index of each vertex for the underlayer face. That way we don't have to re-compute it each time
    /// Might not be necessary if blocks don't change too often and we find we're using too much memory
    /// </summary>
    [SerializeField]
    int[] _underlayerVertFirstIndex;
    
    [SerializeField]
    SceneChunk _parentChunk;

    /// <summary>
    /// Because the parent SceneChunk might have multiple materials in association, each block will have to know its material so when it goes to the chunk for change, 
    /// the chunk can quickly know which SceneChunkRenderer to modify
    /// </summary>
    [SerializeField]
    HashSet<uint> _associatedRendererUID;
    #endregion

    #region Gets/Sets
    public Vec2Int WorldPos { get { return _worldPos; } }
    public int WorldPosIndex { get { return _worldPosIndex; } }
    public Vec2Int ChunkPos { get { return _chunkPos; } }
    public int ChunkPosIndex { get { return _chunkPosIndex; } }
    public HashSet<uint> AssociatedRendererUID
    {
        get { return _associatedRendererUID; }
        set { _associatedRendererUID = value; }
    }
    #endregion

    public void Create(Vec2Int worldPosition, Vec2Int chunkPosition, int worldPositionIndex, int chunkPositionIndex, 
        int[] facesFirstIndex, int[] vertFirstIndex, int[] underlayerFaceFirstIndex, int[] underlayerVertFirstIndex, SceneChunk parentChunk)
    {
        _associatedRendererUID = new HashSet<uint>();

        _worldPos = worldPosition;
        _chunkPos = chunkPosition;

        _worldPosIndex = worldPositionIndex;
        _chunkPosIndex = chunkPositionIndex;

        _facesFirstTriangleIndex = facesFirstIndex;
        _vertFirstIndex = vertFirstIndex;

        _underlayerFacesFirstTriangleIndex = underlayerFaceFirstIndex;
        _underlayerVertFirstIndex = underlayerVertFirstIndex;

        _parentChunk = parentChunk;
    }

    /// <summary>
    /// Call this when you need to collapse the whole block
    /// </summary>
    //public void CollapseBlock()
    //{
    //    //Collapse vertical walls and ceiling
    //    for (int i = 0; i < 6; i++)
    //    {
    //        int faceIndex = _facesFirstTriangleIndex[i];

    //        _parentChunk.ModifyTriangles(_associatedRendererUID, faceIndex, _vertFirstIndex[i], _vertFirstIndex[i], _vertFirstIndex[i], _vertFirstIndex[i]);
    //    }
    //}

    public void UpdateLights(MapData.MapTileData mapTileData)
    {
        GameData.GameBlockData.FaceInfo[] faceInfo = mapTileData.BlockType.Faceinfo;
        for (int i = 0; i < faceInfo.Length; i++)
        {
            foreach (uint uid in _associatedRendererUID)
            {
                _parentChunk.ModifyColorNoUpdate(uid, _vertFirstIndex[i], mapTileData.LightColor);
            }
        }
    }

    public void UpdateSpecificFaceLights(GameData.GameBlockData.BlockFaces face, Color32 newColor)
    {
        int faceValueIndex = (int)face;

        foreach (uint uid in _associatedRendererUID)
        {
            _parentChunk.ModifyColorNoUpdate(uid, _vertFirstIndex[faceValueIndex], newColor);
        }
    }

    public void UpdateFaces(MapData.MapTileData mapTileData, bool isFirstTime = false)
    {
        GameData.GameBlockData.FaceInfo[] faceInfo = mapTileData.BlockType.Faceinfo;

        //Going to assign a random color
        //float f = (float)mapTileData.TilePosition.x / 16f;
        //byte v = (byte)(255 * f);

        //Color32 c = new Color32(v, v, v, v);

        for (int i = 0; i < faceInfo.Length; i++)
        {
            //First find the EDSSSprite info
            EDSSSprite sprite = null;
            bool foundSprite = GameManager.Singleton.Gamedata.GetSprite((uint)mapTileData.FaceSpritesUID[i], out sprite);

            if (!foundSprite)
            {
                Debug.LogError(string.Format("Can't find sprite with UID: {0}", (uint)mapTileData.FaceSpritesUID[i]));
            }

            uint uid = sprite.SpriteSheet.MaterialUID;
            _associatedRendererUID.Add(uid);

            int faceIndex = _facesFirstTriangleIndex[i];

            //Is the face even visible? If false, then we'll just collapse it and be done
            if (!faceInfo[i]._visible)
            {
                //We feed in all the same index and thus make the face basically be non existent
                _parentChunk.ModifyTrianglesNoUpdate(uid, faceIndex, _vertFirstIndex[i], _vertFirstIndex[i], _vertFirstIndex[i], _vertFirstIndex[i]);
            }
            else //Face is visible, let's figure out which direction
            {
                //Faces outwards from center of box
                if (faceInfo[i]._faceDir == GameData.GameBlockData.FaceInfo.FaceDirection.Forward)
                {
                    _parentChunk.ModifyTrianglesNoUpdate(uid, faceIndex, _vertFirstIndex[i], _vertFirstIndex[i] + 1, _vertFirstIndex[i] + 2, _vertFirstIndex[i] + 3);
                }
                else //Inverted faces
                {
                    _parentChunk.ModifyTrianglesNoUpdate(uid, faceIndex, _vertFirstIndex[i] + 2, _vertFirstIndex[i] + 1, _vertFirstIndex[i], _vertFirstIndex[i] + 3);
                }

                //_parentChunk.AssignMaterial(sprite);

                //UVs can use the same vert index since they match in count
                _parentChunk.ModifyUVNoUpdate(uid, _vertFirstIndex[i], sprite.GetUVCoords(), sprite.uvOffset);
            }

            //_parentChunk.ModifyColor(_associatedRendererUID, _vertFirstIndex[i], c);
        }

        foreach (uint uid in _associatedRendererUID)
        {
            _parentChunk.UpdateMesh(uid, isFirstTime);
            _parentChunk.UpdateUV(uid);
            _parentChunk.UpdateColors(uid);
        }
    }

    public void UpdateUnderLayerFaces(MapData.MapTileData mapTileData, bool isFirstTime = false)
    {
        uint[] underLayerFaceSpritesUID = mapTileData.UnderLayerFaceSpritesUID;

        for (int i = 0; i < underLayerFaceSpritesUID.Length; i++)
        {
            //Look up sprite
            EDSSSprite sprite = null;
            bool foundSprite = GameManager.Singleton.Gamedata.GetSprite(underLayerFaceSpritesUID[i], out sprite);

            if (!foundSprite)
            {
                Debug.LogError(string.Format("Can't find sprite with UID: {0}", (uint)underLayerFaceSpritesUID[i]));
            }

            uint uid = sprite.SpriteSheet.MaterialUID;
            _associatedRendererUID.Add(uid);

            int faceIndex = _underlayerFacesFirstTriangleIndex[i];

            //Is the face visible? We tell this by the UID
            if (underLayerFaceSpritesUID[i] == 0)
            {
                //It should be visible, so collapse it
                _parentChunk.ModifyUnderlayerTrianglesNoUpdate(uid, faceIndex, _underlayerVertFirstIndex[i], _underlayerVertFirstIndex[i], _underlayerVertFirstIndex[i], _underlayerVertFirstIndex[i]);
            }
            else //Face has something present
            {
                _parentChunk.ModifyUnderlayerTrianglesNoUpdate(uid, faceIndex, _underlayerVertFirstIndex[i], _underlayerVertFirstIndex[i] + 1, _underlayerVertFirstIndex[i] + 2, _underlayerVertFirstIndex[i] + 3);
            }

            _parentChunk.ModifyUnderlayerUVNoUpdate(uid, _underlayerVertFirstIndex[i], sprite.GetUVCoords(), sprite.uvOffset);
        }

        foreach(uint uid in _associatedRendererUID)
        {
            _parentChunk.UpdateUnderlayerMesh(uid, isFirstTime);
            _parentChunk.UpdateUnderlayerUV(uid);
            _parentChunk.UpdateUnderlayerColors(uid);
        }
    }
}