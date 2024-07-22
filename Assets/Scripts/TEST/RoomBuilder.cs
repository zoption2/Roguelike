using Gameplay;
using Obstacles;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public interface IRoomBuilder
{
    GameObject BuildRoom(RoomTemplateSO.Template template, Transform parent);
    public void OnNavigationCreate(Transform parent);
    Transform PlayersParent { get; set; }
    Transform EnemiesParent { get; set; }
    Transform BuffsParent { get; set; }
    Transform WallsParent { get; set; }
    Transform FloorsParent { get; set; }
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

    public RoomBuilder(
        INavigationFactory navigationFactory,
        IRoomObjectsFactory roomObjectsFactory)
    {
        _navigationFactory = navigationFactory;
        _roomObjectsFactory = roomObjectsFactory;
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

                _roomObjectsFactory.Build(floorPosition, FloorsParent, RoomObjectType.Floor);

                switch (elementType)
                {
                    case TemplateElementType.DefaultWall:
                        _roomObjectsFactory.Build(position, WallsParent, RoomObjectType.DefaultWall);
                        break;
                }
            }
        }

        BuildExits(templateElements, coordinates);
        CenterCamera(roomTemplate);
        //OnNavigationCreate(roomObject.transform);

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
                        GameObject exit = _roomObjectsFactory.Build(centerPos, WallsParent, RoomObjectType.Exit);
                        exit.transform.rotation = Quaternion.Euler(0, 90, 0);

                        Exit exitComponent = exit.GetComponent<Exit>();
                        exitComponent.Transform = exit.transform;
                        //SetExitDirection(exitComponent, centerPos, coordinates, rows, cols);
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
                        GameObject exit = _roomObjectsFactory.Build(centerPos, WallsParent, RoomObjectType.Exit);

                        Exit exitComponent = exit.GetComponent<Exit>();
                        exitComponent.Transform = exit.transform;
                        //SetExitDirection(exitComponent, centerPos, coordinates, rows, cols);
                        exits.Add(exitComponent);

                        ICompleatedRoomTrigger trigger = exit.GetComponent<ICompleatedRoomTrigger>();
                        RoomContext.CompleatedRoomTriggers.Add(trigger);

                        templateElements[i, j] = TemplateElementType.None;
                        templateElements[i + 2, j] = TemplateElementType.None;
                    }
                }
            }
        }

        //SetExitTypes(exits);
    }

    private void SetExitDirection(Exit exit, Vector3 position, Vector3[,] coordinates, int rows, int cols)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (coordinates[i, j].x < minX) minX = coordinates[i, j].x;
                if (coordinates[i, j].x > maxX) maxX = coordinates[i, j].x;
                if (coordinates[i, j].z < minZ) minZ = coordinates[i, j].z;
                if (coordinates[i, j].z > maxZ) maxZ = coordinates[i, j].z;
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

    private void SetExitTypes(List<Exit> exits)
    {
        if (exits.Count == 0) return;

        exits[0].SetExitType(TemplateElementType.ExitToStoryRoom);

        for (int i = 1; i < exits.Count; i++)
        {
            if (i % 2 == 0)
            {
                exits[i].SetExitType(TemplateElementType.ExitToBountyRoom);
            }
            else
            {
                exits[i].SetExitType(TemplateElementType.ExitToRandomeRoom);
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
