using Gameplay;
using Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IRewardPanelView : IMyPoolable
{
    public void Init(RewardUIPooler pooler, RewardType rewardType, int rewardCount);
    public event OnEndAnimation On_End_Animation;
}
public delegate void OnEndAnimation(RewardType rewardType, int rewardCount);
public class RewardPanelView : MonoBehaviour, IRewardPanelView
{
    private RewardUIPooler _rewardUIPooler;
    private RewardType _rewardType;
    private int _rewardCount;
    public event OnEndAnimation On_End_Animation;

    public void Init(RewardUIPooler pooler, RewardType rewardType, int rewardCount)
    {
        _rewardUIPooler = pooler;
        _rewardType = rewardType;
        _rewardCount = rewardCount;
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    void Start()
    {
        Invoke("PushPanelToPool",4f);
    }

    private void PushPanelToPool()
    {
        On_End_Animation?.Invoke(_rewardType,_rewardCount);
        Destroy(gameObject);
    }

    public void OnCreate()
    {
    }

    public void OnPull()
    {
    }

    public void OnRelease()
    {
    }
}
