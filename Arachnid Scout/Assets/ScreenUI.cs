using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenUI : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI UIscoreText;
    public TextMeshProUGUI UIeggsLeftText;
    
    private void Start(){
        Score.Instance.ScoreChanged += UpdateUI;
    }
    private void OnDestroy() {
        Score.Instance.ScoreChanged -= UpdateUI;
    }


    // Update is called once per frame
    private void UpdateUI()
    {
        Debug.Log("UpdateUI");
        UIscoreText.text = Score.Instance.scoreText.text;
        UIeggsLeftText.text = Score.Instance.eggsLeftText.text;
    }
}
