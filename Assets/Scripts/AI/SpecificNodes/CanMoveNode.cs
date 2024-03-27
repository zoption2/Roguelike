using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class CanMoveNode : ConditionalNode
    {
        protected override string _key { get; } = "CanMove";
    }
}
