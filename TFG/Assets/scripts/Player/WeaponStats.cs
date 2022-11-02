using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    internal enum WeaponType { COMMON, FIRE }
    [SerializeField] internal float weaponDamage;
    [SerializeField] internal WeaponType weaponType;
}
