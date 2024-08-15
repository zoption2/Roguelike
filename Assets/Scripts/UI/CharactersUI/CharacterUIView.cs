using DG.Tweening;
using Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private TextMeshProUGUI _healthText;

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

        _healthText.text = _maxHealth.ToString();
        _damageText.gameObject.SetActive(false);

        _viewmodel.ReactiveHealth.Subscribe(ChangeHealthBar);

    }

    private void FixedUpdate()
    {
        transform.position = _characterView.transform.position;
    }

    private void ChangeHealthBar(int newHealth)
    {
        int damage = Mathf.RoundToInt(_greenBar.value - newHealth);
        ShowDamageText(damage);

        _greenBar.value = newHealth;

        _healthText.text = newHealth.ToString();
    }

    private void ShowDamageText(int damage)
    {
        _damageText.gameObject.SetActive(true);

        _damageText.text = $"-{damage}";

        Vector2 initialPosition = _damageText.rectTransform.anchoredPosition;
        float initialAlpha = _damageText.color.a;

        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(0.4f, 0.8f);

        _damageText.rectTransform.DOAnchorPos(new Vector2(randomX, randomY), 0.5f).SetRelative(true);

        _damageText.DOFade(0, 1f).OnComplete(() =>
        {
            _damageText.rectTransform.anchoredPosition = initialPosition;

            _damageText.color = new Color(_damageText.color.r, _damageText.color.g, _damageText.color.b, initialAlpha);

            _damageText.gameObject.SetActive(false);
        });
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
