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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using EveryDaySpaceStation;
using EveryDaySpaceStation.Utils;

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
    public Material defaultMaterial;
    public Shader defaultShader;
}