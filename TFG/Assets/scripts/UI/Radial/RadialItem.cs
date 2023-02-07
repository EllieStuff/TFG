using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialItem : MonoBehaviour
{
    private RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        transform.LookAt(transform.position + new Vector3(0, -5, 0));
        Vector3 actualLocalRot = rect.localRotation.eulerAngles;
        rect.localRotation = Quaternion.Euler(0, 0, actualLocalRot.z);
    }
}
