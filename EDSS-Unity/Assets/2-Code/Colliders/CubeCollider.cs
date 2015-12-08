//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// CubeCollider - A Unity monobehaviour class attached to the cube physics collider object
// Mainly used for walls
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

public class CubeCollider : MonoBehaviour
{
    #region Vars
    public Transform _transform { get; private set; }
    BoxCollider _boxCollider;
    public Vec2Int BoxSize { get; private set; }
    #endregion

    #region Gets/Sets
    public BoxCollider Cubecollider { get { return _boxCollider; } }
    #endregion

    public void Create(Vec2Int size)
    {
        BoxSize = size;
        _transform = this.transform;
        _boxCollider = this.gameObject.AddComponent<BoxCollider>();
        _boxCollider.size = new Vector3(size.x, 1f, size.y);
        //Offset by a certain amount to use x/z as anchor since boxcolliders are centered
        _boxCollider.center = new Vector3(0.5f, 0.5f, size.y * 0.5f);
    }

    public void Reset()
    {
        this.gameObject.SetActive(false);
        _transform.parent = PoolManager.Singleton._transform;
    }
}