using Interactions;
using UnityEngine;

public abstract class Ability : IAbility
{
    public abstract int ReloadTime { get; }
    public IInteraction Interaction { get; }
    public bool ReadyForUse { get; private set; }
    public int TurnsLeftToReload { get; private set; }

    public void TickReload()
    {
        if(TurnsLeftToReload > 0)
            TurnsLeftToReload--;
        Debug.Log(TurnsLeftToReload + " turns left till reload of " + this);
        if (TurnsLeftToReload == 0)
        {
            ReadyForUse = true;
            Debug.Log(this + " has reloaded!");
        }
    }

    public int GetUsefulness()
    {
        //For now we will think that the more damage skill gives the more useful it is
        return Interaction.GetDamage();
    }

    public void UseAbility()
    {
        
    }
}
