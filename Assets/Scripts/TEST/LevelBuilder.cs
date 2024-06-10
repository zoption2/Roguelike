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
    public int RoomCount { get; set; } = 5;

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
        _roomConnections.Add(new RoomConnection { Room = mainRoom, Position = Vector3.zero, ConnectedExits = new List<Vector3> { Vector3.right, Vector3.down } });

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
        // Randomly determine the number of exits (between 1 and 4)
        int exitCount = UnityEngine.Random.Range(1, 5);

        List<Vector3> exits = new List<Vector3>
        {
            mainRoom.leftExit == TemplateElementType.Exit ? new Vector3(0, 0, -1) : Vector3.zero,
            mainRoom.rightExit == TemplateElementType.Exit ? new Vector3(0, 0, 1) : Vector3.zero,
            mainRoom.topExit == TemplateElementType.Exit ? new Vector3(1, 0, 0) : Vector3.zero,
            mainRoom.bottomExit == TemplateElementType.Exit ? new Vector3(-1, 0, 0) : Vector3.zero
        };

        exits = exits.Where(e => e != Vector3.zero).OrderBy(x => UnityEngine.Random.value).Take(exitCount).ToList();

        foreach (var exit in exits)
        {
            // Set the exit type in the main room template
            if (exit == new Vector3(0, 0, -1))
                mainRoom.leftExit = TemplateElementType.Exit;
            else if (exit == new Vector3(0, 0, 1))
                mainRoom.rightExit = TemplateElementType.Exit;
            else if (exit == new Vector3(1, 0, 0))
                mainRoom.topExit = TemplateElementType.Exit;
            else if (exit == new Vector3(-1, 0, 0))
                mainRoom.bottomExit = TemplateElementType.Exit;
        }

        Debug.Log($"Main room exits set: {exitCount} exits");
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
        var direction = randomOpenConnection.ConnectedExits[0]; // беремо перший відкритий вихід
        var matchingExitDirection = GetOppositeDirection(direction);

        var availableTemplates = _templates
            .Where(t => t.name != "MainRoom" && t.HasExit(matchingExitDirection))
            .ToList();

        if (!availableTemplates.Any())
        {
            Debug.LogError("No available templates with required exits.");
            return false;
        }

        var newRoomTemplate = availableTemplates[UnityEngine.Random.Range(0, availableTemplates.Count)];
        var newRoomPosition = randomOpenConnection.Position + direction;

        randomOpenConnection.ConnectExit(direction);
        _roomConnections.Add(new RoomConnection { Room = newRoomTemplate, Position = newRoomPosition, ConnectedExits = new List<Vector3> { matchingExitDirection } });

        Debug.Log($"Room '{newRoomTemplate.name}' added at position {newRoomPosition}");
        return true;
    }


    private Vector3 GetOppositeDirection(Vector3 direction)
    {
        if (direction == Vector3.right) return Vector3.left;
        if (direction == Vector3.left) return Vector3.right;
        if (direction == Vector3.up) return Vector3.down;
        if (direction == Vector3.down) return Vector3.up;
        return Vector3.zero;
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

        //CreatePlayers(playersParent);
        //CreateEnemies(enemiesParent);
        //CreateBuffs(buffsParent);

        OnNavigationCreate();

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

    private void CreateBuffs(Transform parent)
    {
        var buffTypes = Enum.GetValues(typeof(BuffType)).Cast<BuffType>().Where(t => t != BuffType.None).ToList();
        var shuffledSpawnPoints = _buffSpawnPoints.OrderBy(x => UnityEngine.Random.value).ToList();

        int buffsToSpawn = Math.Min(BuffCount, shuffledSpawnPoints.Count);

        for (int i = 0; i < buffsToSpawn; i++)
        {
            Vector3 spawnPosition = shuffledSpawnPoints[i];
            BuffType buffType = buffTypes[UnityEngine.Random.Range(0, buffTypes.Count)];
            _buffFactory.CreateBuff(spawnPosition, parent, buffType);
        }
    }

    private void CreatePlayers(Transform parent)
    {
        for (int i = 0; i < _playerSpawnPoints.Count; i++)
        {
            Vector3 position = _playerSpawnPoints[i];
            CharacterType playerType = DataTransfer.TypeCollection[i];
            IPlayerController newPlayer = _playerFactory.CreatePlayer(position, parent, playerType);
            newPlayer.SetCharacterContext(_characters);
            _characters.Players.Add(newPlayer);
        }
    }

    private void CreateEnemies(Transform parent)
    {
        var enemyTypes = Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().Where(t => t != EnemyType.None).ToList();
        var shuffledSpawnPoints = _enemySpawnPoints.OrderBy(x => UnityEngine.Random.value).ToList();

        int enemiesToSpawn = Math.Min(EnemyCount, shuffledSpawnPoints.Count);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 spawnPosition = shuffledSpawnPoints[i];
            CharacterType enemyType = (CharacterType)enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count)];
            IEnemyController newEnemy = _enemyFactory.CreateEnemy(spawnPosition, parent, enemyType);
            newEnemy.SetCharacterContext(_characters);
            _characters.Enemies.Add(newEnemy);
        }
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
