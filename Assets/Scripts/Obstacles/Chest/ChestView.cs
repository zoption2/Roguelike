using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChestView
{
    public event OnAnimationEnd On_Animation_End;

    public event OnTryOpen On_Try_Open;

    public void TryOpenChest();
    public void Init(IChestOpener opener);
}

public delegate void OnAnimationEnd();
public delegate void OnTryOpen();

public class ChestView : MonoBehaviour, IChestView
{
    public event OnAnimationEnd On_Animation_End;
    public event OnTryOpen On_Try_Open;

    [SerializeField]
    private Animator _animator;

    private const string OPENED = "Opened";

    public void Init(IChestOpener opener)
    {
        opener.On_Chest_Open += OpenChest;
    }
    public void TryOpenChest()
    {
        On_Try_Open?.Invoke();
    }

    private void OpenChest()
    {
        _animator.SetTrigger(OPENED);
    }

    private void OnOpenAnimationEnd()
    {
        On_Animation_End?.Invoke();
    }
}
