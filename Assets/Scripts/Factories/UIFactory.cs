using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

public interface IUIFactory
{
    public IEffectIconView CreateEffectIcon(EffectType type, Vector3 position, Transform parent);
    public void RemoveEffectIcon(EffectType type, IEffectIconView myPoolable);
    public IAbilityIconView CreateAbilityIcon(AbilityType type, Vector3 position, Transform parent);
    public IRewardPanelView CreateRewardPanel(RewardType type, int count);
}

public class UIFactory : IUIFactory
{
    private EffectPooler _effectPooler;
    private AbilityIconPooler _abilityIconPooler;
    private RewardUIPooler _rewardUIPooler;

    [Inject]
    public void Construct(IPoolManager poolManager)
    {
        _effectPooler = poolManager.UseEffectPooler();
        _abilityIconPooler = poolManager.UseAbilityIconPooler();
        _rewardUIPooler = poolManager.UseRewardUIPooler();
    }

    public IEffectIconView CreateEffectIcon(EffectType type, Vector3 position, Transform parent)
    {
        IEffectIconView effectIcon =  _effectPooler.Pull<IEffectIconView>(type, position, Quaternion.Euler(90, 0, 0), parent);
        return effectIcon;
    }

    public void RemoveEffectIcon(EffectType type, IEffectIconView myPoolable)
    {
        _effectPooler.Push(type, myPoolable);
    }

    public IAbilityIconView CreateAbilityIcon(AbilityType type, Vector3 position, Transform parent)
    {
        IAbilityIconView abilityIcon =  _abilityIconPooler.Pull<IAbilityIconView>(type, position, Quaternion.Euler(90, 0, 0), parent);
        return abilityIcon;
    }

    public IRewardPanelView CreateRewardPanel(RewardType type, int count)
    {
        IRewardPanelView rewardPanel = _rewardUIPooler.Pull<IRewardPanelView>(type, Vector3.zero, Quaternion.identity);
        rewardPanel.Init(_rewardUIPooler, type, count);
        return rewardPanel;
    }
}
