using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class SpiderAI : MonoBehaviour
{
    private SpiderBaseState m_currentState;
    [HideInInspector]
    public SpiderPatrollingState PatrollingState = new SpiderPatrollingState();
    [HideInInspector]
    public SpiderChasingState ChasingState = new SpiderChasingState();
    [HideInInspector]
    public SpiderSearchingState SearchingState = new SpiderSearchingState();

    
    [Header("Navigation")]
    public NavMeshAgent Agent;
    public Transform[] Waypoints;
    // private Rigidbody m_rigidbody;

    [Header("Spider Stats")]
    public bool isMatron = false;

    [Header("Detection")]
    public float DetectionRadius = 50f;
    public float screechRadius = 50f;
    public float FieldOfViewAngle = 170f;
    public Transform PlayerTransform;
    public GameObject SpiderCanvas;
    private float spiderRadius = 1.5f;

    private ThirdPersonController m_playerController;
    // private GameManager gameManager;
    // Start is called before the first frame update
    // void Awake() 
    // {
    //     // m_rigidbody = GetComponent<Rigidbody>();
    // }
    private void Awake() {
        // gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_playerController = PlayerTransform.GetComponent<ThirdPersonController>();
    }
    void Start()
    {
        // PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_currentState = PatrollingState;
        m_currentState.EnterState(this);

        Agent.updatePosition = false;
        Agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        m_currentState.UpdateState(this);

        if(Agent.hasPath)
        {
            // m_rigidbody.MovePosition(Agent.nextPosition); //To move kinematic rb using navmesh
            transform.position = Agent.nextPosition;
            // Rotate towards the target
            Vector3 direction = Agent.steeringTarget - transform.position;
            if(direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
                //later on can add smooth rotation
            }
        }
    }

    public bool CanHearPlayer()
    {
        if(m_playerController.isRunning && Vector3.Distance(transform.position, PlayerTransform.position) <= DetectionRadius)
        {
            return true;
        }else return false;
    }
    public void CheckContactWithPlayer()
    {
        // IF SPIDER IS WITHIN CONTACT DISTANCE THEN GAME OVER
        if(Vector3.Distance(transform.position, PlayerTransform.position) < spiderRadius)
        {
            Debug.Log("player distance: " + Vector3.Distance(transform.position, PlayerTransform.position) + "... radius: " + spiderRadius);
            Debug.Log("Player has been caught");
            GameManager.Instance.PauseGame();
            GameManager.Instance.EnableReloadOnGameOver();
            
        }
        Debug.Log("player distance: " + Vector3.Distance(transform.position, PlayerTransform.position));
        // Debug.Log("Player has been caught");
        // GameManager.Instance.EnableReloadOnGameOver();
        // GameManager.Instance.PauseGame();

    }
    public void SwitchState(SpiderBaseState newState)
    {
        m_currentState = newState;
        m_currentState.EnterState(this);
    }

    public bool CanSeePlayer()
    {
        
        // check if player is within range for spiders max distance to see
        if(Vector3.Distance(transform.position, PlayerTransform.position) > DetectionRadius)
        {
            // If the player is not even in range of the detection radius then return false
            // Debug.Log(Vector3.Distance(transform.position, PlayerTransform.position));
            // Debug.Log(DetectionRadius);
            return false;
        }
        // Debug.Log("Player is within range");
        //If player is within range then do rest of calculations to see if player is in field of view
        Vector3 directionToPlayer = PlayerTransform.position - transform.position;
        float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward); // The angle between spiders forward direction and player direction

        // Debug.Log("Angle to player: " + angleToPlayer);
        if(angleToPlayer < FieldOfViewAngle * 0.5f) // Spiders forward direction is halfway between the field of view angle
        {
            // Debug.Log("Player is within field of view");
            RaycastHit hit;
            if(Physics.Raycast(transform.position + new Vector3(0,1,0), directionToPlayer, out hit, DetectionRadius))
            {
                //draw raycast gizmo
                Debug.DrawRay(transform.position, directionToPlayer, Color.red);
                if(hit.collider.CompareTag("Player"))
                {
                    // Debug.Log("Player is in sight");
                    return true;
                }
            }
            
        }
        

        return false;
    }
    
    // Copilot recommendation for OnDrawGizmos
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);

        Vector3 leftBoundary = Quaternion.AngleAxis(FieldOfViewAngle * 0.5f, transform.up) * transform.forward;
        Vector3 rightBoundary = Quaternion.AngleAxis(-FieldOfViewAngle * 0.5f, transform.up) * transform.forward;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, leftBoundary * DetectionRadius);
        Gizmos.DrawRay(transform.position, rightBoundary * DetectionRadius);
    }

    // This coroutine is called when the spider sees the player while patrolling. 
    // The spider will stop, turn around, look at the player for half a second and then start chasing the player.
    // During that time the spider will screech and if within a certain radius of its eggs, the eggs will hatch.

    public IEnumerator ScreechToEggs()
    {
        // Play screech sound
        SpiderCanvas.SetActive(true);
        //check within screech radius for game objects with egg tag

        // Only the big spiders screeches can hatch an egg
        if(isMatron)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, screechRadius);
            foreach(Collider hitCollider in hitColliders)
            {
                if(hitCollider.CompareTag("Egg"))
                {
                    hitCollider.GetComponent<EggScript>().HatchEgg();
                }
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        SpiderCanvas.SetActive(false);
    }
    

}
