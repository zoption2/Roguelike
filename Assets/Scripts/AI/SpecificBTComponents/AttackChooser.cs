using Interactions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourTree
{
    
    public class AttackChooser
    {
        public List<IInteraction> AllAttacks { get; set; }
        public IDefaultBehaviourTree DefaultBT { get;}


        public AttackChooser(IDefaultBehaviourTree defaultBehaviourTree, List<IInteraction> allAttacks)
        {
            DefaultBT = defaultBehaviourTree;
            AllAttacks = allAttacks;
        }
        public IInteraction ChooseAttack() 
        {
            List<IInteraction> availableAttacks = AllAttacks.Where(x => x.CouldUseAbility() == true ).ToList();
            availableAttacks.OrderByDescending(x => x.GetDamage() );
            IInteraction chosenAttack = null;
            Transform target = DefaultBT.GetTarget();
            foreach (IInteraction attackType in availableAttacks)
            {
                Debug.Log(attackType);
                if (DefaultBT.SphereCastHitTheTarget(target, DefaultBT.GetCharacterPosition(),attackType.GetLaunchMultiplier()))
                {
                    chosenAttack = attackType;
                    Debug.Log("enemy choosed " + chosenAttack);
                    DefaultBT.SetCurrentAttack(chosenAttack);
                    return chosenAttack;
                }
            }
            return chosenAttack;
        }
    }
}
