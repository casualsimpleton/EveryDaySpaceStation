//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// TileLight - A light that affects the tile colors of the world. Not to be confused with Unity's lighting system
// Created: December 7 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 7 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class TileLight
{
    #region Vars
    Color32 _lightColor;
    int _range = 1;
    Vec2Int _tilePos;
    int _tileIndex;
    bool _isMobile;
    //CenteredSquareBounds _bounds;
    #endregion

    #region Gets/Sets
    public Color32 LightColor 
    { 
        get { return _lightColor; }
        set { _lightColor = value; }
    }

    public int LightRange
    {
        get { return _range; }
        set { _range = value; }
    }

    public Vec2Int TilePos 
    { 
        get { return _tilePos; }
        set { _tilePos = value; }
    }

    public int TileIndex
    {
        get { return _tileIndex; }
        set { _tileIndex = value; }
    }

    public bool IsMobile
    {
        get { return _isMobile; }
        set { _isMobile = value; }
    }
    #endregion

    public TileLight(Color32 initColor, int range)
    {
        _lightColor = initColor;
        _range = range;

        //_bounds = new CenteredSquareBounds(Vec2Int.Zero, new Vec2Int(Mathf.RoundToInt(_range * 0.5f), Mathf.RoundToInt(_range * 0.5f)));
    }

    /// <summary>
    /// Convert world position to tile position and index
    /// </summary>
    /// <param name="worldPosition"></param>
    public void UpdatePosition(Vector3 worldPosition)
    {
        _tilePos = new Vec2Int((int)worldPosition.x, (int)worldPosition.z);

        _tileIndex = Helpers.IndexFromVec2Int(_tilePos, GameManager.Singleton.Mapdata._mapSize.x);
    }
}