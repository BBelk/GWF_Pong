using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    
    public List<string> allLeaderboardStrings;
    public List<int> allLeaderboardScores;

    public List<TMP_Text> allLeaderboardInitialTexts;
    public List<TMP_Text> allLeaderboardScoreTexts;

    public List<string> prepopulateStrings;
    public List<int> prepopulateScores;
    // Start is called before the first frame update
    void Start()
    {
        GenerateTexts();
        
        allLeaderboardStrings = new List<string>(new string[9]); 
        allLeaderboardScores = new List<int>(new int[9]); 

        var firstRun = PlayerPrefs.GetInt("firstRun");
        if(firstRun == 0){
            PrepopulateTMP_Texts();
            PlayerPrefs.SetInt("firstRun", 1);
            SaveInitialsAndScores();
            PopulateTMP_Texts();
            return;
        }
        LoadInitialsAndScores();
        PopulateTMP_Texts();

        // AddNewScore("killme??", 1);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash)){
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs deleted");
        }
    }

    public void LoadInitialsAndScores(){
        for(int x = 0; x < allLeaderboardStrings.Count; x++){
            allLeaderboardStrings[x] = PlayerPrefs.GetString("playerInitialString" + x);
            allLeaderboardScores[x] = PlayerPrefs.GetInt("playerScoreInt" + x);
        }
    }

    public void SaveInitialsAndScores(){
        for(int x = 0; x < allLeaderboardStrings.Count; x++){
            PlayerPrefs.SetString("playerInitialString" + x, allLeaderboardStrings[x]);
            PlayerPrefs.SetInt("playerScoreInt" + x, allLeaderboardScores[x]);
        }
    }

    public void GenerateTexts()
    {
        for(int x = 0; x < 8; x++){
            var newStringText = Instantiate(allLeaderboardInitialTexts[x].gameObject, allLeaderboardInitialTexts[x].transform.parent);
            allLeaderboardInitialTexts.Add(newStringText.GetComponent<TMP_Text>());
            //
            var newScoreText = Instantiate(allLeaderboardScoreTexts[x].gameObject, allLeaderboardScoreTexts[x].transform.parent);
            allLeaderboardScoreTexts.Add(newScoreText.GetComponent<TMP_Text>());
        }
    }

    public void PrepopulateTMP_Texts()
    {
        for(int x = 0; x < prepopulateStrings.Count; x++){
            allLeaderboardStrings[x] = prepopulateStrings[x];
            allLeaderboardScores[x] = prepopulateScores[x];
        }
    }
    public void PopulateTMP_Texts()
    {
        for(int x = 0; x < allLeaderboardStrings.Count; x++){
            allLeaderboardInitialTexts[x].text = allLeaderboardStrings[x];
            allLeaderboardScoreTexts[x].text = "" + allLeaderboardScores[x];
        }
    }

    public void AddNewScore(string newInitial, int newScore)
    {
        int insertIndex = -1;   
        for (int i = 0; i < allLeaderboardScores.Count; i++){
            if (newScore > allLeaderboardScores[i]){
                insertIndex = i;
                break;
            }
            else if (newScore == allLeaderboardScores[i]){
                insertIndex = i;
                break;
            }
        }
        if (insertIndex == -1){return;}
        for (int i = allLeaderboardScores.Count - 1; i > insertIndex; i--){
            allLeaderboardScores[i] = allLeaderboardScores[i - 1];
            allLeaderboardStrings[i] = allLeaderboardStrings[i - 1];
        }
        allLeaderboardScores[insertIndex] = newScore;
        allLeaderboardStrings[insertIndex] = newInitial;

        SaveInitialsAndScores();
        PopulateTMP_Texts();
    }

}
