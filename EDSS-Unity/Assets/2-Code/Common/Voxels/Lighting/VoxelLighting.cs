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

        public static void LightFloodFillForward(int centerX, int centerZ, int maxDist, Color32 centerColor, ref VoxelBlock[, ,] voxelData, LightFloodFillQueueItem.FillDirection fillDirection)
        {
#if DEBUGCLIENT
            Profiler.BeginSample("LightFill");
#endif
            _stopWatch.Reset();
            _stopWatch.Start();

            int curDist = maxDist;

            //int fillIndexModifier = 0;
            int fillIndexModifierTangent1 = 0;
            int fillIndexModifierTangent2 = 0;
            int initialCenterX = 0;
            int initialCenterY = 0;

            ////Set up the fill modifier, which is relative to the direction we're going. Tangents 1 is always the right tangent of direction and Tangent 2 is left
            //switch (fillDirection)
            //{
            //    default:
            //    case LightFloodFillQueueItem.FillDirection.Up:
            //        fillIndexModifierTangent1 = 1;
            //        fillIndexModifierTangent2 = -1;
            //        initialCenterX = 0;
            //        initialCenterY = 1;
            //        break;

            //    case LightFloodFillQueueItem.FillDirection.Right:
            //        fillIndexModifierTangent1 = 1;
            //        fillIndexModifierTangent2 = -1;
            //        initialCenterX = 0;
            //        initialCenterY = 1;
            //        break;
            //}
        }
    }
}