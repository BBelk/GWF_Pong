using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics.Tracing;
using TMPro;
using System;
using System.Linq;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine.Video;

public class UIManager : MonoBehaviour, IPointerClickHandler
{
    public GameManager GameManager;
    public List<GameObject> allScreenObjects;
    // 0 screensaver, 1 main menu, 2 game, 3 leaderboard, 4 main menu instruct/options

    public Sprite[] spriteArray;
    public RectTransform instructPanelHolderRT;
    public List<GameObject> allInstructButtons;
    public List<GameObject> allInstructPanels;
    private int instructPanelIndex;
    public VideoPlayer myVideoPlayer;

    public List<GameObject> webglChangingObjects; 

    void Start()
    {
        myVideoPlayer.url = System.IO.Path.Combine (Application.streamingAssetsPath,"Screensaver.mp4");
        LoadTexturesFromFolder("Instructions");

        StartInstructPanels();
        #if UNITY_WEBGL
            WebglStuff();
        #endif
    }

    public void WebglStuff(){
        //change a few things around to work better in webgl
        webglChangingObjects[0].SetActive(true);
        webglChangingObjects[1].SetActive(true);
        webglChangingObjects[2].GetComponent<Button>().interactable = false;
        webglChangingObjects[3].SetActive(false);
        webglChangingObjects[4].SetActive(true);

        
    }

    void LoadTexturesFromFolder(string folderPath)
    {
        Texture2D[] loadedTextures = Resources.LoadAll<Texture2D>(folderPath);

        if (loadedTextures != null && loadedTextures.Length > 0)
        {
            List<Sprite> spritesList = new List<Sprite>();

            foreach (Texture2D texture in loadedTextures)
            {
                // Create a sprite from the loaded texture
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                spritesList.Add(sprite);
            }

            spriteArray = spritesList.ToArray();
        }
    }

    public void SwitchScreen(int newIndex){
        colorChoicePanelObject.SetActive(false);
        highscorePanelObject.SetActive(false);
        instructionContainerPanelObject.SetActive(false);
        foreach(GameObject newObj in allScreenObjects){newObj.SetActive(false);}
        if(allScreenObjects[newIndex] != null)
        {
            allScreenObjects[newIndex].SetActive(true);
        }
        var logString = allScreenObjects[newIndex].gameObject.name;
        GameManager.LogToFile($"Screen shown {logString}");
        if(newIndex == 0){myVideoPlayer.Play();}
    }

    public void ScreenSaverClicked(PointerEventData newEventData){
        SwitchScreen(1);
        GameManager.LogToFile($"Screensaver clicked {newEventData.position}");
    }

    public void OnPointerClick(PointerEventData eventData) {

    }
    // void LoadSpritesFromFolder(string folderPath)
    // {
    //     List<Sprite> spritesList = new List<Sprite>();

    //     string[] files = Directory.GetFiles(folderPath);

    //     foreach (string file in files)
    //     {
    //         if (!file.EndsWith(".meta"))
    //         {
    //             byte[] fileData = File.ReadAllBytes(file);
    //             Texture2D texture = new Texture2D(2, 2);
    //             if (texture.LoadImage(fileData))
    //             {
    //                 Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    //                 spritesList.Add(sprite);
    //             }
    //         }
    //     }
    //     spriteArray = spritesList.ToArray();
    // }

    public GameObject colorChoicePanelObject;
    public GameObject instructionContainerPanelObject;
    public void ColorChoicePanelUp(){
        foreach(GameObject newObj in allInstructButtons){
            newObj.GetComponent<Button>().interactable = true;
            newObj.gameObject.SetActive(false);
        }
        allScreenObjects[4].SetActive(true);
        allInstructButtons[3].gameObject.SetActive(true);
        colorChoicePanelObject.SetActive(true);
        instructionContainerPanelObject.SetActive(false);
    }

    public void InstructionContainerPanelUp(){
        InstructButton(-2);
        colorChoicePanelObject.SetActive(false);
        instructionContainerPanelObject.SetActive(true);

    }

    public void FinishInstructions(){
        SwitchScreen(2);
        StartScreenSaverTimer();
        GameManager.MainGamePanelUp();
    }


    public void SetInstructButton()
    {
        foreach(GameObject newObj in allInstructButtons){
            newObj.GetComponent<Button>().interactable = true;
            newObj.gameObject.SetActive(false);
        }
        for(int x = 0; x < 3; x++){
            allInstructButtons[x].gameObject.SetActive(true);
        }
        if(instructPanelIndex == 0){allInstructButtons[0].GetComponent<Button>().interactable = false;}
        if(instructPanelIndex == allInstructPanels.Count - 1){allInstructButtons[2].GetComponent<Button>().interactable = false;}
    }
    public void InstructButton(int newInt){
        StartScreenSaverTimer();
        instructPanelIndex += newInt;
        if(newInt == -2){
            instructPanelIndex = 0;
        }
        MoveInstructPanels();
        SetInstructButton();

        var logString = "Instruction Back";
        if(newInt == 0){logString = "Instruction Done";FinishInstructions();}
        if(newInt == 1){logString = "Instruction Next";}
        GameManager.LogToFile(logString);
    }

public float keepWidth;
    public void StartInstructPanels()
    {
        instructPanelIndex = 0;
        var getParentRT = allInstructPanels[0].transform.parent.GetComponent<RectTransform>(); 
        allInstructPanels[0].GetComponent<RectTransform>().sizeDelta = new Vector2(getParentRT.rect.width, getParentRT.rect.height);
        keepWidth = getParentRT.rect.width + getParentRT.gameObject.GetComponent<HorizontalLayoutGroup>().spacing;
        allInstructPanels[0].GetComponent<Image>().sprite = spriteArray[0];
        for(int x = 1; x < spriteArray.Length; x++){
            var newPanel = Instantiate(allInstructPanels[0], getParentRT.transform);
            newPanel.GetComponent<Image>().sprite = spriteArray[x];
            allInstructPanels.Add(newPanel);
        }
        SetInstructButton();
        allInstructPanels[0].transform.parent.localPosition = new Vector3((keepWidth * instructPanelIndex) * -1f, 0f, 0f);
    }

    public void DragStarted(){
        if(instructSlideCo != null){StopCoroutine(instructSlideCo);instructSlideCo = null;}
    }

    public void DragEnded(Transform panelTransform){
        float xPos = panelTransform.localPosition.x;
        int closestIndex = 0;
        float closestDistance = Mathf.Abs(xPos - 0);

        for (int i = 1; i < allInstructPanels.Count; i++)
        {
            float expectedX = i * (keepWidth * -1f);
            float distance = Mathf.Abs(xPos - expectedX);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        instructPanelIndex = closestIndex;
        MoveInstructPanels();
        SetInstructButton();

    }

    public void MoveInstructPanels(){
        var newPosition = (keepWidth * instructPanelIndex) * -1f;
        if(instructSlideCo != null){StopCoroutine(instructSlideCo);instructSlideCo = null;}
        instructSlideCo = StartCoroutine(InstructSlide(newPosition));
    }

    public Coroutine instructSlideCo;
    public IEnumerator InstructSlide(float newPosX)
    {
        var elapsedTime = 0f;
        var loadTime = 1f;
        var getParentObj = allInstructPanels[0].transform.parent.gameObject;
        var getX = 0f;
    	while(elapsedTime < loadTime){
            getX = Mathf.Lerp(getParentObj.transform.localPosition.x, newPosX, elapsedTime / loadTime);
            getParentObj.transform.localPosition = new Vector3(getX, 0f, 0f);
    		elapsedTime += Time.deltaTime;
        	yield return new WaitForEndOfFrame();
    	}
    }

    public List<Sprite> allColorSprites;
    public List<Texture> allColorTextures;
    public List<int> allPlayerColorIndices;
    public List<Image> allPlayerColorImages;
    public void PlayerChangeColor(int dirIndex, int playerIndex, IPointerButtonScript newIPBS = null){
        if(newIPBS != null){newIPBS.ImageFlicker();}

        var otherPlayer = 0;
        if(playerIndex == 0){otherPlayer = 1;}

        allPlayerColorIndices[playerIndex] = allPlayerColorIndices[playerIndex] + dirIndex;

        CheckOutOfBounds(playerIndex);

        if(allPlayerColorIndices[playerIndex] == allPlayerColorIndices[otherPlayer])
        {
            allPlayerColorIndices[playerIndex] = allPlayerColorIndices[playerIndex] + dirIndex;
            CheckOutOfBounds(playerIndex);
        }

        allPlayerColorImages[playerIndex].sprite = allColorSprites[allPlayerColorIndices[playerIndex]];
        GameManager.SetPaddleControllerColors(allPlayerColorIndices);
    }

    public void CheckOutOfBounds(int playerIndex){
        if(allPlayerColorIndices[playerIndex] > allColorSprites.Count -1 ){allPlayerColorIndices[playerIndex] = 0;}
        if(allPlayerColorIndices[playerIndex] < 0){allPlayerColorIndices[playerIndex] = allColorSprites.Count -1 ;}
    }

    public Image timerWheelImage;
    public TMP_Text timerText;

    private Coroutine timerCo;
    private Coroutine fillAmountCo;

    public void ResetTimer()
    {
        if (timerCo != null){StopCoroutine(timerCo);timerCo = null;}
        if (fillAmountCo != null){StopCoroutine(fillAmountCo);fillAmountCo = null;}
        timerText.text = "";
        timerWheelImage.fillAmount = 1f;
    }

    public void StartTimer()
    {
        ResetTimer();
        timerCo = StartCoroutine(TimerEnum());
        fillAmountCo = StartCoroutine(UpdateFillAmount());
    }

    public IEnumerator TimerEnum()
    {
        int initialTime = 60;
        
        while (initialTime > 0)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(initialTime);
            timerText.text = string.Format("{0:D}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

            yield return new WaitForSeconds(1f);
            initialTime--;
        }
        GameManager.GameOver();
        timerText.text = "0:00";
    }

    public IEnumerator UpdateFillAmount()
    {
        float duration = 60f;
        float elapsed = 0f;

        while (elapsed < duration){
            float fillAmount = 1f - elapsed / duration;
            timerWheelImage.fillAmount = fillAmount;
            elapsed += Time.deltaTime;
            yield return null;
        }
        timerWheelImage.fillAmount = 0f;
    }

    public List<TMP_Text> allPlayerScoreTMP_Texts;
    

    public void UpdateGameScore(int playerIndex, int playerScoreInt){
        allPlayerScoreTMP_Texts[playerIndex].text = "" + playerScoreInt;
    }

    public TMP_Text newHighscorePlayerText;
    public TMP_InputField newHighscoreInputField;
    public GameObject highscorePanelObject;
    public GameObject highscoreInputObject;
    public GameObject noNewHighscorePanelObject;

    public void ResetNewHighscoreInputField(){
        newHighscoreInputField.text ="ABC";
    }

    public GameObject mainGameCountdownImageObject;
    public TMP_Text countdownTimerTMP_Text;
    private Coroutine countdownCo;
    public CanvasGroup countdownCG;
    public void MainGameCountdownStart(){
        timerText.text = "0:00";
        timerWheelImage.fillAmount = 1f;

        countdownTimerTMP_Text.text = "";
        if(countdownCo != null){StopCoroutine(countdownCo);countdownCo = null;}
        countdownCo = StartCoroutine(MainGameCountdownIEnum());
        mainGameCountdownImageObject.SetActive(true);
        countdownCG.alpha = 1f;
    }

    public IEnumerator MainGameCountdownIEnum(){
        var elapsedTime = 0f;
        var timeToLoad = 0.5f;
        countdownTimerTMP_Text.text = "3";
        yield return new WaitForSeconds(timeToLoad);
        countdownTimerTMP_Text.text = "2";
        yield return new WaitForSeconds(timeToLoad);
        countdownTimerTMP_Text.text = "1";
        yield return new WaitForSeconds(timeToLoad);
        countdownTimerTMP_Text.text = "GO!";
        yield return new WaitForSeconds(timeToLoad /2f);
        while(elapsedTime < timeToLoad){
            countdownCG.alpha = Mathf.Lerp(countdownCG.alpha, 0f, elapsedTime / timeToLoad);
    		elapsedTime += Time.deltaTime;
        	yield return new WaitForEndOfFrame();
        }
        GameManager.StartMatch();
        countdownTimerTMP_Text.text = "";
        mainGameCountdownImageObject.SetActive(false);
    }

    public List<int> playerScoreHighscore;

    public void FindNewHighscore(List<int> allPlayerScores){
        playerScoreHighscore.Clear();
        highscorePanelObject.SetActive(true);
        highscoreInputObject.SetActive(false);
        noNewHighscorePanelObject.SetActive(false);
        //
        // var leaderboardScores = GameManager.LeaderboardManager.allLeaderboardScores;
        
        for(int x = 0; x < allPlayerScores.Count; x++){
            if(allPlayerScores[x] >= GameManager.LeaderboardManager.allLeaderboardScores[GameManager.LeaderboardManager.allLeaderboardScores.Count -1]){
                playerScoreHighscore.Add(allPlayerScores[x]);
                continue;
            }
            playerScoreHighscore.Add(0);
        }
        if(playerScoreHighscore[0] < GameManager.LeaderboardManager.allLeaderboardScores.Last() && playerScoreHighscore[1] < GameManager.LeaderboardManager.allLeaderboardScores.Last()){
            noNewHighscorePanelObject.SetActive(true);
            return;
        }
        else{
            if(playerScoreHighscore[0] >= GameManager.LeaderboardManager.allLeaderboardScores.Last()){SetNewHighscoreInput(0);return;}
            if(playerScoreHighscore[1] >= GameManager.LeaderboardManager.allLeaderboardScores.Last()){SetNewHighscoreInput(1);return;}
        }
    }

    private int highscorePlayerIndex;

    public void SetNewHighscoreInput(int playerIndex){
        highscorePlayerIndex = playerIndex;
        newHighscoreInputField.text = "ABC";
        highscoreInputObject.SetActive(true);
        newHighscorePlayerText.text = $"PLAYER {playerIndex + 1}'s SCORE WAS: {playerScoreHighscore[playerIndex]}";
    }

    public void FinishLeaderboardInitial(){
        if(!string.IsNullOrWhiteSpace(newHighscoreInputField.text)){
            GameManager.LeaderboardManager.AddNewScore(newHighscoreInputField.text.ToUpper(), playerScoreHighscore[highscorePlayerIndex]);
            //
            if(playerScoreHighscore[1] >= GameManager.LeaderboardManager.allLeaderboardScores.Last() && highscorePlayerIndex == 0){SetNewHighscoreInput(1);return;}
            SwitchScreen(3);
            return;
        }
    }

    public Coroutine screenSaverCo;
    public void StartScreenSaverTimer(){
        if(screenSaverCo != null){StopCoroutine(screenSaverCo); screenSaverCo = null;}
        screenSaverCo = StartCoroutine(ScreenSaverIEnum());
    }
    
    public IEnumerator ScreenSaverIEnum(){
        yield return new WaitForSeconds(90);
        SwitchScreen(0);
    }
}