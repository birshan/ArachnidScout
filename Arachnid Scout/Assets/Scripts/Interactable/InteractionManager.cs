using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem; // Later when changing player system, move this logic to a playerController script.

public class InteractionManager : MonoBehaviour
{
    // public delegate void PlayerEnteredInteractable();
    // public event PlayerEnteredInteractable OnPlayerEnteredInteractable;

    // public delegate void PlayerLeftInteractable();
    // public event PlayerLeftInteractable OnPlayerLeftInteractable;
    
    private GeneratedInputFile _playerInput;
    private InputAction _interactAction;
    private GameObject _currentInteractableObject;
    private bool _isCurrentlyCarryingObject = false;
    private List<GameObject> _carriedEggs = new List<GameObject>();//Multiple Egg carry support 
    private int _maxCarryLimit = 4;
    public Transform EggCarryPosition;
    // public Transform EggHolder1;
    // public Transform EggHolder2;
    // public Transform EggHolder3;
    // public Transform EggHolder4;
    public List<GameObject> EggCarryPositions;

    private List<GameObject> _nearbyEggs = new List<GameObject>();//Quality of Life change: Cycle through a list of all nearby eggs so that player dont need to re enter trigger every time to pick up an egg.
    private int _currentEggIndex = 0;//index of currently cycled egg in vicinity
    public bool IsPlayerBeingChased = false;
    public bool IsHiding = false;
    public int GrassHideCount =0;
    public GameObject Score;


    //hacky solution for egg placement
    private Vector3 _eggPlacementOffset = new Vector3(-0.55f, -0.21f, 0.08f);
    // public GameManager gameManager;


    //For crouching 
    // public GameObject hips;
    // private InputAction _crouchAction;
    // private bool crouching = false;
    
    public void UpdateHidingStatus(){
        IsHiding = GrassHideCount>0; //unless he is not in a grass clump he should be hiding

    }
    public void SetIsPlayerBeingChased(bool value)
    {
        IsPlayerBeingChased = value;
    }
    void Awake(){
        _playerInput = new GeneratedInputFile();
        // gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    
    void OnEnable(){
        _interactAction = _playerInput.Player.Interact;
        // _crouchAction = _playerInput.Player.Crouch;
        _interactAction.Enable();
        // _crouchAction.Enable();

        _interactAction.performed += OnInteractPerformed;
        // _crouchAction.performed += OnCrouchPerformed;
    }

    void OnDisable(){
        _interactAction.Disable();
        _interactAction.performed -= OnInteractPerformed;
        // _crouchAction.Disable();
        // _crouchAction.performed -= OnCrouchPerformed;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        // if(other.gameObject.tag.Equals("Egg"))
        // {
        //     //Parent this object to the player
        //     // transform.parent = other.transform;

        //     // Debug.Log("player entered collider");
        //     // OnPlayerEnteredInteractable?.Invoke();

        //     // Checking to see if theres already an interactable object near player, since the UI should only appear for one at a time.
        //     if(_currentInteractableObject == null)
        //     {
        //         other.gameObject.GetComponent<EggScript>().EnableCanvas();
        //     }

        // }

        //Update: Egg Cycling 
        if(other.gameObject.tag.Equals("Egg"))
        {
            if(!_nearbyEggs.Contains(other.gameObject) && !_carriedEggs.Contains(other.gameObject) &&_carriedEggs.Count < _maxCarryLimit)//If not already in the list of nearby eggs and not already carried
            {
                _nearbyEggs.Add(other.gameObject); //Will add any nearby egg to this list upoon trigger enter
                if(_currentInteractableObject == null)
                {
                    // _currentInteractableObject = other.gameObject;//Why wasnt this done earlier
                    other.gameObject.GetComponent<EggScript>().EnableCanvas();
                }
            }
        }



        // This doesnt really work, I accidentally deleted EggLocationCenter. Remake later.
        // if player is not beinng chased and approaches spiderneggs then switch to suspensful music
        // if(other.gameObject.tag.Equals("EggLocationCenter") && !IsPlayerBeingChased)
        // {
        //     AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.TerrorAmbience, 0.5f);
        // }

        // else if (other.gameObject.tag.Equals("Spider"))
        // {
        //     GameManager.Instance.PauseGame();
        //     GameManager.Instance.EnableReloadOnGameOver();
        // }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag.Equals("Egg"))
        {
            //Parent this object to the player
            // transform.parent = other.transform;

            // Debug.Log("Player is in the trigger collider");

            // if player pressed interact then do something
            // if(Input.GetKeyDown(KeyCode.E))
            // {
            //     Debug.Log("Player has interacted with the object");
            //     transform.parent = other.transform;
            // }

            //UPDATED COMMENT for multiple egg carry: this part is just for checking interactable eggs in the environment to enable canvas and pickability... so it should be none of the carried eggs.
            if(_currentInteractableObject == null &&!_carriedEggs.Contains(other.gameObject)) //[OUTDATED COMMENT- for single egg] if player is not carrying an egg then set currentInteractableObject
            {
                _currentInteractableObject = other.gameObject;
            }
            
            
            
        }
        else if(other.gameObject.tag.Equals("Ship"))// This is where the player drops the eggs off
        {
            // Debug.Log("Player is at the goal");
            // if player is carrying an egg then disable egg and increase score
            // ---------------- Temporary Solution ---------------- //
            // if(_isCurrentlyCarryingObject)
            // {
            //     _currentInteractableObject.GetComponent<EggScript>().DisableCanvas();
            //     _currentInteractableObject.transform.parent = null;
            //     _currentInteractableObject.SetActive(false);
            //     _currentInteractableObject = null;
            //     _isCurrentlyCarryingObject = false;
            //     Score.GetComponent<Score>().IncreaseScore();
            // }



            // ---------------- Multiple Egg Carry Solution ---------------- //
            foreach(var gameObject in EggCarryPositions){
                gameObject.transform.localPosition = _eggPlacementOffset;
            }
            if(_carriedEggs.Count>0)// If carrying atleast one egg
            {
                foreach(var egg in _carriedEggs)//Then get rid of carried eggs at the ship
                {
                    egg.GetComponent<EggScript>().DisableCanvas();
                    egg.transform.parent = null;
                    egg.SetActive(false);
                    Score.GetComponent<Score>().IncreaseScore();// Increase score for each.
                }
                _carriedEggs.Clear();// Clear the list of carried eggs
                AudioManager.Instance.PlaySoundAtPosition(AudioManager.Instance.DropEggAtShip, transform.position, 0.8f, 1f);
            }
            // AudioManager.Instance.PlaySoundAtPosition(AudioManager.Instance.DropEggAtShip, transform.position, 0.8f, 1f);
        }

    }

    private void OnTriggerExit(Collider other) {

        // if(other.gameObject.tag.Equals("Egg"))
        // {
        //     // check if player is currently carrying object if so then do not set currentInteractableObject to null
        //     // if(!_isCurrentlyCarryingObject)
        //     // {
        //     //     // Debug.Log("Player has left the trigger collider");
        //     //     // OnPlayerLeftInteractable?.Invoke();
        //     //     other.gameObject.GetComponent<EggScript>().DisableCanvas();
        //     //     _currentInteractableObject = null;
        //     // }

        //     //--- For Multiple Egg Carry ---//
        //     if(!_carriedEggs.Contains(other.gameObject))
        //     {
        //         other.gameObject.GetComponent<EggScript>().DisableCanvas();
        //         _currentInteractableObject = null;
        //     }
            
        // }

        //Update: Egg Cycling
        if(other.gameObject.tag.Equals("Egg"))
        {
            if(_nearbyEggs.Contains(other.gameObject))
            {
                other.gameObject.GetComponent<EggScript>().DisableCanvas();
                _nearbyEggs.Remove(other.gameObject);
            }

            //cycle
            if(_currentInteractableObject==other.gameObject)
            {
                _currentInteractableObject = _nearbyEggs.Count>0?_nearbyEggs[0]:null;
                if(_currentInteractableObject!=null)//here add conditioon to check if already carrying?
                {
                    _currentInteractableObject.GetComponent<EggScript>().EnableCanvas();
                }
            }
        }

        
        // if player is not beinng chased and leaves spiderneggs then switch to normal music
        // if(other.gameObject.tag.Equals("EggLocationCenter") && !IsPlayerBeingChased)
        // {
        //     AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.NightAmbience, 0.5f);
        // }
    }

    // This method runs when player presses Interact button.
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        // Debug.Log("Player has pressed interact button");
        
        
        // is there an object to interact with
        if(_currentInteractableObject != null) 
        {
            // if egg has already been picked up then drop egg

            // if(_currentInteractableObject.GetComponent<EggScript>().GetStoredStatus())
            // {
            //     return;
            // }

            EggScript eggScript = _currentInteractableObject.GetComponent<EggScript>();
            if(eggScript.GetPickedStatus() ) 
            {
                // if player is at goal then disable egg and increase score
                // if(_currentInteractableObject.GetComponent<EggScript>().GetIsStored())
                // {
                //     _currentInteractableObject.GetComponent<EggScript>().DisableCanvas();
                //     _currentInteractableObject.SetActive(false);
                //     _currentInteractableObject = null;
                //     return;
                // }
                
                //---Previous code here---
                // _currentInteractableObject.transform.parent = null; // drop egg
                // eggScript.SetPickedStatus(false);
                // // _isCurrentlyCarryingObject = false;
                // _carriedEggs.Remove(_currentInteractableObject);
                //---Previous code here---

                //--New - No more dropping eggs OPTION!---
                _currentInteractableObject.GetComponent<EggScript>().DisableCanvas();
                _nearbyEggs.Remove(_currentInteractableObject);
                _currentInteractableObject = null;
                
            }
            else if(_carriedEggs.Count < _maxCarryLimit)// if egg is not picked up then pick up egg //Update: If can carry more eggs then pick up egg
            {
                int backPackIndex = _carriedEggs.Count;
                GameObject eggCarryPosition = EggCarryPositions[backPackIndex];
                // Debug.Log("Player has interacted with the object");
                // _currentInteractableObject.transform.parent = transform;
                // _currentInteractableObject.transform.position = eggCarryPosition.transform.position; // For later
                _currentInteractableObject.transform.parent = eggCarryPosition.transform;//parent to that so it follows when crouching
                _currentInteractableObject.transform.localPosition = eggCarryPosition.transform.localPosition;
                _currentInteractableObject.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
                _currentInteractableObject.transform.localRotation = Quaternion.Euler(0,0,0);
                eggCarryPosition.transform.localPosition = Vector3.zero;
                eggScript.SetPickedStatus(true);
                // _isCurrentlyCarryingObject = true;
                _carriedEggs.Add(_currentInteractableObject);// add egg to carry list

                //disable canvas
                _currentInteractableObject.GetComponent<EggScript>().DisableCanvas();
                _nearbyEggs.Remove(_currentInteractableObject);
                _currentInteractableObject = null;
            }
            else{
                Debug.Log("Cannot carry more eggs");
            }


            //EGG CYCLING CODE -- after interacting - cycle to next
            if(_nearbyEggs.Count>0)
            {
                _currentEggIndex = (_currentEggIndex+1)%_nearbyEggs.Count;//cycle
                _currentInteractableObject = _nearbyEggs[_currentEggIndex];
                _currentInteractableObject.GetComponent<EggScript>().EnableCanvas();
            }
            else
            {
                _currentInteractableObject = null;
            }
            
        }
        
    }




    ////----------------------OUTDATED do not uncomment this
    // // treating it as a toggle for now
    // private void OnCrouchPerformed(InputAction.CallbackContext context)
    // {
    //     if(!crouching){
    //         hips.transform.position = new Vector3(hips.transform.position.x, hips.transform.position.y - 0.5f, hips.transform.position.z);
    //         crouching = true;
    //     }
    //     else{
    //         hips.transform.position = new Vector3(hips.transform.position.x, hips.transform.position.y + 0.5f, hips.transform.position.z);
    //         crouching = false;
    //     }
        
    // }

    //ToDo: Think about what to do when the player is already carrying an egg and enters another eggs collider and presses interact

}
