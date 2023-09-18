using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public GameManager GameManager;
    public RectTransform panelRectTransform;
    private Vector2 initialPointerPosition;
    private Vector3 initialPanelPosition;
    private bool isDragging = false;


    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            initialPointerPosition = eventData.position;
            initialPanelPosition = panelRectTransform.position;
            GameManager.UIManager.DragStarted();
            isDragging = true;
            GameManager.LogToFile($"Instruction panels dragged {eventData.position}");
            GameManager.UIManager.StartScreenSaverTimer();
        }
        float offsetX = eventData.position.x - initialPointerPosition.x;
        Vector3 newPosition = new Vector3(initialPanelPosition.x + offsetX, initialPanelPosition.y, initialPanelPosition.z);
        panelRectTransform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        GameManager.UIManager.DragEnded(this.transform);
    }
}
