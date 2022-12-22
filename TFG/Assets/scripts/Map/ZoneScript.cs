using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneScript : MonoBehaviour
{
    bool zoneEnabled;
    bool zoneDefused;
    [SerializeField] internal int enemiesQuantity;
    [SerializeField] float closeDoorAnimDestroyTime;
    [SerializeField] Animation doorCloseAnim;
    [SerializeField] Animation doorOpenAnim;

    private void Start()
    {
        doorCloseAnim.gameObject.SetActive(false);
    }

    void Update()
    {
        if(zoneEnabled && !zoneDefused && enemiesQuantity <= 0)
        {
            if(!doorOpenAnim.isPlaying)
                doorOpenAnim.Play();

            closeDoorAnimDestroyTime -= Time.deltaTime;
            if(closeDoorAnimDestroyTime <= 0)
            {
                zoneDefused = true;
                Destroy(doorOpenAnim.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player") && !zoneEnabled)
        {
            zoneEnabled = true;
            doorCloseAnim.gameObject.SetActive(true);
            doorCloseAnim.Play();
        }
    }
}
