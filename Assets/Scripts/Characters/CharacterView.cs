using Enemy;
using Obstacles;
using Player;
using Pool;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICharacterView
{
    public void Init(CharacterModel model);
    public void Init(EnemyModel model);
    public void AddImpulse(Vector2 forceVector);
    public void ChangeDirection(Vector2 direction);
    bool IsPlayerMoving { get; set; }

    event Action<Transform, PointerEventData> ON_CLICK;
    event Action<PointerEventData> ON_BEGINDRAG;
}

public class CharacterView : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, ICharacterView, IMyPoolable
{
    public event Action<Transform, PointerEventData> ON_CLICK;
    public event Action<PointerEventData> ON_BEGINDRAG;

    [SerializeField] Transform _viewTransform;
    public bool IsPlayerMoving { get; set; }

    private CharacterModel _playerModel;
    private EnemyModel _enemyModel;
    private Rigidbody _rigidbody;
    public void Init(CharacterModel model)
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
        if (IsPlayerMoving) ViewRotation();
    }

    private void FixedUpdate()
    {
        if (IsPlayerMoving)
        {
            _playerModel.Velocity.Value = _rigidbody.velocity.magnitude;
        }

        if (IsPlayerMoving) ViewRotation();
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

