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
            _blackboard.SetData(_attackKey, true);
            _blackboard.SetData(_moveKey, true);
        }

        protected void FindTargetIfNeeded()
        {
            if(_blackboard.GetData(_targetKey) == null)
            {
                FindTarget();
            }
        }
        protected void FindTarget()
        {
            if(_characterScenarioContext.Players.Count > 0)
            {
                Vector3 target = _characterScenarioContext.Players[0].GetTransform().position;
                Vector3 enemy = _characterController.GetTransform().position;
                float minDistance = Vector3.Distance(target,enemy);
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
        }
    }
}
