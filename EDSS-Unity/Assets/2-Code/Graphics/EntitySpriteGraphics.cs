//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// EntitySpriteGraphics - Class containing visual elements for EntitySpriteGameObject
// Created: January 17 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 17 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class EntitySpriteGraphics : MonoBehaviour
{
    #region Vars
    public Transform _transform { get; private set; }
    protected EntitySpriteGameObject _entitySpriteGO;
    protected MeshQuad _meshQuad;
    protected EDSSSprite _sprite;
    #endregion

    #region Gets/Sets
    public MeshQuad Meshquad { get { return _meshQuad; } }
    public EDSSSprite Sprite { get { return _sprite; } }
    public Material _material { get; private set; }
    #endregion

    public void OnEnable()
    {
        if (_meshQuad != null)
        {
            _meshQuad.gameObject.SetActive(true);
        }
    }

    public void OnDisable()
    {
        if (_meshQuad != null)
        {
            _meshQuad.gameObject.SetActive(false);
        }
    }

    public void Create(EntitySpriteGameObject parentESGO)
    {
        if (_transform == null)
        {
            _transform = this.transform;
        }

        _entitySpriteGO = parentESGO;

        if (parentESGO == null)
        {
            Reset();
            return;
        }
    
        if (_meshQuad == null)
        {
            //_mesh = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _meshQuad = PoolManager.Singleton.RequestMeshQuad();
            _meshQuad.AssignToEntitySpriteGraphics(this);
        }

        UpdateSprite(parentESGO.EntityData.CurrentEntityState.StateTemplate.SpriteUID);
        UpdateMaterial();

        UpdateMesh(parentESGO.EntityData.CurrentEntityState.StateTemplate);
        UpdateUVs();

        AttachToEntitySpriteGO(parentESGO);
    }

    public void UpdateSprite(uint spriteUID)
    {
        GameManager.Singleton.Gamedata.GetSprite(spriteUID, out _sprite);
    }

    public void UpdateMaterial()
    {
        _material = _meshQuad.Material;
    }

    public void Reset()
    {
        _sprite = null;
        _transform = this.gameObject.transform;
        _transform.parent = PoolManager.Singleton._transform;

        if (_meshQuad != null)
        {
            PoolManager.Singleton.ReturnMeshQuad(_meshQuad);
            _meshQuad = null;
        }

        this.gameObject.SetActive(false);
    }

    public void UpdateMesh(GameData.EntityDataTemplate.StateTemplate stateTemplate, bool updateMaterial = true, bool updateScale = true)
    {
        if (updateMaterial)
        {
            _meshQuad.UpdateMaterial(_sprite.SpriteSheet.Material, _sprite.SpriteSheet.MaterialUID);
        }

        if (updateScale)
        {
            UpdateScale(stateTemplate.StateGraphicsSize);
        }
    }

    public void UpdateScale(Vector3 newScale)
    {
        _transform.localScale = new Vector3(newScale.x, newScale.y, newScale.z);
        
        if (_sprite.SpriteSheet.Material.HasProperty("_Scale"))
        {
            Vector4 scale = _meshQuad.Material.GetVector("_Scale");
            scale.x = newScale.x;
            scale.y = newScale.y;

            //_meshQuad.Material.SetVector("_Scale", scale);
        }
    }

    public void UpdateOffset(Vector3 offset)
    {
        _transform.localPosition = offset;
    }

    public void UpdateUVs()
    {
        Vector4 uvs = _sprite.GetUVCoords();
        Vector2 offset = _sprite.uvOffset;
        _meshQuad.ModifyUV(0, uvs, offset);

        _meshQuad.UpdateUV();
    }

    public void UpdateShaderColors(Color32 newColor)
    {
        if (_material == null)
            return;
        
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

    public virtual void AttachToEntitySpriteGO(EntitySpriteGameObject esgo)
    {
        _transform.parent = esgo._transform;
        _transform.localPosition = Vector3.zero;
        _transform.localRotation = Quaternion.identity;

        this.gameObject.SetActive(true);
    }

    public void Detach()
    {
        _transform.parent = null;
        this.gameObject.SetActive(true);
    }

    public void Disable()
    {
        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Enable()
    {
        if (!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }
    }
}