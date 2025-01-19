using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCaptain : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Talk");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // IEnumerator Talk()
    // {
    //     yield return new WaitForSeconds(1f);
    //     animator.SetTrigger("Talk");

    // }
}
