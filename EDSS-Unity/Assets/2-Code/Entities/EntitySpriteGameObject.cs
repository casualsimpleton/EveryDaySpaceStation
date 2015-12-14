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
    protected EDSSSprite _sprite;
    public Transform _transform { get; private set; }
    public GameObject _mesh { get; private set; }
    public Material _material { get; private set; }
    protected float _updateTimer;
    protected float _updateTimerDelta;
    protected MapData.EntityData _entityData { get; private set; }

    public bool IsClown = false;
    #endregion

    #region Gets/Sets
    public EDSSSprite Sprite { get { return _sprite; } }
    #endregion

    void Start()
    {
        _transform = this.transform;
    }

    public void Create(MapData.EntityData entityData)
    {
        if (_transform == null)
        {
            _transform = this.transform;
        }

        _entityData = entityData;

        if (_mesh == null)
        {
            _mesh = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _mesh.transform.parent = _transform;
            _mesh.transform.localScale = Vector3.one * 0.5f;
            _mesh.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        }

        if (_entityData == null)
            return;

        GameManager.Singleton.Gamedata.GetSprite(_entityData.CurrentEntityState.StateTemplate.SpriteUID, out _sprite);

        UpdateMesh();
    }

    public void Reset()
    {
        _sprite = null;
        _transform = this.gameObject.transform;
        _transform.parent = PoolManager.Singleton._transform;

        this.gameObject.SetActive(false);
    }

    public void UpdateMesh(GameObject meshObject)
    {
        meshObject.renderer.sharedMaterial = _sprite.SpriteSheet.Material;
    }

    public void UpdateMesh()
    {
        UpdateMesh(_mesh);
    }

    public void UpdateUVs()
    {
        _material = _mesh.renderer.sharedMaterial;
        Vector4 uvs = _sprite.GetUVCoords();
        Vector2 uvOffsets = _sprite.uvOffset;
        _material.SetTextureOffset("_MainTex", new Vector2(uvs.x + uvOffsets.x, uvs.y - uvs.w + uvOffsets.y));
        _material.SetTextureScale("_MainTex", new Vector2(uvs.x + uvs.z - uvOffsets.x, uvs.y - uvOffsets.y));
    }

    void Update()
    {
        if (_updateTimer > Time.time)
            return;

UpdateShaderColors();

        _updateTimer = Time.time + _updateTimerDelta;
    }

    public void UpdateShaderColors()
    {
        Color32 newColor = _entityData.MapTile.LightColor;

        _material.SetColor("_Color", newColor);
    }
}