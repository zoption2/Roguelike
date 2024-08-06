using DG.Tweening;
using UnityEngine;

public interface IAnimationController
{
    public void SetCharacter(ICharacterController character);
    public void Attack(Transform target, bool isAnimationComplete);
}

public class AnimationController : IAnimationController
{
    private ICharacterController _characterController;

    public void SetCharacter(ICharacterController character)
    {
        _characterController = character;
    }

    public void Attack(Transform target, bool animationStatus)
    {
        Transform characterTransform = _characterController.GetTransform();

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(characterTransform.transform.DOLookAt(target.position, 1f));
        sequence.AppendInterval(0.5f);
        sequence.Append(characterTransform.transform.DOShakePosition(0.3f, new Vector3(0.5f, 0, 0.5f), 20, 20f));

        sequence.OnComplete(() =>
        {
            _characterController.Attack();
            animationStatus = true;
        });

        sequence.Play();
    }
}
