using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourTree
{
    public interface IDefaultBehaviourTree : IBehaviourTree
    {
        public Transform GetTarget();
    }
    public class DefaultBehaviourTree : BehaviourTree, IDefaultBehaviourTree
    {
        private string _attackKey = "CanAttack", _moveKey = "CanMove",_targetKey ="Target";
        protected override Node SetupRootNode()
        {
            Node rootNode = new Selector( new List<Node>
            {
                new Sequence(new List<Node>
                {
                    new CanAttackNode(),
                    new TaskAttackNode(),
                }),
                //new Sequence(new List<Node>
                //{
                //    new CanMoveNode(),
                //    new TaskMoveNode(),
                //}),
                 new Sequence(new List<Node>
                {
                    new DoNothingNode(),
                })
            });
            return rootNode;
        }

        public Transform GetTarget()
        {
            return (Transform)_blackboard.GetData(_targetKey);
        }
        protected override void UpdateBlackboard()
        {
            FindTarget();
            if(GetTarget() != null)
            {
                
                CheckIfCanAttack();
                //CheckIfCanMove();   
            }
            else
            {
                _blackboard.SetData(_attackKey, false);
                //_blackboard.SetData(_moveKey, false);
            }
        }

        private void CheckIfCanMove()
        {
            if (!_characterController.IsStunned)
            {
                _blackboard.SetData(_moveKey, true);
            } else
            {
                _blackboard.SetData(_moveKey, false);
            }
        }

        protected void CheckIfCanAttack()
        {
            Transform  target = GetTarget();
            if (SphereCastHitTheTarget(target) && !_characterController.IsStunned)
            {
                _blackboard.SetData(_attackKey, true);
            }
            else
            {
                _blackboard.SetData(_attackKey, false);
            }

        }

        protected bool SphereCastHitTheTarget(Transform target)
        {
            Transform character = _characterController.GetTransform();
            Vector3 direction = target.position - character.position;
            direction.Normalize();
            float radius = 0.4f;
            RaycastHit hit;
            Physics.SphereCast(character.position, radius, direction, out hit);
            Transform hitTransform = hit.transform;
            if (hit.transform.childCount > 0)
            {
                hitTransform = hit.transform.GetChild(0);
            }

            if (hitTransform == target)
            {
                return true;
            }
            else
                return false;
        }
        protected void FindTarget()
        {
            Vector3 characterPosition = _characterController.GetTransform().position;
            List<Transform> allTargets= new List<Transform>();
            foreach (ICharacterController characterController in _characterScenarioContext.Players)
            {
                allTargets.Add(characterController.GetTransform());
            }
            allTargets = allTargets.OrderBy(x => Vector3.Distance(x.position,characterPosition)).ToList();
            if(allTargets.Count > 0)
            {
                Transform finalTarget = allTargets[0];
                foreach (Transform target in allTargets)
                {
                    if (SphereCastHitTheTarget(target))
                    {
                        _blackboard.SetData(_targetKey, target);
                        return;
                    }
                }
                _blackboard.SetData(_targetKey, finalTarget);
            }
            else
            {
                _blackboard.SetData(_targetKey, null);
            }
        }
    }
}
