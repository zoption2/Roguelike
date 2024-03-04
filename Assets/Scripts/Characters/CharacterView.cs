using Pool;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICharacterView
{
    event Action<Transform, PointerEventData> ON_CLICK;
    event Action<PointerEventData> ON_BEGINDRAG;
}

public class CharacterView : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, ICharacterView, IMyPoolable
{
    public event Action<Transform, PointerEventData> ON_CLICK;
    public event Action<PointerEventData> ON_BEGINDRAG;
    private Transform _transform;

    public void OnCreate()
    {
        //Debug.LogWarning($"Hello from {this.name} view");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _transform = transform;
        ON_CLICK?.Invoke(_transform, eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("hELLO&&!&!&!&");
        _transform = transform;
        ON_BEGINDRAG?.Invoke(eventData);
    }

    public void OnPull()
    {

    }

    public void OnRelease()
    {
    }
}

