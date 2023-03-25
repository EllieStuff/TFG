using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    [SerializeField] float delay = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (delay <= 0) Destroy(gameObject);
        else Destroy(gameObject, delay);
    }

}
