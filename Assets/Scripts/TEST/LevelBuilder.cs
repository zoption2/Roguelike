using CharactersStats;
using Enemy;
using Gameplay;
using Obstacles;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public interface ILevelBuilder
{
    void BuildLevel();
}

public class LevelBuilder : ILevelBuilder
{
    public IScenario _scenario { get; }
    public ICharacterScenarioContext _characters { get; }
    private IStatsProvider _statsProvider;
    private IBuffFactory _buffFactory;
    private IPlayerFactory _playerFactory;
    private IEnemyFactory _enemyFactory;
    private INavigationFactory _navigationFactory;
    private IRoomObjectsFactory _roomObjectsFactory;
    private RoomTemplateSO _roomTemplate;

    private List<Vector3> _playerSpawnPoints;
    private List<Vector3> _enemySpawnPoints;
    private List<Vector3> _buffSpawnPoints;

    public int EnemyCount { get; set; } = 2;
    public int BuffCount { get; set; } = 2;
    public int RoomCount { get; set; } = 1;

    private Dictionary<string, RoomTemplateSO.Template> _templatesByName;
    private List<RoomTemplateSO.Template> _templates;
    private List<RoomConnection> _roomConnections;

    public LevelBuilder(IScenario scenario,
            ICharacterScenarioContext context,
            IStatsProvider provider,
            IBuffFactory buffFactory,
            IPlayerFactory playerFactory,
            IEnemyFactory enemyFactory,
            INavigationFactory navigationFactory,
            IRoomObjectsFactory roomObjectsFactory,
            RoomTemplateSO roomTemplate)
    {
        _scenario = scenario;
        _characters = context;
        _statsProvider = provider;
        _buffFactory = buffFactory;
        _playerFactory = playerFactory;
        _enemyFactory = enemyFactory;
        _navigationFactory = navigationFactory;
        _roomObjectsFactory = roomObjectsFactory;
        _roomTemplate = roomTemplate;

        _templatesByName = _roomTemplate.Templates.ToDictionary(t => t.name, t => t);
        _templates = _roomTemplate.Templates;
    }

    public void BuildLevel()
    {
        Debug.Log("Start building level");
        AnalyzeTemplate();
        BuildRooms();
    }

    private void AnalyzeTemplate()
    {
        foreach (var template in _templates)
        {
            template.leftExit = TemplateElementType.None;
            template.rightExit = TemplateElementType.None;
            template.topExit = TemplateElementType.None;
            template.bottomExit = TemplateElementType.None;

            var templateElements = template.TemplateElement;
            var coordinates = template.Coordinates;

            for (int i = 0; i < templateElements.GetLength(0); i++)
            {
                for (int j = 0; j < templateElements.GetLength(1); j++)
                {
                    TemplateElementType elementType = templateElements[i, j];
                    Vector3 position = coordinates[i, j];

                    if (elementType == TemplateElementType.Exit)
                    {
                        if (i == 0) template.topExit = elementType;
                        if (i == templateElements.GetLength(0) - 1) template.bottomExit = elementType;
                        if (j == 0) template.leftExit = elementType;
                        if (j == templateElements.GetLength(1) - 1) template.rightExit = elementType;
                    }
                }
            }
            Debug.Log($"Template '{template.name}' analyzed: TopExit={template.topExit}, BottomExit={template.bottomExit}, LeftExit={template.leftExit}, RightExit={template.rightExit}");
        }
    }

    private void BuildRooms()
    {
        _roomConnections = new List<RoomConnection>();
        RoomTemplateSO.Template mainRoom = _templatesByName["MainRoom"];
        SetupMainRoomExits(mainRoom);

        for (int i = 0; i < RoomCount - 1; i++)
        {
            if (!TryAddRoom())
            {
                Debug.LogError("Failed to generate room layout with specified room count.");
                return;
            }
        }

        foreach (var roomConnection in _roomConnections)
        {
            BuildRoom(roomConnection.Room, roomConnection.Position);
        }
    }

    private void SetupMainRoomExits(RoomTemplateSO.Template mainRoom)
    {
        int exitCount = UnityEngine.Random.Range(1, 5);

        RoomTemplateSO.Template mainRoomClone = CloneRoomTemplate(mainRoom);
        SetUnusedExitsToWalls(mainRoomClone, exitCount);

        Debug.Log($"Main room exits set: {exitCount} exits");

        _roomConnections.Add(new RoomConnection
        {
            Room = mainRoomClone,
            Position = Vector3.zero,
            ConnectedExits = GetRoomExits(mainRoomClone)
        });

        var elements = mainRoomClone.TemplateElement;
        int rows = elements.GetLength(0);
        int cols = elements.GetLength(1);
    }

    private bool TryAddRoom()
    {
        var openConnections = _roomConnections.Where(rc => rc.HasOpenExits).ToList();

        if (!openConnections.Any())
        {
            Debug.LogError("No open connections available to add a new room.");
            return false;
        }

        var randomOpenConnection = openConnections[UnityEngine.Random.Range(0, openConnections.Count)];
        var direction = randomOpenConnection.ConnectedExits[0];
        var matchingExitDirection = GetOppositeDirection(direction);

        var availableTemplates = _templates
            .Where(t => t.name != "MainRoom" && t.HasExit(matchingExitDirection))
            .ToList();

        if (!availableTemplates.Any())
        {
            Debug.LogError("No available templates with required exits.");
            return false;
        }

        var newRoomTemplate = CloneRoomTemplate(availableTemplates[UnityEngine.Random.Range(0, availableTemplates.Count)]);
        SetUnusedExitsToWalls(newRoomTemplate, GetRequiredExitsCount());

        var newRoomPosition = randomOpenConnection.Position + direction + GetOffsetForExit(newRoomTemplate, matchingExitDirection);

        randomOpenConnection.ConnectExit(direction);
        _roomConnections.Add(new RoomConnection
        {
            Room = newRoomTemplate,
            Position = newRoomPosition,
            ConnectedExits = GetRoomExits(newRoomTemplate, matchingExitDirection)
        });

        Debug.Log($"Room '{newRoomTemplate.name}' added at position {newRoomPosition} and connected to {randomOpenConnection.Room.name} at position {randomOpenConnection.Position} with exit {direction}");

        return true;
    }

    private int GetRequiredExitsCount()
    {
        int remainingRooms = RoomCount - _roomConnections.Count;
        int remainingExits = _roomConnections.Sum(rc => rc.OpenExitsCount);
        return Math.Min(remainingRooms, remainingExits);
    }

    private Vector3 GetOppositeDirection(Vector3 direction)
    {
        if (direction == Vector3.right) return Vector3.left;
        if (direction == Vector3.left) return Vector3.right;
        if (direction == Vector3.up) return Vector3.down;
        if (direction == Vector3.down) return Vector3.up;
        return Vector3.zero;
    }

    private Vector3 GetOffsetForExit(RoomTemplateSO.Template template, Vector3 exitDirection)
    {
        if (exitDirection == Vector3.left && template.rightExit == TemplateElementType.Exit)
            return new Vector3(template.TemplateElement.GetLength(1), 0, 0);
        if (exitDirection == Vector3.right && template.leftExit == TemplateElementType.Exit)
            return new Vector3(-template.TemplateElement.GetLength(1), 0, 0);
        if (exitDirection == Vector3.up && template.bottomExit == TemplateElementType.Exit)
            return new Vector3(0, 0, -template.TemplateElement.GetLength(0));
        if (exitDirection == Vector3.down && template.topExit == TemplateElementType.Exit)
            return new Vector3(0, 0, template.TemplateElement.GetLength(0));
        return Vector3.zero;
    }

    private void SetUnusedExitsToWalls(RoomTemplateSO.Template roomTemplate, int exitCount)
    {
        List<Vector3> allExits = new List<Vector3>();
        if (roomTemplate.leftExit == TemplateElementType.Exit) allExits.Add(Vector3.left);
        if (roomTemplate.rightExit == TemplateElementType.Exit) allExits.Add(Vector3.right);
        if (roomTemplate.topExit == TemplateElementType.Exit) allExits.Add(Vector3.up);
        if (roomTemplate.bottomExit == TemplateElementType.Exit) allExits.Add(Vector3.down);

        Debug.Log($"All available exits: {allExits.Count}");

        if (exitCount > allExits.Count)
        {
            Debug.LogError("Exit count is greater than available exits");
            exitCount = allExits.Count;
        }

        List<Vector3> selectedExits = allExits.OrderBy(x => UnityEngine.Random.value).Take(exitCount).ToList();
        Debug.Log($"Selected exits count: {selectedExits.Count}");

        roomTemplate.leftExit = selectedExits.Contains(Vector3.left) ? TemplateElementType.Exit : TemplateElementType.DefaultWall;
        roomTemplate.rightExit = selectedExits.Contains(Vector3.right) ? TemplateElementType.Exit : TemplateElementType.DefaultWall;
        roomTemplate.topExit = selectedExits.Contains(Vector3.up) ? TemplateElementType.Exit : TemplateElementType.DefaultWall;
        roomTemplate.bottomExit = selectedExits.Contains(Vector3.down) ? TemplateElementType.Exit : TemplateElementType.DefaultWall;

        var templateElements = roomTemplate.TemplateElement;
        int rows = templateElements.GetLength(0);
        int cols = templateElements.GetLength(1);

        if (roomTemplate.leftExit == TemplateElementType.DefaultWall)
        {
            templateElements[rows / 2, 0] = TemplateElementType.DefaultWall;
        }
        if (roomTemplate.rightExit == TemplateElementType.DefaultWall)
        {
            templateElements[rows / 2, cols - 1] = TemplateElementType.DefaultWall;
        }
        if (roomTemplate.topExit == TemplateElementType.DefaultWall)
        {
            templateElements[0, cols / 2] = TemplateElementType.DefaultWall;
        }
        if (roomTemplate.bottomExit == TemplateElementType.DefaultWall)
        {
            templateElements[rows - 1, cols / 2] = TemplateElementType.DefaultWall;
        }

        roomTemplate.TemplateElement = templateElements;

        Debug.Log($"Exits updated. Total exits: {selectedExits.Count}");
        foreach (var exit in selectedExits)
        {
            Debug.Log($"Selected exit: {exit}");
        }
    }

    private RoomTemplateSO.Template CloneRoomTemplate(RoomTemplateSO.Template original)
    {
        var clone = new RoomTemplateSO.Template
        {
            name = original.name,
            id = original.id,
            DateAdded = original.DateAdded,
            rows = original.rows,
            cols = original.cols,
            TemplateElementsFlat = new List<TemplateElementType>(original.TemplateElementsFlat),
            CoordinatesFlat = new List<Vector3>(original.CoordinatesFlat),
            leftExit = original.leftExit,
            rightExit = original.rightExit,
            topExit = original.topExit,
            bottomExit = original.bottomExit
        };

        return clone;
    }

    private void BuildRoom(RoomTemplateSO.Template template, Vector3 position)
    {
        GameObject roomObject = new GameObject(template.name);
        roomObject.transform.position = position;

        Transform playersParent = CreateParent("Players", roomObject.transform);
        Transform enemiesParent = CreateParent("Enemies", roomObject.transform);
        Transform buffsParent = CreateParent("Buffs", roomObject.transform);
        Transform wallsParent = CreateParent("Walls", roomObject.transform);
        Transform floorsParent = CreateParent("Floors", roomObject.transform);

        var templateElements = template.TemplateElement;
        var coordinates = template.Coordinates;

        for (int i = 0; i < templateElements.GetLength(0); i++)
        {
            for (int j = 0; j < templateElements.GetLength(1); j++)
            {
                TemplateElementType elementType = templateElements[i, j];
                Vector3 localPosition = coordinates[i, j] + position;
                Vector3 floorPosition = new Vector3(localPosition.x, localPosition.y - 1, localPosition.z);

                _roomObjectsFactory.Build(floorPosition, floorsParent, RoomObjectType.Floor);

                switch (elementType)
                {
                    case TemplateElementType.DefaultWall:
                        _roomObjectsFactory.Build(localPosition, wallsParent, RoomObjectType.DefaultWall);
                        break;
                }
            }
        }

        Debug.Log($"Room '{template.name}' built at position {position}");
    }

    private Transform CreateParent(string name, Transform parent)
    {
        GameObject parentObject = new GameObject(name);
        parentObject.transform.SetParent(parent);
        parentObject.transform.localPosition = Vector3.zero;
        parentObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        return parentObject.transform;
    }

    public void OnNavigationCreate()
    {
        NavMeshSurface navMeshSurface = _navigationFactory.CreateNavigation();
        _characters.NavMeshSurface = navMeshSurface;
        navMeshSurface.BuildNavMesh();
    }

    private List<Vector3> GetRoomExits(RoomTemplateSO.Template room, Vector3 matchedExit = default)
    {
        List<Vector3> exits = new List<Vector3>();
        if (room.leftExit == TemplateElementType.Exit && matchedExit != Vector3.right) exits.Add(Vector3.left);
        if (room.rightExit == TemplateElementType.Exit && matchedExit != Vector3.left) exits.Add(Vector3.right);
        if (room.topExit == TemplateElementType.Exit && matchedExit != Vector3.down) exits.Add(Vector3.up);
        if (room.bottomExit == TemplateElementType.Exit && matchedExit != Vector3.up) exits.Add(Vector3.down);
        return exits;
    }

    private class RoomConnection
    {
        public RoomTemplateSO.Template Room { get; set; }
        public Vector3 Position { get; set; }
        public List<Vector3> ConnectedExits { get; set; } = new List<Vector3>();

        public int OpenExitsCount => ConnectedExits.Count;

        public bool HasOpenExits => OpenExitsCount > 0;

        public void ConnectExit(Vector3 direction)
        {
            ConnectedExits.Remove(direction);
        }

        public bool HasExit(Vector3 direction)
        {
            if (direction == Vector3.left && Room.rightExit == TemplateElementType.Exit) return true;
            if (direction == Vector3.right && Room.leftExit == TemplateElementType.Exit) return true;
            if (direction == Vector3.up && Room.bottomExit == TemplateElementType.Exit) return true;
            if (direction == Vector3.down && Room.topExit == TemplateElementType.Exit) return true;
            return false;
        }
    }
}
