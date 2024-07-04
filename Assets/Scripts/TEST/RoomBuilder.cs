using Gameplay;
using Obstacles;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;

public interface IRoomBuilder
{
    void BuildLevel(RoomTemplateSO.Template template);
    public Transform PlayersParent { get; set; }
    public Transform EnemiesParent { get; set; }
    public Transform BuffsParent { get; set; }
}

public class RoomBuilder : IRoomBuilder
{
    public IScenario Scenario { get; }
    public ICharacterScenarioContext Characters { get; }
    public Transform PlayersParent { get; set; }
    public Transform EnemiesParent { get; set; }
    public Transform BuffsParent { get; set; }
    public Transform WallsParent { get; set; }
    public Transform FloorsParent { get; set; }

    private INavigationFactory _navigationFactory;
    private IRoomObjectsFactory _roomObjectsFactory;
    private RoomTemplateSO _roomTemplate;

    public RoomBuilder(
        IScenario scenario,
        ICharacterScenarioContext context,
        INavigationFactory navigationFactory,
        IRoomObjectsFactory roomObjectsFactory,
        RoomTemplateSO roomTemplate)
    {
        Scenario = scenario;
        Characters = context;
        _navigationFactory = navigationFactory;
        _roomObjectsFactory = roomObjectsFactory;
        _roomTemplate = roomTemplate;
    }

    public void BuildLevel(RoomTemplateSO.Template template)
    {
        AnalyzeTemplate(template);
        BuildRoom(template);
        OnNavigationCreate();
        CenterCamera(template);
    }

    private void AnalyzeTemplate(RoomTemplateSO.Template roomTemplate)
    {
        RoomTemplateSO.Template template = roomTemplate;
        if (template == null)
        {
            Debug.LogError("Template not found");
            return;
        }

        var templateElements = template.TemplateElement;
        var coordinates = template.Coordinates;

        for (int i = 0; i < templateElements.GetLength(0); i++)
        {
            for (int j = 0; j < templateElements.GetLength(1); j++)
            {
                TemplateElementType elementType = templateElements[i, j];
                Vector3 position = coordinates[i, j];

                switch (elementType)
                {
                    case TemplateElementType.Player:
                        Characters.PlayerSpawnPoints.Add(new PlayerSpawnPointWithType { SpawnPoint = position, Type = CharacterType.Warrior });
                        break;

                    case TemplateElementType.Barbarian:
                    case TemplateElementType.Summoner:
                    case TemplateElementType.Thrower:
                        Characters.EnemySpawnPoints.Add(new EnemySpawnPointWithType { SpawnPoint = position, Type = (CharacterType)elementType });
                        Debug.Log($"Added enemy spawn point at {position} of type {elementType}");
                        break;

                    case TemplateElementType.RandomBuff:
                        Characters.BuffSpawnPoints.Add(new BuffSpawnPointWithType { SpawnPoint = position, Type = (BuffType)elementType });
                        break;
                }
            }
        }

        Debug.Log($"Analyzed template {template}: PlayerSpawnPoints={Characters.PlayerSpawnPoints.Count}, EnemySpawnPoints={Characters.EnemySpawnPoints.Count}, BuffSpawnPoints={Characters.BuffSpawnPoints.Count}");
    }

    private void BuildRoom(RoomTemplateSO.Template roomTemplate)
    {
        RoomTemplateSO.Template template = roomTemplate;

        if (template == null)
        {
            Debug.LogError("Template not found");
            return;
        }

        GameObject roomObject = new GameObject(template.name);
        roomObject.transform.position = new Vector3(0, 0, 0);

        PlayersParent = CreateParent("Players", roomObject.transform);
        EnemiesParent = CreateParent("Enemies", roomObject.transform);
        BuffsParent = CreateParent("Buffs", roomObject.transform);
        WallsParent = CreateParent("Walls", roomObject.transform);
        FloorsParent = CreateParent("Floors", roomObject.transform);

        var templateElements = template.TemplateElement;
        var coordinates = template.Coordinates;

        for (int i = 0; i < templateElements.GetLength(0); i++)
        {
            for (int j = 0; j < templateElements.GetLength(1); j++)
            {
                TemplateElementType elementType = templateElements[i, j];
                Vector3 position = coordinates[i, j];
                Vector3 floorPosition = new Vector3(position.x, position.y - 1, position.z);

                _roomObjectsFactory.BuildAsync(floorPosition, FloorsParent, RoomObjectType.Floor);

                switch (elementType)
                {
                    case TemplateElementType.DefaultWall:
                        _roomObjectsFactory.BuildAsync(position, WallsParent, RoomObjectType.DefaultWall);
                        break;
                }
            }
        }

        BuildExitsAsync(templateElements, coordinates);
    }

    private async void BuildExitsAsync(TemplateElementType[,] templateElements, Vector3[,] coordinates)
    {
        int rows = templateElements.GetLength(0);
        int cols = templateElements.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (templateElements[i, j] == TemplateElementType.Exit)
                {
                    if (j + 2 < cols &&
                        templateElements[i, j + 1] == TemplateElementType.Exit &&
                        templateElements[i, j + 2] == TemplateElementType.Exit)
                    {
                        Vector3 centerPos = coordinates[i, j + 1];
                        GameObject exit = await _roomObjectsFactory.BuildAsync(centerPos, WallsParent, RoomObjectType.Exit);
                        exit.transform.rotation = Quaternion.Euler(0, 90, 0);

                        ICompleatedRoomTrigger trigger = exit.GetComponent<ICompleatedRoomTrigger>();
                        Characters.CompleatedRoomTriggers.Add(trigger);
                        trigger.Init(Scenario.GameplayService);

                        templateElements[i, j] = TemplateElementType.None;
                        templateElements[i, j + 2] = TemplateElementType.None;
                    }
                    else if (i + 2 < rows &&
                             templateElements[i + 1, j] == TemplateElementType.Exit &&
                             templateElements[i + 2, j] == TemplateElementType.Exit)
                    {
                        Vector3 centerPos = coordinates[i + 1, j];
                        GameObject exit = await _roomObjectsFactory.BuildAsync(centerPos, WallsParent, RoomObjectType.Exit);

                        ICompleatedRoomTrigger trigger = exit.GetComponent<ICompleatedRoomTrigger>();
                        Characters.CompleatedRoomTriggers.Add(trigger);
                        trigger.Init(Scenario.GameplayService);

                        templateElements[i, j] = TemplateElementType.None;
                        templateElements[i + 2, j] = TemplateElementType.None;
                    }

                    
                }
            }
        }
    }

    private void CenterCamera(RoomTemplateSO.Template roomTemplate)
    {
        RoomTemplateSO.Template template = roomTemplate;

        if (template == null)
        {
            Debug.LogError("Template not found");
            return;
        }

        var coordinates = template.Coordinates;
        int rows = coordinates.GetLength(0);
        int cols = coordinates.GetLength(1);

        Vector3 bottomLeft = coordinates[0, 0];
        Vector3 topRight = coordinates[rows - 1, cols - 1];
        Vector3 center = (bottomLeft + topRight) / 2;

        Camera.main.transform.position = new Vector3(center.x, Camera.main.transform.position.y, center.z);
        Camera.main.transform.LookAt(new Vector3(center.x, 0, center.z));
    }

    private Transform CreateParent(string name, Transform parent)
    {
        GameObject parentObject = new GameObject(name);
        parentObject.transform.SetParent(parent);
        parentObject.transform.localPosition = Vector3.zero;
        parentObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        return parentObject.transform;
    }

    public async void OnNavigationCreate()
    {
        NavMeshSurface navMeshSurface = await _navigationFactory.CreateNavigation();
        Characters.NavMeshSurface = navMeshSurface;
        navMeshSurface.BuildNavMesh();
    }
}
