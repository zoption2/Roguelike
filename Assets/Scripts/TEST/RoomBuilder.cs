using System.Linq;
using UnityEngine;
using Zenject;

public interface IRoomBuilder
{
    public void Init();
    public void BuildRoom();
}

public class RoomBuilder : IRoomBuilder
{
    IPlayerFactory _playerFactory;
    IEnemyFactory _enemyFactory;
    IBuffFactory _buffFactory;
    IRoomObjectsFactory _roomObjectsFactory;
    RoomTemplateSO _roomTemplateSO;

    [Inject]
    public void Construct(
        IPlayerFactory playerFactory,
        IEnemyFactory enemyFactory,
        IBuffFactory buffFactory,
        IRoomObjectsFactory roomObjectsFactory,
        RoomTemplateSO roomTemplateSO
        )
    {
        _playerFactory = playerFactory;
        _enemyFactory = enemyFactory;
        _buffFactory = buffFactory;
        _roomObjectsFactory = roomObjectsFactory;
        _roomTemplateSO = roomTemplateSO;
    }

    public void Init()
    {
        _buffFactory.Init();
        _roomObjectsFactory.Init();
    }

    public void BuildRoom()
    {
        RoomTemplateSO.Template template = _roomTemplateSO.Templates.FirstOrDefault(t => t.name == "TEST BUILD");
        if (template == null)
        {
            Debug.LogError("Template not found");
            return;
        }

        GameObject roomObject = new GameObject(template.name);
        roomObject.transform.position = new Vector3(0, 0, 0);

        Transform playersParent = new GameObject("Players").transform;
        playersParent.SetParent(roomObject.transform);
        playersParent.localPosition = Vector3.zero;

        Transform enemiesParent = new GameObject("Enemies").transform;
        enemiesParent.SetParent(roomObject.transform);
        enemiesParent.localPosition = Vector3.zero;

        Transform buffsParent = new GameObject("Buffs").transform;
        buffsParent.SetParent(roomObject.transform);
        buffsParent.localPosition = Vector3.zero;

        Transform wallsParent = new GameObject("Walls").transform;
        wallsParent.SetParent(roomObject.transform);
        wallsParent.localPosition = Vector3.zero;

        Transform floorsParent = new GameObject("Floors").transform;
        floorsParent.SetParent(roomObject.transform);
        floorsParent.localPosition = Vector3.zero;

        var templateElements = template.TemplateElement;
        var coordinates = template.Coordinates;

        for (int i = 0; i < templateElements.GetLength(0); i++)
        {
            for (int j = 0; j < templateElements.GetLength(1); j++)
            {
                TemplateElement elementType = templateElements[i, j];
                if (elementType != TemplateElement.None)
                {
                    Vector3 position = coordinates[i, j];
                    Vector3 floorPosition = new Vector3(position.x, position.y, position.z + 1);

                    _roomObjectsFactory.Build(floorPosition, floorsParent, RoomObjectType.Floor);

                    switch (elementType)
                    {
                        case TemplateElement.Player:
                            _playerFactory.CreatePlayer(position, playersParent, (CharacterType)elementType);
                            break;

                        case TemplateElement.Enemy:
                            _enemyFactory.CreateEnemy(position, enemiesParent, (CharacterType)elementType);
                            break;

                        case TemplateElement.Buff:
                            _buffFactory.CreateBuff(position, buffsParent, (BuffType)elementType);
                            break;

                        case TemplateElement.DefaultWall:
                            _roomObjectsFactory.Build(position, wallsParent, RoomObjectType.DefaultWall);
                            break;
                    }
                }
            }
        }
    }
}
