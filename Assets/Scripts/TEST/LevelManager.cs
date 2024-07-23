using Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;
using Zenject;

public interface ILevelManager
{
    void LoadLevel();
    void LoadMenu();
    RoomTemplateSO.Template GetTemplate();
    Queue<TypeOfScenario> RoomsOrder { get; set; }
    void SwitchToNextRoom();
    void BuildNextRoom();
}

public class LevelManager : ILevelManager
{
    private List<TypeOfScenario> _mainRoomOrder;

    public Queue<TypeOfScenario> RoomsOrder { get; set; }
    private RoomTemplateSO _roomTemplate;
    private RoomTemplateSO.Template _template;
    private IGameplayService _gameplayService;
    public IPoolManager PoolManager { get; private set; }

    private Transform _globalPoolParent;
    private ILevelContext _levelContext;
    private RoomBuilder _roomBuilder;
    private Transform _roomsParent;
    private GameObject _currentRoom;

    public Transform GlobalPoolParent
    {
        get
        {
            if (_globalPoolParent == null)
            {
                GameObject globalParentObject = GameObject.Find("GlobalPoolParent");
                if (globalParentObject == null)
                {
                    globalParentObject = new GameObject("GlobalPoolParent");
                }
                _globalPoolParent = globalParentObject.transform;
            }
            return _globalPoolParent;
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
        IPoolManager poolManager,
        IGameplayService gameplayService,
        INavigationFactory navigationFactory,
        IRoomObjectsFactory roomObjectsFactory,
        ILevelContext levelContext,
        IChestFactory chestFactory
    )
    {
        _roomTemplate = roomTemplateSO;
        PoolManager = poolManager;
        _gameplayService = gameplayService;
        _levelContext = levelContext;

        _roomBuilder = new RoomBuilder(
            navigationFactory,
            roomObjectsFactory,
            chestFactory
        );

        GameObject poolManagerObject = new GameObject("PoolManager");
        PoolManager.Init(poolManagerObject);
    }

    public void LoadLevel()
    {
        _mainRoomOrder = new List<TypeOfScenario>
        {
            TypeOfScenario.MainRoom,
            TypeOfScenario.DefaultRoom
        };

        RoomsOrder = new Queue<TypeOfScenario>(_mainRoomOrder);

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

                GameObject roomsObject = new GameObject("Rooms");
                SceneManager.MoveGameObjectToScene(roomsObject, scene);

                CreateInitialRoom(roomsObject.transform);

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
                _levelContext.CreateRoomContext(roomName);
                RoomContext roomContext = _levelContext.GetRoomContext(roomName);

                _roomBuilder.RoomContext = roomContext;

                GameObject roomObject = _roomBuilder.BuildRoom(template, parent);
                _currentRoom = roomObject;

                roomObject.transform.position = Vector3.zero;

                _roomBuilder.OnNavigationCreate(parent);

                roomObject.SetActive(true);
                _levelContext.CurrentRoomContext = roomContext;
                _levelContext.CurrentRoomName = roomName;
                _levelContext.CurrentRoomType = typeOfScenario;

                SetExitTypesAndDirections(roomContext, template);
                _gameplayService.StartCurrentRoom();
            }
        }
    }

    public void LoadMenu()
    {
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

    private void SetExitTypesAndDirections(RoomContext roomContext, RoomTemplateSO.Template template)
    {
        var exits = roomContext.CompleatedRoomTriggers;
        var exitTypes = new List<TemplateElementType>
        {
            TemplateElementType.ExitToStoryRoom,
            TemplateElementType.ExitToBountyRoom,
            TemplateElementType.ExitToRandomRoom
        };

        for (int i = 0; i < exits.Count; i++)
        {
            var exit = exits[i];
            exit.Init(_gameplayService, this);
            exit.SetExitType(exitTypes[i % exitTypes.Count]);

            Vector3 position = exit.Transform.position;
            float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, maxZ = float.MaxValue;

            var coordinates = template.Coordinates;

            for (int row = 0; row < coordinates.GetLength(0); row++)
            {
                for (int col = 0; col < coordinates.GetLength(1); col++)
                {
                    var coord = coordinates[row, col];
                    if (coord.x < minX) minX = coord.x;
                    if (coord.x > maxX) maxX = coord.x;
                    if (coord.z < minZ) minZ = coord.z;
                    if (coord.z > maxZ) maxZ = coord.z;
                }
            }

            if (position.x == minX)
            {
                exit.SetExitDirection(ExitDirection.Left);
            }
            else if (position.x == maxX)
            {
                exit.SetExitDirection(ExitDirection.Right);
            }
            else if (position.z == minZ)
            {
                exit.SetExitDirection(ExitDirection.Bottom);
            }
            else if (position.z == maxZ)
            {
                exit.SetExitDirection(ExitDirection.Top);
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
                RoomContext roomContext = _levelContext.GetRoomContext(roomName);

                if (roomContext != null)
                {
                    _levelContext.CurrentRoomContext = roomContext;
                    _levelContext.CurrentRoomName = roomName;
                    _levelContext.CurrentRoomType = nextRoomScenario;
                    _gameplayService.StartCurrentRoom();
                }
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
                _levelContext.CreateRoomContext(roomName);
                RoomContext roomContext = _levelContext.GetRoomContext(roomName);

                _roomBuilder.RoomContext = roomContext;

                GameObject roomObject = _roomBuilder.BuildRoom(template, _roomsParent);
                _currentRoom = roomObject;
                roomObject.transform.position = Vector3.zero;
                roomObject.SetActive(true);

                _levelContext.CurrentRoomContext = roomContext;
                _levelContext.CurrentRoomName = roomName;
                _levelContext.CurrentRoomType = nextRoomScenario;

                if (_levelContext.Player != null)
                {
                    _levelContext.CurrentRoomContext.Players.Add(_levelContext.Player);
                    _levelContext.Player.SetCharacterContext(_levelContext.CurrentRoomContext);
                }
                else
                {
                    Debug.LogError("Player is null in LevelContext");
                }

                SetExitTypesAndDirections(roomContext, template);
                _gameplayService.StartCurrentRoom();
            }
        }
        else
        {
            Debug.Log("No more rooms to build.");
        }
    }

    public RoomTemplateSO.Template GetTemplate()
    {
        return _template;
    }

    private RoomTemplateSO.Template SetTemplate(TypeOfScenario type)
    {
        var templatesOfType = _roomTemplate.Templates.Where(t => t.ScenarioType == type).ToList();

        if (templatesOfType.Count == 0)
        {
            Debug.LogError($"No templates available for the scenario type: {type}");
            return null;
        }

        int randomIndex = Random.Range(0, templatesOfType.Count);
        var selectedTemplate = templatesOfType[randomIndex];

        Debug.Log($"Selected template: {selectedTemplate.name} for scenario type: {type}");

        return selectedTemplate;
    }
}
