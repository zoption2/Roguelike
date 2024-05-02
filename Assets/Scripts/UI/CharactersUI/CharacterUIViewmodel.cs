using CharactersStats;
using Interactions;
using Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CharacterUIViewmodel
{
    public ReactiveInt ReactiveHealth;

    private CharacterModel _model;

    private IUIFactory _factory;
    private CharacterUIView _uIView;
    private Dictionary<EffectType, IEffectIconView> _visualizedEffects;
    private IDisposable activateDisposable;

    public void Init(CharacterModel model, IUIFactory uIFactory, CharacterUIView uIView)
    {
        _model = model;
        ReactiveHealth = model.GetReactiveStats().Health;
        _factory = uIFactory;
        _uIView = uIView;
        _visualizedEffects = new Dictionary<EffectType, IEffectIconView>();
        //ReactiveHealth = new ReactiveInt(model.ReactiveHealth.Value);
    }

    public void ActivateSkillsBTNs()
    {
        //activateDisposable = Observable.Timer(TimeSpan.FromSeconds(0.2f))
        //    .Subscribe(_ =>
        //    {
        //        GameObject skillsPanel = _uIView.GetSkillsPanel();
        //        skillsPanel.SetActive(!skillsPanel.activeSelf);
        //    });
    }

    public void DeactivateSkillsBTNs()
    {
        if (activateDisposable != null)
        {
            activateDisposable.Dispose();
            activateDisposable = null;
        }

        _uIView.GetSkillsPanel().SetActive(false);
    }

    public void UpdateStats(ReactiveStats stats)
    {
        ReactiveHealth = stats.Health;
        _model.SetHealth(ReactiveHealth.Value);
    }

    public void VisualiseEffects(List<IEffect> displayedEffects)
    {
        GridLayoutGroup effectPanel = _uIView.GetEffectsPanel();

        if (displayedEffects == null) return;

        foreach (var effect in displayedEffects)
        {
            EffectType effectType = effect.GetEffectType();

            if (effect.Duration > 0)
            {
                if (_visualizedEffects.ContainsKey(effectType))
                {
                    Debug.LogError(effect.Duration);
                    _visualizedEffects[effectType].UpdateDurationText(effect.Duration);
                }
                else
                {
                    IEffectIconView icon = _factory.CreateEffectIcon(effectType, effectPanel.transform.position, effectPanel.transform);
                    _visualizedEffects.Add(effectType, icon);
                    _visualizedEffects[effectType].UpdateDurationText(effect.Duration);
                }
            }
        }

        var keysToRemove = _visualizedEffects.Keys.Where(key => !displayedEffects.Any(e => e.GetEffectType() == key && e.Duration > 0)).ToList();

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
