//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// EDSSSpriteSheet - Sprite sheet containing artwork and logic for EDSSSprites
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
    public class EDSSSpriteSheet : System.IDisposable
    {
        public uint UID { get; private set; }
        public Texture2D Texture { get; private set; }
        public List<EDSSSprite> Sprites { get; private set; }
        public Material Material { get; private set; }

        public void CreateSpriteSheet(uint uid, Texture2D art, Material mat, List<EDSSSprite> existingSprites = null)
        {
            UID = uid;

            Texture = art;

            Material = mat;

            Sprites = existingSprites;
        }

        public void AddSprite(EDSSSprite sprite)
        {
            Sprites.Add(sprite);
        }

        public void RemoveSprite(EDSSSprite sprite)
        {
            Sprites.Remove(sprite);
        }

        public EDSSSprite CreateSprite(uint uid, Vec2Int pos, Vec2Int widthHeight, string spriteName = "")
        {
            EDSSSprite newSprite = new EDSSSprite();
            newSprite.CreateSprite(uid, pos, widthHeight, this, spriteName);

            if (Sprites == null)
            {
                Sprites = new List<EDSSSprite>();
            }

            Sprites.Add(newSprite);

            return newSprite;
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
                    for (int i = 0; i < Sprites.Count; i++)
                    {
                        if (Sprites[i] != null)
                        {
                            Sprites[i].Dispose();
                        }
                    }

                    Sprites.Clear();
                    Sprites = null;

                    GameObject.Destroy(Texture);
                    Texture = null;
                }
            }
        }

        ~EDSSSpriteSheet()
        {
            Dispose(false);
        }
        #endregion
    }
}