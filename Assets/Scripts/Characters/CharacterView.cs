using Interactions;
using Pool;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using System.Collections.Generic;

public interface IMovable
{
    public void ApplyForce(IInteractible provider, IInteractible handler);
}
public interface IInteractible
{
    public void StartInteraction(IInteractible interactible);
    public IControllerInputs ControllerInputs { get; set; }
    public Vector3 Normal {  get; set; }
    public Rigidbody GetRigidbody();
    public Vector3 GetLastVelocity();
    public Vector3 GetVelocity();
}
public interface ICharacterView
{
    public Transform transform { get; }
    public void Init(IControllerInputs controllerInputs);
    public void ChangeDirection(Vector3 direction);
    public Rigidbody GetRigidbody();

    public event Action<Transform, PointerEventData> ON_CLICK;
    public event Action<PointerEventData> ON_BEGINDRAG;
    
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
    [SerializeField] Transform _projectileSpawn;
    public ParticleSystem ParticleTestSystem { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public NavMeshObstacle NavMeshObstacle { get; set; }
    public IControllerInputs ControllerInputs { get; set; }
    public Vector3 Normal { get; set; }

    public float MaxVelocity = 50f;
    private Rigidbody _rigidbody;
    private CollisionHandler _collisionHandler;
    private Queue<Vector3> _lastVelocities = new(2);

    public void Init(IControllerInputs controllerInputs)
    {
        ControllerInputs = controllerInputs;
        NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        NavMeshObstacle = gameObject.GetComponent<NavMeshObstacle>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _collisionHandler = gameObject.AddComponent<CollisionHandler>();
        _collisionHandler.Init(ControllerInputs, this);
    }

    private void FixedUpdate()
    {
        _lastVelocities.Enqueue(_rigidbody.velocity);

        if (_lastVelocities.Count > 2)
        {
            _lastVelocities.Dequeue();
        }

        ControllerInputs.DoUpdate();
    }

    public Vector3 GetLastVelocity()
    {
        return _lastVelocities.Dequeue();
    }

    public Vector3 GetVelocity()
    {
        return _rigidbody.velocity;
    }

    public void ChangeDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);
        _viewTransform.rotation = targetRotation;
    }

    public void StartInteraction(IInteractible interactible)
    {
        var dealerType = ControllerInputs.GetType();
        var handlerType = interactible.ControllerInputs.GetType();

        IInteraction interactionFromDealer = ControllerInputs.GetInteraction();
        
        if (!dealerType.Equals(handlerType) && interactionFromDealer != null)
        {
            interactible.ControllerInputs.ApplyInteraction(interactionFromDealer);
        }
        else
        {
            return;
        }

        IMovable bump = interactionFromDealer.GetBump();
        ControllerInputs.ApplyBump(interactible, bump);
    }

    public void OnCreate()
    {
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

    public Rigidbody GetRigidbody()
    {
        return _rigidbody;
    }

    public Transform GetTransform()
    {
        return _viewTransform;
    }

    public Transform GetProjectileSpawn()
    {
        return _projectileSpawn;
    }
}

