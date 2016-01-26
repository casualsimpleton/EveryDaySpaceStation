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
    #region Singleton
    protected static DefaultFiles m_singleton = null;
    public static DefaultFiles Singleton
    {
        get
        {
            return m_singleton;
        }
    }

    void Awake()
    {
        m_singleton = this;
    }
    #endregion

    public Texture2D defaultTexture;
    public Texture2D colorTint;
    public Texture2D crosshairTexture;
    public Material defaultMaterial;
    public Material crosshairMaterial;
    public Shader defaultShader;
    public Shader billboardShader;
    public Shader twoSidedSpriteShader;
    public Shader lineDrawingShader;
}