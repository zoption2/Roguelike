using Interactions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    public interface IDefaultBehaviourTree : IBehaviourTree
    {
        public Transform GetTarget();
        public Vector3 FindWaypointToObserveTarget(NavMeshPath path, Transform target);
        public bool SphereCastHitTheTarget(Transform target, Vector3 startingPoint, float multiplier = 1);
        public Vector3 GetCharacterPosition();
        public void SetAbilities(List<IAbility> abilities);
        public void SetCurrentAbility(IAbility ability);
    }
    public class DefaultBehaviourTree : BehaviourTree, IDefaultBehaviourTree
    {
        private string _attackKey = "CanAttack", _moveKey = "CanMove", _targetKey ="Target";

        private AttackChooser _attackChooser;

        private List<IAbility> _abilities;
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
        protected override void UpdateData()
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
            NavMeshAgent navAgent = _characterController.NavMeshAgent;
            NavMeshObstacle navObstacle = _characterController.NavMeshObstacle;
            navObstacle.enabled = false;
            navAgent.enabled = true;

            float offset = 0.5f;
            Vector3 position = new Vector3(target.position.x,target.position.y, target.position.z + offset);

            bool pathIsFound = navAgent.CalculatePath(position, path);
            bool couldReachPoint = false;

            if (pathIsFound)
            {
                Vector3 point = FindWaypointToObserveTarget(path, target);
                couldReachPoint = CouldReach(point);
            }
            Debug.Log("path is found: " + pathIsFound);
            Debug.Log("could reach: " + couldReachPoint);

            if (!_characterController.IsStunned && couldReachPoint)
            {
                SetPath(path);
                Debug.Log("Can Move");
                _blackboard.SetData(_moveKey, true);
            } 
            else
            {
                Debug.Log("CAN'T MOVE");
                _blackboard.SetData(_moveKey, false);
            }
            navAgent.enabled = false;
            navObstacle.enabled = true;
        }

        public Vector3 GetCharacterPosition()
        {
            return _characterController.GetTransform().position;
        }
        protected void CheckIfCanAttack()
        {
            Transform  target = GetTarget();
            Vector3 characterPosition = GetCharacterPosition();
            if (_attackChooser.ChooseAbility() != null && !_characterController.IsStunned)
            {
                Debug.Log("Can Attack");
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
            Vector3 direction = target - startingPoint;
            direction.z = 0;
            direction.Normalize();
            float radius = 0.5f;
            RaycastHit hit;
            Physics.SphereCast(startingPoint, radius, direction, out hit, distance, mask);
            return hit;
        }

        public Vector3 FindWaypointToObserveTarget(NavMeshPath path, Transform target)
        {
            Vector3 waypoint = path.corners[0];
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
            Vector3 character = GetCharacterPosition();
            float distance = Vector2.Distance(point,character);
            RaycastHit hit = ShootSphereCastToTarget(point,distance,character);
            if(hit.collider == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected bool CouldReach(Vector3 point)
        {
            float launchPower = _characterController.ModifiableStats.LaunchPower.Value;
            float dragConstant = _characterController.GetRigidbody().drag;
            float maxDistance = launchPower / dragConstant;
            float distance = Vector3.Distance(point, GetCharacterPosition());
            if(distance > maxDistance)
            {
                return false;
            }
            else
                return true;
        }

        public bool SphereCastHitTheTarget(Transform target,Vector3 startingPoint, float multiplier = 1)
        {
            float launchPower = _characterController.ModifiableStats.LaunchPower.Value;
            float dragConstant = _characterController.GetRigidbody().drag;
            float maxDistance = (launchPower / dragConstant) * multiplier;
            
            RaycastHit hit = ShootSphereCastToTarget(target.position,maxDistance, startingPoint);
            Transform hitTransform = hit.transform;
            if (hitTransform != null &&  hitTransform.childCount > 0)
            {
                hitTransform = hit.transform.GetChild(0);
            }

            if (hitTransform != null && hitTransform == target)
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

        public void SetAbilities(List<IAbility> abilities)
        {
            _abilities = abilities;
            _attackChooser = new AttackChooser(this, _abilities);
        }

        public void SetCurrentAbility(IAbility ability)
        {
            _characterController.CurrentAbility = ability;
            Debug.LogWarning("Now using: " +  ability);
        }

        public void SetPath(NavMeshPath path)
        {
            _characterController.Path = path;
        }
    }
}
