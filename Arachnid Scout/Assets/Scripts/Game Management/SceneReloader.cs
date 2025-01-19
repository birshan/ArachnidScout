using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Later when changing player system, move this logic to a playerController script.

public class SceneReloader : MonoBehaviour
{
    //load scene
    // public void LoadScene(string sceneName)
    // {
    //     UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    // }
    private GeneratedInputFile _playerInput;
    private InputAction _restartAction; // Reload the scene
    // private bool _isGameOver = false;

    private bool _hasGameStarted = false;
    void Awake(){
        _playerInput = new GeneratedInputFile();
    }
    public void OnRestartPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Reloading Scene");
        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update() {
        if(_hasGameStarted)return;
        //Used chatgpt to solve issue with hacky solution
        // Check if any key is pressed
        if (Keyboard.current != null && Keyboard.current.anyKey.isPressed)
        {
            OnGameStartButtonInSceneReloader();
        }

        // Check if any mouse button is pressed
        if (Mouse.current != null && (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed || Mouse.current.middleButton.isPressed))
        {
            OnGameStartButtonInSceneReloader();
        }

        // Check if any gamepad button is pressed
        // if (Gamepad.current != null && Gamepad.current.allControls.Exists(control => control.IsPressed()))
        // {
        //     OnGameStartButtonInSceneReloader();
        // }
    }

    void OnEnable(){
       
        _restartAction = _playerInput.Player.Restart;
        _restartAction.Enable();
        _restartAction.performed += OnRestartPerformed;
    }

    void OnDisable(){
        _restartAction.Disable();
        _restartAction.performed -= OnRestartPerformed;
    }

    public void OnGameStartButtonInSceneReloader(){
        GameManager.Instance.OnGameStartButton();
    }
}
