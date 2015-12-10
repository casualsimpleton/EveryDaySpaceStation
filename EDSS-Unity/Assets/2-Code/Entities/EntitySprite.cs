//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// EntitySprite - A sprite for an entity, whether player or chair or whatever
// Created: December 9 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 9 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class EntitySprite
{
    #region Vars
    public Transform _transform { get; set; }
    public EDSSSprite _sprite;
    public string _entityName { get; private set; }
    public EntitySpriteGameObject _parentEntity { get; private set; }
    public Vec2Int _tilePos { get; private set; }
    public int _tileIndex { get; private set; }
    #endregion

    #region Gets/Sets
    #endregion

    public EntitySprite(string name, uint edssSpriteUID, EntitySpriteGameObject parent)
    {
        _parentEntity = parent;
        bool exists = GameManager.Singleton.Gamedata.GetSprite(edssSpriteUID, out _sprite);
    }

    public void UpdateMesh(GameObject meshObject)
    {
        meshObject.renderer.sharedMaterial = _sprite.SpriteSheet.Material;
    }

    public void UpdatePosition()
    {
        Vector3 worldPosition = _parentEntity._transform.position;
        _tilePos = new Vec2Int((int)worldPosition.x, (int)worldPosition.z);

        _tileIndex = Helpers.IndexFromVec2Int(_tilePos, GameManager.Singleton.Mapdata._mapSize.x);
    }
}