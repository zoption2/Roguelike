using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackAbility : Ability
{
    public BasicAttackAbility(IInteraction interaction, int reloadTime, float launchMod, AbilityType type) : base(interaction, reloadTime, launchMod, type)
    {
    }
}
