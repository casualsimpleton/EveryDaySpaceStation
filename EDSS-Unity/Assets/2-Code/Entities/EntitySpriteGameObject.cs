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
    protected MeshQuad _meshQuad;
    protected EDSSSprite _sprite;
    public Transform _transform { get; private set; }
    public Material _material { get; private set; }
    protected float _updateTimer;
    protected float _updateTimerDelta;
    protected MapData.EntityData _entityData { get; private set; }
    protected LightComponent _lightComponent;
    protected DoorComponent _doorComponent;
    protected MultiAngleComponent _multiAngleComponent;

    protected CubeCollider _cubeCollider;

    public bool IsClown = false;
    #endregion

    #region Gets/Sets
    public MeshQuad Meshquad { get { return _meshQuad; } }
    public EDSSSprite Sprite { get { return _sprite; } }
    public MapData.EntityData EntityData { get { return _entityData; } }
    public CubeCollider Cubecollider { get { return _cubeCollider; } }
    #endregion

    void Start()
    {
        _transform = this.transform;
    }

    void OnEnable()
    {
        if (_meshQuad != null)
        {
            _meshQuad.gameObject.SetActive(true);
        }

        if (_cubeCollider != null)
        {
            _cubeCollider.gameObject.SetActive(true);
        }
    }

    void OnDisable()
    {
        if (_meshQuad != null)
        {
            _meshQuad.gameObject.SetActive(false);
        }

        if (_cubeCollider != null)
        {
            _cubeCollider.gameObject.SetActive(false);
        }
    }

    public void Create(MapData.EntityData entityData)
    {
        if (_transform == null)
        {
            _transform = this.transform;
        }

        _entityData = entityData;

        if (_entityData == null)
            return;

        if (_meshQuad == null)
        {
            //_mesh = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _meshQuad = PoolManager.Singleton.RequestMeshQuad();
            _meshQuad.AssignToEntitySpriteGO(this);
        }

        if (_cubeCollider == null)
        {
            _cubeCollider = PoolManager.Singleton.RequestCubeCollider();
            _cubeCollider.Attach(this);
        }

        //GameManager.Singleton.Gamedata.GetSprite(_entityData.CurrentEntityState.StateTemplate.SpriteUID, out _sprite);
        UpdateSprite(_entityData.CurrentEntityState.StateTemplate.SpriteUID);
        UpdateMaterial();

        UpdateMesh();
        UpdateUVs();

        _transform.name = string.Format("{0}-{1}", entityData.EntityName, entityData.EntityUID);
    }

    public void UpdateSprite(uint spriteUID)
    {
        GameManager.Singleton.Gamedata.GetSprite(spriteUID, out _sprite);
    }

    public void UpdateMaterial()
    {
        _material = _meshQuad.Material;
    }

    public void UpdateComponents()
    {
        _lightComponent = this.gameObject.GetComponent<LightComponent>();
        _multiAngleComponent = this.gameObject.GetComponent<MultiAngleComponent>();
        _doorComponent = this.gameObject.GetComponent<DoorComponent>();
    }

    public void Reset()
    {
        _sprite = null;
        _transform = this.gameObject.transform;
        _transform.parent = PoolManager.Singleton._transform;
        _lightComponent = null;

        if (_meshQuad != null)
        {
            PoolManager.Singleton.ReturnMeshQuad(_meshQuad);
            _meshQuad = null;
        }

        this.gameObject.SetActive(false);
    }

    public void UpdateMesh()
    {
        _transform.localScale = new Vector3(_entityData.CurrentEntityState.StateTemplate.StateGraphicsSize.x, _entityData.CurrentEntityState.StateTemplate.StateGraphicsSize.y, _entityData.CurrentEntityState.StateTemplate.StateGraphicsSize.z);

        _meshQuad.UpdateMaterial(_sprite.SpriteSheet.Material, _sprite.SpriteSheet.MaterialUID);

        if (_sprite.SpriteSheet.Material.HasProperty("_Scale"))
        {
            Vector4 scale = _meshQuad.Material.GetVector("_Scale");
            scale.x = _entityData.CurrentEntityState.StateTemplate.StateGraphicsSize.x;
            scale.y = _entityData.CurrentEntityState.StateTemplate.StateGraphicsSize.y;

            //_meshQuad.renderer.sharedMaterial.SetVector("_Scale", scale);
            _meshQuad.Material.SetVector("_Scale", scale);
        }
    }

    public void UpdateUVs()
    {
        Vector4 uvs = _sprite.GetUVCoords();
        Vector2 offset =_sprite.uvOffset;
        _meshQuad.ModifyUV(0, uvs, offset);

        _meshQuad.UpdateUV();
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
        if (_material == null)
            return;

        Color32 newColor;
        if (_lightComponent != null)
        {
            //We've got a light component attached, and we don't want to wash it out, so we'll half the color
            newColor = _lightComponent.LightColor;//new Color32((byte)(_lightComponent.LightColor.r * 0.5f), (byte)(_lightComponent.LightColor.g * 0.5f), (byte)(_lightComponent.LightColor.b * 0.5f), _lightComponent.LightColor.a);
        }
        else
        {
            newColor = _entityData.MapTile.LightColor;
        }

        _material.SetColor("_Color", newColor);
    }

    public void Highlight()
    {
        _meshQuad.ModifyColor(0, GameManager.HighlightColor);
        _meshQuad.UpdateColor();
    }

    public void DeHighlight()
    {
        _meshQuad.ModifyColor(0, _meshQuad.LastColor);
        _meshQuad.UpdateColor();
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

    public void Activate()
    {
        if (_doorComponent != null)
        {
            _doorComponent.Activate();
        }
    }

    public void SetColliderState(bool colliderState)
    {
        _cubeCollider.SetActive(colliderState);
    }
}