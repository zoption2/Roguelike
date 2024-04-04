using Gameplay;
using Pool;
using UnityEngine;

public interface ICharacterController
{
    public bool IsActive { get; set; }
    public void Init(CharacterModel model, CharacterView playerView, CharacterPooler pooler);
    public void UseEffectsOnStart();
    public void AnalizeCondition();
    public void SwitchState(TypeOfConditionState state);
    public void UseEffectsOnEnd();
    public CharacterType GetCharacterType();
    public void PushIfDead();
    public void SkipTurn();
    public void Attack();
    public void Move();
    public void Tick();
    public bool CheckIfMoving();
    public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext);
    public Transform GetTransform();

    public event OnCharacterDeath ON_CHARACTER_DEATH;
}

public delegate void OnCharacterDeath(ICharacterController controller);
