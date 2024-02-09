using UnityEngine.EventSystems;
using UnityEngine;

public class DragInputModule : StandaloneInputModule
{
    static public GameObject dragFocusObject;

    protected override void ProcessDrag(PointerEventData pointerEvent)
    {   
        if (pointerEvent.dragging && (dragFocusObject != null))
        {
            pointerEvent.pointerDrag = dragFocusObject;
        }
        base.ProcessDrag(pointerEvent);
    }
}