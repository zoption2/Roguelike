using CharactersStats;
using Interactions;
using Pool;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CharacterUIViewmodel
{
    public ReactiveInt ReactiveHealth;

    private CharacterUIModel _model;

    private IUIFactory _factory;
    private CharacterUIView _uIView;
    private Dictionary<EffectType, IEffectIconView> _visualizedEffects;

    public void Init(CharacterUIModel model, IUIFactory uIFactory, CharacterUIView uIView)
    {
        _model = model;
        ReactiveHealth = model.ReactiveHealth;
        _factory = uIFactory;
        _uIView = uIView;
        _visualizedEffects = new Dictionary<EffectType, IEffectIconView>();
        //ReactiveHealth = new ReactiveInt(model.ReactiveHealth.Value);
    }

    public void UpdateStats(ReactiveStats stats)
    {
        ReactiveHealth = stats.Health;
        _model.SetHealth(ReactiveHealth.Value);
    }

    public void VisualiseEffects(List<IEffect> displayedEffects)
    {
        GridLayoutGroup effectPanel = _uIView.GetEffectsPanel();

        foreach (var effect in displayedEffects)
        {
            EffectType effectType = effect.GetEffectType();

            if (!_visualizedEffects.ContainsKey(effectType))
            {
                IEffectIconView icon = _factory.CreateEffectIcon(effectType, effectPanel.transform.position, effectPanel.transform);
                _visualizedEffects.Add(effectType, icon);
            }

            _visualizedEffects[effectType].UpdateDurationText(effect.Duration);
        }

        var keysToRemove = _visualizedEffects.Keys.Except(displayedEffects.Select(e => e.GetEffectType())).ToList();
        foreach (var key in keysToRemove)
        {
            _factory.RemoveEffectIcon(key, _visualizedEffects[key]);
            _visualizedEffects.Remove(key);
        }
    }

    public void Submit()
    {
        _model.SetHealth(ReactiveHealth.Value);
    }
}
