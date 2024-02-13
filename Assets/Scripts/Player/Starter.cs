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
        private IPlayerController _controller;
        private IGameplayService _gameplayService;
        private IEnemyController _enemyController = new EnemyController();
        [SerializeField]
        private Transform _playerTransform;
        [SerializeField]
        private Transform _enemyTransform;

        [Inject]
        public void Construct(IPlayerController controller, IGameplayService service)
        {
            _controller = controller;
            _controller.IsActive = true;
            _gameplayService = service;
        }

        public void Start()
        {
            _controller.Init(_playerTransform, PlayerType.Warrior);
            _enemyController.Init();
            _gameplayService.Players.Add(_controller);
            _gameplayService.Enemies.Add(_enemyController);
            _gameplayService.Init(TypeOfScenario.Default);
        }
    }
}

