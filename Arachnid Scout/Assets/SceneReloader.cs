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

    void Awake(){
        _playerInput = new GeneratedInputFile();
    }
    public void OnRestartPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Reloading Scene");
        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
}
