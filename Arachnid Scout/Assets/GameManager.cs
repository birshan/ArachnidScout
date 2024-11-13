using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject reloadUI;
    public GameObject successUI;
    // Start is called before the first frame update
   
    private void Awake()
    {
        // Singleton setup - ChatGPT singleton
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Keeps the GameManager across scene loads
        }
        // else
        // {
        //     Destroy(gameObject); // Destroy duplicate GameManagers
        //     return; // Important to prevent further execution in duplicate instance
        // }
    }
    void Start()
    {
        reloadUI.SetActive(false);
        successUI.SetActive(false);
        Time.timeScale = 1;

    }


    public void EnableReloadOnGameOver()
    {
        // Enable the reload UI
        reloadUI.SetActive(true);
    }

    public void EnableReloadOnSuccess()
    {
        // Enable the reload UI
        successUI.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void RanOutOffEggsText()
    {
        TextMeshProUGUI text = reloadUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = "Not Enough Eggs Left";
        text.fontSize = 22;
    }
}
