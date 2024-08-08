using DG.Tweening;
using System;
using UnityEngine;

public interface IAnimationController
{
    void SetCharacter(ICharacterController character);
    void Attack(bool animationStatus, Transform target = null);
    public void Move(Transform characterTransform, bool isAnimationComplete);
}

public interface IEnemyAnimationController : IAnimationController
{
}

public interface IPlayerAnimationController : IAnimationController
{
}

public abstract class AnimationControllerBase : IAnimationController
{
    protected ICharacterController _characterController;

    public void SetCharacter(ICharacterController character)
    {
        _characterController = character;
    }

    public virtual void Attack(bool isAnimationComplete, Transform target = null)
    {
    }

    public virtual void Move(Transform characterTransform, bool isAnimationComplete)
    {
    }
}

public class EnemyAnimationController : AnimationControllerBase, IEnemyAnimationController
{
    public override void Attack(bool animationStatus, Transform target = null)
    {
        Transform characterTransform = _characterController.GetTransform();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(characterTransform.DOLookAt(target.position, 0.5f));
        sequence.AppendInterval(0.5f);
        sequence.Append(characterTransform.DOShakePosition(0.4f, new Vector3(0.2f, 0, 0.2f), 20, 20f));

        sequence.OnComplete(() =>
        {
            _characterController.Attack();
            animationStatus = true;
        });

        sequence.Play();
    }
}

public class PlayerAnimationController : AnimationControllerBase, IPlayerAnimationController
{
    private bool isAnimationRunning = false;
    private Vector3 initialScale;

    public override void Attack(bool animationStatus, Transform target = null)
    {
        Transform characterTransform = _characterController.GetTransform();

        if (animationStatus && !isAnimationRunning)
        {
            isAnimationRunning = true;
            initialScale = characterTransform.localScale;
            characterTransform.DOKill();
            characterTransform.DOScale(initialScale * 1.1f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear)
                .OnKill(() => characterTransform.localScale = initialScale);
        }
        else if (!animationStatus && isAnimationRunning)
        {
            isAnimationRunning = false;
            characterTransform.DOKill();
            characterTransform.localScale = initialScale;
        }
    }
}

public abstract class AnimationBase
{
    protected ICharacterController _characterController;
    public abstract void Animate(Action onComplete);

    public AnimationBase(ICharacterController characterController)
    {
        _characterController = characterController;
    }
}

public class EnemyAttackAnimation : AnimationBase
{
    public EnemyAttackAnimation(ICharacterController characterController) : base(characterController)
    {
    }

    public override void Animate(Action onComplete)
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

}
