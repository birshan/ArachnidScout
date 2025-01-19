using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score Instance { get; private set; }
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI eggsLeftText;
    private int eggsToWin = 10;

    public delegate void OnScoreChanged();
    public event OnScoreChanged ScoreChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        
    }
    // Start is called before the first frame update
    
    void Start()
    {
        PlayerPrefs.SetInt("EggsCollected", 0);
        PlayerPrefs.SetInt("EggsLeft", 26);
        scoreText.text = "Eggs Collected: " + PlayerPrefs.GetInt("EggsCollected") + "/"+eggsToWin;
        eggsLeftText.text = "Eggs in Map: " + PlayerPrefs.GetInt("EggsLeft");

        ScoreChanged?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseScore()
    {
        PlayerPrefs.SetInt("EggsCollected", PlayerPrefs.GetInt("EggsCollected") + 1);
        scoreText.text = "Eggs Collected: " + PlayerPrefs.GetInt("EggsCollected")+ "/"+eggsToWin;

        if(PlayerPrefs.GetInt("EggsCollected") == eggsToWin)
        {
            GameManager.Instance.EnableReloadOnSuccess();
            GameManager.Instance.PauseGame();
        }
        ReduceEggsLeft();
        ScoreChanged?.Invoke();
    }
    public void ReduceEggsLeft()
    {
        PlayerPrefs.SetInt("EggsLeft", PlayerPrefs.GetInt("EggsLeft") - 1);
        eggsLeftText.text = "Eggs in Map: " + PlayerPrefs.GetInt("EggsLeft");

        if(PlayerPrefs.GetInt("EggsLeft") < PlayerPrefs.GetInt("EggsCollected") - eggsToWin)
        {
            GameManager.Instance.RanOutOffEggsText();
            GameManager.Instance.EnableReloadOnGameOver();
            GameManager.Instance.PauseGame();
        }
        ScoreChanged?.Invoke();
    }
}
