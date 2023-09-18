using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class ScreenSaverScript : MonoBehaviour, IPointerClickHandler
{
    public GameManager GameManager;
    public void OnPointerClick(PointerEventData eventData) {
        GameManager.UIManager.ScreenSaverClicked(eventData);
    }
}
