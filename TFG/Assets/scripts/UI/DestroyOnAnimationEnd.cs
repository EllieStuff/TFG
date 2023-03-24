using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAnimationEnd: MonoBehaviour
{
    public void DestroyParent()
    {
        GameObject parent = gameObject.transform.parent.parent.gameObject;
        Destroy(parent);
    }
}
