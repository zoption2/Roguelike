using Interactions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourTree
{
    
    public class AttackChooser
    {
        public List<InteractionBase> AllAttacks { get; set; }
        public IDefaultBehaviourTree DefaultBT { get;}


        public AttackChooser(IDefaultBehaviourTree defaultBehaviourTree, List<InteractionBase> allAttacks)
        {
            DefaultBT = defaultBehaviourTree;
            AllAttacks = allAttacks;
        }
        public InteractionBase ChooseAttack() 
        {
            List<InteractionBase> availableAttacks = AllAttacks.Where(x => x.CouldUseAbility() == true ).ToList();
            availableAttacks.OrderByDescending(x => x.GetDamage() );
            InteractionBase chosenAttack = null;
            Transform target = DefaultBT.GetTarget();
            foreach (InteractionBase attackType in availableAttacks)
            {
                if (DefaultBT.SphereCastHitTheTarget(target, DefaultBT.GetCharacterPosition()))
                {
                    chosenAttack = attackType;
                    return chosenAttack;
                }
            }
            return chosenAttack;
        }
    }
}
