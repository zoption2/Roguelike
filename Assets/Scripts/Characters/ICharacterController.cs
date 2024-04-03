using Gameplay;
using Pool;
using UnityEngine;

public delegate void OnCharacterDeath(ICharacterController controller);

public interface ICharacterController
{
    public bool IsActive { get; set; }
    public void Init(CharacterModel model, CharacterView playerView, CharacterPooler pooler);
    public void UseEffectsOnStart();
    public void AnalizeCondition();
    void SwitchState(TypeOfConditionState state);
    void UseEffectsOnEnd();
    CharacterType GetCharacterType();
    void PushIfDead();//
    public void Attack();
    public void Move();
    public void Tick();
    public bool CheckIfMoving();
    public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext);
    public Transform GetTransform();

    public event OnCharacterDeath On_Character_Death;
}
