using System.Collections.Generic;


public interface IRewardService
{
    public void ShowReward(CurrencyType type, int count);
    public void AddReward(CurrencyType type, int count);
    public void RemoveReward(CurrencyType type, int count);
    public void Init();
}
public class RewardService : IRewardService
{
    private Dictionary<CurrencyType, int> _rewards;
    private IRewardPanelFactory _rewardPanelFactory;
    private ICurrencyManager _currencyManager;

    public RewardService(IRewardPanelFactory uIFactory, ICurrencyManager currencyManager)
    {
        _rewardPanelFactory = uIFactory;
        _currencyManager = currencyManager;
    }

    public void Init()
    {
        _rewards = new Dictionary<CurrencyType, int>();
    }

    public void ShowReward(CurrencyType type, int count)
    {
        IRewardPanelView rewardPanel = _rewardPanelFactory.CreateRewardPanel(type, count);
        rewardPanel.ON_ANIMATION_END += AddReward;
    }

    public void AddReward(CurrencyType type, int count)
    {
        _currencyManager.AddSomeCurrency(type, count);
    }

    public void RemoveReward(CurrencyType type, int count)
    {
        if (_rewards.ContainsKey(type))
        {
            _rewards[type] -= count;
        }
    }
}
