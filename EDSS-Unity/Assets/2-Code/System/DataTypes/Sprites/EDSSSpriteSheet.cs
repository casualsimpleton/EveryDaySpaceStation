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
    public class EDSSSpriteSheet : System.IDisposable
    {
        public Texture2D ArtFile { get; private set; }
        public List<EDSSSprite> Sprites { get; private set; }

        public void CreateSpriteSheet(Texture2D art, List<EDSSSprite> existingSprites = null)
        {
            ArtFile = art;

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

        public EDSSSprite CreateSprite(Vec2Int pos, Vec2Int widthHeight)
        {
            EDSSSprite newSprite = new EDSSSprite();
            newSprite.CreateSprite(pos, widthHeight, this);

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
                        Sprites[i].Dispose();
                    }

                    Sprites.Clear();
                    Sprites = null;

                    GameObject.Destroy(ArtFile);
                    ArtFile = null;
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