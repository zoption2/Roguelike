using Interactions;
using Pool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Projectiles
{
    public interface IProjectile : IInteractible, IMyPoolable
    {
        public void Init(IControllerInputs controllerInputs, ProjectileType projectileType, ProjectilePooler projectilePooler);

        public ProjectileType ProjectileType { get; set;}

        public void PushToPool();
        public void SetRicochetCount(int count);
    }

    public class Projectile : MonoBehaviour, IProjectile
    {
        private Rigidbody _rigidbody;
        private ProjectilePooler _pooler;
        public IControllerInputs ControllerInputs { get; set; }
        public ProjectileType ProjectileType { get; set; }
        public Vector3 Normal { get; set; }

        private const float X_ROTATION = 90f;
        private Queue<Vector3> _lastVelocities = new Queue<Vector3>(2);

        private ProjectileCollisionHandler _projectileCollisionHandler;
        private float _maxDistanceFromCharacter = 50f;


        public void Init(IControllerInputs controllerInputs,ProjectileType projectileType,ProjectilePooler projectilePooler)
        {
            _pooler = projectilePooler;
            ControllerInputs = controllerInputs;
            _rigidbody = GetComponent<Rigidbody>();
            _projectileCollisionHandler = GetComponent<ProjectileCollisionHandler>();
            ProjectileType = projectileType;
            _projectileCollisionHandler.Init(this);
        }

        public void SetRicochetCount(int count)
        {
            _projectileCollisionHandler.SetRicochetCount(count);
        }

        public void PushToPool()
        {
            _projectileCollisionHandler.ResetCollisions();
            _pooler.Push(ProjectileType, this);
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

            if(Vector3.Distance(transform.position, ControllerInputs.GetTransform().position) > _maxDistanceFromCharacter)
            {
                PushToPool();
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

        public void AddLastVelocity(Vector3 velocity)
        {
            _lastVelocities.Enqueue(velocity);
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
            ControllerInputs.LaunchedProjectiles.Add(this);
        }

        public void OnRelease()
        {
            ControllerInputs.LaunchedProjectiles.Remove(this);
            if(ControllerInputs.LaunchedProjectiles.Count == 0)
                ControllerInputs.HandleStopMovement();
        }
    }
}
