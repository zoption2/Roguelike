using Interactions;
using Pool;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

namespace Projectiles
{
    public interface IProjectile : IInteractible, IMyPoolable
    {
        public void Init(IControllerInputs controllerInputs, ProjectileType projectileType, ProjectilePooler projectilePooler);

        public ProjectileType ProjectileType { get; set;}

        public UniTask PushToPool();
        public void PushToPoolImmediately();
        public void SetRicochetCount(int count);
    }

    public class Projectile : MonoBehaviour, IProjectile
    {
        [SerializeField]
        private ProjectileCollisionHandler _projectileCollisionHandler;
        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private GameObject _view;
        [SerializeField]
        private TrailRenderer _trail;

        private ProjectilePooler _pooler;
        public IControllerInputs ControllerInputs { get; set; }
        public ProjectileType ProjectileType { get; set; }
        public Vector3 Normal { get; set; }

        private const float X_ROTATION = 90f;
        private Queue<Vector3> _lastVelocities = new Queue<Vector3>(2);

        private const float MAX_DISTANCE_FROM_CHARACTER = 30f;


        public void Init(IControllerInputs controllerInputs,ProjectileType projectileType,ProjectilePooler projectilePooler)
        {
            _pooler = projectilePooler;
            ControllerInputs = controllerInputs;
            ProjectileType = projectileType;
            _projectileCollisionHandler.Init(this);
        }

        public void SetRicochetCount(int count)
        {
            _projectileCollisionHandler.SetRicochetCount(count);
        }

        public async UniTask  PushToPool()
        {
            _rigidbody.velocity = Vector3.zero;
            _view.SetActive(false);
            
            if(_trail != null)
                await WaitWhileConditionIsTrue(() => _trail.positionCount != 0);

            _pooler.Push(ProjectileType, this);
        }
        public void PushToPoolImmediately()
        {
            _pooler.Push(ProjectileType, this);
        }

        private async UniTask WaitWhileConditionIsTrue(Func<bool> condition)
        {
            await UniTask.WaitWhile(condition);
        } 
        public Rigidbody GetRigidbody()
        {
            return _rigidbody;
        }

        public Vector3 GetVelocity()
        {
            return _rigidbody.velocity;
        }

        public void StartInteraction(IInteractible interactible)
        {
            var dealerType = ControllerInputs.GetType();
            var handlerType = interactible.ControllerInputs.GetType();

            IInteraction interactionFromDealer = ControllerInputs.GetInteraction();

            if (!dealerType.Equals(handlerType) && interactionFromDealer != null)
            {
                interactible.ControllerInputs.ApplyInteraction(interactionFromDealer);
            }
            else
            {
                return;
            }

            IMovable bump = interactionFromDealer.GetBump();
            ControllerInputs.ApplyBump(interactible, bump);
        }

        private void FixedUpdate()
        {
            _lastVelocities.Enqueue(_rigidbody.velocity);

            if (_lastVelocities.Count > 2)
            {
                _lastVelocities.Dequeue();
            }

            ViewRotation();

            if(Vector3.Distance(transform.position, ControllerInputs.GetTransform().position) > MAX_DISTANCE_FROM_CHARACTER)
            {
                PushToPoolImmediately();
            }
        }

        public void ViewRotation()
        {
            Vector3 velocity = GetVelocity();
            float rotationSpeed = velocity.magnitude;
            float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(X_ROTATION, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public Vector3 GetLastVelocity()
        {
            return _lastVelocities.Dequeue();
        }

        public void OnCreate()
        {
        }

        public void OnPull()
        {
            _view.SetActive(true);
            ControllerInputs.LaunchedProjectiles.Add(this);
        }

        public void OnRelease()
        {
            _projectileCollisionHandler.ResetCollisions();
            ControllerInputs.LaunchedProjectiles.Remove(this);
            if(ControllerInputs.LaunchedProjectiles.Count == 0)
                ControllerInputs.HandleStopMovement();
        }
    }
}
