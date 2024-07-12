using Gameplay;
using Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IRewardPanelView
{
    public void Init(RewardType rewardType, int rewardCount);
    public event OnEndAnimation On_End_Animation;
}
public delegate void OnEndAnimation(RewardType rewardType, int rewardCount);
public class RewardPanelView : MonoBehaviour, IRewardPanelView
{
    public event OnEndAnimation On_End_Animation;
    private RewardType _rewardType;
    private int _rewardCount;

    public void Init(RewardType rewardType, int rewardCount)
    {
        _rewardType = rewardType;
        _rewardCount = rewardCount;
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    void Start()
    {
        Invoke("DestroyPanel", 4f);
    }

    private void DestroyPanel()
    {
        On_End_Animation?.Invoke(_rewardType,_rewardCount);
        Destroy(gameObject);
    }
}
