using Enemy;
using Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;
using Prefab;

namespace Player
{
    public class Starter : MonoBehaviour
    {
        private IGameplayService _gameplayService;
        private IEnemyController _enemyController = new EnemyController();

        private IPlayerFactory _playerFactory;
        [SerializeField]
        private DefaultScenarioContext _scenarioContext;

        [SerializeField]
        private Transform _playerTransform;
        [SerializeField]
        private Transform _enemyTransform;

        [Inject]
        public void Construct(IGameplayService service, IPlayerFactory factory)
        {
            _playerFactory = factory;
            _gameplayService = service;
        }

        public void Start()
        {
            //_playerFactory.CreatePlayer(_playerTransform, PlayerType.Warrior, new PlayerModel());
            //_gameplayService.PlayerSpawnPoints.Add(_playerTransform);

            _enemyController.Init();
            _scenarioContext.Enemies.Add(_enemyController);
            _gameplayService.Init(TypeOfScenario.Default,_scenarioContext);
        }
    }
}

