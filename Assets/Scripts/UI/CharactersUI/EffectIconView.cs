using Pool;
using UnityEngine;
using TMPro;

public interface IEffectIconView : IMyPoolable
{
    void UpdateDurationText(float duration);
}

public class EffectIconView : MonoBehaviour, IEffectIconView
{
    [SerializeField] TMP_Text durationText;

    public void UpdateDurationText(float duration)
    {
        durationText.text = duration.ToString(); 
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
