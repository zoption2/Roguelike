using Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public interface ILevelManager
{
    public Transform PlayerParent { get; }
    public Queue<TypeOfScenario> RoomsOrder { get; set; }
    public RoomTemplateSO.Template GetTemplate();
    public void LoadLevel();
    public void LoadMenu();
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

    public LevelManager(
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
        _gameplayService.SetLevelManager(this);

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

                _gameplayService.CurrentContext = roomContext;
                _gameplayService.CurrentRoomName = roomName;
                _gameplayService.CurrentRoomType = typeOfScenario;

                var exits = roomContext.CompleatedRoomTriggers;
                foreach (var exit in exits)
                {
                    exit.Init(_gameplayService, this);
                }

                OnNavigationCreate(RoomsParent, _levelContext);

                _gameplayService.StartCurrentRoom();
            }
        }
    }

    public void OnNavigationCreate(Transform parent, ILevelContext context)
    {
        GameObject navMeshObject = new GameObject("NavMeshSurface");
        navMeshObject.transform.SetParent(parent);
        navMeshObject.transform.localPosition = Vector3.zero;

        NavMeshSurface navMeshSurface = navMeshObject.AddComponent<NavMeshSurface>();
        navMeshSurface.AddData();
        context.NavMeshSurface = navMeshSurface;
        navMeshSurface.BuildNavMesh();
    }

    public void LoadMenu()
    {
        _usedTemplates.Clear();
        _gameplayService.CurrentScenario = null;
        _gameplayService.CurrentContext = null;

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

                _gameplayService.CurrentContext = roomContext;
                _gameplayService.CurrentRoomName = roomName;
                _gameplayService.CurrentRoomType = nextRoomScenario;

                if (_levelContext.Player != null)
                { 
                    if (_gameplayService.CurrentContext.Players.Count == 0)
                    {
                        _gameplayService.CurrentContext.Players.Add(_levelContext.Player);
                    }
                    
                    _levelContext.Player.SetCharacterContext(_gameplayService.CurrentContext);
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

                NavMeshData newData = new NavMeshData();
                

                _gameplayService.StartCurrentRoom();

                _levelContext.NavMeshSurface = null;

                _levelContext.NavMeshSurface.RemoveData();
                _levelContext.NavMeshSurface.BuildNavMesh();
            }
        }
        else
        {
            Debug.Log("No more rooms to build.");
            _gameplayService.CurrentScenario.LoadMainMenu();
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
