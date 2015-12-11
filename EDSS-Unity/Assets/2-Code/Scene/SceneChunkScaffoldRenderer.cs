//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// SceneChunkScaffoldRenderer - Used by SceneChunk to handle drawing specifically of scaffolding and pipes/wiring. Depending on the needs of the chunk, one or more of these may be used to graphically represent the level
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

public class SceneChunkScaffoldRenderer : SceneChunkRendererBase
{
    #region Vars

    #endregion

    public override void Create()
    {
        _verts = new Vector3[SceneLevelManager.Singleton.BlocksPerChuck * SceneLevelManager.Singleton.BlocksPerChuck * SceneBlock.UnderLayerFaces * 4];
        _normals = new Vector3[_verts.Length];
        _uv = new Vector2[_verts.Length];
        _tri = new int[SceneLevelManager.Singleton.BlocksPerChuck * SceneLevelManager.Singleton.BlocksPerChuck * SceneBlock.UnderLayerFaces * 6];
        _colors = new Color32[_verts.Length];

        _mesh = new Mesh();
        _mesh.name = "SCSR-Mesh";

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

        _mesh.RecalculateBounds();
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
            int[] vertFirstIndex = new int[SceneBlock.UnderLayerFaces];

            #region Verts, Norms, UVs and Triangles
            //Bottom most face, faces UP. Used for scaffold or plating
            vertFirstIndex[(int)GameData.GameBlockData.UnderFaces.BottomLayer] = vertIndex;
            _verts[vertIndex++] = new Vector3(xx * blockSize, -0.4f, zz * blockSize);
            _verts[vertIndex++] = new Vector3(xx * blockSize, -0.4f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, -0.4f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, -0.4f, zz * blockSize);

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

            //Large Pipe Layer, faces UP. Used for large pipes like waste or transit
            vertFirstIndex[(int)GameData.GameBlockData.UnderFaces.LargePipeLayer] = vertIndex;
            _verts[vertIndex++] = new Vector3(xx * blockSize, -0.3f, zz * blockSize);
            _verts[vertIndex++] = new Vector3(xx * blockSize, -0.3f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, -0.3f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, -0.3f, zz * blockSize);

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

            //Thin Pipe Layer, faces UP. Used for thin pipes like atmos
            vertFirstIndex[(int)GameData.GameBlockData.UnderFaces.ThinPipeLayer] = vertIndex;
            _verts[vertIndex++] = new Vector3(xx * blockSize, -0.2f, zz * blockSize);
            _verts[vertIndex++] = new Vector3(xx * blockSize, -0.2f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, -0.2f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, -0.2f, zz * blockSize);

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

            //Wire Layer, faces UP. Used for wiring
            vertFirstIndex[(int)GameData.GameBlockData.UnderFaces.ThinPipeLayer] = vertIndex;
            _verts[vertIndex++] = new Vector3(xx * blockSize, -0.1f, zz * blockSize);
            _verts[vertIndex++] = new Vector3(xx * blockSize, -0.1f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, -0.1f, (z * blockSize) + 0f);
            _verts[vertIndex++] = new Vector3((x * blockSize) + 0f, -0.1f, zz * blockSize);

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
            #endregion

            #region Triangle Order
            //Start point is determined since we're only going to be doing that one cube
            int startPoint = i * SceneBlock.UnderLayerFaces * 6;
            int triLen = startPoint + (SceneBlock.UnderLayerFaces * 6); //Only going to do 6 faces
            int vertCount = 0 + (i * SceneBlock.UnderLayerFaces * 4);
            int[] faceFirstTri = new int[SceneBlock.UnderLayerFaces];
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

    public override void AssignToChunk(SceneChunk parentChunk)
    {
        base.AssignToChunk(parentChunk);
    }

    #region Triangle and Face Modification
    public override void ModifyTriangles(int triIndex, int vertOneIndex, int vertTwoIndex, int vertThreeIndex, int vertFourIndex)
    {
        base.ModifyTriangles(triIndex, vertOneIndex, vertTwoIndex, vertThreeIndex, vertFourIndex);
    }

    /// <summary>
    /// Same functionality as ModifyTriangles but doesn't update the mesh. Useful if modifying large number of faces at once
    /// </summary>
    public override void ModifyTrianglesNoUpdate(int triIndex, int vertOneIndex, int vertTwoIndex, int vertThreeIndex, int vertFourIndex)
    {
        base.ModifyTrianglesNoUpdate(triIndex, vertOneIndex, vertTwoIndex, vertThreeIndex, vertFourIndex);
    }

    /// <summary>
    /// Updates the mesh's assigned verts and triangles. This has some cost associated with it, so doing it too often can cause performance issues
    /// </summary>
    public override void UpdateMesh(bool isFirstTime)
    {
        base.UpdateMesh(isFirstTime);
    }
    #endregion

    #region Texturing and Materials
    public override void ModifyUV(int uvIndex, Vector4 uv)
    {
        base.ModifyUV(uvIndex, uv);
    }

    public override void ModifyUVNoUpdate(int uvIndex, Vector4 uv, Vector2 uvOffset)
    {
        base.ModifyUVNoUpdate(uvIndex, uv, uvOffset);
    }

    /// <summary>
    /// Updates the mesh's assigned uvs. This has some cost associated with it, so doing it too often can cause performance issues
    /// </summary>
    public override void UpdateUV()
    {
        base.UpdateUV();
    }

    public override void AssignMaterial(EDSSSprite sprite)
    {
        //TODO Currently can only do one material per chunk
        base.AssignMaterial(sprite);
    }

    public override void UpdateMaterial(Material mat, uint MatUID)
    {
        base.UpdateMaterial(mat, MatUID);
    }
    #endregion

    #region Colors
    public override void ModifyColor(int colorIndex, Color32 newColor)
    {
        base.ModifyColor(colorIndex, newColor);
    }

    public override void ModifyColorNoUpdate(int colorIndex, Color32 newColor)
    {
        base.ModifyColorNoUpdate(colorIndex, newColor);
    }

    public override void UpdateColors()
    {
        base.UpdateColors();
    }
    #endregion

    #region Materials

    #endregion

    void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
    }
}