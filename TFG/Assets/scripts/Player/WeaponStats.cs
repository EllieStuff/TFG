using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [SerializeField] internal float weaponDamage;
    [SerializeField] internal HealthState.Effect weaponEffect = HealthState.Effect.NORMAL;

    internal HealthState healthStateEffect;

    private void Start()
    {
        healthStateEffect = HealthState.GetHealthStateByEffect(weaponEffect);
    }
}
