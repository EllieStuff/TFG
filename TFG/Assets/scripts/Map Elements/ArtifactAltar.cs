using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactAltar : MonoBehaviour
{
    [SerializeField] ArtifactItem.ArtifactType altarType;
    [SerializeField] internal bool stonePlaced;

    private void Update()
    {
        if(stonePlaced)
        {

        }
    }
}
