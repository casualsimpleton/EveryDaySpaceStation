//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// VoxelTempWorldData - Holds a bunch of different hardcoded temporary data maps
// Created: Febuary 1 2016
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: Febuary 1 2016
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
    public static class VoxelTempWorldData
    {
        public static ushort[, ,] GetTempMap1()
        {
            ushort[, ,] TempMap = new ushort[4, 4, 6];
            #region Bottom Layer
            TempMap[0, 0, 0] = 1;
            TempMap[0, 0, 1] = 1;
            TempMap[0, 0, 2] = 1;
            TempMap[0, 0, 3] = 1;
            TempMap[0, 0, 4] = 1;
            TempMap[0, 0, 5] = 1;

            TempMap[1, 0, 0] = 2;
            TempMap[1, 0, 1] = 2;
            TempMap[1, 0, 2] = 2;
            TempMap[1, 0, 3] = 2;
            TempMap[1, 0, 4] = 2;
            TempMap[1, 0, 5] = 2;

            TempMap[2, 0, 0] = 2;
            TempMap[2, 0, 1] = 2;
            TempMap[2, 0, 2] = 2;
            TempMap[2, 0, 3] = 2;
            TempMap[2, 0, 4] = 2;
            TempMap[2, 0, 5] = 2;

            TempMap[3, 0, 0] = 1;
            TempMap[3, 0, 1] = 1;
            TempMap[3, 0, 2] = 1;
            TempMap[3, 0, 3] = 1;
            TempMap[3, 0, 4] = 1;
            TempMap[3, 0, 5] = 1;
            #endregion

            #region Second Layer
            TempMap[0, 1, 0] = 4;
            TempMap[0, 1, 1] = 4;
            TempMap[0, 1, 2] = 4;
            TempMap[0, 1, 3] = 4;
            TempMap[0, 1, 4] = 4;
            TempMap[0, 1, 5] = 4;

            TempMap[1, 1, 0] = 0;
            TempMap[1, 1, 1] = 0;
            TempMap[1, 1, 2] = 0;
            TempMap[1, 1, 3] = 0;
            TempMap[1, 1, 4] = 0;
            TempMap[1, 1, 5] = 0;

            TempMap[2, 1, 0] = 0;
            TempMap[2, 1, 1] = 0;
            TempMap[2, 1, 2] = 0;
            TempMap[2, 1, 3] = 0;
            TempMap[2, 1, 4] = 0;
            TempMap[2, 1, 5] = 0;

            TempMap[3, 1, 0] = 4;
            TempMap[3, 1, 1] = 4;
            TempMap[3, 1, 2] = 5;
            TempMap[3, 1, 3] = 4;
            TempMap[3, 1, 4] = 4;
            TempMap[3, 1, 5] = 4;
            #endregion

            #region Third Layer
            TempMap[0, 2, 0] = 4;
            TempMap[0, 2, 1] = 4;
            TempMap[0, 2, 2] = 4;
            TempMap[0, 2, 3] = 4;
            TempMap[0, 2, 4] = 4;
            TempMap[0, 2, 5] = 4;

            TempMap[1, 2, 0] = 0;
            TempMap[1, 2, 1] = 0;
            TempMap[1, 2, 2] = 0;
            TempMap[1, 2, 3] = 0;
            TempMap[1, 2, 4] = 0;
            TempMap[1, 2, 5] = 0;

            TempMap[2, 2, 0] = 0;
            TempMap[2, 2, 1] = 0;
            TempMap[2, 2, 2] = 0;
            TempMap[2, 2, 3] = 0;
            TempMap[2, 2, 4] = 0;
            TempMap[2, 2, 5] = 0;

            TempMap[3, 2, 0] = 4;
            TempMap[3, 2, 1] = 4;
            TempMap[3, 2, 2] = 4;
            TempMap[3, 2, 3] = 4;
            TempMap[3, 2, 4] = 4;
            TempMap[3, 2, 5] = 4;
            #endregion

            #region Top Layer
            TempMap[0, 3, 0] = 1;
            TempMap[0, 3, 1] = 1;
            TempMap[0, 3, 2] = 1;
            TempMap[0, 3, 3] = 1;
            TempMap[0, 3, 4] = 1;
            TempMap[0, 3, 5] = 1;

            TempMap[1, 3, 0] = 3;
            TempMap[1, 3, 1] = 3;
            TempMap[1, 3, 2] = 3;
            TempMap[1, 3, 3] = 3;
            TempMap[1, 3, 4] = 3;
            TempMap[1, 3, 5] = 3;

            TempMap[2, 3, 0] = 3;
            TempMap[2, 3, 1] = 3;
            TempMap[2, 3, 2] = 3;
            TempMap[2, 3, 3] = 3;
            TempMap[2, 3, 4] = 3;
            TempMap[2, 3, 5] = 3;

            TempMap[3, 3, 0] = 1;
            TempMap[3, 3, 1] = 1;
            TempMap[3, 3, 2] = 1;
            TempMap[3, 3, 3] = 1;
            TempMap[3, 3, 4] = 1;
            TempMap[3, 3, 5] = 1;
            #endregion

            return TempMap;
        }

        public static ushort[, ,] GetTempMap2()
        {
            ushort[, ,] TempMap = new ushort[20, 4, 20];
            #region Bottom Layer
            //Col 0
            TempMap[0, 0, 0] = 0;
            TempMap[0, 0, 1] = 1;
            TempMap[0, 0, 2] = 1;
            TempMap[0, 0, 3] = 1;
            TempMap[0, 0, 4] = 1;
            TempMap[0, 0, 5] = 1;
            TempMap[0, 0, 6] = 1;
            TempMap[0, 0, 7] = 0;
            TempMap[0, 0, 8] = 0;
            TempMap[0, 0, 9] = 0;
            TempMap[0, 0, 10] = 0;
            TempMap[0, 0, 11] = 0;
            TempMap[0, 0, 12] = 0;
            TempMap[0, 0, 13] = 1;
            TempMap[0, 0, 14] = 1;
            TempMap[0, 0, 15] = 1;
            TempMap[0, 0, 16] = 1;
            TempMap[0, 0, 17] = 1;
            TempMap[0, 0, 18] = 0;
            TempMap[0, 0, 19] = 0;

            //Col 1
            TempMap[1, 0, 0] = 0;
            TempMap[1, 0, 1] = 1;
            TempMap[1, 0, 2] = 1;
            TempMap[1, 0, 3] = 1;
            TempMap[1, 0, 4] = 1;
            TempMap[1, 0, 5] = 1;
            TempMap[1, 0, 6] = 1;
            TempMap[1, 0, 7] = 1;
            TempMap[1, 0, 8] = 0;
            TempMap[1, 0, 9] = 0;
            TempMap[1, 0, 10] = 0;
            TempMap[1, 0, 11] = 0;
            TempMap[1, 0, 12] = 0;
            TempMap[1, 0, 13] = 1;
            TempMap[1, 0, 14] = 1;
            TempMap[1, 0, 15] = 1;
            TempMap[1, 0, 16] = 1;
            TempMap[1, 0, 17] = 1;
            TempMap[1, 0, 18] = 0;
            TempMap[1, 0, 19] = 0;

            //Col 2
            TempMap[2, 0, 0] = 0;
            TempMap[2, 0, 1] = 1;
            TempMap[2, 0, 2] = 1;
            TempMap[2, 0, 3] = 1;
            TempMap[2, 0, 4] = 1;
            TempMap[2, 0, 5] = 1;
            TempMap[2, 0, 6] = 1;
            TempMap[2, 0, 7] = 1;
            TempMap[2, 0, 8] = 1;
            TempMap[2, 0, 9] = 1;
            TempMap[2, 0, 10] = 1;
            TempMap[2, 0, 11] = 1;
            TempMap[2, 0, 12] = 1;
            TempMap[2, 0, 13] = 1;
            TempMap[2, 0, 14] = 1;
            TempMap[2, 0, 15] = 1;
            TempMap[2, 0, 16] = 1;
            TempMap[2, 0, 17] = 1;
            TempMap[2, 0, 18] = 0;
            TempMap[2, 0, 19] = 0;

            //Col 3
            TempMap[3, 0, 0] = 0;
            TempMap[3, 0, 1] = 1;
            TempMap[3, 0, 2] = 1;
            TempMap[3, 0, 3] = 1;
            TempMap[3, 0, 4] = 1;
            TempMap[3, 0, 5] = 1;
            TempMap[3, 0, 6] = 1;
            TempMap[3, 0, 7] = 1;
            TempMap[3, 0, 8] = 2;
            TempMap[3, 0, 9] = 2;
            TempMap[3, 0, 10] = 2;
            TempMap[3, 0, 11] = 2;
            TempMap[3, 0, 12] = 2;
            TempMap[3, 0, 13] = 1;
            TempMap[3, 0, 14] = 1;
            TempMap[3, 0, 15] = 1;
            TempMap[3, 0, 16] = 1;
            TempMap[3, 0, 17] = 1;
            TempMap[3, 0, 18] = 0;
            TempMap[3, 0, 19] = 0;
            
            //Col 4
            TempMap[4, 0, 0] = 0;
            TempMap[4, 0, 1] = 1;
            TempMap[4, 0, 2] = 1;
            TempMap[4, 0, 3] = 1;
            TempMap[4, 0, 4] = 1;
            TempMap[4, 0, 5] = 1;
            TempMap[4, 0, 6] = 1;
            TempMap[4, 0, 7] = 2;
            TempMap[4, 0, 8] = 2;
            TempMap[4, 0, 9] = 2;
            TempMap[4, 0, 10] = 2;
            TempMap[4, 0, 11] = 2;
            TempMap[4, 0, 12] = 2;
            TempMap[4, 0, 13] = 2;
            TempMap[4, 0, 14] = 1;
            TempMap[4, 0, 15] = 1;
            TempMap[4, 0, 16] = 1;
            TempMap[4, 0, 17] = 1;
            TempMap[4, 0, 18] = 1;
            TempMap[4, 0, 19] = 1;

            //Col 5
            TempMap[5, 0, 0] = 1;
            TempMap[5, 0, 1] = 1;
            TempMap[5, 0, 2] = 1;
            TempMap[5, 0, 3] = 1;
            TempMap[5, 0, 4] = 1;
            TempMap[5, 0, 5] = 1;
            TempMap[5, 0, 6] = 1;
            TempMap[5, 0, 7] = 2;
            TempMap[5, 0, 8] = 2;
            TempMap[5, 0, 9] = 2;
            TempMap[5, 0, 10] = 2;
            TempMap[5, 0, 11] = 2;
            TempMap[5, 0, 12] = 2;
            TempMap[5, 0, 13] = 1;
            TempMap[5, 0, 14] = 1;
            TempMap[5, 0, 15] = 1;
            TempMap[5, 0, 16] = 1;
            TempMap[5, 0, 17] = 1;
            TempMap[5, 0, 18] = 1;
            TempMap[5, 0, 19] = 1;

            //Col 6
            TempMap[6, 0, 0] = 1;
            TempMap[6, 0, 1] = 2;
            TempMap[6, 0, 2] = 2;
            TempMap[6, 0, 3] = 2;
            TempMap[6, 0, 4] = 1;
            TempMap[6, 0, 5] = 1;
            TempMap[6, 0, 6] = 1;
            TempMap[6, 0, 7] = 1;
            TempMap[6, 0, 8] = 2;
            TempMap[6, 0, 9] = 2;
            TempMap[6, 0, 10] = 2;
            TempMap[6, 0, 11] = 2;
            TempMap[6, 0, 12] = 2;
            TempMap[6, 0, 13] = 1;
            TempMap[6, 0, 14] = 1;
            TempMap[6, 0, 15] = 1;
            TempMap[6, 0, 16] = 3;
            TempMap[6, 0, 17] = 3;
            TempMap[6, 0, 18] = 1;
            TempMap[6, 0, 19] = 1;

            //Col 7
            TempMap[7, 0, 0] = 1;
            TempMap[7, 0, 1] = 2;
            TempMap[7, 0, 2] = 2;
            TempMap[7, 0, 3] = 2;
            TempMap[7, 0, 4] = 2;
            TempMap[7, 0, 5] = 1;
            TempMap[7, 0, 6] = 1;
            TempMap[7, 0, 7] = 1;
            TempMap[7, 0, 8] = 1;
            TempMap[7, 0, 9] = 1;
            TempMap[7, 0, 10] = 1;
            TempMap[7, 0, 11] = 1;
            TempMap[7, 0, 12] = 1;
            TempMap[7, 0, 13] = 1;
            TempMap[7, 0, 14] = 1;
            TempMap[7, 0, 15] = 1;
            TempMap[7, 0, 16] = 3;
            TempMap[7, 0, 17] = 3;
            TempMap[7, 0, 18] = 1;
            TempMap[7, 0, 19] = 1;

            //Col 8
            TempMap[8, 0, 0] = 1;
            TempMap[8, 0, 1] = 2;
            TempMap[8, 0, 2] = 2;
            TempMap[8, 0, 3] = 2;
            TempMap[8, 0, 4] = 1;
            TempMap[8, 0, 5] = 1;
            TempMap[8, 0, 6] = 1;
            TempMap[8, 0, 7] = 1;
            TempMap[8, 0, 8] = 1;
            TempMap[8, 0, 9] = 1;
            TempMap[8, 0, 10] = 1;
            TempMap[8, 0, 11] = 1;
            TempMap[8, 0, 12] = 1;
            TempMap[8, 0, 13] = 1;
            TempMap[8, 0, 14] = 1;
            TempMap[8, 0, 15] = 1;
            TempMap[8, 0, 16] = 3;
            TempMap[8, 0, 17] = 3;
            TempMap[8, 0, 18] = 1;
            TempMap[8, 0, 19] = 1;

            //Col 9
            TempMap[9, 0, 0] = 1;
            TempMap[9, 0, 1] = 2;
            TempMap[9, 0, 2] = 2;
            TempMap[9, 0, 3] = 2;
            TempMap[9, 0, 4] = 1;
            TempMap[9, 0, 5] = 1;
            TempMap[9, 0, 6] = 3;
            TempMap[9, 0, 7] = 3;
            TempMap[9, 0, 8] = 3;
            TempMap[9, 0, 9] = 3;
            TempMap[9, 0, 10] = 3;
            TempMap[9, 0, 11] = 3;
            TempMap[9, 0, 12] = 3;
            TempMap[9, 0, 13] = 3;
            TempMap[9, 0, 14] = 3;
            TempMap[9, 0, 15] = 3;
            TempMap[9, 0, 16] = 3;
            TempMap[9, 0, 17] = 3;
            TempMap[9, 0, 18] = 1;
            TempMap[9, 0, 19] = 1;

            //Col 10
            TempMap[10, 0, 0] = 1;
            TempMap[10, 0, 1] = 2;
            TempMap[10, 0, 2] = 2;
            TempMap[10, 0, 3] = 2;
            TempMap[10, 0, 4] = 1;
            TempMap[10, 0, 5] = 1;
            TempMap[10, 0, 6] = 3;
            TempMap[10, 0, 7] = 3;
            TempMap[10, 0, 8] = 3;
            TempMap[10, 0, 9] = 3;
            TempMap[10, 0, 10] = 3;
            TempMap[10, 0, 11] = 3;
            TempMap[10, 0, 12] = 3;
            TempMap[10, 0, 13] = 3;
            TempMap[10, 0, 14] = 3;
            TempMap[10, 0, 15] = 3;
            TempMap[10, 0, 16] = 3;
            TempMap[10, 0, 17] = 3;
            TempMap[10, 0, 18] = 1;
            TempMap[10, 0, 19] = 1;

            //Col 11
            TempMap[11, 0, 0] = 1;
            TempMap[11, 0, 1] = 1;
            TempMap[11, 0, 2] = 1;
            TempMap[11, 0, 3] = 1;
            TempMap[11, 0, 4] = 1;
            TempMap[11, 0, 5] = 1;
            TempMap[11, 0, 6] = 1;
            TempMap[11, 0, 7] = 1;
            TempMap[11, 0, 8] = 1;
            TempMap[11, 0, 9] = 1;
            TempMap[11, 0, 10] = 1;
            TempMap[11, 0, 11] = 1;
            TempMap[11, 0, 12] = 1;
            TempMap[11, 0, 13] = 1;
            TempMap[11, 0, 14] = 1;
            TempMap[11, 0, 15] = 1;
            TempMap[11, 0, 16] = 1;
            TempMap[11, 0, 17] = 1;
            TempMap[11, 0, 18] = 1;
            TempMap[11, 0, 19] = 1;

            //Col 12
            TempMap[12, 0, 0] = 0;
            TempMap[12, 0, 1] = 1;
            TempMap[12, 0, 2] = 1;
            TempMap[12, 0, 3] = 1;
            TempMap[12, 0, 4] = 1;
            TempMap[12, 0, 5] = 1;
            TempMap[12, 0, 6] = 1;
            TempMap[12, 0, 7] = 1;
            TempMap[12, 0, 8] = 1;
            TempMap[12, 0, 9] = 1;
            TempMap[12, 0, 10] = 1;
            TempMap[12, 0, 11] = 1;
            TempMap[12, 0, 12] = 1;
            TempMap[12, 0, 13] = 1;
            TempMap[12, 0, 14] = 1;
            TempMap[12, 0, 15] = 1;
            TempMap[12, 0, 16] = 1;
            TempMap[12, 0, 17] = 1;
            TempMap[12, 0, 18] = 1;
            TempMap[12, 0, 19] = 1;

            //Col 13
            TempMap[13, 0, 0] = 0;
            TempMap[13, 0, 1] = 1;
            TempMap[13, 0, 2] = 1;
            TempMap[13, 0, 3] = 1;
            TempMap[13, 0, 4] = 1;
            TempMap[13, 0, 5] = 1;
            TempMap[13, 0, 6] = 1;
            TempMap[13, 0, 7] = 1;
            TempMap[13, 0, 8] = 1;
            TempMap[13, 0, 9] = 1;
            TempMap[13, 0, 10] = 1;
            TempMap[13, 0, 11] = 1;
            TempMap[13, 0, 12] = 1;
            TempMap[13, 0, 13] = 1;
            TempMap[13, 0, 14] = 1;
            TempMap[13, 0, 15] = 1;
            TempMap[13, 0, 16] = 1;
            TempMap[13, 0, 17] = 1;
            TempMap[13, 0, 18] = 1;
            TempMap[13, 0, 19] = 1;

            //Col 14
            TempMap[14, 0, 0] = 0;
            TempMap[14, 0, 1] = 1;
            TempMap[14, 0, 2] = 1;
            TempMap[14, 0, 3] = 1;
            TempMap[14, 0, 4] = 1;
            TempMap[14, 0, 5] = 1;
            TempMap[14, 0, 6] = 1;
            TempMap[14, 0, 7] = 1;
            TempMap[14, 0, 8] = 1;
            TempMap[14, 0, 9] = 1;
            TempMap[14, 0, 10] = 1;
            TempMap[14, 0, 11] = 1;
            TempMap[14, 0, 12] = 1;
            TempMap[14, 0, 13] = 1;
            TempMap[14, 0, 14] = 1;
            TempMap[14, 0, 15] = 1;
            TempMap[14, 0, 16] = 1;
            TempMap[14, 0, 17] = 1;
            TempMap[14, 0, 18] = 1;
            TempMap[14, 0, 19] = 1;

            //Col 15
            TempMap[15, 0, 0] = 0;
            TempMap[15, 0, 1] = 1;
            TempMap[15, 0, 2] = 1;
            TempMap[15, 0, 3] = 1;
            TempMap[15, 0, 4] = 1;
            TempMap[15, 0, 5] = 1;
            TempMap[15, 0, 6] = 1;
            TempMap[15, 0, 7] = 1;
            TempMap[15, 0, 8] = 1;
            TempMap[15, 0, 9] = 1;
            TempMap[15, 0, 10] = 1;
            TempMap[15, 0, 11] = 1;
            TempMap[15, 0, 12] = 1;
            TempMap[15, 0, 13] = 1;
            TempMap[15, 0, 14] = 1;
            TempMap[15, 0, 15] = 1;
            TempMap[15, 0, 16] = 1;
            TempMap[15, 0, 17] = 1;
            TempMap[15, 0, 18] = 0;
            TempMap[15, 0, 19] = 0;

            //Col 16
            TempMap[16, 0, 0] = 0;
            TempMap[16, 0, 1] = 0;
            TempMap[16, 0, 2] = 1;
            TempMap[16, 0, 3] = 1;
            TempMap[16, 0, 4] = 1;
            TempMap[16, 0, 5] = 1;
            TempMap[16, 0, 6] = 1;
            TempMap[16, 0, 7] = 1;
            TempMap[16, 0, 8] = 1;
            TempMap[16, 0, 9] = 1;
            TempMap[16, 0, 10] = 1;
            TempMap[16, 0, 11] = 1;
            TempMap[16, 0, 12] = 1;
            TempMap[16, 0, 13] = 1;
            TempMap[16, 0, 14] = 1;
            TempMap[16, 0, 15] = 1;
            TempMap[16, 0, 16] = 1;
            TempMap[16, 0, 17] = 1;
            TempMap[16, 0, 18] = 0;
            TempMap[16, 0, 19] = 0;

            //Col 17
            TempMap[17, 0, 0] = 0;
            TempMap[17, 0, 1] = 0;
            TempMap[17, 0, 2] = 1;
            TempMap[17, 0, 3] = 1;
            TempMap[17, 0, 4] = 1;
            TempMap[17, 0, 5] = 1;
            TempMap[17, 0, 6] = 1;
            TempMap[17, 0, 7] = 1;
            TempMap[17, 0, 8] = 1;
            TempMap[17, 0, 9] = 1;
            TempMap[17, 0, 10] = 1;
            TempMap[17, 0, 11] = 1;
            TempMap[17, 0, 12] = 1;
            TempMap[17, 0, 13] = 1;
            TempMap[17, 0, 14] = 1;
            TempMap[17, 0, 15] = 1;
            TempMap[17, 0, 16] = 1;
            TempMap[17, 0, 17] = 1;
            TempMap[17, 0, 18] = 0;
            TempMap[17, 0, 19] = 0;

            //Col 18
            TempMap[18, 0, 0] = 0;
            TempMap[18, 0, 1] = 0;
            TempMap[18, 0, 2] = 1;
            TempMap[18, 0, 3] = 1;
            TempMap[18, 0, 4] = 1;
            TempMap[18, 0, 5] = 1;
            TempMap[18, 0, 6] = 1;
            TempMap[18, 0, 7] = 1;
            TempMap[18, 0, 8] = 0;
            TempMap[18, 0, 9] = 0;
            TempMap[18, 0, 10] = 0;
            TempMap[18, 0, 11] = 0;
            TempMap[18, 0, 12] = 1;
            TempMap[18, 0, 13] = 1;
            TempMap[18, 0, 14] = 1;
            TempMap[18, 0, 15] = 1;
            TempMap[18, 0, 16] = 1;
            TempMap[18, 0, 17] = 1;
            TempMap[18, 0, 18] = 0;
            TempMap[18, 0, 19] = 0;

            //Col 19
            TempMap[19, 0, 0] = 0;
            TempMap[19, 0, 1] = 0;
            TempMap[19, 0, 2] = 0;
            TempMap[19, 0, 3] = 1;
            TempMap[19, 0, 4] = 1;
            TempMap[19, 0, 5] = 1;
            TempMap[19, 0, 6] = 1;
            TempMap[19, 0, 7] = 1;
            TempMap[19, 0, 8] = 0;
            TempMap[19, 0, 9] = 0;
            TempMap[19, 0, 10] = 0;
            TempMap[19, 0, 11] = 0;
            TempMap[19, 0, 12] = 1;
            TempMap[19, 0, 13] = 1;
            TempMap[19, 0, 14] = 1;
            TempMap[19, 0, 15] = 1;
            TempMap[19, 0, 16] = 1;
            TempMap[19, 0, 17] = 1;
            TempMap[19, 0, 18] = 0;
            TempMap[19, 0, 19] = 0;
            #endregion

            #region Second Layer
            //Col 0
            TempMap[0, 1, 0] = 0;
            TempMap[0, 1, 1] = 4;
            TempMap[0, 1, 2] = 4;
            TempMap[0, 1, 3] = 4;
            TempMap[0, 1, 4] = 4;
            TempMap[0, 1, 5] = 4;
            TempMap[0, 1, 6] = 4;
            TempMap[0, 1, 7] = 0;
            TempMap[0, 1, 8] = 0;
            TempMap[0, 1, 9] = 0;
            TempMap[0, 1, 10] = 0;
            TempMap[0, 1, 11] = 0;
            TempMap[0, 1, 12] = 0;
            TempMap[0, 1, 13] = 4;
            TempMap[0, 1, 14] = 4;
            TempMap[0, 1, 15] = 4;
            TempMap[0, 1, 16] = 4;
            TempMap[0, 1, 17] = 4;
            TempMap[0, 1, 18] = 0;
            TempMap[0, 1, 19] = 0;

            //Col 1
            TempMap[1, 1, 0] = 0;
            TempMap[1, 1, 1] = 4;
            TempMap[1, 1, 2] = 0;
            TempMap[1, 1, 3] = 0;
            TempMap[1, 1, 4] = 0;
            TempMap[1, 1, 5] = 0;
            TempMap[1, 1, 6] = 4;
            TempMap[1, 1, 7] = 4;
            TempMap[1, 1, 8] = 0;
            TempMap[1, 1, 9] = 0;
            TempMap[1, 1, 10] = 0;
            TempMap[1, 1, 11] = 0;
            TempMap[1, 1, 12] = 0;
            TempMap[1, 1, 13] = 4;
            TempMap[1, 1, 14] = 0;
            TempMap[1, 1, 15] = 0;
            TempMap[1, 1, 16] = 0;
            TempMap[1, 1, 17] = 4;
            TempMap[1, 1, 18] = 0;
            TempMap[1, 1, 19] = 0;

            //Col 2
            TempMap[2, 1, 0] = 0;
            TempMap[2, 1, 1] = 4;
            TempMap[2, 1, 2] = 0;
            TempMap[2, 1, 3] = 0;
            TempMap[2, 1, 4] = 0;
            TempMap[2, 1, 5] = 0;
            TempMap[2, 1, 6] = 0;
            TempMap[2, 1, 7] = 4;
            TempMap[2, 1, 8] = 4;
            TempMap[2, 1, 9] = 4;
            TempMap[2, 1, 10] = 4;
            TempMap[2, 1, 11] = 4;
            TempMap[2, 1, 12] = 4;
            TempMap[2, 1, 13] = 4;
            TempMap[2, 1, 14] = 0;
            TempMap[2, 1, 15] = 0;
            TempMap[2, 1, 16] = 0;
            TempMap[2, 1, 17] = 4;
            TempMap[2, 1, 18] = 0;
            TempMap[2, 1, 19] = 0;

            //Col 3
            TempMap[3, 1, 0] = 0;
            TempMap[3, 1, 1] = 4;
            TempMap[3, 1, 2] = 0;
            TempMap[3, 1, 3] = 0;
            TempMap[3, 1, 4] = 0;
            TempMap[3, 1, 5] = 0;
            TempMap[3, 1, 6] = 0;
            TempMap[3, 1, 7] = 4;
            TempMap[3, 1, 8] = 0;
            TempMap[3, 1, 9] = 0;
            TempMap[3, 1, 10] = 0;
            TempMap[3, 1, 11] = 0;
            TempMap[3, 1, 12] = 0;
            TempMap[3, 1, 13] = 4;
            TempMap[3, 1, 14] = 0;
            TempMap[3, 1, 15] = 0;
            TempMap[3, 1, 16] = 0;
            TempMap[3, 1, 17] = 4;
            TempMap[3, 1, 18] = 0;
            TempMap[3, 1, 19] = 0;

            //Col 4
            TempMap[4, 1, 0] = 0;
            TempMap[4, 1, 1] = 4;
            TempMap[4, 1, 2] = 0;
            TempMap[4, 1, 3] = 0;
            TempMap[4, 1, 4] = 0;
            TempMap[4, 1, 5] = 0;
            TempMap[4, 1, 6] = 0;
            TempMap[4, 1, 7] = 0;
            TempMap[4, 1, 8] = 0;
            TempMap[4, 1, 9] = 0;
            TempMap[4, 1, 10] = 0;
            TempMap[4, 1, 11] = 0;
            TempMap[4, 1, 12] = 0;
            TempMap[4, 1, 13] = 0;
            TempMap[4, 1, 14] = 0;
            TempMap[4, 1, 15] = 0;
            TempMap[4, 1, 16] = 0;
            TempMap[4, 1, 17] = 4;
            TempMap[4, 1, 18] = 4;
            TempMap[4, 1, 19] = 4;

            //Col 5
            TempMap[5, 1, 0] = 4;
            TempMap[5, 1, 1] = 4;
            TempMap[5, 1, 2] = 4;
            TempMap[5, 1, 3] = 4;
            TempMap[5, 1, 4] = 4;
            TempMap[5, 1, 5] = 0;
            TempMap[5, 1, 6] = 0;
            TempMap[5, 1, 7] = 0;
            TempMap[5, 1, 8] = 0;
            TempMap[5, 1, 9] = 0;
            TempMap[5, 1, 10] = 0;
            TempMap[5, 1, 11] = 0;
            TempMap[5, 1, 12] = 0;
            TempMap[5, 1, 13] = 4;
            TempMap[5, 1, 14] = 0;
            TempMap[5, 1, 15] = 0;
            TempMap[5, 1, 16] = 0;
            TempMap[5, 1, 17] = 0;
            TempMap[5, 1, 18] = 0;
            TempMap[5, 1, 19] = 4;

            //Col 6
            TempMap[6, 1, 0] = 4;
            TempMap[6, 1, 1] = 0;
            TempMap[6, 1, 2] = 0;
            TempMap[6, 1, 3] = 0;
            TempMap[6, 1, 4] = 4;
            TempMap[6, 1, 5] = 0;
            TempMap[6, 1, 6] = 0;
            TempMap[6, 1, 7] = 4;
            TempMap[6, 1, 8] = 0;
            TempMap[6, 1, 9] = 0;
            TempMap[6, 1, 10] = 0;
            TempMap[6, 1, 11] = 0;
            TempMap[6, 1, 12] = 4;
            TempMap[6, 1, 13] = 0;
            TempMap[6, 1, 14] = 0;
            TempMap[6, 1, 15] = 0;
            TempMap[6, 1, 16] = 0;
            TempMap[6, 1, 17] = 0;
            TempMap[6, 1, 18] = 0;
            TempMap[6, 1, 19] = 4;

            //Col 7
            TempMap[7, 1, 0] = 4;
            TempMap[7, 1, 1] = 0;
            TempMap[7, 1, 2] = 0;
            TempMap[7, 1, 3] = 0;
            TempMap[7, 1, 4] = 4;
            TempMap[7, 1, 5] = 0;
            TempMap[7, 1, 6] = 0;
            TempMap[7, 1, 7] = 4;
            TempMap[7, 1, 8] = 0;
            TempMap[7, 1, 9] = 0;
            TempMap[7, 1, 10] = 0;
            TempMap[7, 1, 11] = 0;
            TempMap[7, 1, 12] = 0;
            TempMap[7, 1, 13] = 4;
            TempMap[7, 1, 14] = 0;
            TempMap[7, 1, 15] = 0;
            TempMap[7, 1, 16] = 0;
            TempMap[7, 1, 17] = 0;
            TempMap[7, 1, 18] = 0;
            TempMap[7, 1, 19] = 4;

            //Col 8
            TempMap[8, 1, 0] = 4;
            TempMap[8, 1, 1] = 0;
            TempMap[8, 1, 2] = 0;
            TempMap[8, 1, 3] = 0;
            TempMap[8, 1, 4] = 0;
            TempMap[8, 1, 5] = 0;
            TempMap[8, 1, 6] = 0;
            TempMap[8, 1, 7] = 4;
            TempMap[8, 1, 8] = 4;
            TempMap[8, 1, 9] = 4;
            TempMap[8, 1, 10] = 4;
            TempMap[8, 1, 11] = 4;
            TempMap[8, 1, 12] = 4;
            TempMap[8, 1, 13] = 4;
            TempMap[8, 1, 14] = 0;
            TempMap[8, 1, 15] = 0;
            TempMap[8, 1, 16] = 0;
            TempMap[8, 1, 17] = 0;
            TempMap[8, 1, 18] = 0;
            TempMap[8, 1, 19] = 4;

            //Col 9
            TempMap[9, 1, 0] = 4;
            TempMap[9, 1, 1] = 0;
            TempMap[9, 1, 2] = 0;
            TempMap[9, 1, 3] = 0;
            TempMap[9, 1, 4] = 4;
            TempMap[9, 1, 5] = 0;
            TempMap[9, 1, 6] = 0;
            TempMap[9, 1, 7] = 0;
            TempMap[9, 1, 8] = 0;
            TempMap[9, 1, 9] = 0;
            TempMap[9, 1, 10] = 0;
            TempMap[9, 1, 11] = 0;
            TempMap[9, 1, 12] = 0;
            TempMap[9, 1, 13] = 0;
            TempMap[9, 1, 14] = 0;
            TempMap[9, 1, 15] = 0;
            TempMap[9, 1, 16] = 0;
            TempMap[9, 1, 17] = 0;
            TempMap[9, 1, 18] = 0;
            TempMap[9, 1, 19] = 4;

            //Col 10
            TempMap[10, 1, 0] = 4;
            TempMap[10, 1, 1] = 0;
            TempMap[10, 1, 2] = 0;
            TempMap[10, 1, 3] = 0;
            TempMap[10, 1, 4] = 4;
            TempMap[10, 1, 5] = 0;
            TempMap[10, 1, 6] = 0;
            TempMap[10, 1, 7] = 0;
            TempMap[10, 1, 8] = 0;
            TempMap[10, 1, 9] = 0;
            TempMap[10, 1, 10] = 0;
            TempMap[10, 1, 11] = 0;
            TempMap[10, 1, 12] = 0;
            TempMap[10, 1, 13] = 0;
            TempMap[10, 1, 14] = 0;
            TempMap[10, 1, 15] = 0;
            TempMap[10, 1, 16] = 0;
            TempMap[10, 1, 17] = 0;
            TempMap[10, 1, 18] = 0;
            TempMap[10, 1, 19] = 4;

            //Col 11
            TempMap[11, 1, 0] = 4;
            TempMap[11, 1, 1] = 0;
            TempMap[11, 1, 2] = 0;
            TempMap[11, 1, 3] = 0;
            TempMap[11, 1, 4] = 4;
            TempMap[11, 1, 5] = 0;
            TempMap[11, 1, 6] = 0;
            TempMap[11, 1, 7] = 0;
            TempMap[11, 1, 8] = 0;
            TempMap[11, 1, 9] = 0;
            TempMap[11, 1, 10] = 0;
            TempMap[11, 1, 11] = 0;
            TempMap[11, 1, 12] = 0;
            TempMap[11, 1, 13] = 0;
            TempMap[11, 1, 14] = 0;
            TempMap[11, 1, 15] = 0;
            TempMap[11, 1, 16] = 0;
            TempMap[11, 1, 17] = 0;
            TempMap[11, 1, 18] = 0;
            TempMap[11, 1, 19] = 4;

            //Col 12
            TempMap[12, 1, 0] = 4;
            TempMap[12, 1, 1] = 4;
            TempMap[12, 1, 2] = 4;
            TempMap[12, 1, 3] = 4;
            TempMap[12, 1, 4] = 4;
            TempMap[12, 1, 5] = 0;
            TempMap[12, 1, 6] = 0;
            TempMap[12, 1, 7] = 0;
            TempMap[12, 1, 8] = 0;
            TempMap[12, 1, 9] = 0;
            TempMap[12, 1, 10] = 0;
            TempMap[12, 1, 11] = 0;
            TempMap[12, 1, 12] = 0;
            TempMap[12, 1, 13] = 0;
            TempMap[12, 1, 14] = 0;
            TempMap[12, 1, 15] = 0;
            TempMap[12, 1, 16] = 0;
            TempMap[12, 1, 17] = 0;
            TempMap[12, 1, 18] = 0;
            TempMap[12, 1, 19] = 4;

            //Col 13
            TempMap[13, 1, 0] = 0;
            TempMap[13, 1, 1] = 4;
            TempMap[13, 1, 2] = 0;
            TempMap[13, 1, 3] = 0;
            TempMap[13, 1, 4] = 0;
            TempMap[13, 1, 5] = 0;
            TempMap[13, 1, 6] = 0;
            TempMap[13, 1, 7] = 0;
            TempMap[13, 1, 8] = 4;
            TempMap[13, 1, 9] = 4;
            TempMap[13, 1, 10] = 4;
            TempMap[13, 1, 11] = 4;
            TempMap[13, 1, 12] = 0;
            TempMap[13, 1, 13] = 0;
            TempMap[13, 1, 14] = 0;
            TempMap[13, 1, 15] = 0;
            TempMap[13, 1, 16] = 0;
            TempMap[13, 1, 17] = 0;
            TempMap[13, 1, 18] = 0;
            TempMap[13, 1, 19] = 4;

            //Col 14
            TempMap[14, 1, 0] = 0;
            TempMap[14, 1, 1] = 4;
            TempMap[14, 1, 2] = 0;
            TempMap[14, 1, 3] = 0;
            TempMap[14, 1, 4] = 0;
            TempMap[14, 1, 5] = 0;
            TempMap[14, 1, 6] = 0;
            TempMap[14, 1, 7] = 0;
            TempMap[14, 1, 8] = 0;
            TempMap[14, 1, 9] = 4;
            TempMap[14, 1, 10] = 4;
            TempMap[14, 1, 11] = 0;
            TempMap[14, 1, 12] = 0;
            TempMap[14, 1, 13] = 0;
            TempMap[14, 1, 14] = 0;
            TempMap[14, 1, 15] = 0;
            TempMap[14, 1, 16] = 0;
            TempMap[14, 1, 17] = 0;
            TempMap[14, 1, 18] = 0;
            TempMap[14, 1, 19] = 4;

            //Col 15
            TempMap[15, 1, 0] = 0;
            TempMap[15, 1, 1] = 4;
            TempMap[15, 1, 2] = 0;
            TempMap[15, 1, 3] = 0;
            TempMap[15, 1, 4] = 0;
            TempMap[15, 1, 5] = 0;
            TempMap[15, 1, 6] = 0;
            TempMap[15, 1, 7] = 0;
            TempMap[15, 1, 8] = 0;
            TempMap[15, 1, 9] = 0;
            TempMap[15, 1, 10] = 0;
            TempMap[15, 1, 11] = 0;
            TempMap[15, 1, 12] = 0;
            TempMap[15, 1, 13] = 0;
            TempMap[15, 1, 14] = 0;
            TempMap[15, 1, 15] = 0;
            TempMap[15, 1, 16] = 0;
            TempMap[15, 1, 17] = 4;
            TempMap[15, 1, 18] = 4;
            TempMap[15, 1, 19] = 4;

            //Col 16
            TempMap[16, 1, 0] = 0;
            TempMap[16, 1, 1] = 4;
            TempMap[16, 1, 2] = 4;
            TempMap[16, 1, 3] = 0;
            TempMap[16, 1, 4] = 0;
            TempMap[16, 1, 5] = 0;
            TempMap[16, 1, 6] = 0;
            TempMap[16, 1, 7] = 0;
            TempMap[16, 1, 8] = 0;
            TempMap[16, 1, 9] = 0;
            TempMap[16, 1, 10] = 0;
            TempMap[16, 1, 11] = 0;
            TempMap[16, 1, 12] = 0;
            TempMap[16, 1, 13] = 0;
            TempMap[16, 1, 14] = 0;
            TempMap[16, 1, 15] = 0;
            TempMap[16, 1, 16] = 0;
            TempMap[16, 1, 17] = 4;
            TempMap[16, 1, 18] = 0;
            TempMap[16, 1, 19] = 0;

            //Col 17
            TempMap[17, 1, 0] = 0;
            TempMap[17, 1, 1] = 0;
            TempMap[17, 1, 2] = 4;
            TempMap[17, 1, 3] = 0;
            TempMap[17, 1, 4] = 0;
            TempMap[17, 1, 5] = 0;
            TempMap[17, 1, 6] = 0;
            TempMap[17, 1, 7] = 4;
            TempMap[17, 1, 8] = 4;
            TempMap[17, 1, 9] = 4;
            TempMap[17, 1, 10] = 4;
            TempMap[17, 1, 11] = 4;
            TempMap[17, 1, 12] = 4;
            TempMap[17, 1, 13] = 0;
            TempMap[17, 1, 14] = 0;
            TempMap[17, 1, 15] = 0;
            TempMap[17, 1, 16] = 0;
            TempMap[17, 1, 17] = 4;
            TempMap[17, 1, 18] = 0;
            TempMap[17, 1, 19] = 0;

            //Col 18
            TempMap[18, 1, 0] = 0;
            TempMap[18, 1, 1] = 0;
            TempMap[18, 1, 2] = 4;
            TempMap[18, 1, 3] = 4;
            TempMap[18, 1, 4] = 0;
            TempMap[18, 1, 5] = 0;
            TempMap[18, 1, 6] = 0;
            TempMap[18, 1, 7] = 4;
            TempMap[18, 1, 8] = 0;
            TempMap[18, 1, 9] = 0;
            TempMap[18, 1, 10] = 0;
            TempMap[18, 1, 11] = 0;
            TempMap[18, 1, 12] = 4;
            TempMap[18, 1, 13] = 0;
            TempMap[18, 1, 14] = 0;
            TempMap[18, 1, 15] = 0;
            TempMap[18, 1, 16] = 0;
            TempMap[18, 1, 17] = 4;
            TempMap[18, 1, 18] = 0;
            TempMap[18, 1, 19] = 0;

            //Col 19
            TempMap[19, 1, 0] = 0;
            TempMap[19, 1, 1] = 0;
            TempMap[19, 1, 2] = 0;
            TempMap[19, 1, 3] = 4;
            TempMap[19, 1, 4] = 4;
            TempMap[19, 1, 5] = 4;
            TempMap[19, 1, 6] = 4;
            TempMap[19, 1, 7] = 4;
            TempMap[19, 1, 8] = 0;
            TempMap[19, 1, 9] = 0;
            TempMap[19, 1, 10] = 0;
            TempMap[19, 1, 11] = 0;
            TempMap[19, 1, 12] = 4;
            TempMap[19, 1, 13] = 4;
            TempMap[19, 1, 14] = 4;
            TempMap[19, 1, 15] = 4;
            TempMap[19, 1, 16] = 4;
            TempMap[19, 1, 17] = 4;
            TempMap[19, 1, 18] = 0;
            TempMap[19, 1, 19] = 0;
            #endregion

            #region Third Layer
            //Col 0
            TempMap[0, 2, 0] = 0;
            TempMap[0, 2, 1] = 4;
            TempMap[0, 2, 2] = 4;
            TempMap[0, 2, 3] = 4;
            TempMap[0, 2, 4] = 4;
            TempMap[0, 2, 5] = 4;
            TempMap[0, 2, 6] = 4;
            TempMap[0, 2, 7] = 0;
            TempMap[0, 2, 8] = 0;
            TempMap[0, 2, 9] = 0;
            TempMap[0, 2, 10] = 0;
            TempMap[0, 2, 11] = 0;
            TempMap[0, 2, 12] = 0;
            TempMap[0, 2, 13] = 4;
            TempMap[0, 2, 14] = 4;
            TempMap[0, 2, 15] = 4;
            TempMap[0, 2, 16] = 4;
            TempMap[0, 2, 17] = 4;
            TempMap[0, 2, 18] = 0;
            TempMap[0, 2, 19] = 0;

            //Col 1
            TempMap[1, 2, 0] = 0;
            TempMap[1, 2, 1] = 4;
            TempMap[1, 2, 2] = 0;
            TempMap[1, 2, 3] = 0;
            TempMap[1, 2, 4] = 0;
            TempMap[1, 2, 5] = 0;
            TempMap[1, 2, 6] = 4;
            TempMap[1, 2, 7] = 4;
            TempMap[1, 2, 8] = 0;
            TempMap[1, 2, 9] = 0;
            TempMap[1, 2, 10] = 0;
            TempMap[1, 2, 11] = 0;
            TempMap[1, 2, 12] = 0;
            TempMap[1, 2, 13] = 4;
            TempMap[1, 2, 14] = 0;
            TempMap[1, 2, 15] = 0;
            TempMap[1, 2, 16] = 0;
            TempMap[1, 2, 17] = 4;
            TempMap[1, 2, 18] = 0;
            TempMap[1, 2, 19] = 0;

            //Col 2
            TempMap[2, 2, 0] = 0;
            TempMap[2, 2, 1] = 4;
            TempMap[2, 2, 2] = 0;
            TempMap[2, 2, 3] = 0;
            TempMap[2, 2, 4] = 0;
            TempMap[2, 2, 5] = 0;
            TempMap[2, 2, 6] = 0;
            TempMap[2, 2, 7] = 4;
            TempMap[2, 2, 8] = 4;
            TempMap[2, 2, 9] = 4;
            TempMap[2, 2, 10] = 4;
            TempMap[2, 2, 11] = 4;
            TempMap[2, 2, 12] = 4;
            TempMap[2, 2, 13] = 4;
            TempMap[2, 2, 14] = 0;
            TempMap[2, 2, 15] = 0;
            TempMap[2, 2, 16] = 0;
            TempMap[2, 2, 17] = 4;
            TempMap[2, 2, 18] = 0;
            TempMap[2, 2, 19] = 0;

            //Col 3
            TempMap[3, 2, 0] = 0;
            TempMap[3, 2, 1] = 4;
            TempMap[3, 2, 2] = 0;
            TempMap[3, 2, 3] = 0;
            TempMap[3, 2, 4] = 0;
            TempMap[3, 2, 5] = 0;
            TempMap[3, 2, 6] = 0;
            TempMap[3, 2, 7] = 4;
            TempMap[3, 2, 8] = 0;
            TempMap[3, 2, 9] = 0;
            TempMap[3, 2, 10] = 0;
            TempMap[3, 2, 11] = 0;
            TempMap[3, 2, 12] = 0;
            TempMap[3, 2, 13] = 4;
            TempMap[3, 2, 14] = 0;
            TempMap[3, 2, 15] = 0;
            TempMap[3, 2, 16] = 0;
            TempMap[3, 2, 17] = 4;
            TempMap[3, 2, 18] = 0;
            TempMap[3, 2, 19] = 0;

            //Col 4
            TempMap[4, 2, 0] = 0;
            TempMap[4, 2, 1] = 4;
            TempMap[4, 2, 2] = 0;
            TempMap[4, 2, 3] = 0;
            TempMap[4, 2, 4] = 0;
            TempMap[4, 2, 5] = 0;
            TempMap[4, 2, 6] = 0;
            TempMap[4, 2, 7] = 0;
            TempMap[4, 2, 8] = 0;
            TempMap[4, 2, 9] = 0;
            TempMap[4, 2, 10] = 0;
            TempMap[4, 2, 11] = 0;
            TempMap[4, 2, 12] = 0;
            TempMap[4, 2, 13] = 0;
            TempMap[4, 2, 14] = 0;
            TempMap[4, 2, 15] = 0;
            TempMap[4, 2, 16] = 0;
            TempMap[4, 2, 17] = 4;
            TempMap[4, 2, 18] = 4;
            TempMap[4, 2, 19] = 4;

            //Col 5
            TempMap[5, 2, 0] = 4;
            TempMap[5, 2, 1] = 4;
            TempMap[5, 2, 2] = 4;
            TempMap[5, 2, 3] = 4;
            TempMap[5, 2, 4] = 4;
            TempMap[5, 2, 5] = 0;
            TempMap[5, 2, 6] = 0;
            TempMap[5, 2, 7] = 0;
            TempMap[5, 2, 8] = 0;
            TempMap[5, 2, 9] = 0;
            TempMap[5, 2, 10] = 0;
            TempMap[5, 2, 11] = 0;
            TempMap[5, 2, 12] = 0;
            TempMap[5, 2, 13] = 4;
            TempMap[5, 2, 14] = 0;
            TempMap[5, 2, 15] = 0;
            TempMap[5, 2, 16] = 0;
            TempMap[5, 2, 17] = 0;
            TempMap[5, 2, 18] = 0;
            TempMap[5, 2, 19] = 4;

            //Col 6
            TempMap[6, 2, 0] = 4;
            TempMap[6, 2, 1] = 4;
            TempMap[6, 2, 2] = 4;
            TempMap[6, 2, 3] = 4;
            TempMap[6, 2, 4] = 4;
            TempMap[6, 2, 5] = 0;
            TempMap[6, 2, 6] = 0;
            TempMap[6, 2, 7] = 0;
            TempMap[6, 2, 8] = 0;
            TempMap[6, 2, 9] = 0;
            TempMap[6, 2, 10] = 0;
            TempMap[6, 2, 11] = 0;
            TempMap[6, 2, 12] = 0;
            TempMap[6, 2, 13] = 4;
            TempMap[6, 2, 14] = 0;
            TempMap[6, 2, 15] = 0;
            TempMap[6, 2, 16] = 0;
            TempMap[6, 2, 17] = 0;
            TempMap[6, 2, 18] = 0;
            TempMap[6, 2, 19] = 4;

            //Col 7
            TempMap[7, 2, 0] = 4;
            TempMap[7, 2, 1] = 0;
            TempMap[7, 2, 2] = 0;
            TempMap[7, 2, 3] = 0;
            TempMap[7, 2, 4] = 4;
            TempMap[7, 2, 5] = 0;
            TempMap[7, 2, 6] = 0;
            TempMap[7, 2, 7] = 4;
            TempMap[7, 2, 8] = 0;
            TempMap[7, 2, 9] = 0;
            TempMap[7, 2, 10] = 0;
            TempMap[7, 2, 11] = 0;
            TempMap[7, 2, 12] = 0;
            TempMap[7, 2, 13] = 4;
            TempMap[7, 2, 14] = 0;
            TempMap[7, 2, 15] = 0;
            TempMap[7, 2, 16] = 0;
            TempMap[7, 2, 17] = 0;
            TempMap[7, 2, 18] = 0;
            TempMap[7, 2, 19] = 4;

            //Col 8
            TempMap[8, 2, 0] = 4;
            TempMap[8, 2, 1] = 0;
            TempMap[8, 2, 2] = 0;
            TempMap[8, 2, 3] = 0;
            TempMap[8, 2, 4] = 0;
            TempMap[8, 2, 5] = 0;
            TempMap[8, 2, 6] = 0;
            TempMap[8, 2, 7] = 4;
            TempMap[8, 2, 8] = 4;
            TempMap[8, 2, 9] = 4;
            TempMap[8, 2, 10] = 4;
            TempMap[8, 2, 11] = 4;
            TempMap[8, 2, 12] = 4;
            TempMap[8, 2, 13] = 4;
            TempMap[8, 2, 14] = 0;
            TempMap[8, 2, 15] = 0;
            TempMap[8, 2, 16] = 0;
            TempMap[8, 2, 17] = 0;
            TempMap[8, 2, 18] = 0;
            TempMap[8, 2, 19] = 4;

            //Col 9
            TempMap[9, 2, 0] = 4;
            TempMap[9, 2, 1] = 0;
            TempMap[9, 2, 2] = 0;
            TempMap[9, 2, 3] = 0;
            TempMap[9, 2, 4] = 4;
            TempMap[9, 2, 5] = 0;
            TempMap[9, 2, 6] = 0;
            TempMap[9, 2, 7] = 0;
            TempMap[9, 2, 8] = 0;
            TempMap[9, 2, 9] = 0;
            TempMap[9, 2, 10] = 0;
            TempMap[9, 2, 11] = 0;
            TempMap[9, 2, 12] = 0;
            TempMap[9, 2, 13] = 0;
            TempMap[9, 2, 14] = 0;
            TempMap[9, 2, 15] = 0;
            TempMap[9, 2, 16] = 0;
            TempMap[9, 2, 17] = 0;
            TempMap[9, 2, 18] = 0;
            TempMap[9, 2, 19] = 4;

            //Col 10
            TempMap[10, 2, 0] = 4;
            TempMap[10, 2, 1] = 0;
            TempMap[10, 2, 2] = 0;
            TempMap[10, 2, 3] = 0;
            TempMap[10, 2, 4] = 4;
            TempMap[10, 2, 5] = 0;
            TempMap[10, 2, 6] = 0;
            TempMap[10, 2, 7] = 0;
            TempMap[10, 2, 8] = 0;
            TempMap[10, 2, 9] = 0;
            TempMap[10, 2, 10] = 0;
            TempMap[10, 2, 11] = 0;
            TempMap[10, 2, 12] = 0;
            TempMap[10, 2, 13] = 0;
            TempMap[10, 2, 14] = 0;
            TempMap[10, 2, 15] = 0;
            TempMap[10, 2, 16] = 0;
            TempMap[10, 2, 17] = 0;
            TempMap[10, 2, 18] = 0;
            TempMap[10, 2, 19] = 4;

            //Col 11
            TempMap[11, 2, 0] = 4;
            TempMap[11, 2, 1] = 0;
            TempMap[11, 2, 2] = 0;
            TempMap[11, 2, 3] = 0;
            TempMap[11, 2, 4] = 4;
            TempMap[11, 2, 5] = 0;
            TempMap[11, 2, 6] = 0;
            TempMap[11, 2, 7] = 0;
            TempMap[11, 2, 8] = 0;
            TempMap[11, 2, 9] = 0;
            TempMap[11, 2, 10] = 0;
            TempMap[11, 2, 11] = 0;
            TempMap[11, 2, 12] = 0;
            TempMap[11, 2, 13] = 0;
            TempMap[11, 2, 14] = 0;
            TempMap[11, 2, 15] = 0;
            TempMap[11, 2, 16] = 0;
            TempMap[11, 2, 17] = 0;
            TempMap[11, 2, 18] = 0;
            TempMap[11, 2, 19] = 4;

            //Col 12
            TempMap[12, 2, 0] = 4;
            TempMap[12, 2, 1] = 4;
            TempMap[12, 2, 2] = 4;
            TempMap[12, 2, 3] = 4;
            TempMap[12, 2, 4] = 4;
            TempMap[12, 2, 5] = 0;
            TempMap[12, 2, 6] = 0;
            TempMap[12, 2, 7] = 0;
            TempMap[12, 2, 8] = 0;
            TempMap[12, 2, 9] = 0;
            TempMap[12, 2, 10] = 0;
            TempMap[12, 2, 11] = 0;
            TempMap[12, 2, 12] = 0;
            TempMap[12, 2, 13] = 0;
            TempMap[12, 2, 14] = 0;
            TempMap[12, 2, 15] = 0;
            TempMap[12, 2, 16] = 0;
            TempMap[12, 2, 17] = 0;
            TempMap[12, 2, 18] = 0;
            TempMap[12, 2, 19] = 4;

            //Col 13
            TempMap[13, 2, 0] = 0;
            TempMap[13, 2, 1] = 4;
            TempMap[13, 2, 2] = 0;
            TempMap[13, 2, 3] = 0;
            TempMap[13, 2, 4] = 0;
            TempMap[13, 2, 5] = 0;
            TempMap[13, 2, 6] = 0;
            TempMap[13, 2, 7] = 0;
            TempMap[13, 2, 8] = 4;
            TempMap[13, 2, 9] = 4;
            TempMap[13, 2, 10] = 4;
            TempMap[13, 2, 11] = 4;
            TempMap[13, 2, 12] = 0;
            TempMap[13, 2, 13] = 0;
            TempMap[13, 2, 14] = 0;
            TempMap[13, 2, 15] = 0;
            TempMap[13, 2, 16] = 0;
            TempMap[13, 2, 17] = 0;
            TempMap[13, 2, 18] = 0;
            TempMap[13, 2, 19] = 4;

            //Col 14
            TempMap[14, 2, 0] = 0;
            TempMap[14, 2, 1] = 4;
            TempMap[14, 2, 2] = 0;
            TempMap[14, 2, 3] = 0;
            TempMap[14, 2, 4] = 0;
            TempMap[14, 2, 5] = 0;
            TempMap[14, 2, 6] = 0;
            TempMap[14, 2, 7] = 0;
            TempMap[14, 2, 8] = 0;
            TempMap[14, 2, 9] = 4;
            TempMap[14, 2, 10] = 4;
            TempMap[14, 2, 11] = 0;
            TempMap[14, 2, 12] = 0;
            TempMap[14, 2, 13] = 0;
            TempMap[14, 2, 14] = 0;
            TempMap[14, 2, 15] = 0;
            TempMap[14, 2, 16] = 0;
            TempMap[14, 2, 17] = 0;
            TempMap[14, 2, 18] = 0;
            TempMap[14, 2, 19] = 4;

            //Col 15
            TempMap[15, 2, 0] = 0;
            TempMap[15, 2, 1] = 4;
            TempMap[15, 2, 2] = 0;
            TempMap[15, 2, 3] = 0;
            TempMap[15, 2, 4] = 0;
            TempMap[15, 2, 5] = 0;
            TempMap[15, 2, 6] = 0;
            TempMap[15, 2, 7] = 0;
            TempMap[15, 2, 8] = 0;
            TempMap[15, 2, 9] = 0;
            TempMap[15, 2, 10] = 0;
            TempMap[15, 2, 11] = 0;
            TempMap[15, 2, 12] = 0;
            TempMap[15, 2, 13] = 0;
            TempMap[15, 2, 14] = 0;
            TempMap[15, 2, 15] = 0;
            TempMap[15, 2, 16] = 0;
            TempMap[15, 2, 17] = 4;
            TempMap[15, 2, 18] = 4;
            TempMap[15, 2, 19] = 4;

            //Col 16
            TempMap[16, 2, 0] = 0;
            TempMap[16, 2, 1] = 4;
            TempMap[16, 2, 2] = 4;
            TempMap[16, 2, 3] = 0;
            TempMap[16, 2, 4] = 0;
            TempMap[16, 2, 5] = 0;
            TempMap[16, 2, 6] = 0;
            TempMap[16, 2, 7] = 0;
            TempMap[16, 2, 8] = 0;
            TempMap[16, 2, 9] = 0;
            TempMap[16, 2, 10] = 0;
            TempMap[16, 2, 11] = 0;
            TempMap[16, 2, 12] = 0;
            TempMap[16, 2, 13] = 0;
            TempMap[16, 2, 14] = 0;
            TempMap[16, 2, 15] = 0;
            TempMap[16, 2, 16] = 0;
            TempMap[16, 2, 17] = 4;
            TempMap[16, 2, 18] = 0;
            TempMap[16, 2, 19] = 0;

            //Col 17
            TempMap[17, 2, 0] = 0;
            TempMap[17, 2, 1] = 0;
            TempMap[17, 2, 2] = 4;
            TempMap[17, 2, 3] = 0;
            TempMap[17, 2, 4] = 0;
            TempMap[17, 2, 5] = 0;
            TempMap[17, 2, 6] = 0;
            TempMap[17, 2, 7] = 4;
            TempMap[17, 2, 8] = 4;
            TempMap[17, 2, 9] = 4;
            TempMap[17, 2, 10] = 4;
            TempMap[17, 2, 11] = 4;
            TempMap[17, 2, 12] = 4;
            TempMap[17, 2, 13] = 0;
            TempMap[17, 2, 14] = 0;
            TempMap[17, 2, 15] = 0;
            TempMap[17, 2, 16] = 0;
            TempMap[17, 2, 17] = 4;
            TempMap[17, 2, 18] = 0;
            TempMap[17, 2, 19] = 0;

            //Col 18
            TempMap[18, 2, 0] = 0;
            TempMap[18, 2, 1] = 0;
            TempMap[18, 2, 2] = 4;
            TempMap[18, 2, 3] = 4;
            TempMap[18, 2, 4] = 0;
            TempMap[18, 2, 5] = 0;
            TempMap[18, 2, 6] = 0;
            TempMap[18, 2, 7] = 4;
            TempMap[18, 2, 8] = 0;
            TempMap[18, 2, 9] = 0;
            TempMap[18, 2, 10] = 0;
            TempMap[18, 2, 11] = 0;
            TempMap[18, 2, 12] = 4;
            TempMap[18, 2, 13] = 0;
            TempMap[18, 2, 14] = 0;
            TempMap[18, 2, 15] = 0;
            TempMap[18, 2, 16] = 0;
            TempMap[18, 2, 17] = 4;
            TempMap[18, 2, 18] = 0;
            TempMap[18, 2, 19] = 0;

            //Col 19
            TempMap[19, 2, 0] = 0;
            TempMap[19, 2, 1] = 0;
            TempMap[19, 2, 2] = 0;
            TempMap[19, 2, 3] = 4;
            TempMap[19, 2, 4] = 4;
            TempMap[19, 2, 5] = 4;
            TempMap[19, 2, 6] = 4;
            TempMap[19, 2, 7] = 4;
            TempMap[19, 2, 8] = 0;
            TempMap[19, 2, 9] = 0;
            TempMap[19, 2, 10] = 0;
            TempMap[19, 2, 11] = 0;
            TempMap[19, 2, 12] = 4;
            TempMap[19, 2, 13] = 4;
            TempMap[19, 2, 14] = 4;
            TempMap[19, 2, 15] = 4;
            TempMap[19, 2, 16] = 4;
            TempMap[19, 2, 17] = 4;
            TempMap[19, 2, 18] = 0;
            TempMap[19, 2, 19] = 0;
            #endregion

            #region Top Layer
            //Col 0
            TempMap[0, 3, 0] = 0;
            TempMap[0, 3, 1] = 1;
            TempMap[0, 3, 2] = 1;
            TempMap[0, 3, 3] = 1;
            TempMap[0, 3, 4] = 1;
            TempMap[0, 3, 5] = 1;
            TempMap[0, 3, 6] = 1;
            TempMap[0, 3, 7] = 0;
            TempMap[0, 3, 8] = 0;
            TempMap[0, 3, 9] = 0;
            TempMap[0, 3, 10] = 0;
            TempMap[0, 3, 11] = 0;
            TempMap[0, 3, 12] = 0;
            TempMap[0, 3, 13] = 1;
            TempMap[0, 3, 14] = 1;
            TempMap[0, 3, 15] = 1;
            TempMap[0, 3, 16] = 1;
            TempMap[0, 3, 17] = 1;
            TempMap[0, 3, 18] = 0;
            TempMap[0, 3, 19] = 0;

            //Col 1
            TempMap[1, 3, 0] = 0;
            TempMap[1, 3, 1] = 1;
            TempMap[1, 3, 2] = 1;
            TempMap[1, 3, 3] = 1;
            TempMap[1, 3, 4] = 1;
            TempMap[1, 3, 5] = 1;
            TempMap[1, 3, 6] = 1;
            TempMap[1, 3, 7] = 1;
            TempMap[1, 3, 8] = 0;
            TempMap[1, 3, 9] = 0;
            TempMap[1, 3, 10] = 0;
            TempMap[1, 3, 11] = 0;
            TempMap[1, 3, 12] = 0;
            TempMap[1, 3, 13] = 1;
            TempMap[1, 3, 14] = 1;
            TempMap[1, 3, 15] = 1;
            TempMap[1, 3, 16] = 1;
            TempMap[1, 3, 17] = 1;
            TempMap[1, 3, 18] = 0;
            TempMap[1, 3, 19] = 0;

            //Col 2
            TempMap[2, 3, 0] = 0;
            TempMap[2, 3, 1] = 1;
            TempMap[2, 3, 2] = 1;
            TempMap[2, 3, 3] = 1;
            TempMap[2, 3, 4] = 1;
            TempMap[2, 3, 5] = 1;
            TempMap[2, 3, 6] = 1;
            TempMap[2, 3, 7] = 1;
            TempMap[2, 3, 8] = 1;
            TempMap[2, 3, 9] = 1;
            TempMap[2, 3, 10] = 1;
            TempMap[2, 3, 11] = 1;
            TempMap[2, 3, 12] = 1;
            TempMap[2, 3, 13] = 1;
            TempMap[2, 3, 14] = 1;
            TempMap[2, 3, 15] = 1;
            TempMap[2, 3, 16] = 1;
            TempMap[2, 3, 17] = 1;
            TempMap[2, 3, 18] = 0;
            TempMap[2, 3, 19] = 0;

            //Col 3
            TempMap[3, 3, 0] = 0;
            TempMap[3, 3, 1] = 1;
            TempMap[3, 3, 2] = 1;
            TempMap[3, 3, 3] = 1;
            TempMap[3, 3, 4] = 1;
            TempMap[3, 3, 5] = 1;
            TempMap[3, 3, 6] = 1;
            TempMap[3, 3, 7] = 1;
            TempMap[3, 3, 8] = 1;
            TempMap[3, 3, 9] = 1;
            TempMap[3, 3, 10] = 1;
            TempMap[3, 3, 11] = 1;
            TempMap[3, 3, 12] = 1;
            TempMap[3, 3, 13] = 1;
            TempMap[3, 3, 14] = 1;
            TempMap[3, 3, 15] = 1;
            TempMap[3, 3, 16] = 1;
            TempMap[3, 3, 17] = 1;
            TempMap[3, 3, 18] = 0;
            TempMap[3, 3, 19] = 0;

            //Col 4
            TempMap[4, 3, 0] = 0;
            TempMap[4, 3, 1] = 1;
            TempMap[4, 3, 2] = 1;
            TempMap[4, 3, 3] = 1;
            TempMap[4, 3, 4] = 1;
            TempMap[4, 3, 5] = 1;
            TempMap[4, 3, 6] = 1;
            TempMap[4, 3, 7] = 1;
            TempMap[4, 3, 8] = 1;
            TempMap[4, 3, 9] = 1;
            TempMap[4, 3, 10] = 1;
            TempMap[4, 3, 11] = 1;
            TempMap[4, 3, 12] = 1;
            TempMap[4, 3, 13] = 1;
            TempMap[4, 3, 14] = 1;
            TempMap[4, 3, 15] = 1;
            TempMap[4, 3, 16] = 1;
            TempMap[4, 3, 17] = 1;
            TempMap[4, 3, 18] = 1;
            TempMap[4, 3, 19] = 1;

            //Col 5
            TempMap[5, 3, 0] = 1;
            TempMap[5, 3, 1] = 1;
            TempMap[5, 3, 2] = 1;
            TempMap[5, 3, 3] = 1;
            TempMap[5, 3, 4] = 1;
            TempMap[5, 3, 5] = 1;
            TempMap[5, 3, 6] = 1;
            TempMap[5, 3, 7] = 1;
            TempMap[5, 3, 8] = 1;
            TempMap[5, 3, 9] = 1;
            TempMap[5, 3, 10] = 1;
            TempMap[5, 3, 11] = 1;
            TempMap[5, 3, 12] = 1;
            TempMap[5, 3, 13] = 1;
            TempMap[5, 3, 14] = 1;
            TempMap[5, 3, 15] = 1;
            TempMap[5, 3, 16] = 1;
            TempMap[5, 3, 17] = 1;
            TempMap[5, 3, 18] = 1;
            TempMap[5, 3, 19] = 1;

            //Col 6
            TempMap[6, 3, 0] = 1;
            TempMap[6, 3, 1] = 1;
            TempMap[6, 3, 2] = 1;
            TempMap[6, 3, 3] = 1;
            TempMap[6, 3, 4] = 1;
            TempMap[6, 3, 5] = 1;
            TempMap[6, 3, 6] = 1;
            TempMap[6, 3, 7] = 1;
            TempMap[6, 3, 8] = 1;
            TempMap[6, 3, 9] = 1;
            TempMap[6, 3, 10] = 1;
            TempMap[6, 3, 11] = 1;
            TempMap[6, 3, 12] = 1;
            TempMap[6, 3, 13] = 1;
            TempMap[6, 3, 14] = 1;
            TempMap[6, 3, 15] = 1;
            TempMap[6, 3, 16] = 1;
            TempMap[6, 3, 17] = 1;
            TempMap[6, 3, 18] = 1;
            TempMap[6, 3, 19] = 1;

            //Col 7
            TempMap[7, 3, 0] = 1;
            TempMap[7, 3, 1] = 1;
            TempMap[7, 3, 2] = 1;
            TempMap[7, 3, 3] = 1;
            TempMap[7, 3, 4] = 1;
            TempMap[7, 3, 5] = 1;
            TempMap[7, 3, 6] = 1;
            TempMap[7, 3, 7] = 1;
            TempMap[7, 3, 8] = 1;
            TempMap[7, 3, 9] = 1;
            TempMap[7, 3, 10] = 1;
            TempMap[7, 3, 11] = 1;
            TempMap[7, 3, 12] = 1;
            TempMap[7, 3, 13] = 1;
            TempMap[7, 3, 14] = 1;
            TempMap[7, 3, 15] = 1;
            TempMap[7, 3, 16] = 1;
            TempMap[7, 3, 17] = 1;
            TempMap[7, 3, 18] = 1;
            TempMap[7, 3, 19] = 1;

            //Col 8
            TempMap[8, 3, 0] = 1;
            TempMap[8, 3, 1] = 1;
            TempMap[8, 3, 2] = 1;
            TempMap[8, 3, 3] = 1;
            TempMap[8, 3, 4] = 1;
            TempMap[8, 3, 5] = 1;
            TempMap[8, 3, 6] = 1;
            TempMap[8, 3, 7] = 1;
            TempMap[8, 3, 8] = 1;
            TempMap[8, 3, 9] = 1;
            TempMap[8, 3, 10] = 1;
            TempMap[8, 3, 11] = 1;
            TempMap[8, 3, 12] = 1;
            TempMap[8, 3, 13] = 1;
            TempMap[8, 3, 14] = 1;
            TempMap[8, 3, 15] = 1;
            TempMap[8, 3, 16] = 1;
            TempMap[8, 3, 17] = 1;
            TempMap[8, 3, 18] = 1;
            TempMap[8, 3, 19] = 1;

            //Col 9
            TempMap[9, 3, 0] = 1;
            TempMap[9, 3, 1] = 1;
            TempMap[9, 3, 2] = 1;
            TempMap[9, 3, 3] = 1;
            TempMap[9, 3, 4] = 1;
            TempMap[9, 3, 5] = 1;
            TempMap[9, 3, 6] = 1;
            TempMap[9, 3, 7] = 1;
            TempMap[9, 3, 8] = 1;
            TempMap[9, 3, 9] = 1;
            TempMap[9, 3, 10] = 1;
            TempMap[9, 3, 11] = 1;
            TempMap[9, 3, 12] = 1;
            TempMap[9, 3, 13] = 1;
            TempMap[9, 3, 14] = 1;
            TempMap[9, 3, 15] = 1;
            TempMap[9, 3, 16] = 1;
            TempMap[9, 3, 17] = 1;
            TempMap[9, 3, 18] = 1;
            TempMap[9, 3, 19] = 1;

            //Col 10
            TempMap[10, 3, 0] = 1;
            TempMap[10, 3, 1] = 1;
            TempMap[10, 3, 2] = 1;
            TempMap[10, 3, 3] = 1;
            TempMap[10, 3, 4] = 1;
            TempMap[10, 3, 5] = 1;
            TempMap[10, 3, 6] = 1;
            TempMap[10, 3, 7] = 1;
            TempMap[10, 3, 8] = 1;
            TempMap[10, 3, 9] = 1;
            TempMap[10, 3, 10] = 1;
            TempMap[10, 3, 11] = 1;
            TempMap[10, 3, 12] = 1;
            TempMap[10, 3, 13] = 1;
            TempMap[10, 3, 14] = 1;
            TempMap[10, 3, 15] = 1;
            TempMap[10, 3, 16] = 1;
            TempMap[10, 3, 17] = 1;
            TempMap[10, 3, 18] = 1;
            TempMap[10, 3, 19] = 1;

            //Col 11
            TempMap[11, 3, 0] = 1;
            TempMap[11, 3, 1] = 1;
            TempMap[11, 3, 2] = 1;
            TempMap[11, 3, 3] = 1;
            TempMap[11, 3, 4] = 1;
            TempMap[11, 3, 5] = 1;
            TempMap[11, 3, 6] = 1;
            TempMap[11, 3, 7] = 1;
            TempMap[11, 3, 8] = 1;
            TempMap[11, 3, 9] = 1;
            TempMap[11, 3, 10] = 1;
            TempMap[11, 3, 11] = 1;
            TempMap[11, 3, 12] = 1;
            TempMap[11, 3, 13] = 1;
            TempMap[11, 3, 14] = 1;
            TempMap[11, 3, 15] = 1;
            TempMap[11, 3, 16] = 1;
            TempMap[11, 3, 17] = 1;
            TempMap[11, 3, 18] = 1;
            TempMap[11, 3, 19] = 1;

            //Col 12
            TempMap[12, 3, 0] = 0;
            TempMap[12, 3, 1] = 1;
            TempMap[12, 3, 2] = 1;
            TempMap[12, 3, 3] = 1;
            TempMap[12, 3, 4] = 1;
            TempMap[12, 3, 5] = 1;
            TempMap[12, 3, 6] = 1;
            TempMap[12, 3, 7] = 1;
            TempMap[12, 3, 8] = 1;
            TempMap[12, 3, 9] = 1;
            TempMap[12, 3, 10] = 1;
            TempMap[12, 3, 11] = 1;
            TempMap[12, 3, 12] = 1;
            TempMap[12, 3, 13] = 1;
            TempMap[12, 3, 14] = 1;
            TempMap[12, 3, 15] = 1;
            TempMap[12, 3, 16] = 1;
            TempMap[12, 3, 17] = 1;
            TempMap[12, 3, 18] = 1;
            TempMap[12, 3, 19] = 1;

            //Col 13
            TempMap[13, 3, 0] = 0;
            TempMap[13, 3, 1] = 1;
            TempMap[13, 3, 2] = 1;
            TempMap[13, 3, 3] = 1;
            TempMap[13, 3, 4] = 1;
            TempMap[13, 3, 5] = 1;
            TempMap[13, 3, 6] = 1;
            TempMap[13, 3, 7] = 1;
            TempMap[13, 3, 8] = 1;
            TempMap[13, 3, 9] = 1;
            TempMap[13, 3, 10] = 1;
            TempMap[13, 3, 11] = 1;
            TempMap[13, 3, 12] = 1;
            TempMap[13, 3, 13] = 1;
            TempMap[13, 3, 14] = 1;
            TempMap[13, 3, 15] = 1;
            TempMap[13, 3, 16] = 1;
            TempMap[13, 3, 17] = 1;
            TempMap[13, 3, 18] = 1;
            TempMap[13, 3, 19] = 1;

            //Col 14
            TempMap[14, 3, 0] = 0;
            TempMap[14, 3, 1] = 1;
            TempMap[14, 3, 2] = 1;
            TempMap[14, 3, 3] = 1;
            TempMap[14, 3, 4] = 1;
            TempMap[14, 3, 5] = 1;
            TempMap[14, 3, 6] = 1;
            TempMap[14, 3, 7] = 1;
            TempMap[14, 3, 8] = 1;
            TempMap[14, 3, 9] = 1;
            TempMap[14, 3, 10] = 1;
            TempMap[14, 3, 11] = 1;
            TempMap[14, 3, 12] = 1;
            TempMap[14, 3, 13] = 1;
            TempMap[14, 3, 14] = 1;
            TempMap[14, 3, 15] = 1;
            TempMap[14, 3, 16] = 1;
            TempMap[14, 3, 17] = 1;
            TempMap[14, 3, 18] = 1;
            TempMap[14, 3, 19] = 1;

            //Col 15
            TempMap[15, 3, 0] = 0;
            TempMap[15, 3, 1] = 1;
            TempMap[15, 3, 2] = 1;
            TempMap[15, 3, 3] = 1;
            TempMap[15, 3, 4] = 1;
            TempMap[15, 3, 5] = 1;
            TempMap[15, 3, 6] = 1;
            TempMap[15, 3, 7] = 1;
            TempMap[15, 3, 8] = 1;
            TempMap[15, 3, 9] = 1;
            TempMap[15, 3, 10] = 1;
            TempMap[15, 3, 11] = 1;
            TempMap[15, 3, 12] = 1;
            TempMap[15, 3, 13] = 1;
            TempMap[15, 3, 14] = 1;
            TempMap[15, 3, 15] = 1;
            TempMap[15, 3, 16] = 1;
            TempMap[15, 3, 17] = 1;
            TempMap[15, 3, 18] = 0;
            TempMap[15, 3, 19] = 0;

            //Col 16
            TempMap[16, 3, 0] = 0;
            TempMap[16, 3, 1] = 0;
            TempMap[16, 3, 2] = 1;
            TempMap[16, 3, 3] = 1;
            TempMap[16, 3, 4] = 1;
            TempMap[16, 3, 5] = 1;
            TempMap[16, 3, 6] = 1;
            TempMap[16, 3, 7] = 1;
            TempMap[16, 3, 8] = 1;
            TempMap[16, 3, 9] = 1;
            TempMap[16, 3, 10] = 1;
            TempMap[16, 3, 11] = 1;
            TempMap[16, 3, 12] = 1;
            TempMap[16, 3, 13] = 1;
            TempMap[16, 3, 14] = 1;
            TempMap[16, 3, 15] = 1;
            TempMap[16, 3, 16] = 1;
            TempMap[16, 3, 17] = 1;
            TempMap[16, 3, 18] = 0;
            TempMap[16, 3, 19] = 0;

            //Col 17
            TempMap[17, 3, 0] = 0;
            TempMap[17, 3, 1] = 0;
            TempMap[17, 3, 2] = 1;
            TempMap[17, 3, 3] = 1;
            TempMap[17, 3, 4] = 1;
            TempMap[17, 3, 5] = 1;
            TempMap[17, 3, 6] = 1;
            TempMap[17, 3, 7] = 1;
            TempMap[17, 3, 8] = 1;
            TempMap[17, 3, 9] = 1;
            TempMap[17, 3, 10] = 1;
            TempMap[17, 3, 11] = 1;
            TempMap[17, 3, 12] = 1;
            TempMap[17, 3, 13] = 1;
            TempMap[17, 3, 14] = 1;
            TempMap[17, 3, 15] = 1;
            TempMap[17, 3, 16] = 1;
            TempMap[17, 3, 17] = 1;
            TempMap[17, 3, 18] = 0;
            TempMap[17, 3, 19] = 0;

            //Col 18
            TempMap[18, 3, 0] = 0;
            TempMap[18, 3, 1] = 0;
            TempMap[18, 3, 2] = 1;
            TempMap[18, 3, 3] = 1;
            TempMap[18, 3, 4] = 1;
            TempMap[18, 3, 5] = 1;
            TempMap[18, 3, 6] = 1;
            TempMap[18, 3, 7] = 1;
            TempMap[18, 3, 8] = 0;
            TempMap[18, 3, 9] = 0;
            TempMap[18, 3, 10] = 0;
            TempMap[18, 3, 11] = 0;
            TempMap[18, 3, 12] = 1;
            TempMap[18, 3, 13] = 1;
            TempMap[18, 3, 14] = 1;
            TempMap[18, 3, 15] = 1;
            TempMap[18, 3, 16] = 1;
            TempMap[18, 3, 17] = 1;
            TempMap[18, 3, 18] = 0;
            TempMap[18, 3, 19] = 0;

            //Col 19
            TempMap[19, 3, 0] = 0;
            TempMap[19, 3, 1] = 0;
            TempMap[19, 3, 2] = 0;
            TempMap[19, 3, 3] = 1;
            TempMap[19, 3, 4] = 1;
            TempMap[19, 3, 5] = 1;
            TempMap[19, 3, 6] = 1;
            TempMap[19, 3, 7] = 1;
            TempMap[19, 3, 8] = 0;
            TempMap[19, 3, 9] = 0;
            TempMap[19, 3, 10] = 0;
            TempMap[19, 3, 11] = 0;
            TempMap[19, 3, 12] = 1;
            TempMap[19, 3, 13] = 1;
            TempMap[19, 3, 14] = 1;
            TempMap[19, 3, 15] = 1;
            TempMap[19, 3, 16] = 1;
            TempMap[19, 3, 17] = 1;
            TempMap[19, 3, 18] = 0;
            TempMap[19, 3, 19] = 0;
            #endregion

            return TempMap;
        }

        public static ushort[, ,] GetMapFromFile()
        {
            if (DefaultFiles.Singleton.TestMap == null || DefaultFiles.Singleton.TestMap.Length < 1)
                return new ushort[1, 1, 1];

            int xW = DefaultFiles.Singleton.TestMap[0].width;
            int yH = DefaultFiles.Singleton.TestMap.Length;
            int zL = DefaultFiles.Singleton.TestMap[0].height;
            ushort[, ,] loadedData = new ushort[xW, yH, zL];

            for (int y = 0; y < yH; y++)
            {
                Color32[] colors = DefaultFiles.Singleton.TestMap[y].GetPixels32();

                for (int x = 0; x < xW; x++)
                {
                    for (int z = 0; z < zL; z++)
                    {
                        loadedData[x, y, z] = GetBlockFromColor(colors, x, z, xW);
                    }
                }
            }

            return loadedData;
        }

        public static ushort GetBlockFromColor(Color32[] colors, int x, int z, int w)
        {
            int index = Helpers.IndexFromVec2Int(x, z, w);

            Color32 c = colors[index];

            //Transparent - Nothing
            if (c.a == 0)
            {
                return 0;
            }
            //Grey - Scaffold
            else if (c.r == 128 && c.g == 128 && c.b == 128)
            {
                return 1;
            }
            //Green - Unfinished Floor
            else if (c.r == 0 && c.g == 255 && c.b == 0)
            {
                return 2;
            }
            //White - Wall
            else if (c.r == 255 && c.g == 255 && c.b == 255)
            {
                return 3;
            }
            //Red - Normal Floor
            else if (c.r == 255 && c.g == 0 && c.b == 0)
            {
                return 4;
            }
            //Blue - DOOR
            else if (c.r == 0 && c.g == 0 && c.b == 255)
            {
                return 5;
            }

            return 0;
        }
    }
}