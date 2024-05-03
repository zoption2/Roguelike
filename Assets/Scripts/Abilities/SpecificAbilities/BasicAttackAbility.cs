using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackAbility : Ability
{
    public BasicAttackAbility(IInteraction interaction, int reloadTime, float launchMod) : base(interaction,reloadTime,launchMod)
    {
        
    }
}
