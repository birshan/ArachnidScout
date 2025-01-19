using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHatchlings : MonoBehaviour
{

    public GameObject HatchlingSpider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnHatchling(Vector3 position, Quaternion rotation)
    {
        GameObject newHatchling = Instantiate(HatchlingSpider, position, rotation);
        AudioManager.Instance.PlaySoundAtPosition(AudioManager.Instance.HatchEgg, position);
        
    }
}
