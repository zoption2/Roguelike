using Enemy;
using Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;
using Prefab;

namespace Gameplay
{
    public class Starter : MonoBehaviour
    {
        private IGameplayService _gameplayService;

        //[SerializeField]
        //private Transform[] _playerTransform;
        //[SerializeField]
        //private Transform[] _enemyTransform;

        [SerializeField] private MySceneContext _sceneContext;

        [Inject]
        public void Construct(IGameplayService service)
        {
            _gameplayService = service;
        }

        public void Start()
        {
            foreach (var player in _sceneContext.PlayersSpawnPoints)
            {
                _gameplayService.PlayerSpawnPoints.Add(player);
            }

            foreach (var enemy in _sceneContext.EnemiesSpawnPoints)
            {
                _gameplayService.EnemySpawnPoints.Add(enemy);
            }

            _gameplayService.Init(TypeOfScenario.Default);
            _gameplayService.Init(TypeOfScenario.Default, _sceneContext);
        }
    }
}

