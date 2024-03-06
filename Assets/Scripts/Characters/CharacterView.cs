using Enemy;
using Player;
using Pool;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICharacterView
{
    public void Init(PlayerModel model);
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

    private PlayerModel _playerModel;
    private EnemyModel _enemyModel;
    private Rigidbody2D _rigidbody;
    public void Init(PlayerModel model)
    {
        _playerModel = model;
    }

    public void Init(EnemyModel model)
    {
        _enemyModel = model;
    }

    private void Start()
    {
        _rigidbody = gameObject.GetComponentInChildren<Rigidbody2D>();
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
        _rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
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

