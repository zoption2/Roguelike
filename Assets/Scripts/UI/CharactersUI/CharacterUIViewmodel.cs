using CharactersStats;
using Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIViewmodel
{
    public ReactiveInt ReactiveHealth;

    private CharacterModel _model;

    private ICharacterController _characterController;
    private IDisposable activateDisposable;
    private IUIFactory _factory;
    private CharacterUIView _uIView;
    private Dictionary<EffectType, IEffectIconView> _visualizedEffects;
    private GridLayoutGroup _abilityPanel;
    private GameObject _abilityBTNs;
    private Image _activeIndicator;
    private Button[] _buttons;
    private List<IAbility> _abilities;
    private List<IAbilityIconView> _abilityIcons;
    private bool isActivated = false;
    private bool _isActive = false;

    private Vector3 _previousPanelPosition;

    public void Init(CharacterModel model, IUIFactory uIFactory, CharacterUIView uIView, ICharacterController characterController)
    {
        _characterController = characterController;
        _model = model;
        ReactiveHealth = model.GetReactiveStats().Health;
        _factory = uIFactory;
        _uIView = uIView;
        _visualizedEffects = new Dictionary<EffectType, IEffectIconView>();
        _abilityBTNs = _uIView.GetAbilityBTNs();
        _abilities = _model.Abilities;
        _abilityIcons = new List<IAbilityIconView>();
        _activeIndicator = _uIView.GetActiveIndicator();
        _abilityPanel = _uIView.GetAbilityPanel();
        VisualiseAbilities();
    }

    public async void ActivateSkillsBTNs()
    {
        isActivated = !isActivated; 
        await Task.Delay(200);
        if (isActivated)
        {
            _abilityBTNs.SetActive(true);
            ChangeAbilityPanelPos();
        }
        else
        {
            _abilityBTNs.SetActive(false);
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
        _abilityBTNs.SetActive(false);
    }

    public void UpdateStats(ReactiveStats stats)
    {
        ReactiveHealth = stats.Health;
        _model.SetHealth(ReactiveHealth.Value);
    }

    private void ChangeAbilityPanelPos()
    {
        
        // Отримання розмірів панелі
        RectTransform panelRect = _abilityPanel.GetComponent<RectTransform>();
        Vector2 panelSize = panelRect.sizeDelta;

        // Отримання розмірів камери
        Camera mainCamera = Camera.main;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Отримання меж камери
        float cameraHalfWidth = cameraWidth / 2f;
        float cameraHalfHeight = mainCamera.orthographicSize;

        // Позиція панелі
        Vector3 panelPosition = _abilityPanel.transform.position;
        float panelHalfWidth = panelSize.x / 2f;
        float panelHalfHeight = panelSize.y / 2f;

        // Розрахунок відстані панелі від камери
        Vector3 panelDistanceFromCamera = mainCamera.transform.position - panelPosition;
        float horizontalDistanceFromCamera = Mathf.Abs(Vector3.Dot(panelDistanceFromCamera, mainCamera.transform.right));
        float verticalDistanceFromCamera = Mathf.Abs(Vector3.Dot(panelDistanceFromCamera, mainCamera.transform.up));

        // Перевірка, чи виходить панель за межі камери на 20% і більше
        bool isHorizontalOutOfCamera = horizontalDistanceFromCamera - panelHalfWidth > cameraHalfWidth;
        bool isVerticalOutOfCamera = verticalDistanceFromCamera - panelHalfHeight > cameraHalfHeight;

        float newX = panelPosition.x;
        float newY = panelPosition.y;

        _abilityPanel.transform.position = new Vector3(0, 0, panelPosition.z);

        if (isVerticalOutOfCamera)
        {
            newY += 1.25f; // Змінюємо позицію по Y вгору
        }

        float distanceToRightEdge = cameraWidth - panelPosition.x;
        float distanceToLeftEdge = panelPosition.x;

        // Переміщення панелі в більш віддалену від краю сторону
        if (distanceToRightEdge > distanceToLeftEdge) // Панель ближче до правого краю
        {
            newX += 2.55f; // Змінюємо позицію по X направо
        }
        else // Панель ближче до лівого краю
        {
            newX -= 2.55f; // Змінюємо позицію по X наліво
        }


        // Змінення позиції панелі
        _abilityPanel.transform.position = new Vector3(newX, newY, panelPosition.z);
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
