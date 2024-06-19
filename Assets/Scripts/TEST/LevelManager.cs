using CharactersStats;
using Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private List<TypeOfScenario> _roomsOrder;

    private IGameplayService _gameplayService;
    private IPlayerFactory _playerFactory;
    private IEnemyFactory _enemyFactory;
    private IStatsProvider _statsProvider;
    private IScenarioFactory _scenarioFactory;
    private TypeOfScenario _nextRoom;
    private Scene _currentRoomScene;

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
        _gameplayService.LevelManager = this;
    }

    public void Start()
    {
        _nextRoom = GetNextRoom();
        LoadRoomScene(_nextRoom);
    }

    public TypeOfScenario GetNextRoom()
    {
        if (_gameplayService.RoomsOrder.Count > 0)
        {
            TypeOfScenario firstRoom = _gameplayService.RoomsOrder.Dequeue();
            return firstRoom;
        }
        else
        {
            Debug.LogError("No rooms in the sequence to start the level.");
            return TypeOfScenario.DefaultRoom;
        }
    }

    public void LoadNextRoom()
    {
        _nextRoom = GetNextRoom();
        LoadRoomScene(_nextRoom);
    }

    public void LoadRoomScene(TypeOfScenario type)
    {
        if (_currentRoomScene.IsValid())
        {
            SceneManager.UnloadSceneAsync(_currentRoomScene);
        }

        SceneManager.LoadScene("Room", LoadSceneMode.Additive);

        SceneManager.sceneLoaded += OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Room")
            {
                string baseSceneName = type.ToString();
                string newSceneName = baseSceneName;
                int sceneNumber = 1;

                while (SceneManager.GetSceneByName(newSceneName).IsValid())
                {
                    sceneNumber++;
                    newSceneName = baseSceneName + sceneNumber;
                }

                Scene newScene = SceneManager.CreateScene(newSceneName);

                foreach (GameObject obj in scene.GetRootGameObjects())
                {
                    SceneManager.MoveGameObjectToScene(GameObject.Instantiate(obj), newScene);
                }

                SceneManager.UnloadSceneAsync("Room");

                SceneManager.SetActiveScene(newScene);

                _currentRoomScene = newScene;

                GameObject roomConfig = newScene.GetRootGameObjects().FirstOrDefault();
                if (roomConfig != null)
                {
                    RoomStarter roomStarter = roomConfig.GetComponent<RoomStarter>();
                    if (roomStarter != null)
                    {
                        roomStarter.Init(_gameplayService);
                        roomStarter.StartRoom(type);
                    }
                }

                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
    }
}
