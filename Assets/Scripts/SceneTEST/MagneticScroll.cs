using System.Collections;
using System.Collections.Generic;
using UI;
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
    private ILevelSelectorView _levelSelectorView;
    private List<ICharacterPanelView> _characterPanelViews;

    private void Start()
    {
        //_levelSelectorView = FindObjectOfType<LevelSelectorView>();
    }

    public void Init(ILevelSelectorView levelSelectorView)
    {
        _levelSelectorView = levelSelectorView;
        _characterPanelViews = _levelSelectorView.GetCharacterPanels();

        characterPanels = new RectTransform[_characterPanelViews.Count];
        for (int i = 0; i < _characterPanelViews.Count; i++)
        {
            characterPanels[i] = _characterPanelViews[i].GameObject.GetComponent<RectTransform>();
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
            Debug.Log("Before while");
            while (Vector2.Distance(contentPanel.anchoredPosition, targetPosition) > snapThreshold)
            {
                contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, targetPosition, snapSpeed * Time.deltaTime);
                yield return null;
            }
            Debug.Log("After while");
            contentPanel.anchoredPosition = targetPosition;

            SelectClosestPanel(closestPanel);
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

    private void SelectClosestPanel(RectTransform closestPanel)
    {
        foreach (var characterPanelView in _characterPanelViews)
        {
            if (characterPanelView.GameObject.GetComponent<RectTransform>() == closestPanel)
            {
                ICharacterPanelController charcterPanelController = characterPanelView.GetCharacterPanelController();
                charcterPanelController.ChangeBool(true); 
            }
            else
            {
                characterPanelView.GetCharacterPanelController().ChangeBool(false);
            }
        }
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
