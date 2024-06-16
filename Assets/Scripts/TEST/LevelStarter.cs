using CharactersStats;
using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelStarter : MonoBehaviour
{
    [SerializeField]
    private List<TypeOfScenario> _roomsOrder;
    private Camera _mainCamera;

    private IGameplayService _gameplayService;
    private IPlayerFactory _playerFactory;
    private IEnemyFactory _enemyFactory;    
    private IStatsProvider _statsProvider;
    private IScenarioFactory _scenarioFactory;

    [Inject]
    public void Construct(
        IStatsProvider statsProvider,
        IScenarioFactory scenarioFactory,
        IPlayerFactory playerFactory,
        IEnemyFactory enemyFactory
        )
    {
        _statsProvider = statsProvider;
        _scenarioFactory = scenarioFactory;
        _playerFactory = playerFactory;
        _enemyFactory = enemyFactory;
        
    }

    private void Awake()
    {
        _gameplayService = new GameplayService(
            _statsProvider,
            _scenarioFactory,
            _playerFactory,
            _enemyFactory,
            _roomsOrder
            );
    }

    public void Start()
    {
        _mainCamera = Camera.main;
        DontDestroyOnLoad(_mainCamera.gameObject);

        _gameplayService.InitRoom();
    }
}
