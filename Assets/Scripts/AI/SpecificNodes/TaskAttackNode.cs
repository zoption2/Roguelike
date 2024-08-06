using DG.Tweening;
using UnityEngine;

namespace BehaviourTree
{
    public class TaskAttackNode : Node
    {
        private IDefaultBehaviourTree _behaviourTree;
        private bool _animationCompleted = false;

        public TaskAttackNode(IDefaultBehaviourTree behaviourTree)
        {
            _behaviourTree = behaviourTree;
        }

        public override NodeState Evaluate()
        {
            _state = NodeState.Running;

            Transform target = _behaviourTree.GetTarget();
            Transform objectToAnimate = _characterController.GetTransform();

            RotateAndShakeSync(target, objectToAnimate);


            if (_animationCompleted)
            {
                _animationCompleted = false;
                _state = NodeState.Success;
            }
            

            return _state;
        }

        private void RotateAndShakeSync(Transform target, Transform objectToAnimate)
        {
            DG.Tweening.Sequence sequence = DOTween.Sequence();

            sequence.Append(objectToAnimate.transform.DOLookAt(target.position, 1f));
            sequence.AppendInterval(0.5f);
            sequence.Append(objectToAnimate.transform.DOShakePosition(0.3f, new Vector3(0.5f, 0, 0.5f), 20, 20f));

            sequence.OnComplete(() =>
            {
                _characterController.Attack();
                _animationCompleted = true;
            });

            sequence.Play();
        }
    }
}
