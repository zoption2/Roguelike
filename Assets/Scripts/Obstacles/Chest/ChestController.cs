using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void OnChestInteraction(RewardType type, int count);
public delegate void OnChestOpen();


public interface IChestOpener
{
    public event OnChestOpen On_Chest_Open;
}
public interface IChestController : IChestOpener
{
    public IChestView ChestView { get; set; }
    public ChestModel ChestModel { get; set; }

    public event OnChestInteraction On_Chest_Interact;
    public void Init(IChestView chestView, ChestModel chestModel);
    public void LockChest();
    public void UnlockChest();

}
public class ChestController : IChestController
{
    public IChestView ChestView { get; set; }
    public ChestModel ChestModel { get; set; }

    public event OnChestInteraction On_Chest_Interact;

    public event OnChestOpen On_Chest_Open;

    private bool _isLocked, _wasOpened;

    public void Init(IChestView chestView, ChestModel chestModel)
    {
        ChestView = chestView;
        ChestModel = chestModel;

        ChestView.On_Try_Open += TryOpenChest;
        ChestView.On_Animation_End += TakeSomeStuff;

        ChestModel.TypeOfReward = RewardType.Coin;
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
        On_Chest_Open?.Invoke();
    }

    private void TakeSomeStuff()
    {
        On_Chest_Interact?.Invoke(ChestModel.TypeOfReward, ChestModel.RewardCount);
        UnsubscribeEvents();
    }

    private void UnsubscribeEvents()
    {
        On_Chest_Interact = null;
        On_Chest_Open = null;
        ChestView.On_Animation_End -= TakeSomeStuff;
        ChestView.On_Try_Open -= TryOpenChest;
    }
}
