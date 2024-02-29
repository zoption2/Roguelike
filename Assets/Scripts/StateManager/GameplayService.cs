using Enemy;
using CharactersStats;
using Player;
using Gameplay;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;

namespace Gameplay
{
    public enum TypeOfScenario
    {
        Default,
        Boss
    }
    public interface IGameplayService
    {
        public List<IPlayerController> Players { get; }
        public List<IEnemyController> Enemies { get; }
        public List<Transform> PlayerSpawnPoints { get; }
        public List<Transform> EnemySpawnPoints { get; }
        //void Init(TypeOfScenario type);
        void Init(TypeOfScenario type, ISceneContextMarker sceneContext);

        public IPlayerFactory _playerFactory { get; }
        public IEnemyFactory _enemyFactory { get; }
        public IStatsProvider _statsProvider { get; }
    }
    public class GameplayService : IGameplayService
    {
        public List<IPlayerController> Players { get; }
        public List<IEnemyController> Enemies { get; }
        public List<Transform> PlayerSpawnPoints { get; }
        public List<Transform> EnemySpawnPoints { get; }

        [Inject]
        public IPlayerFactory _playerFactory { get; }

        [Inject]
        public IEnemyFactory _enemyFactory { get; }

        [Inject]
        public IStatsProvider _statsProvider { get; }

        public BaseScenario<MySceneContext> ScenarioType;

        public GameplayService()
        {
            Players = new List<IPlayerController>();
            Enemies = new List<IEnemyController>();
            PlayerSpawnPoints = new List<Transform>();
            EnemySpawnPoints = new List<Transform>();
        }


        //public void Init(TypeOfScenario type)
        //{
        //    ScenarioType = ScenarioFactory.GetScenario(type, this);
        //    ScenarioType.Init();
        //    //ScenarioType._fullService = this;
        //}

        public void Init(TypeOfScenario type, ISceneContextMarker sceneContext)
        {
            ScenarioType = ScenarioFactory.GetScenario(type, this);
            ScenarioType.Init(sceneContext);
            //ScenarioType._fullService = this;
        }
    }

    public class ScenarioFactory
    {
        public static BaseScenario<MySceneContext> GetScenario(TypeOfScenario type, IGameplayService fullService)
        {
            BaseScenario<MySceneContext> scenario = null;
            switch (type)
            {
                case TypeOfScenario.Default:
                    scenario = new TestScenario();
                    //Resolve
                    break;
                //case TypeOfScenario.Boss:
                //    scenario = new BossScenario(fullService);
                //    break;
            }
            return scenario;
        }
    }

    //public interface ISceneContextMarker
    //{

    //}

    //public interface IStateHandler
    //{
    //    void CompleteState();
    //}

    //public interface IForest : IStateHandler
    //{
    //    void GrowTree(Action callback = null);
    //}

    //public class DefaultScenarioContext : ISceneContextMarker, IForest
    //{
    //    [field: SerializeField] public List<IPlayerController> Players { get; private set; }
    //    [field: SerializeField] public List<IEnemyController> Enemies { get; private set; }
    //    [field: SerializeField] public List<System.Object> Decors { get; private set; }

    //    public void CompleteState()
    //    {

    //    }

    //    public void GrowTree(Action callback = null)
    //    {

    //    }
    //}

    //public abstract class BaseScenario<T> where T : ISceneContextMarker
    //{
    //    protected T _sceneContext;
    //    public void SetSceneContext(ISceneContextMarker context)
    //    {
    //        _sceneContext = (T)context;

    //    }

    //    public void CompleteCurrentState()
    //    {

    //    }
    //}

    //public class PersistScenario : BaseScenario<DefaultScenarioContext>
    //{
    //    void DoSomething()
    //    {
    //        _sceneContext.Decors.Clear();
    //    }
    //}

    //public class ForestState
    //{
    //    public void Init(ISceneContextMarker context)
    //    {
    //        var tempContext = (IForest)context;
    //        tempContext.GrowTree();
    //        tempContext.CompleteState();
    //    }
    //}
}
