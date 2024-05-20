using Interactions;
using Obstacles;
using Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Exit : MonoBehaviour, ICompleatedRoomTrigger, IMyPoolable
{
    [SerializeField] private TriggerType _type;
    private BoxCollider _collider;

    [Inject]
    private TriggerPooler _pooler;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = false;
    }

    public void ActivateTrigger()
    {
        Debug.LogWarning("Exit trigger!");
        _pooler.Init();
    }

    public void UseTrigger()
    {
        throw new System.NotImplementedException();
    }

    public void DisableTrigger()
    {
        _pooler.Push(_type, this);
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
