using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "2D RougeLike/Character Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }
    [SerializeField]
    string characterName;
    public string CharacterName { get => characterName; private set => characterName = value; }
    [SerializeField]
    WeaponData startingWeapon;
    public WeaponData StartingWeapon { get => startingWeapon; private set => startingWeapon = value; }
    [System.Serializable]
    public struct Stats
    {
        public float maxHealth, recovery, moveSpeed;
        public float might, speed, pickupRange;

        public Stats(float maxHealth = 100, float recovery = 0, float moveSpeed = 1f, float might = 1f, float speed = 1f, float pickupRange = 30f)
        {
            this.maxHealth = maxHealth;
            this.recovery = recovery;
            this.moveSpeed = moveSpeed;
            this.might = might;
            this.speed = speed;
            this.pickupRange = pickupRange;
        }

        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = s1;
            result.maxHealth = s1.maxHealth + s2.maxHealth;
            result.recovery = s1.recovery + s2.recovery;
            result.moveSpeed = s1.moveSpeed + s2.moveSpeed;
            result.might = s1.might + s2.might;
            result.speed = s1.speed + s2.speed;
            result.pickupRange = s1.pickupRange + s2.pickupRange;
            return result;
        }
    }

    public Stats stats = new Stats(100);
}
