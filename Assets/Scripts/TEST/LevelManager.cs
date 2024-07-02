using CharactersStats;
using Gameplay;
using Pool;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public interface ILevelManager
{
}

public class LevelManager : MonoBehaviour
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
    public IPoolManager PoolManager { get; set; }

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

        _levelContext = new LevelContext();
        _gameplayService.LevelContext = _levelContext;
        _gameplayService.LevelManager = this;
    }

    private void Start()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
    }

    public void LoadLevel()
    {
        RoomsOrder = new Queue<TypeOfScenario>(_roomsOrderInitList);
        _gameplayService.LevelManager = this;

        GameObject poolManagerObject = new GameObject("PoolManager");
        PoolManager.Init(poolManagerObject);

        _nextRoom = GetNextRoom();

        // Unload the Menu scene before loading the new room
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
        GameObject player = _gameplayService.Player;
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

                SceneManager.UnloadSceneAsync("Room");

                if (playerParent != null)
                {
                    MoveObjectToScene(playerParent.gameObject, newSceneName);
                    Debug.Log("Player parent moved to new scene: " + playerParent.name);
                }
                else
                {
                    Debug.LogWarning("Player parent is null when trying to move to new scene.");
                }

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
