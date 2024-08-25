using System;


public delegate void OnChestInteraction(CurrencyType type, int count);
public delegate void OnChestOpen(Action onOpened);


public interface IChestController
{
    public IChestView ChestView { get; set; }
    public ChestModel ChestModel { get; set; }

    public void Init(IChestView chestView, ChestModel chestModel);
    public void LockChest();
    public void UnlockChest();

}
public class ChestController : IChestController
{
    public IChestView ChestView { get; set; }
    public ChestModel ChestModel { get; set; }

    private bool _isLocked, _wasOpened;

    private IRewardService _rewardService;

    public ChestController(IRewardService rewardService)
    {
        _rewardService = rewardService;
    }

    public void Init(IChestView chestView, ChestModel chestModel)
    {
        ChestView = chestView;
        ChestModel = chestModel;

        ChestView.ON_TRY_OPEN += TryOpenChest;

        ChestModel.TypeOfReward = CurrencyType.Coin;
        ChestModel.RewardCount = 10;
    }

    public void LockChest()
    {
        _isLocked = true;
    }

    public void UnlockChest()
    {
        _isLocked = false;
    }

    public void TryOpenChest()
    {
        if (!_isLocked && !_wasOpened)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        _wasOpened = true;
        ChestView.OpenChest(() =>
        {
            TakeSomeStuff();
        });
    }

    private void TakeSomeStuff()
    {
        _rewardService.ShowReward(ChestModel.TypeOfReward, ChestModel.RewardCount);
        UnsubscribeEvents();
    }

    private void UnsubscribeEvents()
    {
        ChestView.ON_TRY_OPEN -= TryOpenChest;
    }
}
