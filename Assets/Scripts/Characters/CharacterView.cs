using Pool;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICharacterView
{
    event Action<Transform, PointerEventData> ON_CLICK;
}

public class CharacterView : MonoBehaviour, IPointerDownHandler, ICharacterView, IMyPoolable
{
    public event Action<Transform, PointerEventData> ON_CLICK;
    private Transform _transform;

    public void OnCreate()
    {
        Debug.LogWarning($"Hello from {this.name} view");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _transform = transform;
        ON_CLICK?.Invoke(_transform, eventData);
    }

    public void OnPull()
    {

    }

    public void OnRelease()
    {
    }




}

