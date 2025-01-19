using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BillBoardUI : MonoBehaviour
{

    public TextMeshProUGUI Text;
    private GameObject _playerFollowCamera;
    // private Vector3 _cameraPositionOffset; 

    private void Awake() {
        Text = GetComponentInChildren<TextMeshProUGUI>();
        _playerFollowCamera = GameObject.FindGameObjectWithTag("FollowCamera");
        // _player.GetComponent<InteractionManager>().OnPlayerEnteredInteractable += ShowText;
        // _player.GetComponent<InteractionManager>().OnPlayerLeftInteractable += HideText;

    }
    void Start()
    {
        // Hide the canvas initially
        // interactionCanvas.enabled = false;

        // Color rgba = Text.color;
        // rgba.a = 0.0f; // make text invisible initially
        // Text.color = rgba;
        
        
    }

    private void Update() {
        // turn the canvas to face the player
        //flip text its worng way
        // _cameraPositionOffset = _playerFollowCamera.transform.position;
        // _cameraPositionOffset.y = _playerFollowCamera.transform.position.y - 180;
        Text.transform.LookAt(_playerFollowCamera.transform);
        Text.transform.Rotate(0, 180, 0);
        //flip the text
        
    }

    // public void ShowText()
    // {
    //     Debug.Log("Showing text");
    //     Color rgba = Text.color;
    //     rgba.a = 1.0f; // make text visible
    //     Text.color = rgba;
    // }
    // public void HideText(){
    //     Color rgba = Text.color;
    //     rgba.a = 0.0f; // make text invisible
    //     Text.color = rgba;
    // }
    // Using enable disabling game object instead. 
    
}
