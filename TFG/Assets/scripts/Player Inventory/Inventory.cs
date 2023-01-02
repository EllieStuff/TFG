using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] internal bool waterGem;
    [SerializeField] internal bool fireGem;
    [SerializeField] internal bool magicGem;

    private void Start()
    {
        waterGem = false;
        fireGem = false;
        magicGem = false;
    }
}
