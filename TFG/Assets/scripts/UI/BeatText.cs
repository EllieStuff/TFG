using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BeatText : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Animator>().enabled = false;
        StartCoroutine(StartBeating());
    }

    IEnumerator StartBeating()
    {
        
        yield return new WaitForSeconds(2.5f);
        this.gameObject.GetComponent<Animator>().enabled = true;
    }

}
