using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree {

    public class CanAttackNode : ConditionalNode
    {
        protected override string _key { get; } = "CanAttack";
        
    }
}
