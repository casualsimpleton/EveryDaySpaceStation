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
    public Vector3 BoxSize { get; private set; }
    EntitySpriteGameObject _entitySpriteGO;
    #endregion

    #region Gets/Sets
    public BoxCollider Cubecollider { get { return _boxCollider; } }
    public EntitySpriteGameObject EntitySpriteGO { get { return _entitySpriteGO; } }
    #endregion

    public void Create(Vector3 size)
    {
        if (_transform == null)
        {
            _transform = this.transform;
        }

        BoxSize = size;
        _boxCollider = this.gameObject.AddComponent<BoxCollider>();
        UpdateSize(BoxSize, new Vector3(0.5f, 0.5f, size.y * 0.5f));
    }

    public void UpdateSize(Vector3 size, Vector3 center)
    {
        _boxCollider.size = size;
        //Offset by a certain amount to use x/z as anchor since boxcolliders are centered
        _boxCollider.center = center;//
    }

    public void Reset()
    {
        this.gameObject.SetActive(false);
        _transform.parent = PoolManager.Singleton._transform;
    }

    public void Attach(EntitySpriteGameObject esgo, bool isTrigger = false)
    {
        _transform.parent = esgo._transform;
        _entitySpriteGO = esgo;
        UpdateSize(_entitySpriteGO.EntityData.CurrentEntityState.StateTemplate.StateColliderSize, 
            new Vector3(0, _entitySpriteGO.EntityData.CurrentEntityState.StateTemplate.StateColliderSize.y * 0.5f, 0));

        if (isTrigger)
        {
            this.gameObject.layer = ClientGameManager.ClientTriggerLayer;
        }
        else
        {
            this.gameObject.layer = ClientGameManager.ClientEntityLayer;
        }

        _boxCollider.isTrigger = isTrigger;
    }

    public void SetActive(bool newState)
    {
        if (newState != gameObject.activeSelf)
        {
            gameObject.SetActive(newState);
        }
    }
}