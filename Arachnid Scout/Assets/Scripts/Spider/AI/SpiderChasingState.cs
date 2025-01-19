using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderChasingState : SpiderBaseState
{
    private float m_timer = 7f;
    private bool m_isPlayerSeen;
    private bool m_isHatchling; // If hatchling then i want them to only stay active for a brief period, cuz they are too annoying
    private float m_awarenessIncreaseRate = 0.3f;//Awareness increase per second
    private float m_suspicionThreshold = 0.5f;//When this threshold is reached the spider will tirn around and look at he player
    private float m_currentAwareness = 1f;

    private bool m_playedOnce = false;
    public override void EnterState(SpiderAI spider)
    {
        Debug.Log("Entering Chasing State");

        m_isHatchling = spider.gameObject.CompareTag("Hatchling");
        spider.Agent.ResetPath(); // Clear patrolling paths if any and start chasing
        spider.Agent.SetDestination(spider.PlayerTransform.position); // chase player
        spider.StopAllCoroutines();
        spider.StartCoroutine(CallCanSeePlayer(spider));
        m_timer = Random.Range(5f, 10f);
        spider.PlayerTransform.GetComponent<InteractionManager>().SetIsPlayerBeingChased(true);

        // if(!m_playedOnce)
        // {
        //     AudioManager.Instance.TurnOffAudioSourceLoop();
        //     AudioManager.Instance.PlayImmediate(AudioManager.Instance.BeingDetected, 0.3f);
        //     m_playedOnce = true;
        // }
        
    }

    public override void UpdateState(SpiderAI spider)
    {
        // WHile chasing player, if contact with player, end game
        spider.CheckContactWithPlayer();

        // If this is a hatchling, decrement the timer
        if (m_isHatchling)
        {
            m_timer -= Time.deltaTime;

            if (m_timer <= 0)
            {
                Debug.Log("Hatchling timer expired. Exploding and dying.");
                ExplodeAndDie(spider);
                return; // Exit the method to avoid further updates
            }
        }
        // while chasing the player, screech to eggs in radius around you.
        
        // While chasing, if spider can still see player, update the destination
        
        if(m_isPlayerSeen)
        {
            if(!spider.gameObject.CompareTag("Hatchling")){
                spider.ChangeAwarenessMeterColor(Color.red);
                m_currentAwareness = 1f;
                spider.UpdateAwarenessMeter(1f);
            }
            
            spider.Agent.SetDestination(spider.PlayerTransform.position);
        }
        else
        {
            if(!spider.gameObject.CompareTag("Hatchling")){
                spider.ChangeAwarenessMeterColor(Color.yellow);
                m_currentAwareness -= m_awarenessIncreaseRate*Time.deltaTime;
                m_currentAwareness = Mathf.Clamp01(m_currentAwareness);
                spider.UpdateAwarenessMeter(m_currentAwareness);
                if(m_currentAwareness <= m_suspicionThreshold)
                {
                    // If player is not seen, switch to searching state
                    spider.StopCoroutine(CallCanSeePlayer(spider));
                    Debug.Log("SWITCH FROM CHASING TO PATROLLING");
                    spider.PlayerTransform.GetComponent<InteractionManager>().SetIsPlayerBeingChased(false);
                    // AudioManager.Instance.TurnOnAudioSourceLoop();
                    // AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.NightAmbience, 0.5f);
                    spider.StartCoroutine(spider.IsThePlayerStillBeingChasedIfNotSwitchMusic());
                    spider.SwitchState(spider.PatrollingState);                // ITS PATROLLING STATE FOR NOW -  Change to Searching State
                    
                }
            }
            
            // // If player is not seen, switch to searching state
            // spider.StopCoroutine(CallCanSeePlayer(spider));
            // Debug.Log("SWITCH FROM CHASING TO PATROLLING");
            // spider.SwitchState(spider.PatrollingState);              // ITS PATROLLING STATE FOR NOW -  Change to Searching State
        }
    }
    
    private void ExplodeAndDie(SpiderAI spider)
    {
        // Explode and die
        spider.gameObject.SetActive(false);
        AudioManager.Instance.PlaySoundAtPosition(AudioManager.Instance.HatchlingExplosiveDeath, spider.transform.position);
        //ToDO: instantiate vfx!
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

