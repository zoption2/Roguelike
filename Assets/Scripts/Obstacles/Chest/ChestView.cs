using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChestView
{

    public event OnTryOpen On_Try_Open;

    public void TryOpenChest();
    public void OpenChest();
    public void OpenChest(Action onOpened);

}

public delegate void OnTryOpen();

public class ChestView : MonoBehaviour, IChestView
{
    public event OnTryOpen On_Try_Open;

    [SerializeField]
    private Animator _animator;

    private const string OPENED = "Opened";

    public void TryOpenChest()
    {
        On_Try_Open?.Invoke();
    }

    public void OpenChest()
    {
        _animator.SetTrigger(OPENED);
    }
    public async void OpenChest(Action onOpened)
    {
        _animator.SetTrigger(OPENED);
        float duration = _animator.GetCurrentAnimatorStateInfo(0).length;
        int delay = (int)duration * 1000;
        await UniTask.Delay(delay);
        onOpened?.Invoke();
    }
}
