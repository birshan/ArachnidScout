using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine  ;
public class GameManager : MonoBehaviour
{
    public CinemachineVirtualCamera VC1;
    public CinemachineVirtualCamera VC2;
    public CinemachineVirtualCamera VC3;
    public CinemachineVirtualCamera VC4;
    public CinemachineVirtualCamera VCPlayerFollow;
    private bool transitioningToVC2 = false;
    private CinemachineBrain cinemachineBrain;
    public static GameManager Instance { get; private set; }
    public GameObject reloadUI;
    public GameObject successUI;
    public GameObject StartGameUI;
    public GameObject PressAnyButtonTextGameObject; //starts out inactive - then becomes active after a few seconds 
    // Start is called before the first frame update
   
    private void Awake()
    {
        // Singleton setup - ChatGPT singleton (only for the setting up - forgot how)
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

        StartCoroutine(ShowPressAnyButtonTextAfterDelay());
        // cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        // cinemachineBrain.m_CameraActivatedEvent.AddListener(OnCameraActivated);

        // start the transition
        // VC1.gameObject.SetActive(false); //should transition to next prio cam
        //use priority to switch otherwise cant track event
        // VC1.Priority = 0; // Now will switch to next highest.

        
    }

    IEnumerator ShowPressAnyButtonTextAfterDelay()
    {
        yield return new WaitForSeconds(6);
        PressAnyButtonTextGameObject.SetActive(true);
    }
    public void OnGameStartButton(){
        Debug.Log("Game Start Button Clicked");
        VC1.Priority = 0;
        StartCoroutine(TransitionToVC3());
        AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.NightAmbience, 0.5f);
        StartGameUI.SetActive(false);
    }

    IEnumerator TransitionToVC3()
    {
        yield return new WaitForSeconds(2);
        // VC2.gameObject.SetActive(true);
        VC2.Priority = 0;
        // transitioningToVC2 = true;
        StartCoroutine(TransitionToVC4());
    }

    IEnumerator TransitionToVC4()
    {
        yield return new WaitForSeconds(6);
        VC3.Priority = 0;
        StartCoroutine(TransitionToVCPlayerFollow());
    }

    IEnumerator TransitionToVCPlayerFollow()
    {
        yield return new WaitForSeconds(4);
        VC4.Priority = 0;
    }

    // private void OnCameraActivated(ICinemachineCamera fromCam, ICinemachineCamera toCam)
    // {
    //     if (transitioningToVC2 && toCam == VC2)
    //     {
    //         Debug.Log("reached VC2, now switch to VC3");
    //         transitioningToVC2 = false;
    //         // VC2.gameObject.SetActive(false);
    //         VC2.Priority = 0;
    //     }
    //     if(toCam==VCPlayerFollow){
    //         Debug.Log("reached VCPlayerFollow");
    //     }
    // }

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
        AudioManager.Instance.StopAllAudio();
    }

    public void RanOutOffEggsText()
    {
        TextMeshProUGUI text = reloadUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = "Not Enough Eggs Left";
        text.fontSize = 22;
    }
}
