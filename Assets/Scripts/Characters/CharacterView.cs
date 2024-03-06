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
    bool IsMoving { get; set; }
    GameObject CharacterViewObject { get; }
    event Action<Transform, PointerEventData> ON_CLICK;
    event Action<PointerEventData> ON_BEGINDRAG;
}

public class CharacterView : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, ICharacterView, IMyPoolable
{
    [SerializeField]
    GameObject _characterViewObject;
    public GameObject CharacterViewObject => _characterViewObject;
    public bool IsMoving { get; set; }
    public event Action<Transform, PointerEventData> ON_CLICK;
    public event Action<PointerEventData> ON_BEGINDRAG;
    private Rigidbody2D _rigidbody;
    private Transform _transform;
    private PlayerModel _playerModel;
    private EnemyModel _enemyModel;

    private void Start()
    {
        _rigidbody = _characterViewObject.GetComponent<Rigidbody2D>();
    }

    public void Init(PlayerModel model)
    {
        _playerModel = model;
    }

    public void Init(EnemyModel model)
    {
        _enemyModel = model;
    }


    private void FixedUpdate()
    {
        if(IsMoving)
        {
            _playerModel.Velocity.Value = _rigidbody.velocity.magnitude;
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

