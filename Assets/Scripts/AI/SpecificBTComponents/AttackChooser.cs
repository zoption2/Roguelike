using Interactions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Abilities;


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
            bool attackWouldReachTarget;

            foreach (IAbility abilityType in availableAbilities)
            {
                if (abilityType.ProjectileType== ProjectileType.None)
                    attackWouldReachTarget = DefaultBT.SphereCastHitTheTarget(target, startingPoint, abilityType.GetLaunchModifier(), remainingDistance);
                else
                    attackWouldReachTarget = DefaultBT.RemoteSphereCastHitTarget(target,startingPoint);

                if (attackWouldReachTarget)
                {
                    chosenAbility = abilityType;
                    DefaultBT.SetCurrentAbility(chosenAbility);
                    return chosenAbility;
                }
            }

            DefaultBT.SetCurrentAbility(chosenAbility);
            return chosenAbility;
        }
    }
}
