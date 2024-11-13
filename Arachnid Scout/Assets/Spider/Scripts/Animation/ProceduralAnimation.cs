using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimation : MonoBehaviour
{
    public LayerMask GroundLayer; // The raycast should only affect the ground, since want to move IK target to next position

    [Header("IK Targets")]// The IK targets for each leg (Left 1, Left 2, Right 1, Right 2)
    public Transform IKTargetL1;
    public Transform IKTargetL2;
    public Transform IKTargetR1;
    public Transform IKTargetR2; 
    // private List<Transform> m_IKTargets = new List<Transform>(); // List of all the IK targets
    [Header("Raycast Origins")]// The origin of the raycast for each leg
    public Transform RaycastOriginL1;
    public Transform RaycastOriginL2;
    public Transform RaycastOriginR1;
    public Transform RaycastOriginR2; 
    // private List<Transform> m_RaycastOrigins = new List<Transform>(); // List of all the raycast origins

    [Header("Variables")] //IF CHANGING SCALE OF SPIDER HAVE TO TWEAK THIS VALUE
    public float RayCastMaxDistance = 5f; // The distance of the raycast for each leg from the origins

    public float LegMaxDistance = 0.1f; 
    private bool m_hasCouroutineStarted = false;
    public float raycastInterval = 0.2f; // The interval between each raycast
    /* The minimum distance between the leg and the next raycast target point
     in which the leg can stay still grounded, before it has to move
    */
    private List<LegData> m_LegData = new List<LegData>(); // List of all the leg data
    void Awake() 
    {
        // m_IKTargets.Add(IKTargetL1);
        // m_IKTargets.Add(IKTargetL2);
        // m_IKTargets.Add(IKTargetR1);
        // m_IKTargets.Add(IKTargetR2);

        // m_RaycastOrigins.Add(RaycastOriginL1);
        // m_RaycastOrigins.Add(RaycastOriginL2);
        // m_RaycastOrigins.Add(RaycastOriginR1);
        // m_RaycastOrigins.Add(RaycastOriginR2);
        m_LegData.Add(new LegData(IKTargetL1, RaycastOriginL1, false));
        m_LegData.Add(new LegData(IKTargetL2, RaycastOriginL2, false));
        m_LegData.Add(new LegData(IKTargetR1, RaycastOriginR1, false));
        m_LegData.Add(new LegData(IKTargetR2, RaycastOriginR2, false));
    }
    
    void Start()
    {
        // Place all the legs down at the targets at start of game.

        for (int i = 0; i < m_LegData.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_LegData[i].RaycastOrigin.position, Vector3.down, out hit, RayCastMaxDistance, GroundLayer))
            {
                m_LegData[i].IKTarget.position = hit.point;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Instead of firing rays every frame, we can fire every second to save performance
        if(!m_hasCouroutineStarted)
        {
            StartCoroutine(Raycast());
            m_hasCouroutineStarted = true;
        }
        // StartCoroutine(Raycast());
    }

    IEnumerator Raycast()
    {
        while (true)
        {
            
        
        Debug.Log("raycastesd");
        for (int i = 0; i < m_LegData.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_LegData[i].RaycastOrigin.position, Vector3.down, out hit, RayCastMaxDistance, GroundLayer))
            {
                // Debug.Log(" previous position: " + m_LegData[i].IKTarget.position + " new position: " + hit.point);
                // Debug.Log("Distance: " + Vector3.Distance(m_LegData[i].IKTarget.position, hit.point));
                
                // If leg is already moving, skip. 
                // If leg is not moving and the distance between the leg and the next target point is greater than the max distance, move the leg.
                if(!m_LegData[i].IsLegMoving && Vector3.Distance(m_LegData[i].IKTarget.position, hit.point) > LegMaxDistance )
                {
                    
                    // Debug.Log("Moving leg");
                    // m_LegData[i].IKTarget.position = hit.point;
                    // Tween the IK target to the new position
                    // m_LegData[i].IKTarget.position = Vector3.Lerp(m_LegData[i].IKTarget.position, hit.point, 0.1f);
                    // m_LegData[i].IsLegMoving = true;
                    // StartCoroutine(TweenPosition(m_LegData[i], hit.point, 0.1f));

                    if(i == 1 || i == 2){
                        if(!m_LegData[0].IsLegMoving && !m_LegData[3].IsLegMoving){
                            // Debug.Log("Moving leg");
                            m_LegData[i].IsLegMoving = true;
                            StartCoroutine(TweenPosition(m_LegData[i], hit.point, 0.1f));
                        }
                    }

                    if(i == 0 || i == 3){
                        if(!m_LegData[1].IsLegMoving && !m_LegData[2].IsLegMoving){
                            // Debug.Log("Moving leg");
                            m_LegData[i].IsLegMoving = true;
                            StartCoroutine(TweenPosition(m_LegData[i], hit.point, 0.1f));
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
    IEnumerator TweenPosition(LegData legData, Vector3 endPosition, float duration)
    {
        Vector3 startPosition = legData.IKTarget.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            legData.IKTarget.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the final position is set
        legData.IKTarget.position = endPosition;
        legData.IsLegMoving = false;
    }
    
}
