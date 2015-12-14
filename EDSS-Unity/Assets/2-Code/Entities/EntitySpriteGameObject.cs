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
    protected LightComponent _lightComponent;

    public bool IsClown = false;
    #endregion

    #region Gets/Sets
    public EDSSSprite Sprite { get { return _sprite; } }
    public MapData.EntityData EntityData { get { return _entityData; } }
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
            //_mesh = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _mesh = this.gameObject;
            _mesh.transform.parent = _transform;
            _mesh.transform.localScale = Vector3.one * 0.5f;
            _mesh.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        }

        if (_entityData == null)
            return;

        GameManager.Singleton.Gamedata.GetSprite(_entityData.CurrentEntityState.StateTemplate.SpriteUID, out _sprite);

        UpdateMesh();
        UpdateUVs();
    }

    public void UpdateLightComponent()
    {
        _lightComponent = this.gameObject.GetComponent<LightComponent>();
    }

    public void Reset()
    {
        _sprite = null;
        _transform = this.gameObject.transform;
        _transform.parent = PoolManager.Singleton._transform;
        _lightComponent = null;

        this.gameObject.SetActive(false);
    }

    public void UpdateMesh(GameObject meshObject)
    {
        _transform.localScale = new Vector3(_entityData.CurrentEntityState.StateTemplate.StateSize.x, _entityData.CurrentEntityState.StateTemplate.StateSize.y, _entityData.CurrentEntityState.StateTemplate.StateSize.z);
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
        //Vector2 uvOffsets = _sprite.uvOffset;
        //Vector2 offset = new Vector2(uvs.x + uvOffsets.x, uvs.y - uvs.w + uvOffsets.y);
        //Vector2 scale = new Vector2(uvs.z - uvOffsets.x, uvs.w - uvOffsets.y);
        Vector2 offset = new Vector2(uvs.x, uvs.y - uvs.w);
        Vector2 scale = new Vector2(uvs.z, uvs.w);
        _material.SetTextureOffset("_MainTex", offset);
        _material.SetTextureScale("_MainTex", scale);
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
        Color32 newColor;
        if (_lightComponent != null)
        {
            //We've got a light component attached, and we don't want to wash it out, so we'll half the color
            newColor = new Color32((byte)(_lightComponent.LightColor.r * 0.5f), (byte)(_lightComponent.LightColor.g * 0.5f), (byte)(_lightComponent.LightColor.b * 0.5f), _lightComponent.LightColor.a);
        }
        else
        {
            newColor = _entityData.MapTile.LightColor;
        }

        _material.SetColor("_Color", newColor);
    }

    public void Detach()
    {
        _transform.parent = null;
        this.gameObject.SetActive(true);
    }

    public void UpdatePosition()
    {
        float yRot = 0f;
        //Since we're using a center oriented Quad, we need to offset by 0.5f in either x or z depending on anchor
        float xOffSet = 0;
        float zOffset = 0f;

        switch (_entityData.WallAnchor)
        {
            case MapData.EntityData.EntityWallAnchor.Front:
                yRot = 0f;
                xOffSet = 0.5f;
                zOffset = 0.95f;
                break;

            case MapData.EntityData.EntityWallAnchor.Right:
                yRot = 90f;
                zOffset = 0.5f;
                xOffSet = 0.95f;
                break;

            case MapData.EntityData.EntityWallAnchor.Back:
                yRot = 180f;
                xOffSet = 0.5f;
                break;

            case MapData.EntityData.EntityWallAnchor.Left:
                yRot = 270f;
                zOffset = 0.5f;
                break;

            case MapData.EntityData.EntityWallAnchor.Ceiling:
                yRot = 0f;
                xOffSet = 0.5f;
                zOffset = 0.5f;
                break;

            case MapData.EntityData.EntityWallAnchor.Floor:
                yRot = 0f;
                xOffSet = 0.5f;
                zOffset = 0.5f;
                break;
        }

        _transform.rotation = Quaternion.Euler(_entityData.Rotation.x, yRot + _entityData.Rotation.y, _entityData.Rotation.z);
        _transform.position = _entityData.MapTile.GetWorldPosition() + _entityData.CurrentEntityState.StateTemplate.StatePositionOffset + new Vector3(xOffSet, 0f, zOffset);
        
    }
}