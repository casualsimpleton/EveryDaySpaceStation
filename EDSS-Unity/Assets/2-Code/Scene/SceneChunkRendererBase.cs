//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// SceneChunkRendererBase - Base class for holding the meshes and dealing with rendering of the map world
// Is responsible for rendering blocks to allow batching and minimize meshes in an orderly, predictable fashion
// Created: December 10 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 10 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public abstract class SceneChunkRendererBase : MonoBehaviour
{
    #region Vars
#if UNITY_EDITOR
    public bool ShowVertGizmos = false;
#endif

    public Transform _transform { get; protected set; }

    public SceneChunk _parentChunk { get; protected set; }

    public uint _MaterialUID { get; set; }

    [SerializeField]
    protected Mesh _mesh;

    [SerializeField]
    protected MeshRenderer _meshRenderer;

    [SerializeField]
    protected MeshFilter _meshFilter;

    [SerializeField]
    protected Quaternion _rotation;

    [SerializeField]
    protected Material _material;

    [SerializeField]
    protected Vector3[] _verts;

    [SerializeField]
    protected Vector3[] _normals;

    [SerializeField]
    protected Vector2[] _uv;

    [SerializeField]
    protected Color32[] _colors;

    [SerializeField]
    protected int[] _tri;

    [SerializeField]
    protected MeshCollider _meshCollider;
    #endregion

    /// <summary>
    /// Resets stuff before being returned to pool
    /// </summary>
    public virtual void Reset()
    {
        _transform = this.gameObject.transform;
        _transform.parent = PoolManager.Singleton._transform;

        this.gameObject.SetActive(false);
    }

    public abstract void Create();
    protected abstract void PrepareBlockElements();
    
    public virtual void AssignToChunk(SceneChunk parentChunk)
    {
        _parentChunk = parentChunk;

        _transform.parent = parentChunk._transform;
        _transform.localPosition = Vector3.zero;
    }
    
    #region Triangle and Face Modification
    public virtual void ModifyTriangles(int triIndex, int vertOneIndex, int vertTwoIndex, int vertThreeIndex, int vertFourIndex)
    {
        ModifyTrianglesNoUpdate(triIndex, vertOneIndex, vertTwoIndex, vertThreeIndex, vertFourIndex);

        UpdateMesh(true);
    }

    /// <summary>
    /// Same functionality as ModifyTriangles but doesn't update the mesh. Useful if modifying large number of faces at once
    /// </summary>
    public virtual void ModifyTrianglesNoUpdate(int triIndex, int vertOneIndex, int vertTwoIndex, int vertThreeIndex, int vertFourIndex)
    {
        _tri[triIndex] = vertOneIndex;
        _tri[triIndex + 1] = vertTwoIndex;
        _tri[triIndex + 2] = vertThreeIndex;

        _tri[triIndex + 3] = vertThreeIndex;
        _tri[triIndex + 4] = vertFourIndex;
        _tri[triIndex + 5] = vertOneIndex;

#if DEBUGCLIENT
        if (_tri[triIndex] > _verts.Length ||
            _tri[triIndex + 1] > _verts.Length ||
            _tri[triIndex + 2] > _verts.Length ||
            _tri[triIndex + 3] > _verts.Length ||
            _tri[triIndex + 4] > _verts.Length ||
            _tri[triIndex + 5] > _verts.Length)
        {
            Debug.LogError("Triangle refers to out of bounds vert");
        }
#endif
    }

    /// <summary>
    /// Updates the mesh's assigned verts and triangles. This has some cost associated with it, so doing it too often can cause performance issues
    /// </summary>
    public virtual void UpdateMesh(bool isFirstTime)
    {
        _mesh.vertices = _verts;
        _mesh.triangles = _tri;

        if (!isFirstTime)
        {
            _meshCollider.enabled = false;
            _meshCollider.sharedMesh = _mesh;
            _meshCollider.enabled = true;
        }
    }
    #endregion

    #region Texturing and Materials
    public virtual void ModifyUV(int uvIndex, Vector4 uv)
    {
        ModifyUV(uvIndex, uv);

        UpdateUV();
    }

    public virtual void ModifyUVNoUpdate(int uvIndex, Vector4 uv, Vector2 uvOffset)
    {
        _uv[uvIndex] = new Vector2(uv.x + uv.z - uvOffset.x, uv.y - uvOffset.y);
        _uv[uvIndex + 1] = new Vector2(uv.x + uv.z - uvOffset.x, uv.y - uv.w + uvOffset.y);
        _uv[uvIndex + 2] = new Vector2(uv.x + uvOffset.x, uv.y - uv.w + uvOffset.y);
        _uv[uvIndex + 3] = new Vector2(uv.x + uvOffset.x, uv.y - uvOffset.y);

#if DEBUGCLIENT
        if (uvIndex < 0 || uvIndex > _uv.Length - 1)
        {
            Debug.LogError(string.Format("UV index {0} is out of bounds", uvIndex));
        }
#endif
    }

    /// <summary>
    /// Updates the mesh's assigned uvs. This has some cost associated with it, so doing it too often can cause performance issues
    /// </summary>
    public virtual void UpdateUV()
    {
        _mesh.uv = _uv;
    }

    public virtual void AssignMaterial(EDSSSprite sprite)
    {
        //TODO Currently can only do one material per chunk
        //if (_meshRenderer.material != null && _meshRenderer.material != sprite.SpriteSheet.Material)
        //{
        //    return;
        //}

        _meshRenderer.material = sprite.SpriteSheet.Material;
    }

    public virtual void UpdateMaterial(Material mat, uint MatUID)
    {
        _meshRenderer.material = _material = mat;
        _MaterialUID = MatUID;
    }
    #endregion

    #region Colors
    public virtual void ModifyColor(int colorIndex, Color32 newColor)
    {
        ModifyColorNoUpdate(colorIndex, newColor);

        UpdateColors();
    }

    public virtual void ModifyColorNoUpdate(int colorIndex, Color32 newColor)
    {
        _colors[colorIndex] = newColor;
        _colors[colorIndex + 1] = newColor;
        _colors[colorIndex + 2] = newColor;
        _colors[colorIndex + 3] = newColor;
    }

    public virtual void UpdateColors()
    {
        _mesh.colors32 = _colors;
    }
    #endregion

    #region Materials

    #endregion

    protected void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (ShowVertGizmos)
        {
            Gizmos.color = Color.red;
            int len = _verts.Length;
            for (int i = 0; i < len; i++)
            {
                Gizmos.DrawWireSphere(_verts[i], 0.15f);
            }
        }
#endif
    }
}