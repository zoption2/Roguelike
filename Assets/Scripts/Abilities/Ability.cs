using Interactions;
using UnityEngine;

public abstract class Ability : IAbility
{
    public int ReloadTime { get; }
    public IInteraction Interaction { get; }
    public bool ReadyForUse { get; private set; }
    public int TurnsLeftToReload { get; private set; }

    protected float _launchModifier;

    protected Ability(IInteraction interaction,int reloadTime,float launchMod)
    {
        ReadyForUse = true;
        TurnsLeftToReload = 0;
        Interaction = interaction;
        ReloadTime = reloadTime;
        _launchModifier = launchMod;
    }

    public void TickReload()
    {
        if (TurnsLeftToReload > 0)
        {
            TurnsLeftToReload--;
            Debug.Log(TurnsLeftToReload + " turns left till reload of " + this);
        }
        if (TurnsLeftToReload == 0 && ReadyForUse==false)
        {
            ReadyForUse = true;
            Debug.Log(this + " has reloaded!");
        }
    }

    public void SetForReload()
    {
        TurnsLeftToReload = ReloadTime;
        if (TurnsLeftToReload > 0)
        {
            ReadyForUse = false;
            Debug.Log(this + " is now reloading. It will be reloading for " + TurnsLeftToReload + " turns");
        }
    }

    public int GetUsefulness()
    {
        //For now we will think that the more damage skill gives the more useful it is
        return Interaction.GetDamage();
    }

    public void UseAbility()
    {
        // do something and then set for reload
        SetForReload();
    }

    public TypeOfUse GetUseType()
    {
        return TypeOfUse.MeleeUse;
    }

    public float GetLaunchModifier()
    {
        return _launchModifier;
    }
}
