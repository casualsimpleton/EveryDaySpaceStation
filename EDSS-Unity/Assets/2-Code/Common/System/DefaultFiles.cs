//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// DefaultFiles - A single resource for all default files. This can sit in a scene and be easily accessible via singleton
// Created: December 4 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 4 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

//using EveryDaySpaceStation;
//using EveryDaySpaceStation.Utils;

public class DefaultFiles : MonoBehaviour
{
    //#region Singleton
    //protected static DefaultFiles m_singleton = null;
    //public static DefaultFiles Singleton
    //{
    //    get
    //    {
    //        return m_singleton;
    //    }
    //}

    //void Awake()
    //{
    //    m_singleton = this;
    //}
    //#endregion

    static public Texture2D defaultTexture
    {
        get
        {
            return (Resources.Load("Defaults/Textures/defaulttexture") as Texture2D);
        }
    }

    static public Texture2D colorTint
    {
        get
        {
            return (Resources.Load("Defaults/Textures/colortint") as Texture2D);
        }
    }

    static public Texture2D crosshairTexture
    {
        get
        {
            return (Resources.Load("Defaults/Textures/crosshair") as Texture2D);
        }
    }

    static public Material defaultMaterial
    {
        get
        {
            return (Resources.Load("Defaults/Materials/defaultmaterial") as Material);
        }
    }

    static public Material crosshairMaterial
    {
        get
        {
            return (Resources.Load("Defaults/Materials/crosshairmaterial") as Material);
        }
    }

    static public Shader defaultShader
    {
        get
        {
            return (Resources.Load("Defaults/Shaders/ZWriteTransparency") as Shader);
        }
    }

    static public Shader billboardShader
    {
        get
        {
            return (Resources.Load("Defaults/Shaders/SpriteBillboard") as Shader);
        }
    }

    static public Shader twoSidedSpriteShader
    {
        get
        {
            return (Resources.Load("Defaults/Shaders/Sprite2Sided") as Shader);
        }
    }

    static public Shader lineDrawingShader
    {
        get
        {
            return (Resources.Load("Defaults/Shaders/LineDrawing") as Shader);
        }
    }

    //static public Material[] testMaterials;

    //static public Texture2D[] TestMap;
}