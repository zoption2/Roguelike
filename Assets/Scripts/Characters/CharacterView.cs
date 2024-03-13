using Enemy;
using Obstacles;
using Player;
using Pool;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICharacterView
{
    public void Init(SavedPlayerModel model);
    public void Init(EnemyModel model);
    public void AddImpulse(Vector2 forceVector);
    public void ChangeDirection(Vector2 direction);
    bool IsMoving { get; set; }

    event Action<Transform, PointerEventData> ON_CLICK;
    event Action<PointerEventData> ON_BEGINDRAG;
}

public class CharacterView : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, ICharacterView, IMyPoolable
{
    public event Action<Transform, PointerEventData> ON_CLICK;
    public event Action<PointerEventData> ON_BEGINDRAG;

    [SerializeField] Transform _viewTransform;
    public bool IsMoving { get; set; }

    private SavedPlayerModel _playerModel;
    private EnemyModel _enemyModel;
    private Rigidbody _rigidbody;
    public void Init(SavedPlayerModel model)
    {
        _playerModel = model;
    }

    public void Init(EnemyModel model)
    {
        _enemyModel = model;
    }

    private void Start()
    {
        _rigidbody = gameObject.GetComponentInChildren<Rigidbody>();
    }

    private void Update()
    {
        Vector3 velocity = _rigidbody.velocity;
        float rotationSpeed = velocity.magnitude;

        if (velocity.magnitude > 0.5f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);

            _viewTransform.rotation = Quaternion.Slerp(_viewTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if(IsMoving)
        {
            _playerModel.Velocity.Value = _rigidbody.velocity.magnitude;
        } 
    }

    public void ChangeDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _viewTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }


    public void AddImpulse(Vector2 forceVector)
    {
        IsMoving = true;
        _rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
    }
    

    //private void OnTriggerEnter(Collider collider)
    //{
    //    if (collider.gameObject.TryGetComponent(out IObstacle obstacle))
    //    {
    //        obstacle.ProcessCollision(_rigidbody);
    //        Debug.Log("Collision processed");
    //    }
    //}

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

