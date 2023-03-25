using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WaterMask : MonoBehaviour
{
    WaterMaskManager manager;

    private void Start()
    {
        manager = GetComponentInParent<WaterMaskManager>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("floor"))
        {
            manager.AddObjectMask(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("floor"))
        {
            manager.RemoveObjectMask(other.transform);
        }
    }

}
