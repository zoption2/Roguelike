using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public interface ITestingBehaviourTree : IBehaviourTree
    {
        public Transform GetTarget();
    }
    public class TestingBehaviourTree : BehaviourTree, ITestingBehaviourTree
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
                new Sequence(new List<Node>
                {
                    new CanMoveNode(),
                    new TaskMoveNode(),
                }),
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
                CheckIfCanMove();   
            }
            else
            {
                _blackboard.SetData(_attackKey, false);
                _blackboard.SetData(_moveKey, false);
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
            Transform  character= _characterController.GetTransform();
            Transform  target = GetTarget();
            Vector3 direction = target.position - character.position;
            direction.Normalize();
            //float length = 15f;
            float radius = 0.4f;
            RaycastHit hit;
            Physics.SphereCast(character.position, radius, direction,out hit);
            //Debug.Log(hit.transform.gameObject.name + "was hit at: " + hit.point);
            Transform hitTransform = hit.transform;
            if(hit.transform.childCount > 0)
            {
                hitTransform = hit.transform.GetChild(0);
            }
            if (hitTransform == target && !_characterController.IsStunned)
            {
                _blackboard.SetData(_attackKey, true);
                Debug.Log("CanAttack");
            }
            else
            {
                Debug.Log("CANT_Attack");
                _blackboard.SetData(_attackKey, false);
            }

        }
        protected void FindTarget()
        {
            if(_characterScenarioContext.Players.Count > 0)
            {
                Vector3 target; //= _characterScenarioContext.Players[0].GetTransform().position;
                Vector3 enemy = _characterController.GetTransform().position;
                float minDistance = 100;//Vector3.Distance(target,enemy);
                float currentDistance;
                int minIndex = 0;
                for (int i=0,n = _characterScenarioContext.Players.Count; i < n; i++)
                {
                    target = _characterScenarioContext.Players[i].GetTransform().position;
                    currentDistance = Vector3.Distance(target, enemy);
                    if(currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        minIndex = i;
                    }
                }
                Transform transform = _characterScenarioContext.Players[minIndex].GetTransform();
                _blackboard.SetData(_targetKey, transform);
            }
            else
            {
                _blackboard.SetData(_targetKey, null);
            }
        }
    }
}
