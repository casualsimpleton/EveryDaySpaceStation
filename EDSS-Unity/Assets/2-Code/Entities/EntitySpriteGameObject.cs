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
    public Transform _transform { get; private set; }
    protected EntitySpriteGraphics _spriteGraphics;
    protected float _updateTimer;
    protected float _updateTimerDelta;
    protected MapData.EntityData _entityData { get; private set; }
    protected LightComponent _lightComponent;
    protected DoorComponent _doorComponent;
    protected MultiAngleComponent _multiAngleComponent;
    protected ContainerComponent _containerComponent;

    protected CubeCollider _cubeCollider;
    protected CubeCollider _triggerCollider;

    public bool IsClown = false;
    #endregion

    #region Gets/Sets
    public EntitySpriteGraphics SpriteGraphics { get { return _spriteGraphics; } }
    public MapData.EntityData EntityData { get { return _entityData; } }
    public CubeCollider Cubecollider { get { return _cubeCollider; } }
    public CubeCollider Triggercollider { get { return _triggerCollider; } }
    #endregion

    void Start()
    {
        _transform = this.transform;
    }

    void OnEnable()
    {
        if (_cubeCollider != null)
        {
            _cubeCollider.gameObject.SetActive(true);
        }

        if (_triggerCollider != null)
        {
            _triggerCollider.gameObject.SetActive(true);
        }
    }

    void OnDisable()
    {
        if (_cubeCollider != null)
        {
            _cubeCollider.gameObject.SetActive(false);
        }

        if (_triggerCollider != null)
        {
            _triggerCollider.gameObject.SetActive(false);
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

        if (_spriteGraphics == null)
        {
            _spriteGraphics = PoolManager.Singleton.RequestEntitySpriteGraphics();
            _spriteGraphics.Detach();
        }

        _spriteGraphics.Create(this);

        if (_cubeCollider == null)
        {
            _cubeCollider = PoolManager.Singleton.RequestCubeCollider();
            _cubeCollider.Attach(this);
        }

        if (_triggerCollider == null)
        {
            _triggerCollider = PoolManager.Singleton.RequestCubeCollider();
            _triggerCollider.Attach(this, true);
        }

        _transform.name = string.Format("{0}-{1}", entityData.EntityName, entityData.EntityUID);
    }

    public void UpdateComponents()
    {
        _lightComponent = this.gameObject.GetComponent<LightComponent>();
        _multiAngleComponent = this.gameObject.GetComponent<MultiAngleComponent>();
        _doorComponent = this.gameObject.GetComponent<DoorComponent>();
        _containerComponent = this.gameObject.GetComponent<ContainerComponent>();
    }

    public void Reset()
    {
        if (_spriteGraphics != null)
        {
            _spriteGraphics.Reset();
        }

        _transform = this.gameObject.transform;
        _transform.parent = PoolManager.Singleton._transform;
        _lightComponent = null;

        this.gameObject.SetActive(false);
    }

    public void UpdateMesh()
    {
        if (_spriteGraphics == null)
            return;

        _spriteGraphics.UpdateMesh(_entityData.CurrentEntityState.StateTemplate);
    }

    public void UpdateSprite(uint spriteUID)
    {
        if (_spriteGraphics == null)
            return;

        _spriteGraphics.UpdateSprite(spriteUID);
    }

    public void UpdateMaterial()
    {
        if (_spriteGraphics == null)
            return;

        _spriteGraphics.UpdateMaterial();
    }

    public void UpdateUVs()
    {
        if (_spriteGraphics == null)
            return;

        _spriteGraphics.UpdateUVs();
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
        if (_spriteGraphics == null)
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

        _spriteGraphics.UpdateShaderColors(newColor);
    }

    public void Highlight()
    {
        _spriteGraphics.Highlight();
    }

    public void DeHighlight()
    {
        _spriteGraphics.DeHighlight();
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

        if (_containerComponent != null)
        {
            _containerComponent.Activate();
        }
    }

    public void TEMP_AltActivate()
    {
        if (_doorComponent != null)
        {
            _doorComponent.HackActivate();
        }
    }

    public void TEMP_TertiaryActivate()
    {
        if (_doorComponent != null)
        {
            _doorComponent.WeldActivate();
        }
    }

    public void TEMP_ToggleLockState()
    {
        if (_doorComponent != null)
        {
            _doorComponent.LockActivate();
        }
    }

    public void TEMP_PowerActivate()
    {
        if (_doorComponent != null)
        {
            _doorComponent.PowerActivate();
        }
    }

    public void SetColliderState(bool colliderState)
    {
        _cubeCollider.SetActive(colliderState);
    }

    public void SetTriggerState(bool triggerState)
    {
        _triggerCollider.SetActive(triggerState);
    }
}