using DG.Tweening;
using System;
using UnityEngine;

public class EnemyAttackAnimation : AnimationBase
{
    public EnemyAttackAnimation(ICharacterController characterController) : base(characterController)
    {
    }

    public override void Play(Action onComplete)
    {
        Transform characterTransform = _characterController.GetTransform();
        Transform target = _characterController.DefaultBehaviourTree.GetTarget();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(characterTransform.DOLookAt(target.position, 0.5f));
        sequence.AppendInterval(0.5f);
        sequence.Append(characterTransform.DOShakePosition(0.4f, new Vector3(0.2f, 0, 0.2f), 20, 20f));

        sequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });

        sequence.Play();
    }

    public override void Stop(Action onComplete)
    {
    }
}