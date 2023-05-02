using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }
    [field: SerializeField] public EventReference playerAttack { get; private set; }

    [field: Header("UI")]
    //[field: SerializeField] public EventReference uiButton { get; private set; }
    //[field: SerializeField] public EventReference uiSelectHability { get; private set; }
    //[field: SerializeField] public EventReference gameOver { get; private set; }

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }
}
