using Interactions;
using Obstacles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (!_characterView.IsMoving)
            {
                bool activeStatus = _characterView.ControllerInputs.GetActiveStatus();
                if (activeStatus)
                {
                    List<IEffect> effects = buff.UseBuff();

                    _characterView.ControllerInputs.AddEffects(effects);

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
