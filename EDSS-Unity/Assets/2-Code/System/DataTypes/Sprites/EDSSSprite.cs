//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// EDSSSprite - Sprite data useable for Every Day Space Station
// Created: December 5 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 5 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

namespace EveryDaySpaceStation.DataTypes
{
    [System.Serializable]
    public class EDSSSprite : System.IDisposable
    {
        public uint UID { get; private set; }
        public string Name { get; private set; }
        public Vec2Int TopLeft { get; private set; }
        public Vec2Int WidthHeight { get; private set; }
        public EDSSSpriteSheet SpriteSheet { get; private set; }
        public List<string> Flags { get; private set; }

        private Vector4 uvCoords = Vector4.zero;
        public Vector4 GetUVCoords()
        {
            if (uvCoords == Vector4.zero)
            {
                float xPos = (float)TopLeft.x / (float)SpriteSheet.Texture.width;
                float yPos = (float)(TopLeft.y + WidthHeight.y) / (float)SpriteSheet.Texture.height;
                float width = (float)WidthHeight.x / (float)SpriteSheet.Texture.width;
                float height = (float)WidthHeight.y / (float)SpriteSheet.Texture.height;

                uvCoords = new Vector4(xPos, yPos, width, height);
            }

            return uvCoords;
        }

        public void CreateSprite(uint uid, Vec2Int topLeft, Vec2Int widthHeight, EDSSSpriteSheet spriteSheet, string name = "")
        {
            UID = uid;
            Name = name;
            TopLeft = topLeft;
            WidthHeight = widthHeight;
            SpriteSheet = spriteSheet;
        }

        #region Dispose
        ///////////
        //IDisposable Overrides
        protected bool _isDisposed = false;

        public virtual void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //Dispose here
                    SpriteSheet = null;
                }
            }
        }

        ~EDSSSprite()
        {
            Dispose(false);
        }
        #endregion
    }
}