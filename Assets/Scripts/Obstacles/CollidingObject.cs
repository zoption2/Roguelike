using Interactions;
using Obstacles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CollidingObject : MonoBehaviour
{
    [SerializeField] CharacterView _characterView;

    private Rigidbody _rigidbody;
    private bool _isStoppedInsideTrigger;
    Queue<Vector3> _lastVelocities = new(2);


    void FixedUpdate()
    {
        _lastVelocities.Enqueue(_rigidbody.velocity);

        if(_lastVelocities.Count > 2 )
        {
            _lastVelocities.Dequeue();
        }
    }

    public Vector3 GetVelocity()
    {
        return _lastVelocities.Dequeue();
    }

    private void Start()
    {
        _rigidbody = _characterView.Rigidbody;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.TryGetComponent(out IWall obstacle))
        {
            Vector3 velocity = GetVelocity();
            obstacle.ProcessCollision(_rigidbody, velocity);
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
            StopCoroutine(CheckPlayerStopped(buff));
        }
    }

    private IEnumerator CheckPlayerStopped(IBuff buff)
    {
        while (_isStoppedInsideTrigger)
        {
            if (!_characterView.IsPlayerMoving)
            {
                List<IEffect> effects = buff.ProcessTrigger();

                _characterView.ControllerInputs.AddEffects(effects);

                Debug.LogWarning(buff + " effects were added");
                buff.DisableBuff();
                _isStoppedInsideTrigger = false;
                yield break;
            }
            yield return null;
        }
    }




}
