using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChest
{
    public void UnlockChest();
    public void LockChest();
    public void TryOpenChest();
}
public class Chest : MonoBehaviour, IChest
{
    [SerializeField]
    private Animator _animator;

    private bool _isLocked = true;
    private bool _wasOpened = false;

    public void Start()
    {
        
    }

    public void LockChest()
    {
       _isLocked = true;
    }

    public void TryOpenChest()
    {
        if (!_isLocked && !_wasOpened )
        {
            TakeSomeStuff();
        }
        else
        {
            Debug.LogWarning("не заслужив ти сундук...");
        }
    }

    public void UnlockChest()
    {
        _isLocked = false;
    }

    private void TakeSomeStuff()
    {
        _wasOpened = true;
        Debug.LogWarning("Відкрив сундук і получив по будці");
    }

    private void GenerateSomeRundomStuff()
    {

    }
}
