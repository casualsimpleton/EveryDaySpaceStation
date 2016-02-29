//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// VoxelChunk - Voxel Chunk, contains voxel blocks
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
    [System.Serializable]
    public class VoxelChunk
    {
        protected VoxelBlock[, ,] _blocks;
        protected UniqueList<ChunkRenderer> _chunkRenderers;
        public VoxelChunkOrganizer ChunkGameObject { get; private set; }
        public Vec3Int ChunkSize { get; private set; }
        protected List<Vec3Int> _removedBlocks = new List<Vec3Int>();

        //System.Diagnostics.Stopwatch _timer;

        public bool IsDirty { get; set; }
        public bool NeedsRebuilt { get; set; }

        public VoxelChunk(int xWidth, int yHeight, int zLength)
        {
            _blocks = new VoxelBlock[xWidth, yHeight, zLength];
            IsDirty = false;
            NeedsRebuilt = false;
            _chunkRenderers = new UniqueList<ChunkRenderer>();
        }

        public void SetChunkGameObject(VoxelChunkOrganizer chunkOrg)
        {
            ChunkGameObject = chunkOrg;
        }

        public void TestRandomChunkData()
        {
            //int xw = _blocks.GetLength(0);
            //int yh = _blocks.GetLength(1);
            //int zl = _blocks.GetLength(2);

            //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            //timer.Start();
            //for (int x = 0; x < xw; x++)
            //{
            //    for (int y = 0; y < yh; y++)
            //    {
            //        for (int z = 0; z < zl; z++)
            //        {
            //            Random.seed = 10 + x + y + z;
            //            int random = Random.Range(0, 3);

            //            _blocks[x, y, z] = new VoxelBlock((ushort)random, this);
            //            //Debug.Log("X " + x + " Y " + y + " Z " + z + " Random " + random);
            //        }
            //    }
            //}
            //timer.Stop();
            //Debug.Log("Created " + (xw * yh * zl) + " blocks in " + timer.ElapsedMilliseconds);

            //NeedsRebuilt = true;
            //IsDirty = true;
        }

        public void LoadChunkData(ushort[, ,] data)
        {
            //int xw = _blocks.GetLength(0);
            //int yh = _blocks.GetLength(1);
            //int zl = _blocks.GetLength(2);

            //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            //timer.Start();
            //for (int x = 0; x < xw; x++)
            //{
            //    for (int y = 0; y < yh; y++)
            //    {
            //        for (int z = 0; z < zl; z++)
            //        {
            //            _blocks[x, y, z] = new VoxelBlock(data[x, y, z], this);
            //            //Debug.Log("X " + x + " Y " + y + " Z " + z + " Random " + random);
            //        }
            //    }
            //}
            //timer.Stop();
            //Debug.Log("Created " + (xw * yh * zl) + " blocks in " + timer.ElapsedMilliseconds);

            //IsDirty = true;
            //NeedsRebuilt = true;
        }

        public void LoadChunkDataPreSet(Vec3Int chunkSize)
        {
            //_timer = new System.Diagnostics.Stopwatch();
            //_timer.Start();

            _blocks = new VoxelBlock[chunkSize.x, chunkSize.y, chunkSize.z];
            ChunkSize = chunkSize;
        }

        public void LoadChunkDataPiecemeal(MapDataV2.MapBlock block, Vec3Int xyz, bool useBlockDefaults = true)
        {
            if (xyz.x < 0 || xyz.x > ChunkSize.x - 1)
            {
                Debug.Log(string.Format("Block {0} is out of bounds {1}", block.BlockType, xyz));
                return;
            }

            if (xyz.y < 0 || xyz.y > ChunkSize.y - 1)
            {
                Debug.Log(string.Format("Block {0} is out of bounds {1}", block.BlockType, xyz));
                return;
            }

            if (xyz.z < 0 || xyz.z > ChunkSize.z - 1)
            {
                Debug.Log(string.Format("Block {0} is out of bounds {1}", block.BlockType, xyz));
                return;
            }

            _blocks[xyz.x, xyz.y, xyz.z] = new VoxelBlock(block, this, useBlockDefaults);
        }

        /// <summary>
        /// Change a single block and mark this chunk dirt. Don't use when making big changes
        /// </summary>
        /// <param name="block"></param>
        /// <param name="xyz"></param>
        public void ChangeDataBlock(MapDataV2.MapBlock block, ushort prevBlockType, Vec3Int xyz, bool useBlockDefaults = true)
        {
            if (xyz.x < 0 || xyz.x > ChunkSize.x - 1)
            {
                Debug.Log(string.Format("Block {0} is out of bounds {1}", block.BlockType, xyz));
                return;
            }

            if (xyz.y < 0 || xyz.y > ChunkSize.y - 1)
            {
                Debug.Log(string.Format("Block {0} is out of bounds {1}", block.BlockType, xyz));
                return;
            }

            if (xyz.z < 0 || xyz.z > ChunkSize.z - 1)
            {
                Debug.Log(string.Format("Block {0} is out of bounds {1}", block.BlockType, xyz));
                return;
            }

            _blocks[xyz.x, xyz.y, xyz.z] = new VoxelBlock(block, this, useBlockDefaults);

            //If the new block type is 0 (empty) we need to handle it separately
            if (block.BlockType == 0)
            {
                _removedBlocks.Add(xyz);
                NeedsRebuilt = true;
            }
            else if (block.BlockType != prevBlockType)
            {
                NeedsRebuilt = true;
            }

            IsDirty = true;
        }

        /// <summary>
        /// Change a single face of a block
        /// </summary>
        /// <param name="faceSide"></param>
        /// <param name="newBlockFaceUID"></param>
        public void ChangeDataFace(Vec3Int xyz, GameManifestV2.BlockDataTemplate.ShowFaceDirection faceSide, ushort newBlockFaceUID)
        {
            if (xyz.x < 0 || xyz.x > ChunkSize.x - 1)
            {
                Debug.Log(string.Format("Block is out of bounds {0}", xyz));
                return;
            }

            if (xyz.y < 0 || xyz.y > ChunkSize.y - 1)
            {
                Debug.Log(string.Format("Block is out of bounds {0}", xyz));
                return;
            }

            if (xyz.z < 0 || xyz.z > ChunkSize.z - 1)
            {
                Debug.Log(string.Format("Block is out of bounds {0}", xyz));
                return;
            }

            VoxelBlock block = _blocks[xyz.x, xyz.y, xyz.z];

            if (block.MapBlock.BlockFacesSpriteUIDs[(int)faceSide] == newBlockFaceUID)
            {
                Debug.Log(string.Format("Block face is the same: {0}", newBlockFaceUID));
                return;
            }

            block.MapBlock.BlockFacesSpriteUIDs[(int)faceSide] = newBlockFaceUID;
            NeedsRebuilt = true;
            IsDirty = true;
        }

        public void LoadChunkDataPostSet()
        {
            //_timer.Stop();
            //Debug.Log("Created " + (_blocks.GetLength(0) * _blocks.GetLength(1) * _blocks.GetLength(2)) + " blocks in " + _timer.ElapsedMilliseconds);

            NeedsRebuilt = true;
            IsDirty = true;
        }

        public ChunkRenderer GetChunkRenderer(ushort uid)
        {
            //Special case
            if(uid == 0)
            {
                return null;
            }

            List<ChunkRenderer> cr = _chunkRenderers.List;
            for (int i = 0; i < cr.Count; i++)
            {
                if (cr[i].ChunkMaterialUID == uid)
                {
                    return cr[i];
                }
            }

            //ChunkRenderer newCR = new ChunkRenderer();
            ChunkRenderer newCR = ChunkRenderer.NewChunkRenderer(ChunkGameObject.transform);
            //newCR.Init(uid, DefaultFiles.testMaterials[uid - 1], this);
            Material mat = null;
            GameManifestV2.Singleton.GetMaterial(uid, out mat);
            newCR.Init(uid, mat, this);

            _chunkRenderers.AddUnique(newCR);

            return newCR;
        }

        public void ChunkLateUpdate()
        {
            List<ChunkRenderer> crs = _chunkRenderers.List;

            for (int i = 0; i < crs.Count; i++)
            {
                crs[i].OurRender();
            }
        }

        protected void ResetChunkRenderers()
        {
            List<ChunkRenderer> crs = _chunkRenderers.List;
            for (int i = 0; i < crs.Count; i++)
            {
                GameObject.Destroy(crs[i].gameObject);
            }

            _chunkRenderers.Clear();
        }

        public void ChunkUpdate()
        {
            if (!IsDirty && !NeedsRebuilt)
            {
                return;
            }

            if (NeedsRebuilt)
            {
                ResetChunkRenderers();
            }

            int xw = _blocks.GetLength(0);
            int yh = _blocks.GetLength(1);
            int zl = _blocks.GetLength(2);

            ChunkRenderer lastCR = null;
            GameManifestV2.SpriteDataTemplate lastSprite = null;

            for (int x = 0; x < xw; x++)
            {
                for (int y = 0; y < yh; y++)
                {
                    for (int z = 0; z < zl; z++)
                    {
                        VoxelBlock block = _blocks[x, y, z];

                        //Don't draw blocks "0"
                        if (block.BlockDataTemplate.BlockUID == 0)
                        {
                            Vec3Int xyz = new Vec3Int(x, y, z);
                            if (!_removedBlocks.Contains(xyz))
                            {
                                continue;
                            }

                            _removedBlocks.Remove(xyz);
                        }

                        for (int f = 0; f < block.BlockDataTemplate.BlockDrawFaces.Length; f++)
                        {
                            ChunkRenderer cr = lastCR;

                            GameManifestV2.SpriteDataTemplate sprite = lastSprite;
                            bool found = true;

                            //Check if its the same, so we don't have to look it up again
                            //if (lastSprite == null || lastSprite.SpriteUID != block.BlockDataTemplate.BlockDefaultFaceUIDs[f])
                            if (lastSprite == null || lastSprite.SpriteUID != block.MapBlock.BlockFacesSpriteUIDs[f])
                            {
                                //found = GameManifestV2.Singleton.GetSprite(block.BlockDataTemplate.BlockDefaultFaceUIDs[f], out sprite);
                                found = GameManifestV2.Singleton.GetSprite(block.MapBlock.BlockFacesSpriteUIDs[f], out sprite);
                            }
                             
                            ushort spriteMaterialUID = 0;

                            if (found)
                            {
                                spriteMaterialUID = sprite.SpriteSheetTemplate.MaterialUID;
                            }
                            else
                            {
                                sprite = GameManifestV2.DefaultSprite;
                                spriteMaterialUID = sprite.SpriteSheetTemplate.MaterialUID;
                            }

                            if (cr == null || cr.ChunkMaterialUID != spriteMaterialUID)
                            {
                                cr = GetChunkRenderer(spriteMaterialUID);
                            }

                            UpdateChunkRendererBlock(cr, block, (GameManifestV2.BlockDataTemplate.ShowFaceDirection)f, sprite, x, y, z, xw, yh, zl);

                            lastCR = cr;
                            lastSprite = sprite;
                        }
                    }
                }
            }

            List<ChunkRenderer> crs = _chunkRenderers.List;

            for (int i = 0; i < crs.Count; i++)
            {
                crs[i].BuildMesh();
            }

            IsDirty = false;
            NeedsRebuilt = false;
        }

        protected void UpdateChunkRendererBlock(ChunkRenderer chunkRenderer, VoxelBlock block, 
            GameManifestV2.BlockDataTemplate.ShowFaceDirection blockFace,
            GameManifestV2.SpriteDataTemplate sprite,
            int curX, int curY, int curZ,
            int maxX, int maxY, int maxZ)
        {
            //Empty block
            if (block.MapBlock.BlockType == 0)
            {
                return;
            }

            #region Neighbor Checks
            bool needBottomFace = false;
            bool needTopFace = false;
            bool needFrontFace = false;
            bool needBackFace = false;
            bool needRightFace = false;
            bool needLeftFace = false;

            Vector4 uvs = sprite.GetUVCoords();

            //Check bottom
            if (blockFace == GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceYMinus)
            {
                if (curY == 0)
                {
                    //It's at the bottom, so add bottom face
                    needBottomFace = true;
                }
                else //if (_blocks[curX, curY - 1, curZ].MapBlock.BlockType == 0)
                {
                    GameManifestV2.BlockDataTemplate templateBottom;
                    GameManifestV2.Singleton.GetBlockTemplate(_blocks[curX, curY - 1, curZ].MapBlock.BlockType, out templateBottom);

                    if (templateBottom.BlockUID == 0)
                    {
                        needBottomFace = true;
                    }
                    else if (templateBottom.BlockFlags.ContainsFlag("Transparent"))
                    {
                        needBottomFace = true;
                    }

                    //Bottom neighbor is empty
                    //needBottomFace = true;
                }
                //else if (block.BlockDataTemplate.BlockFlags.ContainsFlag("Transparent"))
                //{
                //    needBottomFace = true; 
                //}


            }

            //Check top
            else if (blockFace == GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceYPlus)
            {
                //if (curY == maxY - 1)
                //{
                //    //It's the top, so add top face
                //    needTopFace = true;
                //}
                //else if (_blocks[curX, curY + 1, curZ].MapBlock.BlockType == 0)
                //{
                //    //Top neighbor is empty
                //    needTopFace = true;
                //}
                ////else if (block.BlockDataTemplate.BlockFlags.ContainsFlag("Transparent"))
                ////{
                ////    needTopFace = true;
                ////}

                if (curY == maxY - 1)
                {
                    //It's at the bottom, so add bottom face
                    needTopFace = true;
                }
                else
                {
                    GameManifestV2.BlockDataTemplate templateTop;
                    GameManifestV2.Singleton.GetBlockTemplate(_blocks[curX, curY + 1, curZ].MapBlock.BlockType, out templateTop);

                    if (templateTop.BlockUID == 0)
                    {
                        needTopFace = true;
                    }
                    else if (templateTop.BlockFlags.ContainsFlag("Transparent"))
                    {
                        needTopFace = true;
                    }
                }
            }

            //Check front
            else if (blockFace == GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceZPlus)
            {
                //if (curZ == maxZ - 1)
                //{
                //    //It's the front (Z+) most block, so add front face
                //    needFrontFace = true;
                //}
                //else if (_blocks[curX, curY, curZ + 1].MapBlock.BlockType == 0)
                //{
                //    //Front neighbor is empty
                //    needFrontFace = true;
                //}
                ////else if (block.BlockDataTemplate.BlockFlags.ContainsFlag("Transparent"))
                ////{
                ////    needFrontFace = true;
                ////}
                if (curZ == maxZ - 1)
                {
                    //It's at the bottom, so add bottom face
                    needFrontFace = true;
                }
                else
                {
                    GameManifestV2.BlockDataTemplate templateFront;
                    GameManifestV2.Singleton.GetBlockTemplate(_blocks[curX, curY, curZ + 1].MapBlock.BlockType, out templateFront);

                    if (templateFront.BlockUID == 0)
                    {
                        needFrontFace = true;
                    }
                    else if (templateFront.BlockFlags.ContainsFlag("Transparent"))
                    {
                        needFrontFace = true;
                    }
                }
            }

            //Check back
            else if (blockFace == GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceZMinus)
            {
                //if (curZ == 0)
                //{
                //    //It's the back most block, so add back face
                //    needBackFace = true;
                //}
                //else if (_blocks[curX, curY, curZ - 1].MapBlock.BlockType == 0)
                //{
                //    //Back neighbor is empty
                //    needBackFace = true;
                //}
                ////else if (block.BlockDataTemplate.BlockFlags.ContainsFlag("Transparent"))
                ////{
                ////    needBackFace = true;
                ////}

                if (curZ == 0)
                {
                    //It's at the bottom, so add bottom face
                    needBackFace = true;
                }
                else
                {
                    GameManifestV2.BlockDataTemplate templateBack;
                    GameManifestV2.Singleton.GetBlockTemplate(_blocks[curX, curY, curZ - 1].MapBlock.BlockType, out templateBack);

                    if (templateBack.BlockUID == 0)
                    {
                        needBackFace = true;
                    }
                    else if (templateBack.BlockFlags.ContainsFlag("Transparent"))
                    {
                        needBackFace = true;
                    }
                }
            }

            //Check right
            else if (blockFace == GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceXPlus)
            {
                //if (curX == maxX - 1)
                //{
                //    //It's the right most block, so add right face
                //    needRightFace = true;
                //}
                //else if (_blocks[curX + 1, curY, curZ].MapBlock.BlockType == 0)
                //{
                //    //Right neighbor is empty
                //    needRightFace = true;
                //}
                ////else if (block.BlockDataTemplate.BlockFlags.ContainsFlag("Transparent"))
                ////{
                ////    needRightFace = true;
                ////}

                if (curX == maxX - 1)
                {
                    //It's at the bottom, so add bottom face
                    needRightFace = true;
                }
                else
                {
                    GameManifestV2.BlockDataTemplate templateRight;
                    GameManifestV2.Singleton.GetBlockTemplate(_blocks[curX + 1, curY, curZ].MapBlock.BlockType, out templateRight);

                    if (templateRight.BlockUID == 0)
                    {
                        needRightFace = true;
                    }
                    else if (templateRight.BlockFlags.ContainsFlag("Transparent"))
                    {
                        needRightFace = true;
                    }
                }
            }

            //Check left
            else if (blockFace == GameManifestV2.BlockDataTemplate.ShowFaceDirection.FaceXMinus)
            {
                //if (curX == 0)
                //{
                //    //It's the left most block, so add left face
                //    needLeftFace = true;
                //}
                //else if (_blocks[curX - 1, curY, curZ].MapBlock.BlockType == 0)
                //{
                //    //Left neighbor is empty
                //    needLeftFace = true;
                //}
                ////else if (block.BlockDataTemplate.BlockFlags.ContainsFlag("Transparent"))
                ////{
                ////    needLeftFace = true;
                ////}

                if (curX == 0)
                {
                    //It's at the bottom, so add bottom face
                    needLeftFace = true;
                }
                else
                {
                    GameManifestV2.BlockDataTemplate templateLeft;
                    GameManifestV2.Singleton.GetBlockTemplate(_blocks[curX - 1, curY, curZ].MapBlock.BlockType, out templateLeft);

                    if (templateLeft.BlockUID == 0)
                    {
                        needLeftFace = true;
                    }
                    else if (templateLeft.BlockFlags.ContainsFlag("Transparent"))
                    {
                        needLeftFace = true;
                    }
                }
            }
            #endregion

            Color32 c = new Color32(128, 128, 128, 255);
            //Color32 c2 = new Color32(96, 96, 96, 255);
            //Color32 c3 = new Color32(64, 64, 64, 255);
            //Color32 c4 = new Color32(32, 32, 32, 255);

            #region Face Construction
            if (needTopFace)
            {
                int firstIndex = chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    Vector3.up, new Vector2(uvs.x, uvs.y), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    Vector3.up, new Vector2(uvs.x, uvs.y + uvs.z), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    Vector3.up, new Vector2(uvs.x + uvs.w, uvs.y + uvs.z), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    Vector3.up, new Vector2(uvs.x + uvs.w, uvs.y), c);

                chunkRenderer.AddQuadFace(firstIndex, firstIndex, firstIndex + 1, firstIndex + 2, firstIndex + 3, true);
            }

            if (needBottomFace)
            {
                int firstIndex = chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    -Vector3.up, new Vector2(uvs.x, uvs.y + uvs.z), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    -Vector3.up, new Vector2(uvs.x, uvs.y), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    -Vector3.up, new Vector2(uvs.x + uvs.w, uvs.y), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    -Vector3.up, new Vector2(uvs.x + uvs.w, uvs.y + uvs.z), c);

                chunkRenderer.AddQuadFace(firstIndex, firstIndex + 2, firstIndex + 1, firstIndex, firstIndex + 3, true);
            }

            if (needFrontFace)
            {
                int firstIndex = chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    Vector3.forward, new Vector2(uvs.x + uvs.w, uvs.y + uvs.z), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    Vector3.forward, new Vector2(uvs.x + uvs.w, uvs.y), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    Vector3.forward, new Vector2(uvs.x, uvs.y), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    Vector3.forward, new Vector2(uvs.x, uvs.y + uvs.z), c);

                chunkRenderer.AddQuadFace(firstIndex, firstIndex, firstIndex + 1, firstIndex + 2, firstIndex + 3, true);
            }

            if (needBackFace)
            {
                int firstIndex = chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    -Vector3.forward, new Vector2(uvs.x, uvs.y + uvs.z), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    -Vector3.forward, new Vector2(uvs.x, uvs.y), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    -Vector3.forward, new Vector2(uvs.x + uvs.w, uvs.y), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    -Vector3.forward, new Vector2(uvs.x + uvs.w, uvs.y + uvs.z), c);

                chunkRenderer.AddQuadFace(firstIndex, firstIndex + 2, firstIndex + 1, firstIndex, firstIndex + 3, true);
            }

            if (needRightFace)
            {
                int firstIndex = chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    Vector3.right, new Vector2(uvs.x, uvs.y + uvs.z), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    Vector3.right, new Vector2(uvs.x + uvs.w, uvs.y + uvs.z), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    Vector3.right, new Vector2(uvs.x + uvs.w, uvs.y), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX) + (VoxelBlock.DefaultBlockSize.x),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    Vector3.right, new Vector2(uvs.x, uvs.y), c);

                chunkRenderer.AddQuadFace(firstIndex, firstIndex, firstIndex + 1, firstIndex + 2, firstIndex + 3, true);
            }

            if (needLeftFace)
            {
                int firstIndex = chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    -Vector3.right, new Vector2(uvs.x + uvs.w, uvs.y + uvs.z), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY) + (VoxelBlock.DefaultBlockSize.y),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    -Vector3.right, new Vector2(uvs.x, uvs.y + uvs.z), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ) + (VoxelBlock.DefaultBlockSize.z)),
                    -Vector3.right, new Vector2(uvs.x, uvs.y), c);

                chunkRenderer.AddVertexAndUV(new Vector3(
                    (VoxelBlock.DefaultBlockSize.x * curX),
                    (VoxelBlock.DefaultBlockSize.y * curY),
                    (VoxelBlock.DefaultBlockSize.z * curZ)),
                    -Vector3.right, new Vector2(uvs.x + uvs.w, uvs.y), c);

                chunkRenderer.AddQuadFace(firstIndex, firstIndex + 2, firstIndex + 1, firstIndex, firstIndex + 3, true);
            }
            #endregion
        }

        //public void UpdateColor(Color32 
    }
}