using DG.Tweening;
using UnityEngine;

public interface IAnimationController
{
    void SetCharacter(ICharacterController character);
    void Attack(bool animationStatus, Transform target = null);
    public void Move(bool isAnimationComplete);
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

    public abstract void Attack(bool isAnimationComplete, Transform target = null);

    public void Move(bool isAnimationComplete)
    {
        Transform characterTransform = _characterController.CharacterView.transform;
        float initialRotationY = characterTransform.eulerAngles.y;
        float initialRotationZ = characterTransform.eulerAngles.z;

        if (isAnimationComplete)
        {
            if (!DOTween.IsTweening(characterTransform))
            {
                characterTransform.DOKill();
                characterTransform.DORotate(new Vector3(360f, initialRotationY, initialRotationZ), 0.5f, RotateMode.LocalAxisAdd)
                    .SetLoops(-1, LoopType.Incremental)
                    .SetEase(Ease.Linear);
            }
        }
        else
        {
            characterTransform.DOKill();
            characterTransform.DORotate(new Vector3(0f, initialRotationY, initialRotationZ), 0.5f)
                .SetEase(Ease.Linear);
        }
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
