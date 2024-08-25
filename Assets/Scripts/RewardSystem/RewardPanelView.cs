using UnityEngine;

public interface IRewardPanelView
{
    public void Init(CurrencyType rewardType, int rewardCount);
    public event OnEndAnimation ON_ANIMATION_END;
}
public delegate void OnEndAnimation(CurrencyType rewardType, int rewardCount);
public class RewardPanelView : MonoBehaviour, IRewardPanelView
{
    public event OnEndAnimation ON_ANIMATION_END;
    private CurrencyType _rewardType;
    private int _rewardCount;

    public void Init(CurrencyType rewardType, int rewardCount)
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
        ON_ANIMATION_END?.Invoke(_rewardType, _rewardCount);
        Destroy(gameObject);
    }
}
