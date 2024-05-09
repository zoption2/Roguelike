using Pool;
using UnityEngine;
using UnityEngine.UI;

public interface ICharacterUIView
{
    void Init(CharacterView view, CharacterUIViewmodel viewmodel);
}

public class CharacterUIView : MonoBehaviour, IMyPoolable, ICharacterUIView
{
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private GridLayoutGroup _effectsPanel;
    [SerializeField] private GridLayoutGroup _abilityPanel;
    [SerializeField] private GameObject _abilityBTNs;
    [SerializeField] private Image _activeIndicator;

    private ICharacterView _characterView;
    private CharacterUIViewmodel _viewmodel;

    private float _maxHealth;

    public void Init(CharacterView view, CharacterUIViewmodel viewmodel)
    {
        _characterView = view;
        _viewmodel = viewmodel;

        _maxHealth = _viewmodel.ReactiveHealth.Value;

        float normalizedHealth = _viewmodel.ReactiveHealth.Value / _maxHealth;

        _scrollbar.size = normalizedHealth;

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
        _scrollbar.size = (float)newHealth / _maxHealth;
    }

    public void ChangeHealthBarOnEndTurn()
    {
        float currentSize = _scrollbar.size;
        RectTransform rt = _scrollbar.GetComponent<RectTransform>();

        Vector2 newSizeDelta = rt.sizeDelta;

        float pixelWidth = currentSize * rt.rect.width;

        newSizeDelta.x = pixelWidth;

        rt.sizeDelta = newSizeDelta;
    }


    public GridLayoutGroup GetEffectsPanel()
    {
        return _effectsPanel;
    }

    public GridLayoutGroup GetAbilityPanel()
    {
        return _abilityPanel;
    }

    public GameObject GetAbilityBTNs()
    {
        return _abilityBTNs;
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
