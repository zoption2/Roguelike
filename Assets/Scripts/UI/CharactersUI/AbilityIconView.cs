using Pool;
using UnityEngine;
using UnityEngine.UI;

public interface IAbilityIconView : IMyPoolable
{

}

public class AbilityIconView : MonoBehaviour, IAbilityIconView
{
    [SerializeField] private Image _reloadIndicator;

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
