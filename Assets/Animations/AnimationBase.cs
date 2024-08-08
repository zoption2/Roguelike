using System;

public abstract class AnimationBase
{
    protected ICharacterController _characterController;
    public abstract void Play(Action onComplete);
    public abstract void Stop(Action onComplete);

    public AnimationBase(ICharacterController characterController)
    {
        _characterController = characterController;
    }
}


