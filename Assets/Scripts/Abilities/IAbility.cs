using Interactions;

public interface IAbility 
{
    public bool ReadyForUse { get;}
    public int ReloadTime { get; }
    public int TurnsLeftToReload { get;}
    public IInteraction Interaction { get;}
    public void UseAbility();
    public void TickReload();
    public int GetUsefulness();
}
