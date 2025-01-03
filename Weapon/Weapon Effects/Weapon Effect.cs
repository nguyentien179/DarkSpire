using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponEffect : MonoBehaviour
{
    [HideInInspector] public CharacterStat owner;
    [HideInInspector] public Weapon weapon;

    public float GetDamage()
    {
        return weapon.GetDamage();
    }
}
