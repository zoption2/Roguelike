using DG.Tweening;
using System;
using UnityEngine;

public class MoveAnimation : AnimationBase
{
    private Tween _rotationTween;
    private bool _isAnimating = false;

    public override void Play(Action onComplete)
    {
        Transform characterTransform = _characterController.GetTransform();

        if (!_isAnimating)
        {
            _isAnimating = true;
            _rotationTween = characterTransform.DORotate(new Vector3(360, characterTransform.rotation.y, 0), 2f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear)
                .OnKill(() =>
                {
                    _isAnimating = false;
                    onComplete?.Invoke();
                });
        }
    }

    public override void Stop(Action onComplete)
    {
        if (_isAnimating)
        {
            _rotationTween.Kill();
            _isAnimating = false;

            Transform characterTransform = _characterController.GetTransform();
            characterTransform.localRotation = Quaternion.Euler(0, characterTransform.localRotation.y, characterTransform.rotation.z);

            onComplete?.Invoke();
        }
    }
}
