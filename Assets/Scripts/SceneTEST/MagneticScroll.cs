using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagneticScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private float snapSpeed = 10f;
    [SerializeField] private float snapThreshold = 0.2f;

    private RectTransform[] characterPanels;
    private RectTransform scrollViewport;

    private void Start()
    {
        characterPanels = new RectTransform[contentPanel.childCount];
        for (int i = 0; i < contentPanel.childCount; i++)
        {
            characterPanels[i] = contentPanel.GetChild(i).GetComponent<RectTransform>();
        }

        scrollViewport = scrollRect.viewport;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StopAllCoroutines();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(SnapToClosestPanelCoroutine());
    }

    private IEnumerator SnapToClosestPanelCoroutine()
    {
        yield return new WaitForEndOfFrame();

        RectTransform closestPanel = GetClosestPanel();

        if (closestPanel != null)
        {
            Vector2 targetPosition = GetSnapPosition(closestPanel);

            while (Vector2.Distance(contentPanel.anchoredPosition, targetPosition) > snapThreshold)
            {
                contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, targetPosition, snapSpeed * Time.deltaTime);
                yield return null;
            }

            contentPanel.anchoredPosition = targetPosition;
        }
    }

    private RectTransform GetClosestPanel()
    {
        float closestDistance = Mathf.Infinity;
        RectTransform closestPanel = null;

        foreach (var panel in characterPanels)
        {
            float distance = Mathf.Abs(GetDistanceToCenter(panel));

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPanel = panel;
            }
        }

        return closestPanel;
    }

    private float GetDistanceToCenter(RectTransform panel)
    {
        Vector3 panelWorldPosition = panel.TransformPoint(panel.rect.center);
        Vector3 viewportWorldCenter = scrollViewport.TransformPoint(scrollViewport.rect.center);

        return panelWorldPosition.x - viewportWorldCenter.x;
    }

    private Vector2 GetSnapPosition(RectTransform panel)
    {
        Vector3 panelLocalPosition = contentPanel.InverseTransformPoint(panel.TransformPoint(panel.rect.center));
        Vector3 viewportCenterLocalPosition = contentPanel.InverseTransformPoint(scrollViewport.TransformPoint(scrollViewport.rect.center));

        Vector2 offset = new Vector2(panelLocalPosition.x - viewportCenterLocalPosition.x, 0);
        return contentPanel.anchoredPosition - offset;
    }
}
