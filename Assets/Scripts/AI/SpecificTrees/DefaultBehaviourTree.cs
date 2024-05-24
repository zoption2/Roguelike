using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    public interface IDefaultBehaviourTree : IBehaviourTree
    {
        public Transform GetTarget();
        public bool SphereCastHitTheTarget(Transform target, Vector3 startingPoint, float abilityMultiplier = 1,
            float remainingDistance = -1f);
        public Vector3 GetCharacterPosition();
        public void SetAbilities(List<IAbility> abilities);
        public void SetCurrentAbility(IAbility ability);
        public bool CanAttackAfterMove(NavMeshPath path);
    }
    public class DefaultBehaviourTree : BehaviourTree, IDefaultBehaviourTree
    {
        private string _attackKey = "CanAttack", _moveKey = "CanMove", _targetKey ="Target";

        private AbilityChooser _attackChooser;

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
            // check whether enemy could get hit by player after he moves
            // (whether he will be in direct line of sight)
            // for that i will need to know the end point and then shoot a raycast from player to the endpoint
            // maybe i need to use pathIsClear and CouldReach for that
            // if not than it is okay to move
            // if after moving he could even attack the player than definitely can move

            if (!_characterController.IsStunned)
            {
                Debug.Log("Can Move");
                _blackboard.SetData(_moveKey, true);
            } 
            else
            {
                Debug.Log("CAN'T MOVE");
                _blackboard.SetData(_moveKey, false);
            }
        }

        public Vector3 GetCharacterPosition()
        {
            return _characterController.GetTransform().position;
        }
        protected void CheckIfCanAttack()
        {
            Transform  target = GetTarget();
            Vector3 characterPosition = GetCharacterPosition();
            if (ChooseAbility(characterPosition) != null && !_characterController.IsStunned)
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
        protected float GetMaxLaunchDistance()
        {
            float launchPower = _characterController.ModifiableStats.LaunchPower.Value;
            float dragConstant = _characterController.GetRigidbody().drag;
            float maxDistance = launchPower / dragConstant;
            return maxDistance;
        }
        public bool CanAttackAfterMove(NavMeshPath path)
        {
            Transform target = GetTarget();
            NavMeshAgent navAgent = _characterController.NavMeshAgent;
            float offset = 0.5f;
            float minStoppingDistance=2f;
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, target.position.z + offset);

            float maxDistance = GetMaxLaunchDistance();
            float walkedDistance = 0f;
            float remainingDistance = 0f;
            float distanceToTarget = 0f;

            Vector3[] corners = path.corners;
            Vector2 direction;
            Vector3 neededVector;
            Vector3 endPoint;

            float divider;

            Debug.Log("Distance between enemy and target: " + Vector3.Distance(target.position, GetCharacterPosition()));
            for(int i = 1; i < corners.Length; i++)
            {
                if (walkedDistance + Vector3.Distance(corners[i - 1], corners[i]) < maxDistance)
                {
                    walkedDistance += Vector3.Distance(corners[i - 1], corners[i]);
                    remainingDistance = maxDistance - walkedDistance;
                }
                else
                {
                    direction = corners[i] - corners[i-1];
                    remainingDistance = maxDistance - walkedDistance;
                    divider = direction.magnitude / remainingDistance;
                    neededVector = direction / divider;
                    endPoint = corners[i-1] + neededVector;
                    distanceToTarget = Vector3.Distance(endPoint, targetPosition);
                    if (distanceToTarget < minStoppingDistance)
                    {
                        endPoint -= (Vector3)direction.normalized * (minStoppingDistance - distanceToTarget);
                    }
                    navAgent.SetDestination(endPoint);
                    return false;
                }

                if (remainingDistance >= Vector3.Distance(corners[i], targetPosition))
                {
                    if (ChooseAbility(corners[i], remainingDistance) != null)
                    {
                        direction = corners[i] - corners[i - 1];
                        distanceToTarget = Vector3.Distance(corners[i], targetPosition);
                        endPoint = corners[i];
                        if (distanceToTarget < minStoppingDistance)
                        {
                            endPoint -= (Vector3)direction.normalized * (minStoppingDistance - distanceToTarget);
                        }
                        navAgent.SetDestination(endPoint);
                        return true;
                    }
                }
            }
            return false;
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

        public bool SphereCastHitTheTarget(Transform target, Vector3 startingPoint, float abilityMultiplier = 1,
            float remainingDistance = -1f)
        {
            float maxDistance = GetMaxLaunchDistance() * abilityMultiplier;

            if(remainingDistance > 0)
            {
                maxDistance = Mathf.Min(maxDistance, remainingDistance);
            }

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
            _attackChooser = new AbilityChooser(this, _abilities);
        }

        public IAbility ChooseAbility(Vector3 startingPoint, float remainingDistance = -1f)
        {
            return _attackChooser.ChooseAbility(startingPoint, remainingDistance);
        }

        public void SetCurrentAbility(IAbility ability)
        {
            _characterController.CurrentAbility = ability;
        }
    }
}
