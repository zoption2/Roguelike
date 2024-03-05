using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace Enemy
{

    public interface IEnemyController : ICharacterController
    {
        public void OnClick(Transform point, PointerEventData eventData);
    }
    public class EnemyController : IEnemyController, IDisposable
    {
        Transform _poolableTransform;
        private CharacterView _enemyView;
        private CharacterModel _enemyModel;
        private Rigidbody2D _enemyViewRigidbody;
        public event OnEndTurn OnEndTurn;
        public bool IsActive { get; set; }

        public void Init(Transform poolableTransform, Rigidbody2D rigidbody, CharacterModel enemyModel, CharacterView characterView)
        {
            _enemyModel = enemyModel;
            _poolableTransform = poolableTransform;
            _enemyViewRigidbody = rigidbody;
            _enemyView = characterView;
            _enemyView.ON_CLICK += OnClick;
        }
        public void OnClick(Transform point, PointerEventData eventData)
        {
            if (IsActive)
            {
                //Debug.Log("Enemy was clicked");
                OnEndTurn?.Invoke();
            }
        }

        public void Dispose()
        {
            _enemyView.ON_CLICK -= OnClick;
        }
    }
}