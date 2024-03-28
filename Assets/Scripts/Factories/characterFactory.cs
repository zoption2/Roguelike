using CharactersStats;
using Enemy;
using Gameplay;
using Interactions;
using Player;
using Pool;
using Prefab;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IControllerInputs
{
    ReactiveStats GetCharacterStats();
    IInteraction GetInteraction();
    void ApplyInteraction(IInteraction interactions);
    void AddEffects(List<IEffect> effects);
}

public interface ICharacterController
{
    public bool IsActive { get; set; }
    public event OnEndTurn ON_END_TURN;
    public void Init(CharacterModel model, CharacterView playerView, CharacterPooler pooler);
    public void Attack();
    public void Move();
    public void Tick();
    public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext);
    public Transform GetTransform();

}

public abstract class CharacterFactory<TController>
    where TController : ICharacterController
{
    protected IStatsProvider _statsProvider;
    protected DiContainer _container;
    protected CharacterPooler _characterPooler;
    protected CharacterModel _characterModel;
    protected OriginStats _stats;
    protected CharacterType _type;
    protected IMyPoolable _poolable;


    public CharacterFactory(
        DiContainer container,
        IStatsProvider statsProvider,
        CharacterPooler pooler)
    {
        _container = container;
        _statsProvider = statsProvider;
        _characterPooler = pooler;
        _characterPooler.Init();
    }
    protected virtual TController CreateCharacter(Transform point, CharacterType type)
    {
        TController controller = GetNewController();

        RawMapper mapper = new RawMapper();

        _stats = GetStats(type);
        _characterModel = new CharacterModel(_stats, type);

        _poolable = _characterPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        CharacterView characterView = _poolable.gameObject.GetComponent<CharacterView>();

        controller.Init(_characterModel, characterView, _characterPooler);

        mapper.Controller = controller;
        DataTransfer.RawMappers.Add(mapper);

        return controller;
    }

    protected abstract OriginStats GetStats(CharacterType type);

    protected TController GetNewController()
    {
        return _container.Resolve<TController>();
    }
}