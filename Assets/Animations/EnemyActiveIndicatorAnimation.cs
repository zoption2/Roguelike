using DG.Tweening;
using System;
using UnityEngine;

public class EnemyActiveIndicatorAnimation : AnimationBase
{
    private Tween _scaleTween;

    public override void Play(Action onComplete)
    {
        Transform objectTransform = _gameobjectToAnimate.transform;

        if (_scaleTween != null && _scaleTween.IsActive())
        {
            _scaleTween.Kill();
        }
        objectTransform.localScale = new Vector3(2f, 2f, 2f);

        _scaleTween = objectTransform.DOScale(Vector3.one, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    public override void Stop(Action onComplete)
    {
    }
}
