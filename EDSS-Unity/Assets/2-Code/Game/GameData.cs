//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// GameData - Class for containing all the various data imported from the jsons pertaining to game data, like blocks and sprites
// Created: December 5 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 5 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public sealed class GameData
{
    #region Classes/Structs
    public class GameBlockData : System.IDisposable
    {
        public uint UID { get; private set; }
        public string Name { get; private set; }
        public int DefaultStrength { get; private set; }
        public List<string> Flags { get; private set; }
        /// <summary>
        /// The UID for the type of block that must be present in order for this block to be placed. Not required for mapping, 
        /// but will be used during run-time for dynamic building
        /// </summary>
        public uint RequirementUID { get; private set; }

        public GameBlockData(uint uid, string name, int defaultStrength, string[] flags, uint requirementUID)
        {
            UID = uid;
            Name = name;
            DefaultStrength = defaultStrength;
            Flags = new List<string>(flags);
            RequirementUID = requirementUID;
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
                    Flags.Clear();
                    Flags = null;
                }
            }
        }

        ~GameBlockData()
        {
            Dispose(false);
        }
        #endregion
    }
    #endregion

    #region Vars
    Dictionary<uint, EDSSSprite> _sprites;
    Dictionary<uint, EDSSSpriteSheet> _spriteSheets;
    Dictionary<uint, GameBlockData> _gameBlockData;
    Dictionary<string, Texture2D> _textures;

    private uint _spriteSheetUID = 1;
    #endregion

    #region Gets/Sets
    public uint GetNewSpriteSheetUID() { return _spriteSheetUID++; }
    #endregion

    #region Constructors
    public GameData()
    {
        _sprites = new Dictionary<uint, EDSSSprite>();
        _spriteSheets = new Dictionary<uint, EDSSSpriteSheet>();
        _gameBlockData = new Dictionary<uint, GameBlockData>();
        _textures = new Dictionary<string, Texture2D>();
    }
    #endregion

    public void AddSprite(uint uid, EDSSSprite sprite)
    {
        _sprites.Add(uid, sprite);
    }

    public void AddSpriteSheet(uint uid, EDSSSpriteSheet spriteSheet)
    {
        _spriteSheets.Add(uid, spriteSheet);
    }

    public void AddGameBlock(uint uid, GameBlockData blockData)
    {
        _gameBlockData.Add(uid, blockData);
    }

    public void AddTexture(string name, Texture2D texture)
    {
        _textures.Add(name, texture);
    }

    public bool GetSprite(uint uid, out EDSSSprite sprite)
    {
        bool exists = _sprites.TryGetValue(uid, out sprite);

        return exists;
    }

    public bool GetSpriteSheet(uint uid, out EDSSSpriteSheet spriteSheet)
    {
        bool exists = _spriteSheets.TryGetValue(uid, out spriteSheet);

        return exists;
    }

    /// <summary>
    /// Look for a EDSSSpriteSheet by texture name, since we might not have the UID yet. NOTE - Going to be slower that searching by UID
    /// </summary>
    public bool GetSpriteSheet(string name, out EDSSSpriteSheet spriteSheet)
    {
        spriteSheet = null;
        foreach (KeyValuePair<uint, EDSSSpriteSheet> sheet in _spriteSheets)
        {
            if (sheet.Value.Texture.name.CompareTo(name) == 0)
            {
                spriteSheet = sheet.Value;
                return true;
            }
        }

        return false;
    }

    public bool GetGameBlock(uint uid, out GameBlockData blockData)
    {
        bool exists = _gameBlockData.TryGetValue(uid, out blockData);

        return exists;
    }

    public bool GetTexture(string name, out Texture2D texture)
    {
        bool exists = _textures.TryGetValue(name, out texture);

        return exists;
    }

    public void Cleanup()
    {
        foreach (KeyValuePair<uint, EDSSSprite> sprite in _sprites)
        {
            sprite.Value.Dispose();
        }

        foreach (KeyValuePair<uint, EDSSSpriteSheet> sheet in _spriteSheets)
        {
            sheet.Value.Dispose();
        }

        foreach (KeyValuePair<uint, GameBlockData> block in _gameBlockData)
        {
            block.Value.Dispose();
        }

        foreach (KeyValuePair<string, Texture2D> texture in _textures)
        {
            GameObject.Destroy(texture.Value);
        }

        _sprites.Clear();
        _spriteSheets.Clear();
        _gameBlockData.Clear();
        _textures.Clear();

        _sprites = null;
        _spriteSheets = null;
        _gameBlockData = null;
        _textures = null;
    }
}