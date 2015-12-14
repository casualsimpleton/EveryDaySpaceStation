//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// EntityBuildManager - Unity Monobehaviour that sits on a gameobject and helps to turn EntityDatas in EnityDatas with visual 
// representations and colliders and any other stuff needed for rendering but not underlying gameplay
// Created: December 13 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 13 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class EntityBuildManager : MonoBehaviour
{
    #region Singleton
    protected static EntityBuildManager m_singleton = null;
    public static EntityBuildManager Singleton
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

    #region Classes/Structs
    public struct BuildConstructionQueueHelper
    {
        public bool IsBuildingQueued;
        public bool IsBuilt;
        public bool IsDeconstructQueued;
        public bool IsDeconstructed;
    }
    #endregion

    #region Vars
    Queue<MapData.EntityData> _entitiesAwaitingBuilding; //Entities waiting to be build up from reusable parts
    Queue<MapData.EntityData> _entitiesForDeconstruction; //Entities waiting to be broken down, and parts returned to pools where possible
    int _numOfEntitiesBuildUpPerFrame;
    int _numOfEntitiesDeconstructDownPerFrame;
    #endregion

    public void Init(int numOfEntitiesBuildUpPerFrame, int numOfEntitiesDeconstructDownPerFrame)
    {
        _entitiesAwaitingBuilding = new Queue<MapData.EntityData>();
        _entitiesForDeconstruction = new Queue<MapData.EntityData>();
        _numOfEntitiesBuildUpPerFrame = numOfEntitiesBuildUpPerFrame;
        _numOfEntitiesDeconstructDownPerFrame = numOfEntitiesDeconstructDownPerFrame;
    }

    public void AddEntityToBuildQueue(MapData.EntityData entity)
    {
        //If the entity is already built, don't queue it or it is queued to be destroyed
        if (entity.BuildState.IsBuilt || entity.BuildState.IsDeconstructQueued)
            return;

        _entitiesAwaitingBuilding.Enqueue(entity);
    }

    public void AddEntityToDeconstruction(MapData.EntityData entity)
    {
        //If it's already in the queue, don't add it again
        if (entity.BuildState.IsDeconstructQueued)
            return;

        _entitiesForDeconstruction.Enqueue(entity);
    }

    void Update()
    {
        int entsMade = 0;
        int entsDec = 0;
        while (_entitiesAwaitingBuilding.Count > 0 && entsMade < _numOfEntitiesBuildUpPerFrame)
        {
            entsMade++;
            MapData.EntityData entity = _entitiesAwaitingBuilding.Dequeue();

            BuildEntity(entity);
        }

        while (_entitiesForDeconstruction.Count > 0 && entsDec < _numOfEntitiesDeconstructDownPerFrame)
        {
            entsMade++;
            MapData.EntityData entity = _entitiesForDeconstruction.Dequeue();

            DeconstructEntity(entity);
        }
    }

    private void BuildEntity(MapData.EntityData entity)
    {
        //entity.Sprite;

        entity.BuildState.IsBuildingQueued = false;
        entity.BuildState.IsBuilt = true;
    }

    private void DeconstructEntity(MapData.EntityData entity)
    {
    }
}