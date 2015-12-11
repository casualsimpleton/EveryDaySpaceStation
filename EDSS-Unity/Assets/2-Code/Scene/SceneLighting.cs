//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// TileLightGameObject - A gameobject based monobehaviour for carrying around a tilelight
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
    public static class SceneLighting
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

        static Queue<LightFloodFillQueueItem> _tilesWaitingCheck = new Queue<LightFloodFillQueueItem>(100);
        static System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();

        public static void LightFloodFillForward(int centerX, int centerY, int maxDist, Color32 centerColor, ref MapData mapData, LightFloodFillQueueItem.FillDirection fillDir)
        {
            Profiler.BeginSample("Lightfill");
            _stopWatch.Reset();
            _stopWatch.Start();

            int curDist = maxDist;

            int fillIndexModifier = 0;
            int fillIndexModifierTangent1 = 0;
            int fillIndexModifierTangent2 = 0;
            int initialCenterX = 0;
            int initialCenterY = 0;

            //Set up the fill modifier, which is relative to the direction we're going. Tangents 1 is always the right tangent of direction and Tangent 2 is left
            switch (fillDir)
            {
                default:
                case LightFloodFillQueueItem.FillDirection.Up:
                    fillIndexModifier = mapData._mapSize.x;
                    fillIndexModifierTangent1 = 1;
                    fillIndexModifierTangent2 = -1;
                    initialCenterX = 0;
                    initialCenterY = 1;
                    break;

                case LightFloodFillQueueItem.FillDirection.Right:
                    fillIndexModifier = 1;
                    fillIndexModifierTangent1 = -mapData._mapSize.x;
                    fillIndexModifierTangent2 = mapData._mapSize.x;
                    initialCenterX = 1;
                    initialCenterY = 0;
                    break;

                case LightFloodFillQueueItem.FillDirection.Down:
                    fillIndexModifier = -mapData._mapSize.x;
                    fillIndexModifierTangent1 = -1;
                    fillIndexModifierTangent2 = 1;
                    initialCenterX = 0;
                    initialCenterY = -1;
                    break;

                case LightFloodFillQueueItem.FillDirection.Left:
                    fillIndexModifier = -1;
                    fillIndexModifierTangent1 = mapData._mapSize.x;
                    fillIndexModifierTangent2 = -mapData._mapSize.x;
                    initialCenterX = -1;
                    initialCenterY = 0;
                    break;
            }

            int testIndex = Helpers.IndexFromVec2Int(centerX + initialCenterX, centerY + initialCenterY, mapData._mapSize.x);

            int overflow1 = 0;
            //First march away in direction of fillIndexModifier
            while (curDist > 0 && overflow1 < 100)
            {
                //Make sure it's in bounds and doesn't block light
                if (testIndex > -1 && testIndex < mapData._mapTiles.Length && !mapData._mapTiles[testIndex].BlocksLight)
                {
                    _tilesWaitingCheck.Enqueue(new LightFloodFillQueueItem(fillDir, testIndex, curDist));

                    int neighborRightIndex = testIndex + fillIndexModifierTangent1;
                    int neighborLeftIndex = testIndex + fillIndexModifierTangent2;
                    Vec2Int neighborRightXY = Helpers.Vec2IntFromIndex(neighborRightIndex, mapData._mapSize);
                    Vec2Int neighborLeftXY = Helpers.Vec2IntFromIndex(neighborLeftIndex, mapData._mapSize);

                    int backupTan1 = fillIndexModifierTangent1;
                    int backupTan2 = fillIndexModifierTangent2;

                    int curDist2 = curDist;
                    int overflow3 = 0;
                    while (curDist2 > 0 && overflow3 < 100)
                    {
                        overflow3++;
                        if (neighborRightIndex < mapData._mapTiles.Length && neighborRightXY.x > -1 && neighborRightXY.y > -1 && neighborRightXY.x < mapData._mapSize.x - 1 && neighborRightXY.y < mapData._mapSize.y - 1
                            && !mapData._mapTiles[neighborRightIndex].BlocksLight)
                        {
                            _tilesWaitingCheck.Enqueue(new LightFloodFillQueueItem(fillDir, neighborRightIndex, curDist2 - 1));
                        }
                        else
                        {
                            fillIndexModifierTangent1 = 0;
                        }

                        if (neighborLeftIndex < mapData._mapTiles.Length && neighborLeftXY.x > -1 && neighborLeftXY.y > -1 && neighborLeftXY.x < mapData._mapSize.x - 1 && neighborLeftXY.y < mapData._mapSize.y - 1
                            && !mapData._mapTiles[neighborLeftIndex].BlocksLight)
                        {
                            _tilesWaitingCheck.Enqueue(new LightFloodFillQueueItem(fillDir, neighborLeftIndex, curDist2 - 1));
                        }
                        else
                        {
                            fillIndexModifierTangent2 = 0;
                        }

                        neighborRightIndex = neighborRightIndex + fillIndexModifierTangent1;
                        neighborLeftIndex = neighborLeftIndex + fillIndexModifierTangent2;
                        neighborRightXY = Helpers.Vec2IntFromIndex(neighborRightIndex, mapData._mapSize);
                        neighborLeftXY = Helpers.Vec2IntFromIndex(neighborLeftIndex, mapData._mapSize);

                        curDist2--;
                    }

                    fillIndexModifierTangent1 = backupTan1;
                    fillIndexModifierTangent2 = backupTan2;
                }
                else
                {
                    break;
                }

                testIndex += fillIndexModifier;

                curDist--;
            }

            //Now that the _tilesWaitingCheck is populated, we'll march  through some
            int overflow2 = 0;
            while (_tilesWaitingCheck.Count > 0 && overflow2 < 1000)
            {
                LightFloodFillQueueItem f = _tilesWaitingCheck.Dequeue();

                byte oC = mapData._mapTiles[f._tileIndex].LightColor.r;
                byte c = (byte)Mathf.Min(centerColor.r, (centerColor.r * ((f._stepsRemaining) * 0.2f)));

                if (oC > c)
                    continue;

                mapData._mapTiles[f._tileIndex].LightColor = new Color32(c, c, c, 255);
            }

            _stopWatch.Stop();
            //Debug.Log(string.Format("Elapsed time for {0},{1} fill {2} is {3}ms", centerX, centerY, fillDir, timer1.ElapsedMilliseconds));

            Profiler.EndSample();
        }
    }
}