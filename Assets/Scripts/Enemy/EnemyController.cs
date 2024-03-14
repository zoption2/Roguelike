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
        private CharacterView _enemyView;
        private CharacterModel _enemyModel;
        private Rigidbody2D _enemyViewRigidbody;
        public event OnEndTurn OnEndTurn;
        public bool IsActive { get; set; }

        public void Init(CharacterModel characterModel, CharacterView characterView)
        {
            _enemyModel = characterModel;
            _enemyView = characterView;
            _enemyView.Init(_enemyModel);
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