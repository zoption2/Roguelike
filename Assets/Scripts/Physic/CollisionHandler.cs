using Interactions;
using Obstacles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionHandler
{
    void Init(IControllerInputs controllerInputs, CharacterView characterView);
}

public class CollisionHandler : MonoBehaviour, ICollisionHandler
{
    private CharacterView _characterView;

    private Rigidbody _rigidbody;
    private bool _isStoppedInsideTrigger;
    Queue<Vector3> _lastVelocities = new(2);
    IControllerInputs _controllerInputs;
    

    public void Init(IControllerInputs controllerInputs, CharacterView characterView)
    {
        _controllerInputs = controllerInputs;
        _characterView = characterView;
        _rigidbody = _characterView.GetRigidbody();
    }

    private void FixedUpdate()
    {
        if(_rigidbody != null)
        {
            _lastVelocities.Enqueue(_rigidbody.velocity);

            if (_lastVelocities.Count > 2)
            {
                _lastVelocities.Dequeue();
            }
        }
    }

    public Vector3 GetVelocity()
    {
        return _lastVelocities.Dequeue();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IWall obstacle))
        {
            Vector3 velocity = GetVelocity();
            obstacle.ProcessCollision(collision, _rigidbody, velocity);
        }

        if (collision.gameObject.TryGetComponent(out IInteractible interactible))
        {
            _characterView.StartInteraction(interactible);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IBuff buff))
        {
            _isStoppedInsideTrigger = true;
            StartCoroutine(CheckPlayerStopped(buff));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IBuff buff))
        {
            _isStoppedInsideTrigger = false;
            StartCoroutine(CheckPlayerStopped(buff));
        }
    }

    private IEnumerator CheckPlayerStopped(IBuff buff)
    {
        while (_isStoppedInsideTrigger)
        {
            if (!_controllerInputs.IsMoving)
            {
                bool activeStatus = _controllerInputs.GetActiveStatus();
                if (activeStatus)
                {
                    List<IEffect> effects = buff.UseBuff();

                    _controllerInputs.AddEffects(effects);

                    Debug.Log("<color=#07C3FF>" + buff + " effects were added" + "</color>");

                    buff.DisableBuff();
                    _isStoppedInsideTrigger = false;
                    yield break;
                }
            }
            yield return null;
        }
    }




}
