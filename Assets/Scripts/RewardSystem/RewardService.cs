using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public interface IRewardService
{
    public void ShowReward(RewardType type, int count);
    public void AddReward(RewardType type,int count);
    public void RemoveReward(RewardType type, int count);
    public void Init();
}
public class RewardService : IRewardService 
{
    private Dictionary<RewardType,int> _rewards;
    private IRewardPanelFactory _rewardPanelFactory;

    public RewardService(IRewardPanelFactory uIFactory)
    {
        _rewardPanelFactory = uIFactory;
    }

    public void Init()
    {
        _rewards = new Dictionary<RewardType, int>();
    }

    public void ShowReward(RewardType type, int count)
    {
        IRewardPanelView rewardPanel = _rewardPanelFactory.CreateRewardPanel(type,count);
        rewardPanel.On_End_Animation += AddReward;
    }

    public void AddReward(RewardType type, int count)
    {
        if (_rewards.ContainsKey(type))
        {
            Debug.Log("added reward count");
            _rewards[type] += count;
        }
        else
        {
            Debug.Log("added reward type to dictionary");
            _rewards.Add(type, count);
        }
    }

    public void RemoveReward(RewardType type, int count)
    {
        if (_rewards.ContainsKey(type))
        {
            _rewards[type] -= count;
        }
    }
}
