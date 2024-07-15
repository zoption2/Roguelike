using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prefab;

public interface IRewardPanelFactory
{
    public IRewardPanelView CreateRewardPanel(CurrencyType type, int count);
}
public class RewardPanelFactory : IRewardPanelFactory
{
    private RewardUIPrefabHolder _holder;

    public RewardPanelFactory(RewardUIPrefabHolder holder)
    {
        _holder = holder;
    }
    public IRewardPanelView CreateRewardPanel(CurrencyType type, int count)
    {
        GameObject prefab = _holder.GetPrefab(type);
        GameObject rewardPanelObject = GameObject.Instantiate(prefab);
        IRewardPanelView rewardView = rewardPanelObject.GetComponent<IRewardPanelView>();
        rewardView.Init(type, count);
        return rewardView;
    }
}
