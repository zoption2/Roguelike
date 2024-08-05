using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BehaviourTree
{
    public class TaskAttackNode : Node
    {
        private Transform _target;

        public TaskAttackNode(Transform target)
        { 
            _target = target;
        }

        public override NodeState Evaluate()
        {
            Transform objToMove = _characterController.GetTransform();
            RotateAndShakeAsync(_target, objToMove);
            _characterController.Attack();
            _state = NodeState.Success;
            return _state;
        }

        public async Task RotateAndShakeAsync(Transform target, Transform objectToAnimate)
        {
            await objectToAnimate.transform.DOLookAt(target.position, 1f).AsyncWaitForCompletion();

            await objectToAnimate.transform.DOShakePosition(0.5f, new Vector3(0.5f, 0, 0.5f), 10, 90f).AsyncWaitForCompletion();
        }

    }
}
