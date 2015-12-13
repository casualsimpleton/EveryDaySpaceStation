//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// EntitySpriteGameObject - Gameobject class for holding EntitySprite
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

public class EntitySpriteGameObject : MonoBehaviour
{
    #region Vars
    EntitySprite _entitySprite;
    public Transform _transform { get; private set; }
    public GameObject _mesh { get; private set; }
    public Material _material { get; private set; }
    protected float _updateTimer;
    protected float _updateTimerDelta;
    #endregion

    #region Gets/Sets
    #endregion

    public void Create(EntitySprite newEntity)
    {
        _entitySprite = newEntity;
        if (newEntity != null)
        {
            newEntity._transform = this.transform;
        }
    }

    public void Reset()
    {
        _entitySprite = null;
        _transform = this.gameObject.transform;
        _transform.parent = PoolManager.Singleton._transform;

        this.gameObject.SetActive(false);
    }

    void Start()
    {
        _transform = this.transform;

        EntitySprite newSprite = new EntitySprite("Clown", 100, this);
        Create(newSprite);

        _mesh = GameObject.CreatePrimitive(PrimitiveType.Quad);
        _mesh.transform.parent = _transform;
        _mesh.transform.localScale = Vector3.one * 0.5f;
        _mesh.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        
        UpdateMesh();

        _material = _mesh.renderer.sharedMaterial;
        Vector4 uvs = _entitySprite._sprite.GetUVCoords();
        Vector2 uvOffsets = _entitySprite._sprite.uvOffset;
        _material.SetTextureOffset("_MainTex", new Vector2(uvs.x + uvOffsets.x, uvs.y - uvs.w + uvOffsets.y));
        _material.SetTextureScale("_MainTex", new Vector2(uvs.x + uvs.z - uvOffsets.x, uvs.y - uvOffsets.y));

        //_uv[uvIndex] = new Vector2(uv.x + uv.z - uvOffset.x, uv.y - uvOffset.y);
        //_uv[uvIndex + 1] = new Vector2(uv.x + uv.z - uvOffset.x, uv.y - uv.w + uvOffset.y);
        //_uv[uvIndex + 2] = new Vector2(uv.x + uvOffset.x, uv.y - uv.w + uvOffset.y);
        //_uv[uvIndex + 3] = new Vector2(uv.x + uvOffset.x, uv.y - uvOffset.y);
    }

    public void UpdateMesh()
    {
        _entitySprite.UpdateMesh(_mesh);
    }

    void Update()
    {
        if (_updateTimer > Time.time)
            return;


        _entitySprite.UpdatePosition();
        UpdateShaderColors();

        _updateTimer = Time.time + _updateTimerDelta;
    }

    public void UpdateShaderColors()
    {
        Color32 newColor = GameManager.Singleton.Mapdata._mapTiles[_entitySprite._tileIndex].LightColor;

        _material.SetColor("_Color", newColor);
    }
}