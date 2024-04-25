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

        _viewmodel.ReactiveHealth.Subscribe(ChangeHealth);
    }

    private void FixedUpdate()
    {
        transform.position = _characterView.transform.position;
    }

    private void ChangeHealth(int newHealth)
    {
        Debug.LogWarning("Max Health: " + _maxHealth);
        Debug.LogWarning("Current Health: " + newHealth);
        _scrollbar.size = (float)newHealth / _maxHealth;
    }

    public void AddItemToGrid(GameObject itemPrefab)
    {
        GameObject newItem = Instantiate(itemPrefab, _effectsPanel.transform);
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
