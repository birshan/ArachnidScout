using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SpiderPatrollingState : SpiderBaseState
{
    private Transform[] m_waypoints;
    private bool m_isPlayerSeen;
    private bool m_hasCouroutineStarted = false;

    //Awarness variables 
    private float m_awarenessIncreaseRate = 0.3f;//Awareness increase per second
    private float m_suspicionThreshold = 0.5f;//When this threshold is reached the spider will tirn around and look at he player
    private float m_currentAwareness = 0f;
    private bool m_playedOnce = false;
    private bool m_hasFinishedTurningAroundAndLookingSpooky = false;
    public bool isTurningAroundToPlayer {get; private set;} = false;
    public override void EnterState(SpiderAI spider)
    {
        m_hasCouroutineStarted = false;
        m_isPlayerSeen = false;
        Debug.Log("Entering Patrolling State");
        m_waypoints = spider.Waypoints;

        // Check periodically if player is seen
        spider.StartCoroutine(CallCanSeePlayer(spider));
    }

    public override void UpdateState(SpiderAI spider)
    {
        if(spider.isMatron){
            Debug.Log("current awareness: "+m_currentAwareness);
        }
        if(!m_isPlayerSeen){
            m_currentAwareness -= (m_awarenessIncreaseRate-0.25f)*Time.deltaTime;
            if(m_currentAwareness <= 0.2f)
            {
                m_currentAwareness = 0;
            }
            
        }
        spider.UpdateAwarenessMeter(m_currentAwareness);
        if(spider.gameObject.CompareTag("Hatchling")){
            spider.StopCoroutine(CallCanSeePlayer(spider));
            spider.SwitchState(spider.ChasingState);
        }
        if(spider.CanHearPlayer())
        {   
            // Gradually increase awareness over time
            m_currentAwareness += m_awarenessIncreaseRate * Time.deltaTime;
            //clamp awareness between 0 and 1
            m_currentAwareness = Mathf.Clamp01(m_currentAwareness);
            //Update UI Image
            spider.UpdateAwarenessMeter(m_currentAwareness);
            //check if awareness has reachedd threshold
            if(m_currentAwareness >= m_suspicionThreshold && !m_hasCouroutineStarted)
            {
                // Debug.Log("HEARD PLAYER -> Searching State");
                // spider.SwitchState(spider.ChasingState);
                spider.StartCoroutine(TurnAroundLookAtPlayerAndSwitchState(spider));
                m_hasCouroutineStarted = true;
            }
            
            // Debug.Log("HEARD PLAYER -> Searching State");
            // // spider.SwitchState(spider.ChasingState);
            // spider.StartCoroutine(TurnAroundLookAtPlayerAndSwitchState(spider));
            // m_hasCouroutineStarted = true;
        }
        if(!isTurningAroundToPlayer && !spider.Agent.pathPending && spider.Agent.remainingDistance < 0.5f )
        {
            // Move to next waypoint
            MoveToNextWaypoint(spider);
            
        }
        

        if(m_isPlayerSeen && !m_hasCouroutineStarted)
        {

            // Gradually increase awareness over time
            m_currentAwareness += (m_awarenessIncreaseRate) * Time.deltaTime;
            //clamp awareness between 0 and 1
            m_currentAwareness = Mathf.Clamp01(m_currentAwareness);
            //Update UI Image
            spider.UpdateAwarenessMeter(m_currentAwareness);
            //check if awareness has reachedd threshold
            if(m_currentAwareness >= m_suspicionThreshold && !m_hasCouroutineStarted)
            {
                // Debug.Log("SAW PLAYER -> Chase State");
                spider.StopCoroutine(CallCanSeePlayer(spider));

                // Stop. Turn around. Look at Player for half a second... Start chasing.
                // spider.SwitchState(spider.ChasingState);
                spider.StartCoroutine(TurnAroundLookAtPlayerAndSwitchState(spider));
                m_hasCouroutineStarted = true;
            }


            // // Debug.Log("SAW PLAYER -> Chase State");
            // spider.StopCoroutine(CallCanSeePlayer(spider));

            // // Stop. Turn around. Look at Player for half a second... Start chasing.
            // // spider.SwitchState(spider.ChasingState);
            // spider.StartCoroutine(TurnAroundLookAtPlayerAndSwitchState(spider));
            // m_hasCouroutineStarted = true;



        }
        // if(m_isPlayerSeen && m_hasCouroutineStarted && !m_hasFinishedTurningAroundAndLookingSpooky){
        //     //turn around to  look at player
        //     Vector3 direction = spider.PlayerTransform.position - spider.transform.position;
        //     spider.transform.rotation = Quaternion.LookRotation(direction);
        // }
        // If Spider can hear player then switch to searching state
    }

    private void MoveToNextWaypoint(SpiderAI spider)
    {
        if(isTurningAroundToPlayer|| m_waypoints.Length == 0)
        {
            return;
        }
        // spider.Agent.destination = m_waypoints[Random.Range(0, m_waypoints.Length)].position;
        spider.Agent.SetDestination(m_waypoints[Random.Range(0, m_waypoints.Length)].position);
    }

    // To check every 0.5 seconds if player is seen, if seen it breaks out of the coroutine
    private IEnumerator CallCanSeePlayer(SpiderAI spider)
    {
        while(true)
        {
            if(spider.CanSeePlayer())
            {
                // Debug.Log("SAW PLAYER -> Chase State");
                // spider.SwitchState(spider.ChasingState);
                m_isPlayerSeen = true;
                // yield break; //exit coroutine
            }else{
                m_isPlayerSeen = false;
            }
            // to limit amount of raycasts done
            yield return new WaitForSeconds(0.5f);
        }
        
    }

    private IEnumerator TurnAroundLookAtPlayerAndSwitchState(SpiderAI spider)
    {
        Debug.Log("Detected Player - Menacingly Turning Around Coroutine");
        isTurningAroundToPlayer = true;
        // Stop Spider
        spider.Agent.isStopped = true;
        // Turn around
        // Vector3 direction = spider.PlayerTransform.position - spider.transform.position;
        // Quaternion lookRotation = Quaternion.LookRotation(direction);

        // float rotationSpeed = 2f;
        // spider.Agent.enabled = false;
        // while(Quaternion.Angle(spider.transform.rotation, lookRotation) > 0.1f)
        // {   
        //     spider.transform.rotation = Quaternion.Slerp(spider.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        //     yield return null;
        // }
        // spider.Agent.enabled = true;

        // spider.Agent.updateRotation = true;
        // spider.Agent.SetDestination(spider.PlayerTransform.position);
        // while(Quaternion.Angle(spider.transform.rotation, Quaternion.LookRotation(spider.PlayerTransform.position - spider.transform.position)) > 1f)
        // {
        //      yield return null;
        // }
        // spider.Agent.ResetPath();

        Vector3 direction = spider.PlayerTransform.position - spider.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float rotationSpeed = 2f;
        spider.Agent.enabled = false;
        while(Quaternion.Angle(spider.transform.rotation, lookRotation) > 1f)
        {   
            Debug.Log("Turning Around While Loop");
            spider.transform.rotation = Quaternion.Slerp(spider.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        spider.Agent.enabled = true;
        Debug.Log("Turning Around Done");
        
        // Play spooky music once spider is done turning around and is looking menacingly at player
        if(!AudioManager.Instance.playedOnce)
        {
            AudioManager.Instance.PlayImmediate(AudioManager.Instance.BeingDetected, 0.3f);
            AudioManager.Instance.playedOnce = true;
        }
        spider.UpdateAwarenessMeter(1f);
        spider.ChangeAwarenessMeterColor(Color.red);
        // Look at player for 3 seconds
        yield return new WaitForSeconds(3f);

        m_hasFinishedTurningAroundAndLookingSpooky = true;
        
        //  // screech 
        spider.StartCoroutine(spider.ScreechToEggs());
        AudioManager.Instance.PlaySoundAtPosition(AudioManager.Instance.SpiderScreech, spider.transform.position);
        
        // STart chasing player
        isTurningAroundToPlayer = false;
        spider.SwitchState(spider.ChasingState);

    }

}

