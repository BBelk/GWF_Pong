using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IPointerButtonScript : MonoBehaviour, IPointerClickHandler
{
    public GameManager GameManager;
    public int dirIndex;
    public int playerIndex;
    public bool doFlicker;
    public Coroutine FlickerCo;
    public void OnPointerClick(PointerEventData eventData) {
        GameManager.UIManager.PlayerChangeColor(dirIndex, playerIndex, this);
        GameManager.UIManager.StartScreenSaverTimer();
        GameManager.LogToFile($"Player #{playerIndex} clicked paddle color button");
    }

    public void ImageFlicker(){
        if(!doFlicker){return;}
        if(FlickerCo != null){StopCoroutine(FlickerCo);FlickerCo = null;}
        FlickerCo = StartCoroutine(FlickerRoutine());
    }

    public IEnumerator FlickerRoutine()
    {
        var elapsedTime = 0f;
        var loadTime = 0.7f;
        var thisCG = this.gameObject.GetComponent<CanvasGroup>();
        thisCG.alpha = 0.25f;
    	while(elapsedTime < loadTime){
            thisCG.alpha = Mathf.Lerp(thisCG.alpha, 1f, elapsedTime / loadTime);
    		elapsedTime += Time.deltaTime;
        	yield return new WaitForEndOfFrame();
    	}
    }
}
