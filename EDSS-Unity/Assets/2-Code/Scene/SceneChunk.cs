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
    #region Vars
#if UNITY_EDITOR
    public bool ShowVertGizmos = false;
#endif

    public static float blockSize = 1f;

    #endregion
}