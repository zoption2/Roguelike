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

        [SerializeField]
        private Transform[] _playerTransform;
        [SerializeField]
        private Transform[] _enemyTransform;

        [Inject]
        public void Construct(IGameplayService service)
        {
            _gameplayService = service;
        }

        public void Start()
        {
            foreach (var player in _playerTransform)
            {
                _gameplayService.PlayerSpawnPoints.Add(player);
            }

            foreach (var enemy in _enemyTransform)
            {
                _gameplayService.EnemySpawnPoints.Add(enemy);

                //_enemyController.Init();
                //_gameplayService.Enemies.Add(_enemyController);
                //_gameplayService.Init(TypeOfScenario.Default);
            }
            _gameplayService.Init(TypeOfScenario.Default);
        }
    }
}

