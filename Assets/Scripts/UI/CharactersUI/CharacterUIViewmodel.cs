using CharactersStats;
using Interactions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Abilities;


public class CharacterUIViewmodel
{
    public ReactiveInt ReactiveHealth;

    private CharacterModel _model;
    private ICharacterController _characterController;
    private IUIFactory _factory;
    private CharacterUIView _uIView;
    private Dictionary<EffectType, IEffectIconView> _visualizedEffects;
    private GameObject _abilityPanel;
    private Image _activeIndicator;
    private Button[] _buttons;
    private List<IAbility> _abilities;
    private List<IAbilityIconView> _abilityIcons;
    private bool isActivated = false;
    private bool _isActive = false;

    
    public void Init(CharacterModel model, IUIFactory uIFactory, CharacterUIView uIView, ICharacterController characterController)
    {
        _characterController = characterController;
        _model = model;
        ReactiveHealth = model.GetReactiveStats().Health;
        _factory = uIFactory;
        _uIView = uIView;
        _visualizedEffects = new Dictionary<EffectType, IEffectIconView>();
        _abilities = _model.Abilities;
        _abilityIcons = new List<IAbilityIconView>();
        _activeIndicator = _uIView.GetActiveIndicator();
        _abilityPanel = _uIView.GetAbilityPanel();
        
        VisualiseAbilities();
    }

    public  void ActivateSkillsBTNs()
    {
        isActivated = !isActivated; 
        if (isActivated)
        {
            _abilityPanel.SetActive(true);
            
        }
        else
        {
            _abilityPanel.SetActive(false);

        }
    }

    public void ToggleActiveIndicator()
    {
        if (_characterController.IsActive)
        {
            _activeIndicator.gameObject.SetActive(true);
        }
        else
        { 
            _activeIndicator.gameObject.SetActive(false);
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
        _abilityPanel.SetActive(false);
    }

    public void UpdateStats(ReactiveStats stats)
    {
        ReactiveHealth = stats.Health;
        _model.SetHealth(ReactiveHealth.Value);
    }

    public void VisualiseAbilities()
    {
        foreach (var ability in _abilities)
        {
            AbilityType type = ability.Type;
            IAbilityIconView abilityIcon = _factory.CreateAbilityIcon(type, _abilityPanel.transform.position, _abilityPanel.transform);
            abilityIcon.Init(ability);

            _abilityIcons.Add(abilityIcon);
        }

        _buttons = _abilityPanel.GetComponentsInChildren<Button>();


        for (int i = 0; i < _buttons.Length; i++)
        {
            int index = i; 
            _buttons[i].onClick.AddListener(() => OnAbilityButtonClick(_abilities[index]));
        }
    }

    
    public void ChangeButtonInteractible(IAbility ability,bool value)
    {
        int index = _abilities.IndexOf(ability);
        _buttons[index].interactable = value;
    }

    private void OnAbilityButtonClick(IAbility ability)
    {
        _characterController.RevertReadyUnactiveAbilityButtons();
        _characterController.SetCurrentAbility(ability);
        ChangeButtonInteractible(ability,false);
    }

    public  void VisualiseEffects(List<IEffect> displayedEffects)
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
                    IEffectIconView icon =  _factory.CreateEffectIcon(effectType, effectPanel.transform.position, effectPanel.transform);
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

    public void Dispose()
    {
        _characterController = null;
        _model = null;
        _factory = null;
        _activeIndicator = null;
        _abilityIcons.Clear();
    }
}
