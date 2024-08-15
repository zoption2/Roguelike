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

            Vector3 initialRotation = characterTransform.eulerAngles;

            _rotationTween = DOTween.To(() => characterTransform.localEulerAngles.x,
                                        x => characterTransform.localRotation = Quaternion.Euler(x, characterTransform.localEulerAngles.y, characterTransform.localEulerAngles.z),
                                        360 + initialRotation.x, 2f)
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

            characterTransform.localEulerAngles = new Vector3(0, characterTransform.localEulerAngles.y, 0);

            onComplete?.Invoke();
        }
    }


}
