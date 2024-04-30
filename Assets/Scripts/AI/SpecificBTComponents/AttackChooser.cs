using Interactions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourTree
{
    
    public class AttackChooser
    {
        public List<IAbility> AllAbilities { get; set; }
        public IDefaultBehaviourTree DefaultBT { get;}


        public AttackChooser(IDefaultBehaviourTree defaultBehaviourTree, List<IAbility> allAbilities)
        {
            DefaultBT = defaultBehaviourTree;
            AllAbilities = allAbilities;
        }
        public IAbility ChooseAbility() 
        {
            List<IAbility> availableAbilities = AllAbilities.Where(x => x.ReadyForUse == true ).ToList();
            availableAbilities.OrderByDescending(x => x.GetUsefulness() );
            IAbility chosenAttack = null;
            Transform target = DefaultBT.GetTarget();
            foreach (IAbility abilityType in availableAbilities)
            {
                Debug.Log(abilityType + "   multiplier: " + abilityType.GetLaunchModifier());
                Debug.DrawLine(DefaultBT.GetCharacterPosition(), target.position, Color.red, 4);
                bool attackWouldReachTarget = DefaultBT.SphereCastHitTheTarget(target, DefaultBT.GetCharacterPosition(), abilityType.GetLaunchModifier());
                Debug.Log("Attack would reach target: " + attackWouldReachTarget);
                if (attackWouldReachTarget)
                {
                    chosenAttack = abilityType;
                    Debug.Log("enemy choosed " + chosenAttack);
                    DefaultBT.SetCurrentAbility(chosenAttack);
                    return chosenAttack;
                }
            }
            return chosenAttack;
        }
    }
}
