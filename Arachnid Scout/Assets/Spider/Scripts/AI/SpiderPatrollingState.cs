using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SpiderPatrollingState : SpiderBaseState
{
    private Transform[] m_waypoints;
    private bool m_isPlayerSeen;
    private bool m_hasCouroutineStarted = false;
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
        if(spider.CanHearPlayer())
        {
            Debug.Log("HEARD PLAYER -> Searching State");
            // spider.SwitchState(spider.ChasingState);
            spider.StartCoroutine(TurnAroundLookAtPlayerAndSwitchState(spider));
            m_hasCouroutineStarted = true;
        }
        if(!spider.Agent.pathPending && spider.Agent.remainingDistance < 0.5f)
        {
            // Move to next waypoint
            MoveToNextWaypoint(spider);
        }
        

        if(m_isPlayerSeen && !m_hasCouroutineStarted)
        {
            // Debug.Log("SAW PLAYER -> Chase State");
            spider.StopCoroutine(CallCanSeePlayer(spider));

            // Stop. Turn around. Look at Player for half a second... Start chasing.
            // spider.SwitchState(spider.ChasingState);
            spider.StartCoroutine(TurnAroundLookAtPlayerAndSwitchState(spider));
            m_hasCouroutineStarted = true;
        }

        // If Spider can hear player then switch to searching state
    }

    private void MoveToNextWaypoint(SpiderAI spider)
    {
        if(m_waypoints.Length == 0)
        {
            return;
        }
        // spider.Agent.destination = m_waypoints[Random.Range(0, m_waypoints.Length)].position;
        spider.Agent.SetDestination(m_waypoints[Random.Range(0, m_waypoints.Length)].position);
    }

    private IEnumerator CallCanSeePlayer(SpiderAI spider)
    {
        while(true)
        {
            if(spider.CanSeePlayer())
            {
                // Debug.Log("SAW PLAYER -> Chase State");
                // spider.SwitchState(spider.ChasingState);
                m_isPlayerSeen = true;
                yield break; //exit coroutine
            }
            // to limit amount of raycasts done
            yield return new WaitForSeconds(0.5f);
        }
        
    }

    private IEnumerator TurnAroundLookAtPlayerAndSwitchState(SpiderAI spider)
    {
        // Stop Spider
        spider.Agent.isStopped = true;
        // Turn around
        Vector3 direction = spider.PlayerTransform.position - spider.transform.position;
        spider.transform.rotation = Quaternion.LookRotation(direction);

        // screech 
        
        // Look at player for half a second
        yield return new WaitForSeconds(0.5f);
        // Start chasing
        spider.StartCoroutine(spider.ScreechToEggs());
        spider.SwitchState(spider.ChasingState);
    }

}

