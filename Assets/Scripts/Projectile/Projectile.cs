using Interactions;
using Pool;
using UnityEngine;
using Zenject;

namespace Projectile
{
    public interface IProjectile : IInteractible, IMyPoolable
    {
        public void Init(IControllerInputs controllerInputs, ProjectileType projectileType);

        public ProjectileType ProjectileType { get; set;}

        public void PushToPool();
    }

    public class Projectile : MonoBehaviour, IProjectile
    {
        private Rigidbody _rigidbody;
        private ProjectilePooler _pooler;
        public IControllerInputs ControllerInputs { get; set; }
        public ProjectileType ProjectileType { get; set; }
        public Vector2 Normal { get; set; }

        [Inject]
        public void Construct(ProjectilePooler projectilePooler)
        {
            _pooler = projectilePooler;
        }

        public void Init(IControllerInputs controllerInputs,ProjectileType projectileType)
        {
            ControllerInputs = controllerInputs;
            _rigidbody = GetComponent<Rigidbody>();
            ProjectileType = projectileType;
        }

        public void PushToPool()
        {
            _pooler.Push(ProjectileType, this);
        }
        public Vector3 GetLastVelocity()
        {
            throw new System.NotImplementedException();
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

        public void OnCreate()
        {
        }

        public void OnPull()
        {
        }

        public void OnRelease()
        {
        }
    }
}
