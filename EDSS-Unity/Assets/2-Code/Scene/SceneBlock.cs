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

    public void Create(Vec2Int worldPosition, Vec2Int chunkPosition, int worldPositionIndex, int chunkPositionIndex, int[] facesFirstIndex)
    {
        _worldPos = worldPosition;
        _chunkPos = chunkPosition;

        _worldPosIndex = worldPositionIndex;
        _chunkPosIndex = chunkPositionIndex;

        _facesFirstTriangleIndex = facesFirstIndex;
    }

    #endregion

    public static int FacesPerBlock = 6; //Number of faces per block. Probably will go up as additional default faces are added
}