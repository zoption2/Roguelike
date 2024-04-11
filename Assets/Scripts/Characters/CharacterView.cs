using Interactions;
using Pool;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Prefab;
using UnityEngine.AI;

public interface IMovable
{
    void ApplyForce(Rigidbody providerRb, Rigidbody handlerRb);
}
public interface IInteractible
{
    void StartInteraction(IInteractible interactible);
    IControllerInputs ControllerInputs { get; set; }
}
public interface ICharacterView
{
    void Init(IControllerInputs controllerInputs);
    public void ChangeDirection(Vector2 direction);

    //void HandleMovement(CharacterView otherView);
    bool IsMoving { get; set; }

    event Action<Transform, PointerEventData> ON_CLICK;
    event Action<PointerEventData> ON_BEGINDRAG;
    event OnStopMovement ON_STOP_MOVEMENT;
}

public delegate void OnStopMovement();

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
    public event OnStopMovement ON_STOP_MOVEMENT;

    [SerializeField] Transform _viewTransform;
    public bool IsMoving { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public IControllerInputs ControllerInputs { get; set; } 
    public Rigidbody Rigidbody { get { return _rigidbody; } }
    private Rigidbody _rigidbody;
    private float _maxVelocity = 50f;

    public void Init(IControllerInputs controllerInputs)
    {
        ControllerInputs = controllerInputs;
        NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _rigidbody = gameObject.GetComponentInChildren<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_rigidbody.velocity.magnitude > _maxVelocity)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _maxVelocity;
        }


        if (_rigidbody.velocity.magnitude > 0.5f && !IsMoving)
        {
            IsMoving = true;
        }
        else if (_rigidbody.velocity.magnitude < 0.2f && _rigidbody.velocity.magnitude > 0f && IsMoving)
        {
            IsMoving = false;
            ON_STOP_MOVEMENT?.Invoke();
        }

        if (IsMoving)
        {
            ViewRotation();
        } 
    }

    private void ViewRotation()
    {
        Vector3 velocity = _rigidbody.velocity;
        float rotationSpeed = velocity.magnitude;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRotation;

        IConditionState conditionState = ControllerInputs.GetCurrentConditionState();
        if (conditionState.GetType() == typeof(StunState))
        {
            targetRotation = Quaternion.Euler(0f, 0f, angle + 90f);
        }
        else
        {
            targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        _viewTransform.rotation = Quaternion.Slerp(_viewTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    public void ChangeDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _viewTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    public void StartInteraction(IInteractible interactible)
    {
        //реалізувати логіку обробки прискорення тут
        var dealerType = ControllerInputs.GetType();
        var handlerType = interactible.ControllerInputs.GetType();

        if (!dealerType.Equals(handlerType) && ControllerInputs.GetActiveStatus())
        {
            IInteraction interactionFromDealer = ControllerInputs.GetInteraction();
            interactible.ControllerInputs.ApplyInteraction(interactionFromDealer);
        } 
        else
        {
            Debug.LogWarning("INTERACTION CANCELED");
            return;
        }
    }

    //public void HandleMovement(CharacterView otherView)
    //{
    //    if(ControllerInputs.GetActiveStatus())
    //    {
    //        IMovable movableBehaviour = new StopAndPush();
    //        movableBehaviour.ApplyForce(_rigidbody, otherView._rigidbody);
    //    }
    //    else
    //    {
    //        IMovable movableBehaviour = new Bounce();
    //        movableBehaviour.ApplyForce(_rigidbody, otherView._rigidbody);
    //    }
        
    //}

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

    public void TrySkipTurn()
    {
        ON_STOP_MOVEMENT?.Invoke();
    }


    public Transform GetTransform()
    {
        return _viewTransform;
    }
}

