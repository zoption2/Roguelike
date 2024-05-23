using Interactions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourTree
{
    
    public class AbilityChooser
    {
        public List<IAbility> AllAbilities { get; set; }
        public IDefaultBehaviourTree DefaultBT { get;}


        public AbilityChooser(IDefaultBehaviourTree defaultBehaviourTree, List<IAbility> allAbilities)
        {
            DefaultBT = defaultBehaviourTree;
            AllAbilities = allAbilities;
        }
        public IAbility ChooseAbility(Vector3 startingPoint, float remainingDistance = -1f) 
        {
            List<IAbility> availableAbilities = AllAbilities.Where(x => x.ReadyForUse == true ).ToList();
            availableAbilities = availableAbilities.OrderByDescending(x => x.GetUsefulness()).ToList();
            IAbility chosenAbility = null;
            Transform target = DefaultBT.GetTarget();
            foreach (IAbility abilityType in availableAbilities)
            {
                Debug.Log(abilityType + "   multiplier: " + abilityType.GetLaunchModifier());
                bool attackWouldReachTarget = DefaultBT.SphereCastHitTheTarget(target, startingPoint, abilityType.GetLaunchModifier(),remainingDistance);
                Debug.Log("Attack would reach target: " + attackWouldReachTarget);
                if (attackWouldReachTarget)
                {
                    chosenAbility = abilityType;
                    Debug.Log("enemy choosed " + chosenAbility);
                    DefaultBT.SetCurrentAbility(chosenAbility);
                    return chosenAbility;
                }
            }
            DefaultBT.SetCurrentAbility(chosenAbility);
            return chosenAbility;
        }
    }
}
