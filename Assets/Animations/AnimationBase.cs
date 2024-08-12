using System;
using UnityEngine;

public abstract class AnimationBase
{
    protected ICharacterController _characterController;
    protected GameObject _gameobjectToAnimate;
    public abstract void Play(Action onComplete);
    public abstract void Stop(Action onComplete);

    public void SetCharacterController(ICharacterController characterController)
    {
        _characterController = characterController;
    }

    public void SetGameobjectToAnimate(GameObject gameObject)
    {
        _gameobjectToAnimate = gameObject;
    }
}


