//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// PoolManager - A Singleton'd Unity Monobehaviour based class for coordinating all pool stuff
// Should start disabled as component
// Created: December 7 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 7 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class PoolManager : MonoBehaviour
{
    #region Singleton
    protected static PoolManager m_singleton = null;
    public static PoolManager Singleton
    {
        get
        {
            return m_singleton;
        }
    }

    void Awake()
    {
        m_singleton = this;
        _transform = this.transform;
    }
    #endregion

    #region Vars
    public Transform _transform { get; private set; }

    public bool IsInit { get; private set; }

    protected PoolSceneChunkRenderer _sceneChunkRendererPool;
    protected PoolCubeCollider _cubeCollider1;
    protected PoolCubeCollider _cubeCollider2;
    protected PoolCubeCollider _cubeCollider3;
    protected PoolCubeCollider _cubeCollider4;
    protected PoolCubeCollider _cubeCollider5;
    protected PoolCubeCollider _cubeCollider6;
    protected PoolCubeCollider _cubeCollider7;
    protected PoolCubeCollider _cubeCollider8;
    protected PoolCubeCollider _cubeCollider9;
    protected PoolCubeCollider _cubeCollider10;
    #endregion

    #region Scene Chunk Renderers
    public SceneChunkRenderer RequestSceneChunkRenderer()
    {
        return _sceneChunkRendererPool.RequestObject();
    }

    public void ReturnSceneChunkRenderer(SceneChunkRenderer scr)
    {
        _sceneChunkRendererPool.ReturnObject(scr);
    }
    #endregion

    #region Cube Colliders
    public CubeCollider RequestCubeCollider(int length)
    {
        switch (length)
        {
            default:
            case 1:
                return _cubeCollider1.RequestObject();
            case 2:
                return _cubeCollider2.RequestObject();
            case 3:
                return _cubeCollider3.RequestObject();
            case 4:
                return _cubeCollider4.RequestObject();
            case 5:
                return _cubeCollider5.RequestObject();
            case 6:
                return _cubeCollider6.RequestObject();
            case 7:
                return _cubeCollider7.RequestObject();
            case 8:
                return _cubeCollider8.RequestObject();
            case 9:
                return _cubeCollider9.RequestObject();
            case 10:
                return _cubeCollider10.RequestObject();
        }
    }

    public void ReturnCubeCollider(CubeCollider cc)
    {
        switch (cc.BoxSize.y)
        {
            default:
            case 1:
                _cubeCollider1.ReturnObject(cc);
                break;
            case 2:
                _cubeCollider2.ReturnObject(cc);
                break;
            case 3:
                _cubeCollider3.ReturnObject(cc);
                break;
            case 4:
                _cubeCollider4.ReturnObject(cc);
                break;
            case 5:
                _cubeCollider5.ReturnObject(cc);
                break;
            case 6:
                _cubeCollider6.ReturnObject(cc);
                break;
            case 7:
                _cubeCollider7.ReturnObject(cc);
                break;
            case 8:
                _cubeCollider8.ReturnObject(cc);
                break;
            case 9:
                _cubeCollider9.ReturnObject(cc);
                break;
            case 10:
                _cubeCollider10.ReturnObject(cc);
                break;
        }
    }
    #endregion

    public void EarlyInit()
    {
    }

    public void Init()
    {
        _cubeCollider1 = new PoolCubeCollider();
        _cubeCollider2 = new PoolCubeCollider();
        _cubeCollider3 = new PoolCubeCollider();
        _cubeCollider4 = new PoolCubeCollider();
        _cubeCollider5 = new PoolCubeCollider();
        _cubeCollider6 = new PoolCubeCollider();
        _cubeCollider7 = new PoolCubeCollider();
        _cubeCollider8 = new PoolCubeCollider();
        _cubeCollider9 = new PoolCubeCollider();
        _cubeCollider10 = new PoolCubeCollider();

        _cubeCollider1.Init(10, new Vec2Int(1, 1), 0.1f, 2, 0.1f);
        _cubeCollider2.Init(10, new Vec2Int(1, 2), 0.1f, 2, 0.1f);
        _cubeCollider3.Init(10, new Vec2Int(1, 3), 0.1f, 2, 0.1f);
        _cubeCollider4.Init(10, new Vec2Int(1, 4), 0.1f, 2, 0.1f);
        _cubeCollider5.Init(10, new Vec2Int(1, 5), 0.1f, 2, 0.1f);
        _cubeCollider6.Init(10, new Vec2Int(1, 6), 0.1f, 2, 0.1f);
        _cubeCollider7.Init(10, new Vec2Int(1, 7), 0.1f, 2, 0.1f);
        _cubeCollider8.Init(10, new Vec2Int(1, 8), 0.1f, 2, 0.1f);
        _cubeCollider9.Init(10, new Vec2Int(1, 9), 0.1f, 2, 0.1f);
        _cubeCollider10.Init(10, new Vec2Int(1, 10), 0.1f, 2, 0.1f);
    }

    public void LateInit()
    {
        _sceneChunkRendererPool = new PoolSceneChunkRenderer();

        _sceneChunkRendererPool.Init(4, 0.1f, 5, 0.25f);

        IsInit = true;

        //This component should start off, otherwise Update() could be called before Init() and that'd be less than good
        this.enabled = true;
    }

    public void Update()
    {
        _sceneChunkRendererPool.Maintenance();
    }

    public void Cleanup()
    {
        try
        {
            _sceneChunkRendererPool.Dispose();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning(string.Format("Problem with PoolManager.CleanUP() : '{0}'", ex.Message.ToString()));
        }
    }
}