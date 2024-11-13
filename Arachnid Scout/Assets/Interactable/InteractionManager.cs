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
    public Transform EggCarryPosition;

    public GameObject Score;
    // public GameManager gameManager;
    
    void Awake(){
        _playerInput = new GeneratedInputFile();
        // gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    
    void OnEnable(){
        _interactAction = _playerInput.Player.Interact;
        _interactAction.Enable();

        _interactAction.performed += OnInteractPerformed;
    }

    void OnDisable(){
        _interactAction.Disable();
        _interactAction.performed -= OnInteractPerformed;
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
        if(other.gameObject.tag.Equals("Egg"))
        {
            //Parent this object to the player
            // transform.parent = other.transform;

            // Debug.Log("player entered collider");
            // OnPlayerEnteredInteractable?.Invoke();

            // Checking to see if theres already an interactable object near player, since the UI should only appear for one at a time.
            if(_currentInteractableObject == null)
            {
                other.gameObject.GetComponent<EggScript>().EnableCanvas();
            }
            


        }

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
            if(_currentInteractableObject == null)
            {
                _currentInteractableObject = other.gameObject;
            }
            
            
            
        }
        else if(other.gameObject.tag.Equals("Ship"))
        {
            // Debug.Log("Player is at the goal");
            // if player is carrying an egg then disable egg and increase score
            // ---------------- Temporary Solution ---------------- //
            if(_isCurrentlyCarryingObject)
            {
                _currentInteractableObject.GetComponent<EggScript>().DisableCanvas();
                _currentInteractableObject.transform.parent = null;
                _currentInteractableObject.SetActive(false);
                _currentInteractableObject = null;
                _isCurrentlyCarryingObject = false;
                Score.GetComponent<Score>().IncreaseScore();
            }
        }

    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag.Equals("Egg"))
        {
            // check if player is currently carrying object if so then do not set currentInteractableObject to null
            if(!_isCurrentlyCarryingObject)
            {
                // Debug.Log("Player has left the trigger collider");
                // OnPlayerLeftInteractable?.Invoke();
                other.gameObject.GetComponent<EggScript>().DisableCanvas();
                _currentInteractableObject = null;
            }
            
        }
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
            if(_currentInteractableObject.GetComponent<EggScript>().GetPickedStatus() ) 
            {
                // if player is at goal then disable egg and increase score
                // if(_currentInteractableObject.GetComponent<EggScript>().GetIsStored())
                // {
                //     _currentInteractableObject.GetComponent<EggScript>().DisableCanvas();
                //     _currentInteractableObject.SetActive(false);
                //     _currentInteractableObject = null;
                //     return;
                // }
                _currentInteractableObject.transform.parent = null; // drop egg
                _currentInteractableObject.GetComponent<EggScript>().SetPickedStatus(false);
                _isCurrentlyCarryingObject = false;
            }
            else // if egg is not picked up then pick up egg
            {
                // Debug.Log("Player has interacted with the object");
                _currentInteractableObject.transform.parent = transform;
                _currentInteractableObject.transform.position = EggCarryPosition.position; // For later
                _currentInteractableObject.GetComponent<EggScript>().SetPickedStatus(true);
                _isCurrentlyCarryingObject = true;
            }
            
        }
        
    }

    //ToDo: Think about what to do when the player is already carrying an egg and enters another eggs collider and presses interact

}
