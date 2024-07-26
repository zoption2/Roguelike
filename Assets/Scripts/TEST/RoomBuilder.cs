using Gameplay;
using Obstacles;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public interface IRoomBuilder
{
    public Transform PlayersParent { get; set; }
    public Transform EnemiesParent { get; set; }
    public Transform BuffsParent { get; set; }
    public Transform WallsParent { get; set; }
    public Transform FloorsParent { get; set; }
    public void OnNavigationCreate(Transform parent);
    public GameObject BuildRoom(RoomTemplateSO.Template template, Transform parent);
}

public class RoomBuilder : IRoomBuilder
{
    public IRoomContext RoomContext { get; set; }
    public Transform PlayersParent { get; set; }
    public Transform EnemiesParent { get; set; }
    public Transform BuffsParent { get; set; }
    public Transform WallsParent { get; set; }
    public Transform FloorsParent { get; set; }

    private INavigationFactory _navigationFactory;
    private IRoomObjectsFactory _roomObjectsFactory;
    private IChestFactory _chestFactory;

    public RoomBuilder(
        INavigationFactory navigationFactory,
        IRoomObjectsFactory roomObjectsFactory,
        IChestFactory chestFactory)
    {
        _navigationFactory = navigationFactory;
        _roomObjectsFactory = roomObjectsFactory;
        _chestFactory = chestFactory;
    }

    public GameObject BuildRoom(RoomTemplateSO.Template roomTemplate, Transform parent)
    {
        AnalyzeTemplate(roomTemplate);

        if (roomTemplate == null)
        {
            Debug.LogError("Template not found");
            return null;
        }

        GameObject roomObject = new GameObject(roomTemplate.name);
        roomObject.transform.SetParent(parent);
        roomObject.transform.localPosition = Vector3.zero;

        EnemiesParent = CreateParent("Enemies", roomObject.transform);
        BuffsParent = CreateParent("Buffs", roomObject.transform);
        WallsParent = CreateParent("Walls", roomObject.transform);
        FloorsParent = CreateParent("Floors", roomObject.transform);

        RoomContext.EnemiesParent = EnemiesParent;
        RoomContext.BuffsParent = BuffsParent;

        var templateElements = roomTemplate.TemplateElement;
        var coordinates = roomTemplate.Coordinates;

        for (int i = 0; i < templateElements.GetLength(0); i++)
        {
            for (int j = 0; j < templateElements.GetLength(1); j++)
            {
                TemplateElementType elementType = templateElements[i, j];
                Vector3 position = coordinates[i, j];
                Vector3 floorPosition = new Vector3(position.x, position.y - 1, position.z);

                _roomObjectsFactory.Build(floorPosition, FloorsParent, TemplateElementType.Ground);

                switch (elementType)
                {
                    case TemplateElementType.DefaultWall:
                        _roomObjectsFactory.Build(position, WallsParent, TemplateElementType.DefaultWall);
                        break;
                    case TemplateElementType.Chest:
                        IChestController chestController = _chestFactory.CreateChest(position, WallsParent);
                        RoomContext.Chests.Add(chestController);
                        break;
                }
            }
        }

        BuildExits(templateElements, coordinates);
        CenterCamera(roomTemplate);
        OnNavigationCreate(roomObject.transform);

        Debug.Log($"Room '{roomTemplate.name}' created with context.");

        return roomObject;
    }

    private void AnalyzeTemplate(RoomTemplateSO.Template roomTemplate)
    {
        if (roomTemplate == null)
        {
            Debug.LogError("Template not found");
            return;
        }

        var templateElements = roomTemplate.TemplateElement;
        var coordinates = roomTemplate.Coordinates;

        for (int i = 0; i < templateElements.GetLength(0); i++)
        {
            for (int j = 0; j < templateElements.GetLength(1); j++)
            {
                TemplateElementType elementType = templateElements[i, j];
                Vector3 position = coordinates[i, j];

                switch (elementType)
                {
                    case TemplateElementType.Player:
                        RoomContext.PlayerSpawnPoints.Add(new PlayerSpawnPointWithType { SpawnPoint = position, Type = CharacterType.Warrior });
                        break;

                    case TemplateElementType.Barbarian:
                    case TemplateElementType.Summoner:
                    case TemplateElementType.Thrower:
                        RoomContext.EnemySpawnPoints.Add(new EnemySpawnPointWithType { SpawnPoint = position, Type = (CharacterType)elementType });
                        Debug.Log($"Added enemy spawn point at {position} of type {elementType}");
                        break;

                    case TemplateElementType.RandomBuff:
                        RoomContext.BuffSpawnPoints.Add(new BuffSpawnPointWithType { SpawnPoint = position, Type = (BuffType)elementType });
                        break;
                }
            }
        }

        Debug.Log($"Analyzed template {roomTemplate}: PlayerSpawnPoints={RoomContext.PlayerSpawnPoints.Count}, EnemySpawnPoints={RoomContext.EnemySpawnPoints.Count}, BuffSpawnPoints={RoomContext.BuffSpawnPoints.Count}");
    }

    private void BuildExits(TemplateElementType[,] templateElements, Vector3[,] coordinates)
    {
        int rows = templateElements.GetLength(0);
        int cols = templateElements.GetLength(1);
        List<Exit> exits = new List<Exit>();

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
                        GameObject exit = _roomObjectsFactory.Build(centerPos, WallsParent, TemplateElementType.Exit);
                        exit.transform.rotation = Quaternion.Euler(0, 90, 0);

                        Exit exitComponent = exit.GetComponent<Exit>();
                        exitComponent.Transform = exit.transform;
                        exits.Add(exitComponent);

                        ICompleatedRoomTrigger trigger = exit.GetComponent<ICompleatedRoomTrigger>();
                        RoomContext.CompleatedRoomTriggers.Add(trigger);

                        templateElements[i, j] = TemplateElementType.None;
                        templateElements[i, j + 2] = TemplateElementType.None;
                    }
                    else if (i + 2 < rows &&
                             templateElements[i + 1, j] == TemplateElementType.Exit &&
                             templateElements[i + 2, j] == TemplateElementType.Exit)
                    {
                        Vector3 centerPos = coordinates[i + 1, j];
                        GameObject exit = _roomObjectsFactory.Build(centerPos, WallsParent, TemplateElementType.Exit);

                        Exit exitComponent = exit.GetComponent<Exit>();
                        exitComponent.Transform = exit.transform;
                        exits.Add(exitComponent);

                        ICompleatedRoomTrigger trigger = exit.GetComponent<ICompleatedRoomTrigger>();
                        RoomContext.CompleatedRoomTriggers.Add(trigger);

                        templateElements[i, j] = TemplateElementType.None;
                        templateElements[i + 2, j] = TemplateElementType.None;
                    }
                }
            }
        }
    }

    private void CenterCamera(RoomTemplateSO.Template roomTemplate)
    {
        if (roomTemplate == null)
        {
            Debug.LogError("Template not found");
            return;
        }

        var coordinates = roomTemplate.Coordinates;
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
        parentObject.transform.localRotation = Quaternion.identity;
        return parentObject.transform;
    }

    public void OnNavigationCreate(Transform parent)
    {
        NavMeshSurface navMeshSurface = parent.gameObject.AddComponent<NavMeshSurface>();
        RoomContext.NavMeshSurface = navMeshSurface;
        navMeshSurface.BuildNavMesh();
    }
}

