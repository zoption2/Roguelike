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
    private IControllerInputs _controllerInputs;
    
    
    public void Init(IControllerInputs controllerInputs, CharacterView characterView)
    {
        _controllerInputs = controllerInputs;
        _characterView = characterView;
        _rigidbody = _characterView.GetRigidbody();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IWall obstacle))
        {
            Vector3 velocity = _characterView.GetLastVelocity();
            obstacle.ProcessCollision(collision, _rigidbody, velocity);
        }

        if (collision.gameObject.TryGetComponent(out IInteractible interactible))
        {
            _characterView.Normal = collision.GetContact(0).normal;
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

        if (other.gameObject.TryGetComponent(out ICompleatedRoomTrigger trigger))
        {
             trigger.UseTrigger();
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


                    buff.RemoveBuff();
                    _isStoppedInsideTrigger = false;
                    yield break;
                }
            }
            yield return null;
        }
    }
}
