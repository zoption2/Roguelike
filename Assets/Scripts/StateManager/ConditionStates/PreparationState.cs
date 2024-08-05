using Interactions;
using Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PreparationState : ActiveState, IConditionState
{
    public PreparationState(ICharacterController characterController, ProjectilePooler projectilePooler) : base(characterController, projectilePooler)
    {
    }

    public void Attack()
    {
    }

    public void LaunchYourselfToPoint(Vector3 point)
    {
    }

    public void Move()
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.LogWarning(this);
    }
}
