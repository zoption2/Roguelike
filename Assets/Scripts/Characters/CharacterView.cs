using Interactions;
using Pool;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public interface IMovable
{
    void ApplyForce(Rigidbody providerRb, Rigidbody handlerRb);
}
public interface IInteractible
{
    void StartInteraction(IInteractible interactible);
    IControllerInputs ControllerInputs { get; set; }
    Rigidbody Rigidbody { get; set; }
}
public interface ICharacterView
{
    void Init(IControllerInputs controllerInputs);
    public void ChangeDirection(Vector2 direction);

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
    public NavMeshObstacle NavMeshObstacle { get; set; }
    public IControllerInputs ControllerInputs { get; set; } 
    public Rigidbody Rigidbody { get; set; }
    public float MaxVelocity = 50f;

    public void Init(IControllerInputs controllerInputs)
    {
        ControllerInputs = controllerInputs;
        NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        NavMeshObstacle = gameObject.GetComponent<NavMeshObstacle>();
    }

    private void Awake()
    {
        Rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ControllerInputs.DoUpdate();
    }

    public void ChangeDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        transform.rotation = targetRotation;
    }

    public void StartInteraction(IInteractible interactible)
    {
        var dealerType = ControllerInputs.GetType();
        var handlerType = interactible.ControllerInputs.GetType();

        IInteraction interactionFromDealer = ControllerInputs.GetInteraction();

        if (!dealerType.Equals(handlerType))
        {
            
            interactible.ControllerInputs.ApplyInteraction(interactionFromDealer);
        }
        else
        {
            //Debug.LogWarning("INTERACTION CANCELED");
            return;
        }
        IMovable bump = interactionFromDealer.GetBump();
        bump.ApplyForce(interactible.Rigidbody, Rigidbody);
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

    public Transform GetTransform()
    {
        return _viewTransform;
    }
}

