using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;

public class GameManager : MonoBehaviour
{
    public UIManager UIManager;
    public LeaderboardManager LeaderboardManager;
    public float paddleBound;
    public List<int> allPlayerScores;
    private string logFilePath;

    void Start()
    {
        logFilePath = Path.Combine(Application.persistentDataPath, "usage.txt");

        UIManager.SwitchScreen(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash)){
            WipeFile();
            UnityEngine.Debug.Log("ERASED LOG FILE");
        }
    }

    public void LogToFile(string newString)
    {
        UIManager.StartScreenSaverTimer();
        string logMessage = $"{DateTime.Now}: {newString}";
        File.AppendAllText(logFilePath, logMessage + "\n");
    }

    public void WipeFile(){
        File.WriteAllText(logFilePath, "");
    }

    public void OpenLogDirectory()
    {
        UnityEngine.Debug.Log("OPEN LOG");
        if (File.Exists(logFilePath)){
            Application.OpenURL("file://" + logFilePath.Replace("\\", "/"));
        }
    }


    public List<PaddleController> allPaddleControllers;
    public BallController BallController;

    public void MainGamePanelUp(){
        for(int x = 0; x < allPlayerScores.Count; x++){
            allPlayerScores[x] = 0;
            UIManager.UpdateGameScore(x, 0);
        }
        UIManager.MainGameCountdownStart();
        foreach(PaddleController newPC in allPaddleControllers){
            newPC.ResetPaddle();
        }
    }

    public void StartMatch(){
        foreach(PaddleController newPC in allPaddleControllers){
            newPC.StartMovement();
        }
        BallController.ResetBall();
        BallController.StartMovement();
        UIManager.StartTimer();
        for(int x = 0; x < allPlayerScores.Count; x++){
            allPlayerScores[x] = 0;
            UIManager.UpdateGameScore(x, 0);
        }
    }

    public void SetPaddleControllerColors(List<int> allPlayerColorIndices){
        for(int x = 0; x < allPaddleControllers.Count; x++){
            allPaddleControllers[x].SetPaddleTexture(UIManager.allColorTextures[allPlayerColorIndices[x]]);
        }
    }

    public void GoalHit(int playerInt, int lastHitIndex){
        // Debug.Log($"PLAYER #{playerInt} HIT BY PLAYER #{lastHitIndex}");
        allPlayerScores[playerInt] += 1;
        UIManager.UpdateGameScore(playerInt, allPlayerScores[playerInt]);  
        BallController.Scored();
        Invoke("ResetBall", 1.0f);
    }
    public void ResetBall(){
        BallController.ResetBall();
        BallController.StartMovement();
    }

    public void GameOver(){
        BallController.Scored();
        CancelInvoke();
        foreach(PaddleController newPC in allPaddleControllers){
            newPC.StopMovement();
        }
        UIManager.FindNewHighscore(allPlayerScores);
        UIManager.StartScreenSaverTimer();
    }
}
