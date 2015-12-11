//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// SceneChunkRenderer - Used by SceneChunk to handle drawing. Depending on the needs of the chunk, one or more of these may be used to graphically represent the level
// Is responsible for rendering blocks to allow batching and minimize meshes in an orderly, predictable fashion
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

public class SceneChunkRenderer : SceneChunkRendererBase
{
    #region Vars

    #endregion

    /// <summary>
    /// Resets stuff before being returned to pool
    /// </summary>
    public override void Reset()
    {
        base.Reset();
    }

    public override void Create()
    {
        _verts = new Vector3[SceneLevelManager.Singleton.BlocksPerChuck * SceneLevelManager.Singleton.BlocksPerChuck * SceneBlock.FacesPerBlock * 4]; //Probably 16x16 or 8x8 blocks, with 6 faces, and 4 verts per face
        _normals = new Vector3[_verts.Length];
        _uv = new Vector2[_verts.Length];
        _tri = new int[SceneLevelManager.Singleton.BlocksPerChuck * SceneLevelManager.Singleton.BlocksPerChuck * SceneBlock.FacesPerBlock * 6];
        _colors = new Color32[_verts.Length];

        _mesh = new Mesh();
        _mesh.name = "SCR-Mesh";

        PrepareBlockElements();

        _meshFilter = this.gameObject.AddComponent<MeshFilter>();
        _meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

        _meshRenderer.material = _material;

        _meshFilter.mesh = _mesh;

        _mesh.vertices = _verts;
        _mesh.normals = _normals;
        _mesh.uv = _uv;
        _mesh.triangles = _tri;
        _mesh.colors32 = _colors;

        _meshCollider = this.gameObject.AddComponent<MeshCollider>();
        _meshCollider.sharedMesh = _mesh;

        _mesh.RecalculateBounds();
    }

    public override void AssignToChunk(SceneChunk parentChunk)
    {
        //_parentChunk = parentChunk;

        //_transform.parent = parentChunk._transform;
        //_transform.localPosition = Vector3.zero;
        base.AssignToChunk(parentChunk);
    }

    /// <summary>
    /// Creates each block and sets up all the verts, normals and triangles for that block
    /// </summary>
    protected override void PrepareBlockElements()
    {
        int x, z;
        int xx, zz;

        Color32 initColor = new Color32(1, 1, 1, 255);
        int len = SceneLevelManager.Singleton.BlocksPerChuck * SceneLevelManager.Singleton.BlocksPerChuck;
        int vertIndex = 0;
        int normIndex = 0;
        int uvIndex = 0;
        int colorIndex = 0;
        float blockSize = SceneChunk.blockSize;
        for (int i = 0; i < len; i++)
        {
            z = i / SceneLevelManager.Singleton.BlocksPerChuck;
            x = i - (z * SceneLevelManager.Singleton.BlocksPerChuck);
            xx = x + 1;
            zz = z + 1;

            //TODO this currently assumes only one total chunk
            int[] vertFirstIndex = new int[SceneBlock.FacesPerBlock];

            #region Verts, Norms, UVs and Triangles
            //Forward Face - Z+ - Clockwise LOOKING AT face - normal point Z+
            //Vert 0
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceZForward] = vertIndex;
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, blockSize, zz * blockSize);
            //Vert 1
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, 0f, zz * blockSize);
            //Vert 2
            _verts[vertIndex++] = new Vector3(xx * blockSize, 0f, zz * blockSize);
            //Vert 3
            _verts[vertIndex++] = new Vector3(xx * blockSize, blockSize, zz * blockSize);

            _normals[normIndex++] = new Vector3(0f, 0f, 1f);
            _normals[normIndex++] = new Vector3(0f, 0f, 1f);
            _normals[normIndex++] = new Vector3(0f, 0f, 1f);
            _normals[normIndex++] = new Vector3(0f, 0f, 1f);

            _uv[uvIndex++] = new Vector2(1f, 1f);
            _uv[uvIndex++] = new Vector2(1f, 0);
            _uv[uvIndex++] = new Vector2(0, 0);
            _uv[uvIndex++] = new Vector2(0, 1f);

            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;

            //Right Face - X+ - Clockwise LOOKING AT face - normal point X+
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceXForward] = vertIndex;
            _verts[vertIndex++] = new Vector3(xx * blockSize, blockSize, zz * blockSize);
            _verts[vertIndex++] = new Vector3(xx * blockSize, 0f, zz * blockSize);
            _verts[vertIndex++] = new Vector3(xx * blockSize, 0f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3(xx * blockSize, blockSize, (z * blockSize) + 0f);

            _normals[normIndex++] = new Vector3(1f, 0f, 0f);
            _normals[normIndex++] = new Vector3(1f, 0f, 0f);
            _normals[normIndex++] = new Vector3(1f, 0f, 0f);
            _normals[normIndex++] = new Vector3(1f, 0f, 0f);

            _uv[uvIndex++] = new Vector2(1f, 1f);
            _uv[uvIndex++] = new Vector2(1f, 0);
            _uv[uvIndex++] = new Vector2(0, 0);
            _uv[uvIndex++] = new Vector2(0, 1f);

            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;

            //Back Face - Z- - Clockwise LOOKING at face - normal pointing Z-
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceZBack] = vertIndex;
            _verts[vertIndex++] = new Vector3(xx * blockSize, blockSize, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3(xx * blockSize, 0f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, 0f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, blockSize, (z * blockSize) + 0f);

            _normals[normIndex++] = new Vector3(0f, 0f, -1f);
            _normals[normIndex++] = new Vector3(0f, 0f, -1f);
            _normals[normIndex++] = new Vector3(0f, 0f, -1f);
            _normals[normIndex++] = new Vector3(0f, 0f, -1f);

            _uv[uvIndex++] = new Vector2(1f, 1f);
            _uv[uvIndex++] = new Vector2(1f, 0);
            _uv[uvIndex++] = new Vector2(0, 0);
            _uv[uvIndex++] = new Vector2(0, 1f);

            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;

            //Left Face - X- - Clockwise LOOKING at face - normal pointing X-
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceXBack] = vertIndex;
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, blockSize, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, 0f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, 0f, zz * blockSize);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0, blockSize, zz * blockSize);

            _normals[normIndex++] = new Vector3(1f, 0f, 0f);
            _normals[normIndex++] = new Vector3(1f, 0f, 0f);
            _normals[normIndex++] = new Vector3(1f, 0f, 0f);
            _normals[normIndex++] = new Vector3(1f, 0f, 0f);

            _uv[uvIndex++] = new Vector2(1f, 1f);
            _uv[uvIndex++] = new Vector2(1f, 0);
            _uv[uvIndex++] = new Vector2(0, 0);
            _uv[uvIndex++] = new Vector2(0, 1f);

            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;

            //Top Face - Y+ - since this will be seen from a top down view (for the time being)
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceTop] = vertIndex;
            _verts[vertIndex++] = new Vector3(xx * blockSize, blockSize, zz * blockSize);
            _verts[vertIndex++] = new Vector3(xx * blockSize, blockSize, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, blockSize, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, blockSize, zz * blockSize);

            _normals[normIndex++] = new Vector3(0f, 1f, 0f);
            _normals[normIndex++] = new Vector3(0f, 1f, 0f);
            _normals[normIndex++] = new Vector3(0f, 1f, 0f);
            _normals[normIndex++] = new Vector3(0f, 1f, 0f);

            _uv[uvIndex++] = new Vector2(1f, 1f);
            _uv[uvIndex++] = new Vector2(1f, 0);
            _uv[uvIndex++] = new Vector2(0, 0);
            _uv[uvIndex++] = new Vector2(0, 1f);

            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;

            //Bottom Face - Y+ - Since this is the floor, it needs to point up as well
            vertFirstIndex[(int)GameData.GameBlockData.BlockFaces.FaceBottom] = vertIndex;
            _verts[vertIndex++] = new Vector3(xx * blockSize, 0f, zz * blockSize);
            _verts[vertIndex++] = new Vector3(xx * blockSize, 0f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, 0f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, 0f, zz * blockSize);

            _normals[normIndex++] = new Vector3(0f, -1f, 0f);
            _normals[normIndex++] = new Vector3(0f, -1f, 0f);
            _normals[normIndex++] = new Vector3(0f, -1f, 0f);
            _normals[normIndex++] = new Vector3(0f, -1f, 0f);

            _uv[uvIndex++] = new Vector2(1f, 1f);
            _uv[uvIndex++] = new Vector2(1f, 0);
            _uv[uvIndex++] = new Vector2(0, 0);
            _uv[uvIndex++] = new Vector2(0, 1f);

            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            _colors[colorIndex++] = initColor;
            #endregion

            #region Triangle Order
            //Start point is determined since we're only going to be doing that one cube
            int startPoint = i * SceneBlock.FacesPerBlock * 6;
            int triLen = startPoint + (SceneBlock.FacesPerBlock * 6); //Only going to do 6 faces
            int vertCount = 0 + (i * SceneBlock.FacesPerBlock * 4);
            int[] faceFirstTri = new int[SceneBlock.FacesPerBlock];
            int faceIndex = 0;
            for (int j = startPoint; j < triLen; j += 6)
            {
                faceFirstTri[faceIndex] = j;
                _tri[j] = vertCount;
                _tri[j + 1] = vertCount;// +1;
                _tri[j + 2] = vertCount;// + 2;

                _tri[j + 3] = vertCount;// + 2;
                _tri[j + 4] = vertCount;// + 3;
                _tri[j + 5] = vertCount;

                vertCount += 4;
                faceIndex++;
            }
            #endregion

            //_chunkBlocks[i] = new SceneBlock();
            //_chunkBlocks[i].Create(pos, pos, index, index, faceFirstTri, vertFirstIndex, this);
        }
    }    

    #region Triangle and Face Modification
    public override void ModifyTriangles(int triIndex, int vertOneIndex, int vertTwoIndex, int vertThreeIndex, int vertFourIndex)
    {
        //ModifyTrianglesNoUpdate(triIndex, vertOneIndex, vertTwoIndex, vertThreeIndex, vertFourIndex);

        //UpdateMesh(true);
        base.ModifyTriangles(triIndex, vertOneIndex, vertTwoIndex, vertThreeIndex, vertFourIndex);
    }

    /// <summary>
    /// Same functionality as ModifyTriangles but doesn't update the mesh. Useful if modifying large number of faces at once
    /// </summary>
    public override void ModifyTrianglesNoUpdate(int triIndex, int vertOneIndex, int vertTwoIndex, int vertThreeIndex, int vertFourIndex)
    {
//        _tri[triIndex] = vertOneIndex;
//        _tri[triIndex + 1] = vertTwoIndex;
//        _tri[triIndex + 2] = vertThreeIndex;

//        _tri[triIndex + 3] = vertThreeIndex;
//        _tri[triIndex + 4] = vertFourIndex;
//        _tri[triIndex + 5] = vertOneIndex;

//#if DEBUGCLIENT
//        if (_tri[triIndex] > _verts.Length ||
//            _tri[triIndex + 1] > _verts.Length ||
//            _tri[triIndex + 2] > _verts.Length ||
//            _tri[triIndex + 3] > _verts.Length ||
//            _tri[triIndex + 4] > _verts.Length ||
//            _tri[triIndex + 5] > _verts.Length)
//        {
//            Debug.LogError("Triangle refers to out of bounds vert");
//        }
//#endif
        base.ModifyTrianglesNoUpdate(triIndex, vertOneIndex, vertTwoIndex, vertThreeIndex, vertFourIndex);
    }

    /// <summary>
    /// Updates the mesh's assigned verts and triangles. This has some cost associated with it, so doing it too often can cause performance issues
    /// </summary>
    public override void UpdateMesh(bool isFirstTime)
    {
        //_mesh.vertices = _verts;
        //_mesh.triangles = _tri;

        //if (!isFirstTime)
        //{
        //    _meshCollider.enabled = false;
        //    _meshCollider.sharedMesh = _mesh;
        //    _meshCollider.enabled = true;
        //}        
        base.UpdateMesh(isFirstTime);
    }
    #endregion

    #region Texturing and Materials
    public override void ModifyUV(int uvIndex, Vector4 uv)
    {
        //ModifyUV(uvIndex, uv);

        //UpdateUV();
        base.ModifyUV(uvIndex, uv);
    }

    public override void ModifyUVNoUpdate(int uvIndex, Vector4 uv, Vector2 uvOffset)
    {
//        _uv[uvIndex] = new Vector2(uv.x + uv.z - uvOffset.x, uv.y - uvOffset.y);
//        _uv[uvIndex + 1] = new Vector2(uv.x + uv.z - uvOffset.x, uv.y - uv.w + uvOffset.y);
//        _uv[uvIndex + 2] = new Vector2(uv.x + uvOffset.x, uv.y - uv.w + uvOffset.y);
//        _uv[uvIndex + 3] = new Vector2(uv.x + uvOffset.x, uv.y - uvOffset.y);

//#if DEBUGCLIENT
//        if (uvIndex < 0 || uvIndex > _uv.Length - 1)
//        {
//            Debug.LogError(string.Format("UV index {0} is out of bounds", uvIndex));
//        }
//#endif
        base.ModifyUVNoUpdate(uvIndex, uv, uvOffset);
    }

    /// <summary>
    /// Updates the mesh's assigned uvs. This has some cost associated with it, so doing it too often can cause performance issues
    /// </summary>
    public override void UpdateUV()
    {
        //_mesh.uv = _uv;
        base.UpdateUV();
    }

    public override void AssignMaterial(EDSSSprite sprite)
    {
        //TODO Currently can only do one material per chunk
        //if (_meshRenderer.material != null && _meshRenderer.material != sprite.SpriteSheet.Material)
        //{
        //    return;
        //}

        //_meshRenderer.material = sprite.SpriteSheet.Material;
        base.AssignMaterial(sprite);
    }

    public override void UpdateMaterial(Material mat, uint MatUID)
    {
        //_meshRenderer.material = _material = mat;
        //_MaterialUID = MatUID;
        base.UpdateMaterial(mat, MatUID);
    }
    #endregion

    #region Colors
    public override void ModifyColor(int colorIndex, Color32 newColor)
    {
        //ModifyColorNoUpdate(colorIndex, newColor);

        //UpdateColors();
        base.ModifyColor(colorIndex, newColor);
    }

    public override void ModifyColorNoUpdate(int colorIndex, Color32 newColor)
    {
        //_colors[colorIndex] = newColor;
        //_colors[colorIndex + 1] = newColor;
        //_colors[colorIndex + 2] = newColor;
        //_colors[colorIndex + 3] = newColor;
        base.ModifyColorNoUpdate(colorIndex, newColor);
    }

    public override void UpdateColors()
    {
        //_mesh.colors32 = _colors;
        base.UpdateColors();
    }
    #endregion

    #region Materials

    #endregion

    void OnDrawGizmosSelected()
    {
//#if UNITY_EDITOR
//        if (ShowVertGizmos)
//        {
//            Gizmos.color = Color.red;
//            int len = _verts.Length;
//            for (int i = 0; i < len; i++)
//            {
//                Gizmos.DrawWireSphere(_verts[i], 0.15f);
//            }
//        }
//#endif
        base.OnDrawGizmosSelected();
    }
}