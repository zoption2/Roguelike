using Interactions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IConditionState
{
    public void OnEnter();
    public void OnExit();
    public IInteraction GetInteraction();
    public void ApplyInteraction(IInteraction interaction);
    public void LaunchYourself(Vector3 direction);
    public void AddEffects(List<IEffect> effects);
    public void ViewRotation();
    public void DoUpdate();
    public void UseSlingshotAsync(PointerEventData eventData, Transform slingShotInitPosition);
    public void LaunchYourselfToPoint(Vector3 point);
    public void Attack();
    public void Move();
    public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer);
}

public delegate void OnStopped();