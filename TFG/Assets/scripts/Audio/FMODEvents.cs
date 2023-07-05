using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    //[field: Header("Abilities SFX")]
    //[field: SerializeField] public EventReference criticonSound { get; private set; }
    //[field: SerializeField] public EventReference curaSound { get; private set; }

    [field: Header("Ambients SFX")]
    [field: SerializeField] public EventReference ambientSound { get; private set; }

    [field: Header("Enemies SFX")]
    [field: SerializeField] public EventReference bossAttackSound { get; private set; }
    [field: SerializeField] public EventReference bossChangeElementSound { get; private set; }
    [field: SerializeField] public EventReference bossDeathSound { get; private set; }
    [field: SerializeField] public EventReference batAttack { get; private set; }
    [field: SerializeField] public EventReference plantAttack { get; private set; }
    [field: SerializeField] public EventReference ratAttack { get; private set; }
    [field: SerializeField] public EventReference enemyDeath { get; private set; }

    [field: Header("Music")]
    //[field: SerializeField] public EventReference gameplayMusic { get; private set; }
    //[field: SerializeField] public EventReference menuMusic { get; private set; }
    [field: SerializeField] public EventReference allMusic { get; private set; }
    [field: SerializeField] public EventReference bossMusic { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }
    [field: SerializeField] public EventReference playerAttackSound { get; private set; }
    [field: SerializeField] public EventReference playerDamageSound { get; private set; }

    [field: Header("Props SFX")]
    [field: SerializeField] public EventReference doorOpen { get; private set; }
    [field: SerializeField] public EventReference doorClose { get; private set; }

    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference uiButtonClick { get; private set; }
    [field: SerializeField] public EventReference uiButtonSelect { get; private set; }
    [field: SerializeField] public EventReference uiSelectAbility { get; private set; }
    [field: SerializeField] public EventReference uiChangeElement { get; private set; }
    [field: SerializeField] public EventReference uiBuyItem { get; private set; }
    [field: SerializeField] public EventReference uiGetCoin { get; private set; }

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
