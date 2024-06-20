using Pool;
using UnityEngine;
using Zenject;

public interface IPoolManager
{
    public BuffPooler GetBuffPooler();
    public EffectPooler GetEffectPooler();
    public AbilityIconPooler GetAbilityIconPooler();
    public CharacterPanelPooler GetCharacterPanelPooler();
    public CharacterPooler GetCharacterPooler();
    public CharacterUIPooler GetCharacterUIPooler();
    public ProjectilePooler GetProjectilePooler();
    public SlingshotPooler GetSlingshotPooler();

    public void InitPoolers();
    public void CleanPoolers();
}

public class PoolManager : IPoolManager
{
    private BuffPooler _buffPooler;
    private EffectPooler _effectPooler;
    private AbilityIconPooler _abilityIconPooler;
    private CharacterPanelPooler _characterPanelPooler;
    private CharacterPooler _characterPooler;
    private CharacterUIPooler _characterUIPooler;
    private ProjectilePooler _projectilePooler;
    private SlingshotPooler _slingshotPooler;

    [Inject]
    public void Construct(
        BuffPooler buffPooler,
        EffectPooler effectPooler,
        AbilityIconPooler abilityIconPooler,
        CharacterPanelPooler characterPanelPooler,
        CharacterPooler characterPooler,
        CharacterUIPooler characterUIPooler,
        ProjectilePooler projectilePooler,
        SlingshotPooler slingshotPooler
        )
    {
        _buffPooler = buffPooler;
        _effectPooler = effectPooler;
        _abilityIconPooler = abilityIconPooler;
        _characterPanelPooler = characterPanelPooler;
        _characterPooler = characterPooler;
        _characterUIPooler = characterUIPooler;
        _projectilePooler = projectilePooler;
        _slingshotPooler = slingshotPooler;
    }

    public void InitPoolers()
    {
        _buffPooler.Init();
        _effectPooler.Init();
        _abilityIconPooler.Init();
        _characterPanelPooler.Init();
        _characterPooler.Init();
        _characterUIPooler.Init();
        _projectilePooler.Init();
        _slingshotPooler.Init();
    }

    public void CleanPoolers()
    {
        _buffPooler.CleanPool();
        _effectPooler.CleanPool();
        _abilityIconPooler.CleanPool();
        _characterPanelPooler.CleanPool();
        _characterPooler.CleanPool();
        _characterUIPooler.CleanPool();
        _projectilePooler.CleanPool();
        _slingshotPooler.CleanPool();
    }

    public BuffPooler GetBuffPooler() { return _buffPooler; }
    public EffectPooler GetEffectPooler() { return _effectPooler; }
    public AbilityIconPooler GetAbilityIconPooler() { return _abilityIconPooler; }
    public CharacterPanelPooler GetCharacterPanelPooler() { return _characterPanelPooler; }
    public CharacterPooler GetCharacterPooler() { return _characterPooler; }
    public CharacterUIPooler GetCharacterUIPooler() { return _characterUIPooler; }
    public ProjectilePooler GetProjectilePooler() { return _projectilePooler; }
    public SlingshotPooler GetSlingshotPooler() { return _slingshotPooler; }
}
