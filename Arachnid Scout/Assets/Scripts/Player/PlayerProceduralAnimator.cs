using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerProceduralAnimator : MonoBehaviour
{
    public LayerMask GroundLayer; // The raycast should only affect the ground, since want to move IK target to next position

    [Header("IK Targets")]// The IK targets for each LIMB 
    public Transform IKTargetLeftLeg;
    public Transform IKTargetRightLeg;

    public Transform IKTargetLeftArm;
    public Transform IKTargetRightArm; 
    // private List<Transform> m_IKTargets = new List<Transform>(); // List of all the IK targets
    [Header("Raycast Origins")]// The origin of the raycast for each Limb
    public Transform RaycastOriginLL;
    public Transform RaycastOriginRL;   
    // public Transform RaycastOriginLA;
    // public Transform RaycastOriginRA; 
    // private List<Transform> m_RaycastOrigins = new List<Transform>(); // List of all the raycast origins

    [Header("Variables")] //IF CHANGING SCALE OF SPIDER HAVE TO TWEAK THIS VALUE
    public float RayCastMaxDistance = 5f; // The distance of the raycast for each leg from the origins

    public float LegMaxDistance = 0.1f; 
    private bool m_hasCouroutineStarted = false;
    public float raycastInterval = 0.2f; // The interval between each raycast
    private bool m_alternateLegFlag = false; // did this leg move last time, if so then move the other leg
    /* The minimum distance between the leg and the next raycast target point
     in which the leg can stay still grounded, before it has to move
    */
    private List<LegData> m_LegData = new List<LegData>(); // List of all the leg data

    [SerializeField]
    private Transform m_restingPositionLL;
    [SerializeField]
    private Transform m_restingPositionRL;

    [Header("Crouching Hips")]
    private GeneratedInputFile _playerInput;
    public GameObject hips;
    private InputAction _crouchAction;
    private bool crouching = false;
    private Vector3 crouchLegReadjustment = new Vector3(0, -0.5f, 0);

    void Awake() 
    {
        m_LegData.Add(new LegData(IKTargetLeftLeg, RaycastOriginLL, false));
        m_LegData.Add(new LegData(IKTargetRightLeg, RaycastOriginRL, false));
        _playerInput = new GeneratedInputFile();
        // m_LegData.Add(new LegData(IKTargetLeftArm, RaycastOriginLA, false));
        // m_LegData.Add(new LegData(IKTargetRightArm, RaycastOriginRA, false));
    }
    public bool IsCrouching(){
        return crouching;
    }   
    
    void OnEnable(){
        _crouchAction = _playerInput.Player.Crouch;
        _crouchAction.Enable();
        _crouchAction.performed += OnCrouchPerformed;
    }
    void OnDisable(){
        _crouchAction.Disable();
        _crouchAction.performed -= OnCrouchPerformed;
    }

     private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        if(!crouching){
            hips.transform.position = new Vector3(hips.transform.position.x, hips.transform.position.y - 0.5f, hips.transform.position.z);
            crouching = true;
            IKTargetLeftLeg.position -= crouchLegReadjustment;
            IKTargetRightLeg.position -= crouchLegReadjustment;
        }
        else{
            hips.transform.position = new Vector3(hips.transform.position.x, hips.transform.position.y + 0.5f, hips.transform.position.z);
            crouching = false;
            IKTargetLeftLeg.position += crouchLegReadjustment;
            IKTargetRightLeg.position += crouchLegReadjustment;
        }
        
    }
    void Start()
    {
        // m_restingPositionLL = IKTargetLeftLeg;
        // m_restingPositionRL = IKTargetRightLeg;
        // Place all the legs down at the targets at start of game.

        for (int i = 0; i < m_LegData.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_LegData[i].RaycastOrigin.position, Vector3.down, out hit, RayCastMaxDistance, GroundLayer))
            {
                m_LegData[i].IKTarget.position = hit.point;
            }
        }

        StartCoroutine(Raycast());
        
    }

    // Update is called once per frame
    void Update()
    {
        //Instead of firing rays every frame, we can fire every second to save performance
        // bool isMoving = Player.GetComponent<CharacterController>().velocity.magnitude > 0.3f;
        // if(!m_hasCouroutineStarted)
        // {
        //     StartCoroutine(Raycast());
        //     m_hasCouroutineStarted = true;
        // }
        // StartCoroutine(Raycast());
    }

    IEnumerator Raycast()
    {
        while (true)
        {
            
        for (int i = 0; i < m_LegData.Count; i++)
        {

            // --------------- WHEN THE PLAYER STOPS MOVING LEGS RETURN TO REST POSITION ------------------------
            if(gameObject.GetComponent<CharacterController>().velocity.magnitude < 0.3f)
            {
                // Debug.Log("speed is less than 0.3");
                if(i == 0){
                    // Debug.Log("GOING TO STAND STILL 0");
                    StartCoroutine(TweenLegPosition(m_LegData[1], m_restingPositionRL.position, 0.1f));
                    }
                else
                {
                    StartCoroutine(TweenLegPosition(m_LegData[0], m_restingPositionLL.position, 0.1f));
                    // Debug.Log("GOING TO STAND STILL 0");
                }
                continue;
                // SKIPS REST OF CODE --------------- WHEN THE PLAYER STOPS MOVING LEGS RETURN TO REST POSITION ------------------------
            }

            // --------------- PLAYER IS MOVING SO MOVE LEGS ------------------------
            RaycastHit hit;

            //adjust raycast position according to direction moving
            Transform raycastOrigin = m_LegData[i].RaycastOrigin;
            Vector3 forwardDirection = gameObject.GetComponent<CharacterController>().velocity.normalized;
            // Vector3 adjustedRaycastOrigin = raycastOrigin.position + forwardDirection;
            Vector3 adjustedRaycastOrigin = new Vector3(raycastOrigin.position.x + forwardDirection.x, raycastOrigin.position.y, raycastOrigin.position.z + forwardDirection.z);
            if (Physics.Raycast(adjustedRaycastOrigin, Vector3.down, out hit, RayCastMaxDistance, GroundLayer))
            {
                // Debug.Log(" previous position: " + m_LegData[i].IKTarget.position + " new position: " + hit.point);
                // Debug.Log("Distance: " + Vector3.Distance(m_LegData[i].IKTarget.position, hit.point));
                
                // If leg is already moving, skip. 
                // If leg is not moving and the distance between the leg and the next target point is greater than the max distance, move the leg.
                if(!m_LegData[i].IsLegMoving && Vector3.Distance(m_LegData[i].IKTarget.position, hit.point) > LegMaxDistance )
                {

                    //While moving one leg, the other leg should not move
                    if(i == 1 && m_alternateLegFlag){
                        if(!m_LegData[0].IsLegMoving){
                            // Debug.Log("Moving leg");
                            m_LegData[i].IsLegMoving = true;
                            StartCoroutine(TweenLegPosition(m_LegData[i], hit.point, 0.1f));
                            m_alternateLegFlag = false;
                        }
                    }

                    if(i == 0 && !m_alternateLegFlag){
                        if(!m_LegData[1].IsLegMoving){
                            // Debug.Log("Moving leg");
                            m_LegData[i].IsLegMoving = true;
                            StartCoroutine(TweenLegPosition(m_LegData[i], hit.point, 0.1f));
                            m_alternateLegFlag = true;
                        }
                    }
                    

                }else
                {
                    // Debug.Log("Leg is still");
                }
                
            }
        }
        yield return new WaitForSeconds(raycastInterval);
        }
    }

    // Following code taken and modified from chatgpt example
    IEnumerator TweenLegPosition(LegData legData, Vector3 endPosition, float duration)
    {
        Vector3 startPosition = legData.IKTarget.position;
        float elapsedTime = 0f;

        if (crouching){
            endPosition -= crouchLegReadjustment; // end pos always stop floor it will, so redjust we must
        }
        while (elapsedTime < duration)
        {
            legData.IKTarget.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the final position is set
        legData.IKTarget.position = endPosition;
        legData.IsLegMoving = false;
        if(gameObject.GetComponent<CharacterController>().velocity.magnitude > 0.3f){
            if(IsCrouching())
            {
                AudioManager.Instance.OnFootstep(legData.IKTarget.position, AudioManager.Instance.FootStepCrouchAudioVolume);
            }else{
                AudioManager.Instance.OnFootstep(legData.IKTarget.position, AudioManager.Instance.FootstepAudioVolume); //do only when character is moving
            }
            
        
        }
        

    }
    
    // TO DO: Add a method for animating crouching  - Legs can still move as normal but body should crouch
    // TO DO: Add a method for animating a dash - Legs should be animated differently from walking/crouching movement
    // TO DO: When the player is jumping, the legs shouldnt be moved with teh same logic as walking/crouching
    // TO DO: Animation for hands for carrying eggs.
    // TO DO: Animation for holding hands and moving with another character.
}
