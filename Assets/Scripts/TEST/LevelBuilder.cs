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
    }

    public void BuildLevel()
    {
        AnalyzeTemplate();
        BuildRoom();
    }

    private void AnalyzeTemplate()
    {
        RoomTemplateSO.Template template = _roomTemplate.Templates.FirstOrDefault(t => t.name == "TEST BUILD");
        if (template == null)
        {
            Debug.LogError("Template not found");
            return;
        }

        var templateElements = template.TemplateElement;
        var coordinates = template.Coordinates;

        _playerSpawnPoints = new List<Vector3>();
        _enemySpawnPoints = new List<Vector3>();
        _buffSpawnPoints = new List<Vector3>();

        for (int i = 0; i < templateElements.GetLength(0); i++)
        {
            for (int j = 0; j < templateElements.GetLength(1); j++)
            {
                TemplateElement elementType = templateElements[i, j];
                Vector3 position = coordinates[i, j];

                switch (elementType)
                {
                    case TemplateElement.Player:
                        _playerSpawnPoints.Add(position);
                        break;

                    case TemplateElement.Enemy:
                        _enemySpawnPoints.Add(position);
                        break;

                    case TemplateElement.Buff:
                        _buffSpawnPoints.Add(position);
                        break;
                }
            }
        }
    }

    private void BuildRoom()
    {
        RoomTemplateSO.Template template = _roomTemplate.Templates.FirstOrDefault(t => t.name == "TEST BUILD");
        if (template == null)
        {
            Debug.LogError("Template not found");
            return;
        }

        GameObject roomObject = new GameObject(template.name);
        roomObject.transform.position = new Vector3(0, 0, 0);

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
                TemplateElement elementType = templateElements[i, j];
                Vector3 position = coordinates[i, j];
                Vector3 floorPosition = new Vector3(position.x, position.y - 1, position.z);

                _roomObjectsFactory.Build(floorPosition, floorsParent, RoomObjectType.Floor);

                switch (elementType)
                {
                    case TemplateElement.DefaultWall:
                        _roomObjectsFactory.Build(position, wallsParent, RoomObjectType.DefaultWall);
                        break;
                }
            }
        }

        CreatePlayers(playersParent);
        CreateEnemies(enemiesParent);
        CreateBuffs(buffsParent);

        OnNavigationCreate();
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
}
