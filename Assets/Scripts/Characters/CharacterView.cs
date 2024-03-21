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
    IInteractible GetInterationHandler(IInteractible interactible);
    void GetStatsAfterInteraction(Stats updatetInteractionHandlerStats);

    event Action<Stats> UPDATE_STATS;
    event Action<IInteractible> ON_COLLISION;
    event Action<Stats> ON_INTERACTION_FINISH;
}


public interface ICharacterView
{
    public void Init(CharacterModel model);
    public void AddImpulse(Vector2 forceVector);
    public void ChangeDirection(Vector2 direction);
    bool IsPlayerMoving { get; set; }
    //public bool IsActive { get; set; }

    event Action<Transform, PointerEventData> ON_CLICK;
    event Action<PointerEventData> ON_BEGINDRAG;
    
}

public class CharacterView : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, ICharacterView, IMyPoolable, IInteractible
{
    public event Action<Transform, PointerEventData> ON_CLICK;
    public event Action<PointerEventData> ON_BEGINDRAG;
    public event Action<IInteractible> ON_COLLISION;
    public event Action<Stats> UPDATE_STATS;
    public event Action<Stats> ON_INTERACTION_FINISH;

    [SerializeField] Transform _viewTransform;
    public bool IsPlayerMoving { get; set; }
    //public bool IsActive { get; set; }

    private CharacterModel _model;
    private CharacterModel _enemyModel;
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody { get { return _rigidbody; } }
    public void Init(CharacterModel model)
    {
        _model = model;
    }

    private void Start()
    {
        _rigidbody = gameObject.GetComponentInChildren<Rigidbody>();
    }

    private void Update()
    {
        if (IsPlayerMoving) ViewRotation();
    }

    private void FixedUpdate()
    {
        if (IsPlayerMoving)
        {
            _model.Velocity.Value = _rigidbody.velocity.magnitude;
        }

        if (IsPlayerMoving) ViewRotation();
        Debug.Log($"View {_model.Type} has {_model.Health} hp");
        if(_model.ReactiveHealth.Value <= 0)
        {
            Destroy(gameObject);
        }
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
        IsPlayerMoving = true;
    }

    public IInteractible GetInterationHandler(IInteractible interactible)
    {
        //if (IsActive)
        //{
        //    ON_COLLISION?.Invoke(interactible);
        //}
        ON_COLLISION?.Invoke(interactible);////
        return null;
    }

    public void GetStatsAfterInteraction(Stats updatetInteractionHandlerStats)
    {
        ON_INTERACTION_FINISH?.Invoke(updatetInteractionHandlerStats);
    }

    //////////////////////////////////////////////////
    //public void ProcessInteractions(Queue<IInteraction> queue)
    //{
    //    Stats _interationHandlerStatsCopy = _model.GetStats();
    //    foreach (IInteraction interaction in queue)
    //    {
    //        Debug.LogWarning($"HP before attack: {_model.Health}");
    //        Stats statsAfterInteraction = interaction.Interacte(_interationHandlerStatsCopy);
    //        _model.ReactiveHealth.Value = statsAfterInteraction.Health;
    //        Debug.LogWarning($"Attack type: {interaction.GetType()}, HP after attack: {_interationHandlerStatsCopy.Health}");
    //        queue.Dequeue();
    //    }
    //}
    //////////////////////////////////////////////////

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
}

