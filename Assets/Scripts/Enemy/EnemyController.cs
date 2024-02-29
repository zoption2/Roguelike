using Player;
using Prefab;
using CharactersStats;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

namespace Enemy
{
    public interface ICharacterController
    {
        public bool IsActive { get; set; }
        public event OnSwitchState OnSwitch;

    }

    public interface IEnemyController : ICharacterController
    {
        public void Init(Transform poolableTransform, Rigidbody2D playerViewRigidbody, EnemyModel enemyModel);
        //public void Init(Transform point, PlayerType type, PlayerModel model);
    }
    public class EnemyController : IEnemyController
    {
        Transform _poolableTransform;
        private MyEnemyView _enemyView;
        private EnemyModel _enemyModel;
        private Rigidbody2D _enemyViewRigidbody;
        public event OnSwitchState OnSwitch;
        public bool IsActive { get; set; }

        public void Init(Transform poolableTransform, Rigidbody2D enemyViewRigidbody, EnemyModel enemyModel)
        {
            _enemyModel = enemyModel;
            _poolableTransform = poolableTransform;
            _enemyViewRigidbody = enemyViewRigidbody;
        }

        //TODO: add methods.

        //public void Init(EnemyView enemyView, DefaultEnemyModel enemyModel)
        //{
        //    _enemyView = enemyView;
        //    _enemyView.Initialize(this);
        //    _enemyModel = enemyModel;
        //}
        public void OnClick()
        {
            if (IsActive)
            {
                Debug.Log("Enemy was clicked");
                OnSwitch?.Invoke();
            }
        }
    }
}