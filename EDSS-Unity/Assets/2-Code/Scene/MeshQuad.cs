//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// MeshQuad - Monobehaviour component used to represent a non map object (like like, or chair or player)
// Using this instead of Unity's Quad primitive as we have more control, including the meshes's verts and uvs
// NO COLLIDER ATTACHED HERE. COLLIDER WILL BE RESPONSIBILITY OF THE PARENT COMPONENT, LIKE ENTITYSPRITEGAMEOBJECT
// Created: December 14 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 14 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class MeshQuad : MonoBehaviour
{
    #region Vars
#if UNITY_EDITOR
    public bool ShowVertGizmos = false;
#endif

    public Transform _transform { get; protected set; }
    protected EntitySpriteGameObject _entitySpriteGO;
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
    #endregion

    #region Gets/Sets
    public EntitySpriteGameObject EntitySpriteGO { get { return _entitySpriteGO; } }
    public Material Material { get { return _material; } }
    public MeshRenderer Renderer { get { return _meshRenderer; } }
    #endregion

    public virtual void Reset()
    {
        _transform = this.gameObject.transform;
        _transform.parent = PoolManager.Singleton._transform;

        _entitySpriteGO = null;

        this.gameObject.SetActive(false);
    }

    public virtual void Create()
    {
        _verts = new Vector3[4];
        _normals = new Vector3[_verts.Length];
        _uv = new Vector2[_verts.Length];
        _tri = new int[6];
        _colors = new Color32[_verts.Length];

        _mesh = new Mesh();
        _mesh.name = "MeshQuad";

        #region Verts/Norms/Tris
        Color32 vC = new Color32(128, 128, 128, 255);
        int vertIndex = 0;
        int normIndex = 0;
        int uvIndex = 0;
        int colorIndex = 0;
        float blockSize = SceneChunk.blockSize;

        _verts[vertIndex++] = new Vector3(blockSize * 0.5f, blockSize, 0f);
        _verts[vertIndex++] = new Vector3(blockSize * 0.5f, 0f, 0f);
        _verts[vertIndex++] = new Vector3(-blockSize * 0.5f, 0f, 0f);
        _verts[vertIndex++] = new Vector3(-blockSize * 0.5f, blockSize, 0f);

        _normals[normIndex++] = new Vector3(0f, 0f, 1f);
        _normals[normIndex++] = new Vector3(0f, 0f, 1f);
        _normals[normIndex++] = new Vector3(0f, 0f, 1f);
        _normals[normIndex++] = new Vector3(0f, 0f, 1f);

        _uv[uvIndex++] = new Vector2(1f, 1f);
        _uv[uvIndex++] = new Vector2(1f, 0);
        _uv[uvIndex++] = new Vector2(0, 0);
        _uv[uvIndex++] = new Vector2(0, 1f);

        _colors[colorIndex++] = vC;
        _colors[colorIndex++] = vC;
        _colors[colorIndex++] = vC;
        _colors[colorIndex++] = vC;

        _tri[0] = 0;
        _tri[1] = 1;
        _tri[2] = 2;
        _tri[3] = 2;
        _tri[4] = 3;
        _tri[5] = 0;
        #endregion

        _meshFilter = this.gameObject.AddComponent<MeshFilter>();
        _meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

        _meshRenderer.material = _material;
        _meshFilter.mesh = _mesh;

        _mesh.vertices = _verts;
        _mesh.normals = _normals;
        _mesh.uv = _uv;
        _mesh.triangles = _tri;
        _mesh.colors32 = _colors;

        _mesh.RecalculateBounds();
    }

    public virtual void AssignToEntitySpriteGO(EntitySpriteGameObject esgo)
    {
        _transform.parent = esgo._transform;
        _transform.localPosition = Vector3.zero;
        _entitySpriteGO = esgo;

        AssignMaterial(_entitySpriteGO.Sprite);

        this.gameObject.SetActive(esgo.gameObject.activeSelf);
    }

    public virtual void ModifyUV(int uvIndex, Vector4 uv, Vector2 uvOffset)
    {
        _uv[uvIndex] = new Vector2(uv.x + uv.z - uvOffset.x, uv.y - uvOffset.y);
        _uv[uvIndex + 1] = new Vector2(uv.x + uv.z - uvOffset.x, uv.y - uv.w + uvOffset.y);
        _uv[uvIndex + 2] = new Vector2(uv.x + uvOffset.x, uv.y - uv.w + uvOffset.y);
        _uv[uvIndex + 3] = new Vector2(uv.x + uvOffset.x, uv.y - uvOffset.y);
    }

    public virtual void UpdateUV()
    {
        _mesh.uv = _uv;
    }

    public virtual void AssignMaterial(EDSSSprite sprite)
    {
        if (sprite == null)
        {
            return;
        }

        _meshRenderer.material = sprite.SpriteSheet.Material;
    }

    public virtual void UpdateMaterial(Material mat, uint MatUID)
    {
        _meshRenderer.material = _material = mat;
        _MaterialUID = MatUID;

        if (_entitySpriteGO != null)
        {
            _entitySpriteGO.UpdateMaterial();
        }
    }

    protected virtual void OnDrawGizmosSelected()
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