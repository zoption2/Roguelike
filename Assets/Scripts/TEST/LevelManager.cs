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
    RoomTemplateSO.Template GetTemplate();
    Queue<TypeOfScenario> RoomsOrder { get; set; }
    //void StartCurrentRoom();
    void SwitchToNextRoom();
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

    public Transform GlobalPoolParent
    {
        get
        {
            if (_globalPoolParent == null)
            {
                GameObject globalParentObject = new GameObject("GlobalPoolParent");
                _globalPoolParent = globalParentObject.transform;
            }
            return _globalPoolParent;
        }
    }

    [Inject]
    public void Construct(
        RoomTemplateSO roomTemplateSO,
        IPoolManager poolManager,
        IGameplayService gameplayService,
        INavigationFactory navigationFactory,
        IRoomObjectsFactory roomObjectsFactory,
        ILevelContext levelContext
    )
    {
        _roomTemplate = roomTemplateSO;
        PoolManager = poolManager;
        _gameplayService = gameplayService;

        _levelContext = levelContext;

        _roomBuilder = new RoomBuilder(
            navigationFactory,
            roomObjectsFactory
        );

        GameObject poolManagerObject = new GameObject("PoolManager");
        PoolManager.Init(poolManagerObject);
    }

    public void LoadLevel()
    {
        _mainRoomOrder = new List<TypeOfScenario>
        {
            TypeOfScenario.MainRoom,
            TypeOfScenario.DefaultRoom,
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
                
                CreateRooms(roomsObject.transform);
                
                SceneManager.sceneLoaded -= OnLevelSceneLoaded;
            }
        }
    }

    private void CreateRooms(Transform parent)
    {
        float currentZPosition = 0;
        bool isFirstRoom = true;

        foreach (var typeOfScenario in RoomsOrder)
        {
            var template = SetTemplate(typeOfScenario);
            if (template != null)
            {
                string roomName = template.name;
                _levelContext.CreateRoomContext(roomName);
                RoomContext roomContext = _levelContext.GetRoomContext(roomName);

                _roomBuilder.RoomContext = roomContext;

                GameObject roomObject = _roomBuilder.BuildRoom(template, parent);

                roomObject.transform.position = new Vector3(0, 0, currentZPosition);

                _roomBuilder.OnNavigationCreate(parent);

                currentZPosition += GetRoomHeight(template);

                if (isFirstRoom)
                {
                    roomObject.SetActive(true);
                    _levelContext.CurrentRoomContext = roomContext;
                    _levelContext.CurrentRoomName = roomName;
                    _levelContext.CurrentRoomType = typeOfScenario;

                    isFirstRoom = false;
                }
                else
                {
                    roomObject.SetActive(false);
                }
            }
        }

        _gameplayService.StartCurrentRoom();
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



    //public void StartCurrentRoom()
    //{
    //    _gameplayService.StartCurrentRoom();
    //}

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
