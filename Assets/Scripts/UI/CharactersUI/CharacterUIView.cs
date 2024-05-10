using Pool;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public interface ICharacterUIView
{
    void Init(CharacterView view, CharacterUIViewmodel viewmodel);
}

public class CharacterUIView : MonoBehaviour, IMyPoolable, ICharacterUIView
{
    [SerializeField] private Slider _greenBar;
    [SerializeField] private Slider _redBar;
    [SerializeField] private GridLayoutGroup _effectsPanel;
    [SerializeField] private GameObject _abilityPanel;
    [SerializeField] private Image _activeIndicator;

    private ICharacterView _characterView;
    private CharacterUIViewmodel _viewmodel;

    private float _maxHealth;

    public void Init(CharacterView view, CharacterUIViewmodel viewmodel)
    {
        _characterView = view;
        _viewmodel = viewmodel;

        _maxHealth = _viewmodel.ReactiveHealth.Value;

        _redBar.maxValue = _maxHealth;
        _greenBar.maxValue = _maxHealth;

        _redBar.value = _maxHealth;
        _greenBar.value = _maxHealth;

        _viewmodel.ReactiveHealth.Subscribe(ChangeHealthBar);
        
    }

    private void FixedUpdate()
    {
        transform.position = _characterView.transform.position;
    }

    private void ChangeHealthBar(int newHealth)
    {
        Debug.LogWarning("Max Health: " + _maxHealth);
        Debug.LogWarning("Current Health: " + newHealth);
        _greenBar.value = newHealth;
    }

    public void ChangeHealthBarOnEndTurn()
    {
        float endValue = _greenBar.value;
        float duration = 0.5f;
        _redBar.DOValue(endValue, duration);
    }

    public GridLayoutGroup GetEffectsPanel()
    {
        return _effectsPanel;
    }

    public GameObject GetAbilityPanel()
    {
        return _abilityPanel;
    }

    public Image GetActiveIndicator()
    {
        return _activeIndicator;
    }

    public void OnCreate()
    {
    }

    public void OnPull()
    {
    }

    public void OnRelease()
    {
    }
}
