using DG.Tweening;
using System;
using UnityEngine;

public class PlayerActiveIndicatorAnimation : AnimationBase
{
    private Tween _scaleTween;
    private bool _isAnimating = false;

    public override void Play(Action onComplete)
    {

        if (_isAnimating && _scaleTween.IsActive() && !_scaleTween.IsComplete())
        {
            return;
        }

        Transform objectTransform = _gameobjectToAnimate.transform;

        _isAnimating = true;

        _scaleTween = objectTransform.DOScale(1f, 1f).From(2f)
            .OnComplete(() =>
            {
                _scaleTween = objectTransform.DOScale(1.2f, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            });
    }

    public override void Stop(Action onComplete)
    {
        if (_isAnimating)
        {
            _isAnimating = false;

            Transform objectTransform = _gameobjectToAnimate.transform;

            _scaleTween.Kill();
            _scaleTween = objectTransform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                });
        }
    }
}
