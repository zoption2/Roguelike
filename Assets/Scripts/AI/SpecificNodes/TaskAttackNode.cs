using Enemy;
using UnityEngine;

namespace BehaviourTree
{
    public class TaskAttackNode : Node
    {
        private IDefaultBehaviourTree _behaviourTree;
        private bool _isAnimationCompleted = false;

        public TaskAttackNode(IDefaultBehaviourTree behaviourTree)
        {
            _behaviourTree = behaviourTree;
        }

        public override NodeState Evaluate()
        {
            _state = NodeState.Running;

            Transform target = _behaviourTree.GetTarget();
            Transform objectToAnimate = _characterController.GetTransform();

            IEnemyController enemyController = (IEnemyController)_characterController;
            enemyController.AttackAnimation.Play(() =>
            {
                _isAnimationCompleted = true;
                _characterController.Attack();
            });

            if (_isAnimationCompleted)
            {
                _isAnimationCompleted = false;
                _state = NodeState.Success;
            }


            return _state;
        }
    }
}
