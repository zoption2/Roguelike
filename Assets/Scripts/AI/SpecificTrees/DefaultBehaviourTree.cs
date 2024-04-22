using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    public interface IDefaultBehaviourTree : IBehaviourTree
    {
        public Transform GetTarget();
        public Vector3 FindWaypointToObserveTarget(NavMeshPath path, Transform target);
    }
    public class DefaultBehaviourTree : BehaviourTree, IDefaultBehaviourTree
    {
        private string _attackKey = "CanAttack", _moveKey = "CanMove", _targetKey ="Target";
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
            Transform target = GetTarget();
            NavMeshPath path = new NavMeshPath();
            _characterController.NavMeshObstacle.enabled = false;
            _characterController.NavMeshAgent.enabled = true;
            bool pathIsFound = _characterController.NavMeshAgent.CalculatePath(target.position, path);
            bool couldReachPoint = false;
            if (pathIsFound)
            {
                couldReachPoint = CouldReach(FindWaypointToObserveTarget(path, target));
            }

            if (!_characterController.IsStunned && couldReachPoint)
            {
                Debug.Log("Can Move");
                _blackboard.SetData(_moveKey, true);
            } 
            else
            {
                Debug.Log("CAN'T MOVE");
                _blackboard.SetData(_moveKey, false);
            }
            _characterController.NavMeshAgent.enabled = false;
            _characterController.NavMeshObstacle.enabled = true;
            
        }

        protected void CheckIfCanAttack()
        {
            Transform  target = GetTarget();
            Vector3 characterPosition = _characterController.GetTransform().position;
            if (SphereCastHitTheTarget(target, characterPosition) && !_characterController.IsStunned)
            {
                Debug.Log("!!!Can Attack!!!");
                _blackboard.SetData(_attackKey, true);
            }
            else
            {
                Debug.Log("!!!CAN'T Attack!!!");
                _blackboard.SetData(_attackKey, false);
            }

        }

        protected RaycastHit ShootSphereCastToTarget(Vector3 target, float distance,Vector3 startingPoint)
        {
            LayerMask mask = LayerMask.GetMask("Default", "Enemy","Player");
            Vector3 characterPosition = _characterController.GetTransform().position;
            startingPoint.z = characterPosition.z;
            Vector3 direction = target - startingPoint;
            direction.z = 0;
            direction.Normalize();
            float radius = 0.5f;
            RaycastHit hit;
            //Debug.DrawRay(target, -direction,Color.red,2f);
            Physics.SphereCast(startingPoint, radius, direction, out hit, distance, mask);
            return hit;
        }

        public Vector3 FindWaypointToObserveTarget(NavMeshPath path, Transform target)
        {
            Vector3 waypoint = path.corners[1];
            foreach (Vector3 point in path.corners)
            {
                if (SphereCastHitTheTarget(target, point) && PathToPointIsClear(point) && point != path.corners[0])
                {
                    return point;
                }
                else if (PathToPointIsClear(point) && CouldReach(point) && point != path.corners[0])
                {
                    waypoint = point;
                }
            }
            return waypoint;
        }
        protected bool PathToPointIsClear(Vector3 point)
        {
            Transform character = _characterController.GetTransform();
            float distance = Vector2.Distance(point,character.position);
            RaycastHit hit = ShootSphereCastToTarget(point,distance,character.position);
            if(hit.collider == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected bool CouldReach(Vector2 point)
        {
            float launchPower = _characterController.ModifiableStats.LaunchPower.Value;
            float dragConstant = _characterController.CharacterView.Rigidbody.drag;
            float maxDistance = launchPower / dragConstant;
            float distance = Vector2.Distance(point, _characterController.GetTransform().position);
            if(distance > maxDistance)
            {
                return false;
            }
            else
                return true;
        }

        protected bool SphereCastHitTheTarget(Transform target,Vector3 startingPoint)
        {
            float launchPower = _characterController.ModifiableStats.LaunchPower.Value;
            float dragConstant = _characterController.CharacterView.Rigidbody.drag;
            float maxDistance = launchPower / dragConstant;
            
            RaycastHit hit = ShootSphereCastToTarget(target.position,maxDistance, startingPoint);
            Transform hitTransform = hit.transform;

            if (hitTransform != null &&  hitTransform.childCount > 0)
            {
                hitTransform = hit.transform.GetChild(0);
            }
            //Debug.LogWarning("Hit: " + hitTransform.gameObject.name);
            //Debug.LogWarning("Target: " + target.gameObject.name);
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
                    if (SphereCastHitTheTarget(target, characterPosition))
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
