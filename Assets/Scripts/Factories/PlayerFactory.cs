using CharactersStats;
using Player;
using Pool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Abilities;
using Cysharp.Threading.Tasks;

public interface IPlayerFactory
{
    public UniTask<IPlayerController> CreatePlayerAsync(Vector3 position, Transform parent, CharacterType type);
}

public class PlayerFactory : CharacterFactory<IPlayerController>, IPlayerFactory
{
    public PlayerFactory(DiContainer container, IStatsProvider statsProvider, IPoolManager poolManager, IAbilityFactory abilityFactory) : base(container, statsProvider, poolManager, abilityFactory)
    {
    }

    protected override OriginStats GetStats(CharacterType type)
    {
        return _statsProvider.GetPlayerStats(type);
    }

    protected override List<AbilityType> GetAbilitiesTypes(CharacterType type)
    {
        return _statsProvider.GetCharacterAbilitiesTypes(type);
    }

    public async UniTask<IPlayerController> CreatePlayerAsync(Vector3 position, Transform parent, CharacterType type)
    {
        IPlayerController playerController = await base.CreateCharacterAsync(position, parent, type);
        return playerController;
    }
}