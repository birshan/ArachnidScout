using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour
{
    
    // Start is called before the first frame update
    private GameObject billBoardUI;
    private bool _isPickedUp = false;
    private bool _isStored = false;
    private GameObject _player;
    private SpawnHatchlings _spawnHatchlings;
    private void Awake() {
        _player = GameObject.FindGameObjectWithTag("Player");
        // _player.GetComponent<InteractionManager>().OnPlayerEnteredInteractable += EnableCanvas;
        // _player.GetComponent<InteractionManager>().OnPlayerLeftInteractable += DisableCanvas;
        billBoardUI = transform.GetChild(0).gameObject;
        DisableCanvas();
        _spawnHatchlings = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SpawnHatchlings>();

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPickedStatus(bool status)
    {
        _isPickedUp = status;
        if(status){
            // billBoardUI.GetComponent<BillBoardUI>().HideText();
            DisableCanvas();
        } else if(!_isStored){ // only re enable canvas if the egg is not stored at goal
            // billBoardUI.GetComponent<BillBoardUI>().ShowText();
            EnableCanvas();
        }
    }

    public void SetStoredStatus(bool status){
        _isStored = status;
        if(status){
            // billBoardUI.GetComponent<BillBoardUI>().HideText();
            DisableCanvas();
        } 

    }
    public bool GetStoredStatus(){
        return _isStored;
    }

    public void EnableCanvas()
    {
        billBoardUI.gameObject.SetActive(true);
    }
    public void DisableCanvas()
    {
        billBoardUI.gameObject.SetActive(false);
    }

    public bool GetPickedStatus()
    {
        return _isPickedUp;
    }

    public void HatchEgg()
    {
        // do not hatch eggs that the player is carrying 
        if(_isPickedUp)
        {
            return;
        }
        billBoardUI.gameObject.SetActive(true);
        billBoardUI.GetComponent<BillBoardUI>().Text.text = "!!! SCREEEEECH  !!!";
        StartCoroutine(HatchEggCoroutine());
        
    }

    IEnumerator HatchEggCoroutine()
    {
        yield return new WaitForSeconds(1);
        // GameObject.Instantiate(Resources.Load("Assets/Spider/SpiderHatchling.prefab"), transform.position, Quaternion.identity);
        _spawnHatchlings.SpawnHatchling(transform.position, Quaternion.identity);
        Score.Instance.ReduceEggsLeft();
        Destroy(gameObject);
        
    }

    // private void OnTriggerEnter(Collider other) {
    //     if(other.gameObject.tag.Equals("Player"))
    //     {
    //         //Parent this object to the player
    //         // transform.parent = other.transform;

    //         Debug.Log("player entered collider");
    //     }
    // }

    // private void OnTriggerStay(Collider other) {
    //     if(other.gameObject.tag.Equals("Player"))
    //     {
    //         //Parent this object to the player
    //         // transform.parent = other.transform;

    //         Debug.Log("Player is in the trigger collider");
    //         billBoardUI.GetComponent<BillBoardUI>().ShowText();

    //         // toDO: Check if player pressed input and collect the egg.
    //     }
    // }

    // private void OnTriggerExit(Collider other) {
    //     if(other.gameObject.tag.Equals("Player"))
    //     {

    //         Debug.Log("Player has left the trigger collider");
    //         billBoardUI.GetComponent<BillBoardUI>().HideText();
    //     }
    // }

    //toDo: Use on trigger stay to create an event that UI can listen to
    //toDo: make the collider bigger so that the UI will pop up when player gets close 
}
