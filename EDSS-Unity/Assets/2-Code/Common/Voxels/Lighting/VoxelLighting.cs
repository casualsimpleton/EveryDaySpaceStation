//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// VoxelLighting - Class for calculating lighting
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

namespace EveryDaySpaceStation
{
    public static class VoxelLighting
    {
        public struct LightFloodFillQueueItem
        {
            public enum FillDirection
            {
                Up,
                Right,
                Down,
                Left
            }

            public int _tileIndex;
            public FillDirection _fillDir;
            public int _stepsRemaining;

            public LightFloodFillQueueItem(FillDirection fillDir, int tileIndex, int stepsRemaining)
            {
                _tileIndex = tileIndex;
                _fillDir = fillDir;
                _stepsRemaining = stepsRemaining;
            }

            public void RotateFillDir90ClockWise()
            {
                switch (_fillDir)
                {
                    case FillDirection.Up:
                        _fillDir = FillDirection.Right;
                        break;
                    case FillDirection.Right:
                        _fillDir = FillDirection.Down;
                        break;
                    case FillDirection.Down:
                        _fillDir = FillDirection.Left;
                        break;
                    case FillDirection.Left:
                        _fillDir = FillDirection.Up;
                        break;
                }
            }

            public void RotateFillDir90CounterClockWise()
            {
                switch (_fillDir)
                {
                    case FillDirection.Up:
                        _fillDir = FillDirection.Left;
                        break;
                    case FillDirection.Right:
                        _fillDir = FillDirection.Up;
                        break;
                    case FillDirection.Down:
                        _fillDir = FillDirection.Right;
                        break;
                    case FillDirection.Left:
                        _fillDir = FillDirection.Down;
                        break;
                }
            }

            public void FlipFillDir()
            {
                switch (_fillDir)
                {
                    case FillDirection.Up:
                        _fillDir = FillDirection.Down;
                        break;
                    case FillDirection.Right:
                        _fillDir = FillDirection.Left;
                        break;
                    case FillDirection.Down:
                        _fillDir = FillDirection.Up;
                        break;
                    case FillDirection.Left:
                        _fillDir = FillDirection.Right;
                        break;
                }
            }
        }

        static Queue<LightFloodFillQueueItem> _blocksWaitingCheck = new Queue<LightFloodFillQueueItem>(100);
        static System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();

        public static void LightFloodFillForward(int centerX, int centerZ, int maxDist, Color32 centerColor, ref MapDataV2.MapBlock[, ,] voxelData, ref Queue<MapDataV2.MapBlock> blocksWithLightChanges, LightFloodFillQueueItem.FillDirection fillDir)
        {
#if DEBUGCLIENT
            Profiler.BeginSample("LightFill");
#endif
            _stopWatch.Reset();
            _stopWatch.Start();

            int curDist = maxDist;

            //Y direction modifier
            int fillIndexModifierStepPositive = 0;
            //X direction modifier
            int fillIndexModifierStepNegative = 0;
            int initialCenterX = 0;
            int initialCenterZ = 0;
            int fillIndexModifierX = 0;

            int maxX = voxelData.GetLength(0);
            int maxY = voxelData.GetLength(1);
            int maxZ = voxelData.GetLength(2);
            int maxLen = maxX * maxY * maxZ;

            //Set up the fill modifier, which is relative to the direction we're going. Tangents 1 is always the right tangent of direction and Tangent 2 is left
            switch (fillDir)
            {
                default:
                case LightFloodFillQueueItem.FillDirection.Up:
                    fillIndexModifierX = Helpers.IndexFromVec3Int(maxX, 0, 1, maxX, maxY);
                    fillIndexModifierStepPositive = 1;
                    fillIndexModifierStepNegative = 0;
                    initialCenterX = 0;
                    initialCenterZ = 1;
                    break;

                case LightFloodFillQueueItem.FillDirection.Right:
                    fillIndexModifierX = 1;
                    fillIndexModifierStepPositive = 0;
                    fillIndexModifierStepNegative = 1;
                    initialCenterX = 0;
                    initialCenterZ = 1;
                    break;

                case LightFloodFillQueueItem.FillDirection.Down:
                    fillIndexModifierX = -Helpers.IndexFromVec3Int(0, 0, 1, maxX, maxY);
                    fillIndexModifierStepPositive = -1;
                    fillIndexModifierStepNegative = 0;
                    initialCenterX = 0;
                    initialCenterZ = 1;
                    break;

                case LightFloodFillQueueItem.FillDirection.Left:
                    fillIndexModifierX = -1;
                    fillIndexModifierStepPositive = 0;
                    fillIndexModifierStepNegative = -1;
                    initialCenterX = 0;
                    initialCenterZ = 1;
                    break;
            }

            int testIndex = Helpers.IndexFromVec3Int(centerX + initialCenterX, 1, centerZ + initialCenterZ, maxX, maxY);

            int overflow1 = 0;
            //First march away in direction of fillIndexModifier
            while (curDist > 0 && overflow1 < 100)
            {
                //Make sure its in bounds and doesn't block light
                if (testIndex > -1 && testIndex < maxLen && voxelData[centerX + initialCenterX, 1, centerZ + initialCenterZ].DoesBlockLight == 0)
                {
                    _blocksWaitingCheck.Enqueue(new LightFloodFillQueueItem(fillDir, testIndex, curDist));

                    int neighborRightIndex = testIndex + fillIndexModifierStepPositive;
                    int neighborLeftIndex = testIndex + fillIndexModifierStepNegative;
                    Vec3Int neighborRightPos = Helpers.Vec3IntFromIndex(neighborRightIndex, maxX, maxY);
                    Vec3Int neighborLeftPos = Helpers.Vec3IntFromIndex(neighborLeftIndex, maxX, maxY);

                    int backupTan1 = fillIndexModifierStepPositive;
                    int backupTan2 = fillIndexModifierStepNegative;

                    int curDist2 = curDist;
                    int overflow3 = 0;
                    while (curDist2 > 0 && overflow3 < 100)
                    {
                        overflow3++;
                        if (neighborRightPos.x > -1 && neighborRightPos.x < maxX - 1 &&
                            neighborRightPos.y > -1 && neighborRightPos.y < maxY - 1 &&
                            neighborRightPos.z > -1 && neighborRightPos.z < maxZ - 1 &&
                            voxelData[neighborRightPos.x, neighborRightPos.y, neighborRightPos.z].DoesBlockLight == 0)
                        {
                            _blocksWaitingCheck.Enqueue(new LightFloodFillQueueItem(fillDir, neighborRightIndex, curDist2 - 1));
                        }
                        else
                        {
                            fillIndexModifierStepPositive = 0;
                        }

                        if (neighborLeftPos.x > -1 && neighborLeftPos.x < maxX - 1 &&
                            neighborLeftPos.y > -1 && neighborLeftPos.y < maxY - 1 &&
                            neighborLeftPos.z > -1 && neighborLeftPos.z < maxZ - 1 &&
                            voxelData[neighborLeftPos.x, neighborLeftPos.y, neighborLeftPos.z].DoesBlockLight == 0)
                        {
                            _blocksWaitingCheck.Enqueue(new LightFloodFillQueueItem(fillDir, neighborLeftIndex, curDist2 - 1));
                        }
                        else
                        {
                            fillIndexModifierStepNegative = 0;
                        }

                        neighborRightIndex = neighborRightIndex + fillIndexModifierStepPositive;
                        neighborLeftIndex = neighborLeftIndex + fillIndexModifierStepNegative;
                        neighborRightPos = Helpers.Vec3IntFromIndex(neighborRightIndex, maxX, maxY);
                        neighborLeftPos = Helpers.Vec3IntFromIndex(neighborLeftIndex, maxX, maxY);

                        curDist2--;
                    }

                    fillIndexModifierStepPositive = backupTan1;
                    fillIndexModifierStepNegative = backupTan2;
                }
                else
                {
                    break;
                }

                testIndex += fillIndexModifierX;

                curDist--;
            }

            //Now that _blocksWaitingCheck is populated, we'll march through some
            int overflow2 = 0;
            while (_blocksWaitingCheck.Count > 0 && overflow2 < 1000)
            {
                LightFloodFillQueueItem fItem = _blocksWaitingCheck.Dequeue();

                Vec3Int pos = Helpers.Vec3IntFromIndex(fItem._tileIndex, maxX, maxY);
                MapDataV2.MapBlock block;
                try
                {
                    block = voxelData[pos.x, pos.y, pos.z];
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(string.Format("Problem with " + pos + " ex " + ex.Message));
                    continue;
                }

                block.BlockLight.colorR += centerColor.r;
                block.BlockLight.colorG += centerColor.g;
                block.BlockLight.colorB += centerColor.b;

                block.BlockLight.numLights++;

                blocksWithLightChanges.Enqueue(block);
            }

            _stopWatch.Stop();

#if DEBUGCLIENT
            Profiler.EndSample();
#endif
        }

        public static void CalculateMapLights(ref MapDataV2.MapBlock[, ,] mapData, ref Queue<MapDataV2.MapBlock> blocksWithModifiedLights, VoxelLight light)
        {
            if (light.BlockPosition.x < 0 || light.BlockPosition.y < 0 || light.BlockPosition.z < 0)
            {
                return;
            }

            if (light.BlockPosition.x > mapData.GetLength(0) - 1 || light.BlockPosition.y > mapData.GetLength(1) - 1 || light.BlockPosition.z > mapData.GetLength(2) - 1)
            {
                return;
            }

            VoxelLighting.LightFloodFillForward(light.BlockPosition.x, light.BlockPosition.z, 2, light.ColorLight, ref mapData, ref blocksWithModifiedLights, LightFloodFillQueueItem.FillDirection.Up);
            VoxelLighting.LightFloodFillForward(light.BlockPosition.x, light.BlockPosition.z, 2, light.ColorLight, ref mapData, ref blocksWithModifiedLights, LightFloodFillQueueItem.FillDirection.Right);
            VoxelLighting.LightFloodFillForward(light.BlockPosition.x, light.BlockPosition.z, 2, light.ColorLight, ref mapData, ref blocksWithModifiedLights, LightFloodFillQueueItem.FillDirection.Down);
            VoxelLighting.LightFloodFillForward(light.BlockPosition.x, light.BlockPosition.z, 2, light.ColorLight, ref mapData, ref blocksWithModifiedLights, LightFloodFillQueueItem.FillDirection.Left);
        }
    }
}