using CharactersStats;
using Gameplay;
using Pool;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Zenject;

public interface ILevelManager
{
    void LoadLevel();
    void LoadMenu();
    RoomTemplateSO.Template GetTemplate();
    Queue<TypeOfScenario> RoomsOrder { get; set; }
    void SwitchToNextRoom();
    void BuildNextRooms();
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

                CreateMainRoom(roomsObject.transform);

                SceneManager.sceneLoaded -= OnLevelSceneLoaded;
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

    private void CreateRooms(Transform parent)
    {
        if (RoomsOrder.Count == 0) return;

        var typeOfScenario = RoomsOrder.Dequeue();
        var template = SetTemplate(typeOfScenario);
        if (template != null)
        {
            string roomName = template.name;
            _levelContext.CreateRoomContext(roomName);
            RoomContext roomContext = _levelContext.GetRoomContext(roomName);

            _roomBuilder.RoomContext = roomContext;

            GameObject roomObject = _roomBuilder.BuildRoom(template, parent);

            roomObject.transform.position = Vector3.zero;

            _roomBuilder.OnNavigationCreate(parent);

            roomObject.SetActive(true);
            _levelContext.CurrentRoomContext = roomContext;
            _levelContext.CurrentRoomName = roomName;
            _levelContext.CurrentRoomType = typeOfScenario;

            _gameplayService.StartCurrentRoom();
        }
    }


    public void BuildNextRooms()
    {
        var exits = _levelContext.CurrentRoomContext.CompleatedRoomTriggers.ToList();
        Debug.Log($"Total exits to process: {exits.Count}");
        Debug.Log($"RoomsParent: {RoomsParent.name}");

        foreach (var exit in exits)
        {
            TypeOfScenario nextRoomType = TypeOfScenario.DefaultRoom;
            Vector3 newPosition = Vector3.zero;
            Quaternion newRotation = Quaternion.identity;

            switch (exit.GetExitType())
            {
                case TemplateElementType.ExitToStoryRoom:
                    if (RoomsOrder.Count > 0)
                    {
                        nextRoomType = RoomsOrder.Dequeue();
                    }
                    break;
                case TemplateElementType.ExitToBountyRoom:
                    nextRoomType = TypeOfScenario.BountyRoom;
                    break;
                case TemplateElementType.ExitToRandomeRoom:
                    nextRoomType = TypeOfScenario.RandomeRoom;
                    break;
                default:
                    Debug.LogWarning($"Unhandled exit type: {exit.GetExitType()}");
                    break;
            }

            var template = SetTemplate(nextRoomType);
            if (template != null)
            {
                string nextRoomName = template.name;

                if (_levelContext.GetRoomContext(nextRoomName) == null)
                {
                    _levelContext.CreateRoomContext(nextRoomName);
                    RoomContext nextRoomContext = _levelContext.GetRoomContext(nextRoomName);

                    _roomBuilder.RoomContext = nextRoomContext;

                    switch (exit.GetExitDirection())
                    {
                        case ExitDirection.Top:
                            newPosition = exit.Transform.position + new Vector3(0, 0, 1);
                            newRotation = Quaternion.Euler(0, 0, 0);
                            break;
                        case ExitDirection.Bottom:
                            newPosition = exit.Transform.position + new Vector3(0, 0, -1);
                            newRotation = Quaternion.Euler(0, 180, 0);
                            break;
                        case ExitDirection.Left:
                            newPosition = exit.Transform.position + new Vector3(-1, 0, 0);
                            newRotation = Quaternion.Euler(0, -90, 0);
                            break;
                        case ExitDirection.Right:
                            newPosition = exit.Transform.position + new Vector3(1, 0, 0);
                            newRotation = Quaternion.Euler(0, 90, 0);
                            break;
                        default:
                            Debug.LogWarning("Invalid exit direction");
                            break;
                    }

                    GameObject roomObject = _roomBuilder.BuildRoom(template, RoomsParent);
                    roomObject.transform.position = newPosition;
                    roomObject.transform.rotation = newRotation;
                    //roomObject.SetActive(false);

                    Debug.Log($"Created room {nextRoomName} of type {nextRoomType} at position {newPosition} with rotation {newRotation}");
                }
                else
                {
                    Debug.LogWarning($"Room {nextRoomName} already exists, skipping creation.");
                }

            }
            else
            {
                Debug.LogWarning($"No template found for room type: {nextRoomType}");
            }
        }
    }

    private float GetRoomHeight(RoomTemplateSO.Template template)
    {
        var coordinates = template.Coordinates;
        float maxZ = coordinates[0, 0].z;
        float minZ = coordinates[0, 0].z;

        for (int i = 0; i < coordinates.GetLength(0); i++)
        {
            for (int j = 0; j < coordinates.GetLength(1); j++)
            {
                if (coordinates[i, j].z > maxZ)
                    maxZ = coordinates[i, j].z;
                if (coordinates[i, j].z < minZ)
                    minZ = coordinates[i, j].z;
            }
        }

        return maxZ - minZ + 1;
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

    public void MoveObjectToScene(GameObject obj, string targetSceneName)
    {
        Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
        if (!targetScene.IsValid())
        {
            targetScene = SceneManager.CreateScene(targetSceneName);
        }

        SceneManager.MoveGameObjectToScene(obj, targetScene);
    }
}
