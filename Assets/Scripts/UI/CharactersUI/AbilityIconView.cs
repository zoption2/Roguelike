using Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Abilities;
public interface IAbilityIconView : IMyPoolable
{
    public void Init(IAbility ability);
    public void UpdateReloadIndicators();
}

public class AbilityIconView : MonoBehaviour, IAbilityIconView
{
    [SerializeField] private Image _reloadIndicator;
    [SerializeField] private TMP_Text _reloadText;

    private IAbility _ability;
    private int _previousTurnsLeftToReload = 0;

    public void Init(IAbility ability)
    {
        _ability = ability;
    }

    public void UpdateReloadIndicators()
    {
        int currentTurnsLeftToReload = _ability.TurnsLeftToReload;
        int maxReloadTime = _ability.ReloadTime;

        if(maxReloadTime > 0)
        {
            float reloadPercentage = (float)(currentTurnsLeftToReload + 1) / maxReloadTime;
            _reloadIndicator.fillAmount = reloadPercentage;
        }
        
        _reloadText.text = (currentTurnsLeftToReload + 1).ToString();

        bool sameAsPrevious = currentTurnsLeftToReload == _previousTurnsLeftToReload;

        _previousTurnsLeftToReload = currentTurnsLeftToReload;


        _reloadText.gameObject.SetActive(!sameAsPrevious || currentTurnsLeftToReload > 0);
        if(sameAsPrevious)
        {
            _reloadIndicator.fillAmount = 0;
        }
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
