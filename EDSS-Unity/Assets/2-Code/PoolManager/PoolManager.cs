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
    protected PoolSceneChunkScaffoldRenderer _sceneChunkScaffoldRendererPool;
    protected PoolEntitySpriteGameObject _entitySpritesPool;
    protected PoolMeshQuad _meshQuadPool;
    protected PoolCubeCollider _cubeCollider;
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

    #region Scene Chunk Scaffold Renderers
    public SceneChunkScaffoldRenderer RequestSceneChunkScaffolderRenderer()
    {
        return _sceneChunkScaffoldRendererPool.RequestObject();
    }

    public void ReturnSceneChunkRenderer(SceneChunkScaffoldRenderer sscr)
    {
        _sceneChunkScaffoldRendererPool.ReturnObject(sscr);
    }
    #endregion

    #region Entity Sprites GOs
    public EntitySpriteGameObject RequestEntitySpriteGameObject()
    {
        return _entitySpritesPool.RequestObject();
    }

    public void ReturnEntitySpriteGameObject(EntitySpriteGameObject es)
    {
        _entitySpritesPool.ReturnObject(es);
    }
    #endregion

    #region Mesh Quads
    public MeshQuad RequestMeshQuad()
    {
        return _meshQuadPool.RequestObject();
    }

    public void ReturnMeshQuad(MeshQuad mq)
    {
        _meshQuadPool.ReturnObject(mq);
    }
    #endregion

    #region Cube Colliders
    public CubeCollider RequestCubeCollider()
    {
        return _cubeCollider.RequestObject();
    }

    public void ReturnCubeCollider(CubeCollider cc)
    {
        _cubeCollider.ReturnObject(cc);
    }
    #endregion

    public void EarlyInit()
    {
    }

    public void Init()
    {
        _cubeCollider = new PoolCubeCollider();
        _cubeCollider.Init(10, Vector3.one, 0.1f, 2, 0.1f);
        
        _entitySpritesPool = new PoolEntitySpriteGameObject();
        _entitySpritesPool.Init(1, 0.1f, 2, 0.1f);
    }

    public void LateInit()
    {
        _sceneChunkRendererPool = new PoolSceneChunkRenderer();
        _sceneChunkScaffoldRendererPool = new PoolSceneChunkScaffoldRenderer();
        _meshQuadPool = new PoolMeshQuad();

        _sceneChunkRendererPool.Init(4, 0.1f, 5, 0.25f);
        _sceneChunkScaffoldRendererPool.Init(4, 0.1f, 5, 0.25f);
        _meshQuadPool.Init(1, 0.1f, 5, 0.25f);

        IsInit = true;

        //This component should start off, otherwise Update() could be called before Init() and that'd be less than good
        this.enabled = true;
    }

    public void Update()
    {
        _sceneChunkRendererPool.Maintenance();
        _sceneChunkScaffoldRendererPool.Maintenance();
        _entitySpritesPool.Maintenance();
        _meshQuadPool.Maintenance();

        _cubeCollider.Maintenance();
    }

    public void Cleanup()
    {
        try
        {
            _sceneChunkRendererPool.Dispose();
            _sceneChunkScaffoldRendererPool.Dispose();
            _entitySpritesPool.Dispose();
            _meshQuadPool.Dispose();

            _cubeCollider.Dispose();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning(string.Format("Problem with PoolManager.CleanUP() : '{0}'", ex.Message.ToString()));
        }
    }
}