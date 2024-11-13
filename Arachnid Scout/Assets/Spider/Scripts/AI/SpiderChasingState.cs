using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderChasingState : SpiderBaseState
{
    private float m_timer = 0.5f;
    private bool m_isPlayerSeen;
    public override void EnterState(SpiderAI spider)
    {
        Debug.Log("Entering Chasing State");
        spider.Agent.ResetPath(); // Clear patrolling paths if any and start chasing
        spider.Agent.SetDestination(spider.PlayerTransform.position); // chase player
        spider.StartCoroutine(CallCanSeePlayer(spider));

    }

    public override void UpdateState(SpiderAI spider)
    {
        // WHile chasing player, if contact with player, end game
        spider.CheckContactWithPlayer();
        // while chasing the player, screech to eggs in radius around you.
        
        // While chasing, if spider can still see player, update the destination
        
        if(m_isPlayerSeen)
        {
            spider.Agent.SetDestination(spider.PlayerTransform.position);
        }
        else
        {
            // If player is not seen, switch to searching state
            spider.StopCoroutine(CallCanSeePlayer(spider));
            Debug.Log("SWITCH FROM CHASING TO PATROLLING");
            spider.SwitchState(spider.PatrollingState);              // ITS PATROLLING STATE FOR NOW -  Change to Searching State
        }
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
                // yield break; //exit coroutine
            }
            else{
                m_isPlayerSeen = false;
            }
            // to limit amount of raycasts done
            yield return new WaitForSeconds(0.5f);
        }
        
    }
    
}

