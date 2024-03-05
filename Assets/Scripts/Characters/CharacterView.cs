using Player;
using Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICharacterView
{
    public void Init(CharacterModel model, Rigidbody2D rb);

    bool IsMoving { get; set; }

    event Action<Transform, PointerEventData> ON_CLICK;
    event Action<PointerEventData> ON_BEGINDRAG;
}

public class CharacterView : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, ICharacterView, IMyPoolable
{
    public event Action<Transform, PointerEventData> ON_CLICK;
    public event Action<PointerEventData> ON_BEGINDRAG;
    private Transform _transform;
    private CharacterModel _characterModel;
    private float _currentVelocity;
    private Rigidbody2D _rigidbody;

    public bool IsMoving { get; set; }

    public void Init(CharacterModel model, Rigidbody2D rb)
    {
        _characterModel = model;
        _rigidbody = rb;
    }

    private void FixedUpdate()
    {
        if(IsMoving)
        {
            _characterModel.Velocity.Value = _rigidbody.velocity.magnitude;
        }
        
    }

    public void OnCreate()
    {
        //Debug.LogWarning($"Hello from {this.name} view");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _transform = transform;
        ON_CLICK?.Invoke(_transform, eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _transform = transform;
        ON_BEGINDRAG?.Invoke(eventData);
    }

    public void OnPull()
    {

    }

    public void OnRelease()
    {
    }
}

