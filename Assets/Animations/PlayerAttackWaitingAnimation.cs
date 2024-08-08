using DG.Tweening;
using System;
using UnityEngine;

public class PlayerAttackWaitingAnimation : AnimationBase
{
    private bool _isAnimating = false;
    private Vector3 _initialScale;
    private Tween _scaleTween;

    public PlayerAttackWaitingAnimation(ICharacterController characterController) : base(characterController)
    {
    }

    public override void Play(Action onComplete)
    {
        Transform characterTransform = _characterController.GetTransform();

        if (!_isAnimating)
        {
            _isAnimating = true;
            _initialScale = characterTransform.localScale;
            characterTransform.DOKill();
            _scaleTween = characterTransform.DOScale(_initialScale * 1.1f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear)
                .OnKill(() =>
                {
                    characterTransform.localScale = _initialScale;
                    _isAnimating = false;
                    onComplete?.Invoke();
                });
        }
    }

    public override void Stop(Action onComplete)
    {
        if (_isAnimating)
        {
            _isAnimating = false;
            Transform characterTransform = _characterController.GetTransform();
            characterTransform.DOKill();
            characterTransform.localScale = _initialScale;
            onComplete?.Invoke();
        }
    }
}
