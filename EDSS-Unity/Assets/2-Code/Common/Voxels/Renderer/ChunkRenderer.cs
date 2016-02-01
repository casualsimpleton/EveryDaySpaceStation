//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// ChunkRenderer - Works with VoxelChunk to render
// Created: January 31 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: January 31 2016
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using EveryDaySpaceStation;
using EveryDaySpaceStation.Utils;
using EveryDaySpaceStation.Json;
using EveryDaySpaceStation.DataTypes;

namespace EveryDaySpaceStation
{
    public class ChunkRenderer : MonoBehaviour
    {
        public static ChunkRenderer NewChunkRenderer(Transform parent)
        {
            GameObject go = new GameObject("chunkrender");
            go.transform.parent = parent;
            go.transform.localPosition = Vector3.zero;

            ChunkRenderer cr = go.AddComponent<ChunkRenderer>();
            go.AddComponent<MeshCollider>();

            return cr;
        }

        public VoxelChunk ParentChunk { get; private set; }
        public Transform myTransform { get; private set; }

        public ushort ChunkMaterialUID { get; private set; }
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<int> Triangles = new List<int>();
        public List<Vector2> UVs = new List<Vector2>();
        public List<Color32> Colors = new List<Color32>();
        
        public List<Vector3> CollisionVertices = new List<Vector3>();
        public List<int> CollisionTriangles = new List<int>();

        public Mesh RenderMesh;
        public Mesh CollisionMesh;
        public MeshCollider MeshCollider;

        public Material ChunkRendererMaterial { get; private set; }

        public void Init(ushort chunkMaterialUID, Material material, VoxelChunk parent)
        {
            ChunkMaterialUID = chunkMaterialUID;
            ChunkRendererMaterial = material;
            ParentChunk = parent;
            myTransform = this.transform;

            MeshCollider = this.gameObject.GetComponent<MeshCollider>();
        }

        public void OurUpdate()
        {
        }

        public void OurLateUpdate()
        {
        }

        public void OurRender()
        {
            Graphics.DrawMesh(RenderMesh, myTransform.position, Quaternion.identity, ChunkRendererMaterial, LayerMask.NameToLayer("Default"));
        }

        /// <summary>
        /// Adds a vertex and corresponding uv coordiate. Returns vert index
        /// </summary>
        public int AddVertexAndUV(Vector3 vert, Vector3 norm, Vector2 uv)
        {
            Vertices.Add(vert);
            Normals.Add(norm);
            UVs.Add(uv);

            return Vertices.Count - 1;
        }

        public void AddQuadFace(int firstVertexIndex, int indexOne, int indexTwo, int indexThree, int indexFour, bool addToCollisionMesh = false)
        {
            //Triangle 1
            Triangles.Add(indexOne);
            Triangles.Add(indexTwo);
            Triangles.Add(indexThree);

            //Triangle 2
            Triangles.Add(indexThree);
            Triangles.Add(indexFour);
            Triangles.Add(indexOne);

            if (addToCollisionMesh)
            {
                int startColVertIndex = CollisionVertices.Count;
                AddCollisionVerts(firstVertexIndex, startColVertIndex, indexOne, indexTwo, indexThree, indexFour);
            }
        }

        public void AddCollisionVerts(int firstVertexIndex, int startColVertIndex, int indexOne, int indexTwo, int indexThree, int indexFour)
        {
            CollisionVertices.Add(Vertices[firstVertexIndex]);
            CollisionVertices.Add(Vertices[firstVertexIndex + 1]);
            CollisionVertices.Add(Vertices[firstVertexIndex + 2]);
            CollisionVertices.Add(Vertices[firstVertexIndex + 3]);

            //First triangle
            CollisionTriangles.Add(indexOne);
            CollisionTriangles.Add(indexTwo);
            CollisionTriangles.Add(indexThree);

            //Second triangle
            CollisionTriangles.Add(indexThree);
            CollisionTriangles.Add(indexFour);
            CollisionTriangles.Add(indexOne);
        }

        public void BuildMesh()
        {
            if (RenderMesh == null)
            {
                RenderMesh = new Mesh();
                RenderMesh.name = "RenderMesh";
            }

            RenderMesh.vertices = Vertices.ToArray();
            RenderMesh.triangles = Triangles.ToArray();
            RenderMesh.normals = Normals.ToArray();
            RenderMesh.uv = UVs.ToArray();


            if (CollisionMesh == null)
            {
                CollisionMesh = new Mesh();
                CollisionMesh.name = "CollisionMesh";
            }

            CollisionMesh.vertices = CollisionVertices.ToArray();
            CollisionMesh.triangles = CollisionTriangles.ToArray();

            MeshCollider.sharedMesh = CollisionMesh;
        }
    }
}