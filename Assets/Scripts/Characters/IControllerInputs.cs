using CharactersStats;
using Interactions;
using System.Collections.Generic;

public interface IControllerInputs
{
    public bool IsMoving { get; set; }
    ReactiveStats GetCharacterStats();
    IInteraction GetInteraction();
    void ApplyInteraction(IInteraction interactions);
    void AddEffects(List<IEffect> effects);
    bool GetActiveStatus();
    public void DoUpdate();
    IConditionState GetCurrentConditionState();
}
