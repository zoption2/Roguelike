using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChestView
{
    public event OnChestOpen On_Chest_Open;
    public void UnlockChest();
    public void LockChest();
    public void TryOpenChest();
}

public delegate void OnChestOpen(RewardType type,int count);
public class ChestView : MonoBehaviour, IChestView
{
    public event OnChestOpen On_Chest_Open;

    [SerializeField]
    private Animator _animator;

    private const string OPENED = "Opened";

    private bool _isLocked = true;
    private bool _wasOpened = false;

    private RewardType _typeOfReward;
    private int _rewardCount;

    public void Start()
    {
        _typeOfReward = RewardType.Coin;
        _rewardCount = 10;
    }

    public void LockChest()
    {
       _isLocked = true;
    }

    public void TryOpenChest()
    {
        if (!_isLocked && !_wasOpened )
        {
            OpenChest();
        }
        else
        {
            Debug.LogWarning("Ñhest won't open...");
        }
    }

    public void UnlockChest()
    {
        _isLocked = false;
    }

    private void OpenChest()
    {
        _wasOpened = true;
        _animator.SetTrigger(OPENED);
        Debug.LogWarning("Opened chest");
    }

    private void TakeSomeStuff()
    {
        On_Chest_Open?.Invoke(_typeOfReward,_rewardCount);
    }
}
