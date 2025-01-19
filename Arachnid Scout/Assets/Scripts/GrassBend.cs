using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBend : MonoBehaviour
{
    public float bendSpeed = 5.0f;
    private bool _isPlayerNearby = false;
    private Transform _playerTransform;
    private Quaternion _originalRotation;
    // Start is called before the first frame update
    void Start()
    {
        _originalRotation = transform.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _isPlayerNearby = true;
            _playerTransform = other.transform;
            // other.GetComponent<InteractionManager>().IsHiding = true;
            // Debug.Log("player hidden");

            InteractionManager interactionManager = other.GetComponent<InteractionManager>();
            if(interactionManager!=null)
            {
                interactionManager.GrassHideCount++;
                interactionManager.UpdateHidingStatus();
                Debug.Log("player hidden");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _isPlayerNearby = false;
            _playerTransform = null;
            // other.GetComponent<InteractionManager>().IsHiding = false;
            // Debug.Log("player not hidden");

            InteractionManager interactionManager = other.GetComponent<InteractionManager>();
            if(interactionManager!=null)
            {
                interactionManager.GrassHideCount--;
                interactionManager.UpdateHidingStatus();
                Debug.Log("player not hidden");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(_isPlayerNearby&&_playerTransform!=null)
        {
            // Vector3 direction = _playerTransform.position - transform.position;
            // Quaternion lookRotation = Quaternion.LookRotation(direction);
            // Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * bendSpeed).eulerAngles;
            // transform.rotation = Quaternion.Euler(rotation.x, _originalRotation.y, rotation.z);

            // ----- i used CHATGPT for this section -------
            Vector3 direction = (transform.position - _playerTransform.position).normalized;//direction from player
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);//direction to a rotation
            lookRotation  = Quaternion.Euler(lookRotation.eulerAngles.x, _originalRotation.y, lookRotation.eulerAngles.z);//rotate in x and z
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * bendSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _originalRotation, Time.deltaTime * bendSpeed);
        }
    }
}
