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
    [SerializeField] private GameObject _skillsBTNs;

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
        ChangeHealthBarOnEndTurn(_scrollbar.size);
    }

    public void ChangeHealthBarOnEndTurn(float newSize)
    {
        RectTransform scrollbarRectTransform = _scrollbar.GetComponent<RectTransform>();
        Vector2 newSizeDelta = scrollbarRectTransform.sizeDelta;
        newSizeDelta.x = newSize * _maxHealth;
        scrollbarRectTransform.sizeDelta = newSizeDelta;
    }


    public GridLayoutGroup GetEffectsPanel()
    {
        return _effectsPanel;
    }

    public GameObject GetSkillsPanel()
    {
        return _skillsBTNs;
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
