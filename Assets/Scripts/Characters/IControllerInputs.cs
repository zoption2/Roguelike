using CharactersStats;
using Interactions;
using System.Collections.Generic;

public interface IControllerInputs
{
    ReactiveStats GetCharacterStats();
    IInteraction GetInteraction();
    void ApplyInteraction(IInteraction interactions);
    void AddEffects(List<IEffect> effects);
    bool GetActiveStatus();
}
