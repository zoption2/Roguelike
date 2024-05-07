using CharactersStats;
using Interactions;
using Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CharacterUIViewmodel
{
    public ReactiveInt ReactiveHealth;

    private CharacterModel _model;

    private ICharacterController _characterController;

    private IUIFactory _factory;
    private CharacterUIView _uIView;
    private Dictionary<EffectType, IEffectIconView> _visualizedEffects;
    private IDisposable activateDisposable;
    GameObject _abilityBTNs;
    Button[] _buttons;
    List<IAbility> _abilities;
    List<IAbilityIconView> _abilityIcons;
    private bool isActivated = false;

    public void Init(CharacterModel model, IUIFactory uIFactory, CharacterUIView uIView, ICharacterController characterController)
    {
        _characterController = characterController;
        _model = model;
        ReactiveHealth = model.GetReactiveStats().Health;
        _factory = uIFactory;
        _uIView = uIView;
        _visualizedEffects = new Dictionary<EffectType, IEffectIconView>();
        _abilityBTNs = _uIView.GetAbilityBTNs();
        //ReactiveHealth = new ReactiveInt(model.ReactiveHealth.Value);
        _abilities = _model.Abilities;
        _abilityIcons = new List<IAbilityIconView>();
        VisualiseAbilities();
    }

    public async void ActivateSkillsBTNs()
    {
        isActivated = !isActivated; 
        await Task.Delay(200);
        if (isActivated)
        {
            _abilityBTNs.SetActive(true);
        }
        else
        {
            _abilityBTNs.SetActive(false);
        }
    }

    public void UpdateReloadIndicators()
    {
        foreach (IAbilityIconView abilityIcon in _abilityIcons)
        {
            abilityIcon.UpdateReloadIndicators();
        }
    }

    public void DeactivateSkillsBTNs()
    {
        isActivated = false;
        _abilityBTNs.SetActive(false);
    }

    public void UpdateStats(ReactiveStats stats)
    {
        ReactiveHealth = stats.Health;
        _model.SetHealth(ReactiveHealth.Value);
    }

    public void VisualiseAbilities()
    {
        GridLayoutGroup abilityPanel = _uIView.GetAbilityPanel();
        

        foreach (var ability in _abilities)
        {
            AbilityType type = ability.Type;
            IAbilityIconView abilityIcon = _factory.CreateAbilityIcon(type, abilityPanel.transform.position, abilityPanel.transform);
            abilityIcon.Init(ability);

            _abilityIcons.Add(abilityIcon);//
        }

        _buttons = _abilityBTNs.GetComponentsInChildren<Button>();


        for (int i = 0; i < _buttons.Length; i++)
        {
            int index = i; 
            _buttons[i].onClick.AddListener(() => OnAbilityButtonClick(_abilities[index]));
        }
    }

    public void RevertButtonInteractible(IAbility ability)
    {
        int index = _abilities.IndexOf(ability);
        _buttons[index].interactable = !_buttons[index].interactable;
    }

    private void OnAbilityButtonClick(IAbility ability)
    {
        _characterController.SetCurrentAbility(ability);
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

    public void UpdateHealthBar()
    {
        _uIView.ChangeHealthBarOnEndTurn();
    }

}
