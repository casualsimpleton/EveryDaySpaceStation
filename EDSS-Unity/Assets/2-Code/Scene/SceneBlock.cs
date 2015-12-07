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
    /// Keep a copy of the index of each vertex for the face. That way we don't have to re-compute it each time
    /// Might not be necessary if blocks don't change too often and we find we're using too much memory
    /// </summary>
    [SerializeField]
    int[] _vertFirstIndex;
    
    [SerializeField]
    SceneChunk _parentChunk;
    #endregion

    #region Gets/Sets
    public Vec2Int WorldPos { get { return _worldPos; } }
    public int WorldPosIndex { get { return _worldPosIndex; } }
    public Vec2Int ChunkPos { get { return _chunkPos; } }
    public int ChunkPosIndex { get { return _chunkPosIndex; } }
    #endregion

    public void Create(Vec2Int worldPosition, Vec2Int chunkPosition, int worldPositionIndex, int chunkPositionIndex, int[] facesFirstIndex, int[] vertFirstIndex, SceneChunk parentChunk)
    {
        _worldPos = worldPosition;
        _chunkPos = chunkPosition;

        _worldPosIndex = worldPositionIndex;
        _chunkPosIndex = chunkPositionIndex;

        _facesFirstTriangleIndex = facesFirstIndex;
        _vertFirstIndex = vertFirstIndex;

        _parentChunk = parentChunk;
    }

    public void CollapseAllWalls()
    {
        //Collapse vertical walls and ceiling
        for (int i = 0; i < 5; i++)
        {
            int faceIndex = _facesFirstTriangleIndex[i];

            _parentChunk.ModifyTriangles(faceIndex, faceIndex, faceIndex, faceIndex, faceIndex);
        }
    }

    public void UpdateFaces(MapData.MapTileData mapTileData)
    {
        GameData.GameBlockData.FaceInfo[] faceInfo = mapTileData.BlockType.Faceinfo;

        //Going to assign a random color
        float f = (float)mapTileData.TilePosition.x / 16f;
        byte v = (byte)(255 * f);

        Color32 c = new Color32(v, v, v, v);

        for (int i = 0; i < faceInfo.Length; i++)
        {
            int faceIndex = _facesFirstTriangleIndex[i];

            //Is the face even visible? If false, then we'll just collapse it and be done
            if (!faceInfo[i]._visible)
            {
                //We feed in all the same index and thus make the face basically be non existent
                _parentChunk.ModifyTrianglesNoUpdate(faceIndex, _vertFirstIndex[i], _vertFirstIndex[i], _vertFirstIndex[i], _vertFirstIndex[i]);
            }
            else //Face is visible, let's figure out which direction
            {
                //Faces outwards from center of box
                if (faceInfo[i]._faceDir == GameData.GameBlockData.FaceInfo.FaceDirection.Forward)
                {
                    _parentChunk.ModifyTrianglesNoUpdate(faceIndex, _vertFirstIndex[i], _vertFirstIndex[i] + 1, _vertFirstIndex[i] + 2, _vertFirstIndex[i] + 3);
                }
                else //Inverted faces
                {
                    _parentChunk.ModifyTrianglesNoUpdate(faceIndex, _vertFirstIndex[i] + 2, _vertFirstIndex[i] + 1, _vertFirstIndex[i], _vertFirstIndex[i] + 3);
                }

                //Now find the EDSSSprite info
                EDSSSprite sprite = null;
                bool foundSprite = GameManager.Singleton.Gamedata.GetSprite((uint)mapTileData.FaceSpritesUID[i], out sprite);

                _parentChunk.AssignMaterial(sprite);

                //UVs can use the same vert index since they match in count
                _parentChunk.ModifyUVNoUpdate(_vertFirstIndex[i], sprite.GetUVCoords());
            }

            _parentChunk.ModifyColor(_vertFirstIndex[i], c);
        }

        _parentChunk.UpdateMesh();
        _parentChunk.UpdateUV();
        _parentChunk.UpdateColors();
    }
}