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
    
    public NavMeshAgent NavMeshAgent { get; set; }
    public IControllerInputs ControllerInputs { get; set; } 
    public Rigidbody Rigidbody { get { return _rigidbody; } }
    private Rigidbody _rigidbody;
    public float MaxVelocity = 50f;

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
        ControllerInputs.DoUpdate();
    }

    public void ChangeDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _viewTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    public void StartInteraction(IInteractible interactible)
    {
        var dealerType = ControllerInputs.GetType();
        var handlerType = interactible.ControllerInputs.GetType();

        IInteraction interactionFromDealer = ControllerInputs.GetInteraction();
        interactible.ControllerInputs.ApplyInteraction(interactionFromDealer);
        //if (!dealerType.Equals(handlerType))
        //{
        //    IInteraction interactionFromDealer = ControllerInputs.GetInteraction();
        //    interactible.ControllerInputs.ApplyInteraction(interactionFromDealer);
        //} 
        //else
        //{
        //    Debug.LogWarning("INTERACTION CANCELED");
        //    return;
        //}
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
        //ON_STOP_MOVEMENT?.Invoke();
    }


    public Transform GetTransform()
    {
        return _viewTransform;
    }
}

