using Player;
using Pool;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prefab
{
    public enum CharacterType
    {
        none = 0,
        Warrior = 1,
        Wizard = 2,
        Archer = 3,
        Barbarian = 100,
        Thrower = 101,
        Summoner = 102,
    }

    public enum MonsterType
    {
        Monster1 = 100,
        Monster2 = 101,
        Monster3 = 102,
    }

    //public abstract class AstractCharacter<TControl, TCharacterType> where TCharacterType : Enum
    //{
    //    protected ObjectPooler<TCharacterType> _pooler;
    //    public abstract TControl GetCharacter(TCharacterType characterType);
    //}

    //public class PlayerCharacter : AstractCharacter<IPlayerController, PlayerType>
    //{
    //    private readonly DiContainer _diContainer;
    //    private PlayerType _playerType;

    //    public PlayerCharacter(DiContainer diContainer , ObjectPooler<PlayerType> pooler)
    //    {
    //        _diContainer = diContainer;
    //        _pooler = pooler;
    //    }

    //    public override IPlayerController GetCharacter(PlayerType characterType)
    //    {
    //        IPlayerController controller = _diContainer.Resolve<IPlayerController>();
    //        IPlayerView view = _pooler.Pull<IPlayerView>(characterType);
    //        PlayerModel model = new PlayerModel()
    //    }
    //}

    public enum PlayerType 
    {
        Warrior = 1 ,
        Wizard = 2,
        Archer = 3,
    }

    public enum EnemyType 
    {
        Barbarian = 100,
        Thrower = 101,
        Summoner = 102,
    }

    public enum SlingShotType
    {
        Melee,
        Distance,
        Magic,
    }
    
    public abstract class PrefabHolder<T> : ScriptableObject where T : Enum
    {
        [Serializable]
        public class Mapper 
        {
            public T Key;
            public GameObject Value;
        }

        [SerializeField]
        protected List<Mapper> _prefabs;
        
         public GameObject GetPrefab(T prefabType)
         {
            for (int i = 0;i < _prefabs.Count;i++)
            {
                if (_prefabs[i].Key.Equals(prefabType))
                {
                    return _prefabs[i].Value;
                }
            }
            throw new System.ArgumentException(string.Format("Prefab of type {0} not exists at holder", prefabType));
         }
    }
}
