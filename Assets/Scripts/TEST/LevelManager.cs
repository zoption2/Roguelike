using Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;
using Zenject;

public interface ILevelManager
{
    public Transform PlayerParent { get; }
    public Queue<TypeOfScenario> RoomsOrder { get; set; }
    public RoomTemplateSO.Template GetTemplate();
    public void LoadLevel();
    public void LoadMenu();
    public void SwitchToNextRoom();
    public void BuildNextRoom();
    
}

public class LevelManager : ILevelManager
{
    public Queue<TypeOfScenario> RoomsOrder { get; set; }
    public IPoolManager PoolManager { get; private set; }

    private List<RoomTemplateSO.Template> _usedTemplates = new List<RoomTemplateSO.Template>();
    private RoomTemplateSO _roomTemplate;
    private RoomTemplateSO.Template _template;
    private Transform _globalPoolParent;
    private RoomBuilder _roomBuilder;
    private Transform _roomsParent;
    private Transform _playerParent;
    private GameObject _currentRoom;
    private LevelSetingsSO _levelSettings;
    private IGameplayService _gameplayService;
    private ILevelContext _levelContext;

    public Transform PlayerParent
    {
        get
        {
            if (_playerParent == null)
            {
                GameObject roomsObject = GameObject.Find("Player");
                if (roomsObject == null)
                {
                    roomsObject = new GameObject("Player");
                }
                _playerParent = roomsObject.transform;
            }
            return _playerParent;
        }
    }

    public Transform RoomsParent
    {
        get
        {
            if (_roomsParent == null)
            {
                GameObject roomsObject = GameObject.Find("Rooms");
                if (roomsObject == null)
                {
                    roomsObject = new GameObject("Rooms");
                }
                _roomsParent = roomsObject.transform;
            }
            return _roomsParent;
        }
    }

    [Inject]
    public void Construct(
        RoomTemplateSO roomTemplateSO,
        LevelSetingsSO levelSetingsSO,
        IPoolManager poolManager,
        IGameplayService gameplayService,
        INavigationFactory navigationFactory,
        IRoomObjectsFactory roomObjectsFactory,
        ILevelContext levelContext,
        IChestFactory chestFactory
    )
    {
        _roomTemplate = roomTemplateSO;
        _levelSettings = levelSetingsSO;
        PoolManager = poolManager;
        _gameplayService = gameplayService;
        _levelContext = levelContext;

        _roomBuilder = new RoomBuilder(
            navigationFactory,
            roomObjectsFactory,
            chestFactory
        );
    }

    public void LoadLevel()
    {

        RoomsOrder = new Queue<TypeOfScenario>(_levelSettings.RoomsOrder);

        SceneManager.LoadScene("Level", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnLevelSceneLoaded;

        void OnLevelSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Level")
            {
                if (SceneManager.GetSceneByName("Menu").IsValid())
                {
                    SceneManager.UnloadSceneAsync("Menu");
                }

                SceneManager.MoveGameObjectToScene(RoomsParent.gameObject, scene);
                SceneManager.MoveGameObjectToScene(PlayerParent.gameObject, scene);

                CreateInitialRoom(RoomsParent);

                SceneManager.sceneLoaded -= OnLevelSceneLoaded;
            }
        }
    }

    private void CreateInitialRoom(Transform parent)
    {
        if (RoomsOrder.Count > 0)
        {
            var typeOfScenario = RoomsOrder.Dequeue();
            var template = SetTemplate(typeOfScenario);

            if (template != null)
            {
                string roomName = template.name;

                RoomContext roomContext = new RoomContext();

                _roomBuilder.RoomContext = roomContext;

                GameObject roomObject = _roomBuilder.BuildRoom(template, parent);
                _currentRoom = roomObject;

                roomObject.transform.position = Vector3.zero;

                roomObject.SetActive(true);
                _levelContext.CurrentRoomContext = roomContext;
                _levelContext.CurrentRoomName = roomName;
                _levelContext.CurrentRoomType = typeOfScenario;

                var exits = roomContext.CompleatedRoomTriggers;
                foreach (var exit in exits)
                {
                    exit.Init(_gameplayService, this);
                }

                _gameplayService.StartCurrentRoom();
            }
        }
    }

    public void LoadMenu()
    {
        _usedTemplates.Clear();
        _levelContext.CurrentRoomScenario = null;
        _levelContext.CurrentRoomContext = null;

        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnLevelSceneLoaded;

        void OnLevelSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Menu")
            {
                if (SceneManager.GetSceneByName("Level").IsValid())
                {
                    SceneManager.UnloadSceneAsync("Level");
                }
                SceneManager.sceneLoaded -= OnLevelSceneLoaded;
            }
        }
    }

    public void SwitchToNextRoom()
    {
        if (RoomsOrder.Count > 0)
        {
            var nextRoomScenario = RoomsOrder.Dequeue();
            var template = SetTemplate(nextRoomScenario);

            if (template != null)
            {
                string roomName = template.name;
                RoomContext roomContext = new RoomContext();

                _levelContext.CurrentRoomContext = roomContext;
                _levelContext.CurrentRoomName = roomName;
                _levelContext.CurrentRoomType = nextRoomScenario;
                _gameplayService.StartCurrentRoom();
            }
        }
        else
        {
            Debug.Log("No more rooms to switch to.");
        }
    }

    public void BuildNextRoom()
    {
        GameObject.Destroy(_currentRoom);

        if (RoomsOrder.Count > 0)
        {
            var nextRoomScenario = RoomsOrder.Dequeue();
            var template = SetTemplate(nextRoomScenario);

            if (template != null)
            {
                string roomName = template.name;

                RoomContext roomContext = new RoomContext();

                _roomBuilder.RoomContext = roomContext;

                GameObject roomObject = _roomBuilder.BuildRoom(template, RoomsParent);
                _currentRoom = roomObject;
                roomObject.transform.position = Vector3.zero;
                roomObject.SetActive(true);

                _levelContext.CurrentRoomContext = roomContext;
                Debug.LogWarning(_levelContext.CurrentRoomContext);
                _levelContext.CurrentRoomName = roomName;
                _levelContext.CurrentRoomType = nextRoomScenario;

                if (_levelContext.Player != null)
                { 
                    if (_levelContext.CurrentRoomContext.Players.Count == 0)
                    {
                        _levelContext.CurrentRoomContext.Players.Add(_levelContext.Player);
                    }
                    
                    _levelContext.Player.SetCharacterContext(_levelContext.CurrentRoomContext);
                }
                else
                {
                    Debug.LogError("Player is null in LevelContext");
                }

                var exits = roomContext.CompleatedRoomTriggers;
                foreach (var exit in exits)
                {
                    exit.Init(_gameplayService, this);
                }

                _gameplayService.StartCurrentRoom();
            }
        }
        else
        {
            Debug.Log("No more rooms to build.");
            _levelContext.CurrentRoomScenario.LoadMainMenu();
        }
    }

    public RoomTemplateSO.Template GetTemplate()
    {
        return _template;
    }

    private RoomTemplateSO.Template SetTemplate(TypeOfScenario type)
    {
        var templatesOfType = _roomTemplate.Templates.Where(t => t.ScenarioType == type && !_usedTemplates.Contains(t)).ToList();

        if (templatesOfType.Count == 0)
        {
            Debug.LogError($"No templates available for the scenario type: {type}");
            return null;
        }

        int randomIndex = Random.Range(0, templatesOfType.Count);
        var selectedTemplate = templatesOfType[randomIndex];

        _usedTemplates.Add(selectedTemplate);

        Debug.Log($"Selected template: {selectedTemplate.name} for scenario type: {type}");

        return selectedTemplate;
    }
}
