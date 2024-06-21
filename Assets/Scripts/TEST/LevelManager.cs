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

    private Queue<TypeOfScenario> _roomsOrder;
    private IGameplayService _gameplayService;
    private TypeOfScenario _nextRoom;
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
    }

    public void Start()
    {
        _roomsOrder = new Queue<TypeOfScenario>(_roomsOrderInitList);
        _gameplayService.LevelManager = this;

        GameObject poolManagerObject = new GameObject("PoolManager");
        PoolManager.InitPoolers(poolManagerObject.transform);

        _nextRoom = GetNextRoom();
        LoadRoomScene(_nextRoom);
    }

    public TypeOfScenario GetNextRoom()
    {
        if (_roomsOrderInitList.Count > 0)
        {
            TypeOfScenario firstRoom = _roomsOrder.Dequeue();
            return firstRoom;
        }
        else
        {
            Debug.LogError("No rooms in the sequence to start the level.");
            return TypeOfScenario.DefaultRoom;
        }
    }

    public void LoadNextRoom(GameObject player)
    {
        _nextRoom = GetNextRoom();
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

    public void LoadRoomScene(TypeOfScenario type, GameObject player = null)
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

                if (player != null)
                {
                    MoveObjectToScene(player, newSceneName);
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
