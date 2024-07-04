using CharactersStats;
using Interactions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async void ActivateSkillsBTNs()
    {
        //ChangeAbilityPanelPos();
        isActivated = !isActivated; 
        await Task.Delay(200);
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
        _isActive = !_isActive; 
        _activeIndicator.gameObject.SetActive(_isActive); 
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

    //private void ChangeAbilityPanelPos()
    //{
    //    RectTransform panelRect = _abilityPanel.GetComponent<RectTransform>();
    //    Vector2 panelSize = panelRect.sizeDelta;

    //    Camera mainCamera = Camera.main;
    //    float cameraHeight = 2f * mainCamera.orthographicSize;
    //    float cameraWidth = cameraHeight * mainCamera.aspect;

    //    float cameraHalfWidth = cameraWidth / 2f;
    //    float cameraHalfHeight = mainCamera.orthographicSize;

    //    _abilityPanel.transform.localPosition = new Vector3(0, -2, 0);

    //    Vector3 panelPosition = _abilityPanel.transform.position;

    //    float panelHalfWidth = panelSize.x / 2f;
    //    float panelHalfHeight = panelSize.y / 2f;

    //    Vector3 panelDistanceFromCamera = mainCamera.transform.position - panelPosition;
       
    //    float verticalDistanceFromCamera = Mathf.Abs(Vector3.Dot(panelDistanceFromCamera, mainCamera.transform.up));

    //    bool isVerticalOutOfCamera = verticalDistanceFromCamera - panelHalfHeight > cameraHalfHeight;

    //    if (isVerticalOutOfCamera)
    //    {
    //        float newX = panelPosition.x;

    //        float distanceToRightEdge = cameraHalfWidth - (panelPosition.x + panelHalfWidth);
    //        float distanceToLeftEdge = cameraHalfWidth + (panelPosition.x - panelHalfWidth);

    //        if (distanceToRightEdge > distanceToLeftEdge)
    //        {
    //            newX += 2.55f;
    //        }
    //        else
    //        {
    //            newX -= 2.55f;
    //        }

    //        _abilityPanel.transform.position = new Vector3(newX, panelPosition.y + 1.25f, panelPosition.z);
    //    }
    //}


    public async void VisualiseAbilities()
    {
        
        foreach (var ability in _abilities)
        {
            AbilityType type = ability.Type;
            IAbilityIconView abilityIcon = await _factory.CreateAbilityIconAsync(type, _abilityPanel.transform.position, _abilityPanel.transform);
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

    public async void VisualiseEffects(List<IEffect> displayedEffects)
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
                    IEffectIconView icon = await _factory.CreateEffectIconAsync(effectType, effectPanel.transform.position, effectPanel.transform);
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
