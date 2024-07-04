using CharactersStats;
using Gameplay;
using Pool;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public interface ILevelManager
{
}

public class LevelManager : MonoBehaviour, ILevelManager
{
    [SerializeField]
    private List<TypeOfScenario> _roomsOrderInitList;

    public Queue<TypeOfScenario> RoomsOrder { get; private set; }
    private IGameplayService _gameplayService;
    private TypeOfScenario _nextRoom;
    private ILevelContext _levelContext;
    private Scene _currentRoomScene;
    private RoomTemplateSO _roomTemplate;
    private RoomTemplateSO.Template _template;
    public IPoolManager PoolManager { get; private set; }

    private Transform _globalPoolParent;

    public Transform GlobalPoolParent
    {
        get
        {
            if (_globalPoolParent == null)
            {
                GameObject globalParentObject = new GameObject("GlobalPoolParent");
                _globalPoolParent = globalParentObject.transform;
                DontDestroyOnLoad(globalParentObject);
            }
            return _globalPoolParent;
        }
    }

    [Inject]
    public void Construct(
        RoomTemplateSO roomTemplateSO,
        IGameplayService gameplayService,
        IPoolManager poolManager
    )
    {
        _roomTemplate = roomTemplateSO;
        _gameplayService = gameplayService;
        PoolManager = poolManager;
    }

    private void Start()
    {
        _levelContext = new LevelContext();
        _gameplayService.LevelContext = _levelContext;
        _gameplayService.LevelManager = this;

        GameObject poolManagerObject = new GameObject("PoolManager");
        PoolManager.Init(poolManagerObject);

        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
    }

    public void LoadLevel()
    {
        RoomsOrder = new Queue<TypeOfScenario>(_roomsOrderInitList);
        _gameplayService.LevelManager = this;

        _nextRoom = GetNextRoom();

        if (SceneManager.GetSceneByName("Menu").IsValid())
        {
            SceneManager.UnloadSceneAsync("Menu").completed += (AsyncOperation operation) =>
            {
                LoadRoomScene(_nextRoom);
            };
        }
        else
        {
            LoadRoomScene(_nextRoom);
        }
    }

    public TypeOfScenario GetNextRoom()
    {
        if (_roomsOrderInitList.Count > 0)
        {
            TypeOfScenario firstRoom = RoomsOrder.Dequeue();
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
        GameObject player = _levelContext.Player;
        LoadRoomScene(_nextRoom, player);
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

    public void LoadRoomScene(TypeOfScenario type, GameObject playerParent = null)
    {
        if (_currentRoomScene.IsValid())
        {
            SceneManager.UnloadSceneAsync(_currentRoomScene);
        }

        _template = SetTemplate(type);

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

                if (playerParent != null)
                {
                    MoveObjectToScene(playerParent.gameObject, newSceneName);
                    Debug.Log("Player parent moved to new scene: " + playerParent.name);
                }
                else
                {
                    Debug.LogWarning("Player parent is null when trying to move to new scene.");
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
