using Enemy;
using Interactions;
using Obstacles;
using Player;
using Pool;
using CharactersStats;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Prefab;

public interface IInteractible
{
    void StartInteraction(IInteractible interactible);
    IControllerInputs ControllerInputs { get; set; }
}


public interface ICharacterView
{
    void Init(IControllerInputs controllerInputs);
    public void AddImpulse(Vector2 forceVector);
    public void ChangeDirection(Vector2 direction);
    bool IsMoving { get; set; }

    event Action<Transform, PointerEventData> ON_CLICK;
    event Action<PointerEventData> ON_BEGINDRAG;
    
}

public class CharacterView : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IBeginDragHandler,
    ICharacterView,
    IInteractible,
    IMyPoolable
{
    public event Action<Transform, PointerEventData> ON_CLICK;
    public event Action<PointerEventData> ON_BEGINDRAG;

    [SerializeField] Transform _viewTransform;
    public bool IsMoving { get; set; }
    private ModifiableStats _stats;

    public IControllerInputs ControllerInputs { get; set; } 

    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody { get { return _rigidbody; } }
    public void Init(IControllerInputs controllerInputs)
    {
        ControllerInputs = controllerInputs;
        _stats = controllerInputs.GetCharacterStats();
    }

    private void Start()
    {
        _rigidbody = gameObject.GetComponentInChildren<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (IsMoving)
        {
            _stats.Velocity.Value = _rigidbody.velocity.magnitude;
        }

        if (IsMoving) ViewRotation();
        //Debug.Log($"View {_model.Type} has {_model.Health} hp");
    }

    private void ViewRotation()
    {
        Vector3 velocity = _rigidbody.velocity;
        float rotationSpeed = velocity.magnitude;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);

        _viewTransform.rotation = Quaternion.Slerp(_viewTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void ChangeDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _viewTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    public void AddImpulse(Vector2 forceVector)
    {
        _rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
        _rigidbody.velocity = _rigidbody.velocity.normalized;
        IsMoving = true;
    }

    public void StartInteraction(IInteractible interactible)
    {
         IInteraction interactionFromDealer = ControllerInputs.GetInteraction();
         interactible.ControllerInputs.ApplyInteraction(interactionFromDealer);

    }

    public void OnCreate()
    {
        //Debug.LogWarning($"Hello from {this.name} view");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ON_CLICK?.Invoke(_viewTransform, eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        
        ON_BEGINDRAG?.Invoke(eventData);
    }

    public void OnPull()
    {
    }

    public void OnRelease()
    {
    }

    public Transform GetTransform()
    {
        return _viewTransform;
    }
}

