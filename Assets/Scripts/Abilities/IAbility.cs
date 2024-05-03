using Interactions;


public interface IAbility 
{
    public bool ReadyForUse { get;}
    public int ReloadTime { get; }
    public int TurnsLeftToReload { get;}
    public IInteraction Interaction { get;}
    public AbilityType Type { get;}
    public void UseAbility();
    public void TickReload();
    public void SetForReload();
    public int GetUsefulness();
    public float GetLaunchModifier();
    public TypeOfUse GetUseType();
}
