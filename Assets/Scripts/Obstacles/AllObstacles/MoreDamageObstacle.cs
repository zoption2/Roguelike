using Interactions;
using System.Collections.Generic;
using UnityEngine;

public class MoreDamageObstacle : MonoBehaviour 
{
    public List<IEffect> ProcessTrigger()
    {
        List<IEffect> effects = new List<IEffect>
        {
            new MoreDamageEffect(1),
        };
        return effects;
    }
}
