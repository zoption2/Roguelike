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
    private Dictionary<EffectType, IMyPoolable> _visualizedEffects;

    public void Init(CharacterUIModel model, IUIFactory uIFactory, CharacterUIView uIView)
    {
        _model = model;
        ReactiveHealth = model.ReactiveHealth;
        _factory = uIFactory;
        _uIView = uIView;
        _visualizedEffects = new Dictionary<EffectType, IMyPoolable>();
        //ReactiveHealth = new ReactiveInt(model.ReactiveHealth.Value);
    }

    public void UpdateStats(ReactiveStats stats)
    {
        ReactiveHealth = stats.Health;
        _model.SetHealth(ReactiveHealth.Value);
    }

    public void VisualiseEffects(List<IEffect> displayedEffects)
    {
        EffectType effectType;
        GridLayoutGroup effectPanel = _uIView.GetEffectsPanel();

        foreach (var effect in displayedEffects)
        {
            effectType = effect.GetEffectType();

            if (!_visualizedEffects.ContainsKey(effectType))
            {
                IMyPoolable icon = _factory.CreateEffectIcon(effectType, effectPanel.transform.position, effectPanel.transform);
                _visualizedEffects.Add(effectType, icon);
            }
        }

        List<EffectType> keysToRemove = new List<EffectType>();
        foreach (var effectInVisualized in _visualizedEffects)
        {
            if (!displayedEffects.Any(e => e.GetEffectType() == effectInVisualized.Key))
            {
                _factory.RemoveEffectIcon(effectInVisualized.Key, effectInVisualized.Value);
                keysToRemove.Add(effectInVisualized.Key);
            }
        }

        foreach (var keyToRemove in keysToRemove)
        {
            _visualizedEffects.Remove(keyToRemove);
        }
    }

    public void Submit()
    {
        _model.SetHealth(ReactiveHealth.Value);
    }
}
