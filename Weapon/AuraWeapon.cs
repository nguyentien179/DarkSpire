using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraWeapon : Weapon
{
    protected Aura currentAura;

    protected override void Start()
    {
        // Initialize the aura when the weapon is equipped
        InitializeAura();
    }

    private void InitializeAura()
    {
        if (currentStats.auraPrefab == null) return;

        // Destroy any existing aura before instantiating a new one
        if (currentAura)
        {
            Destroy(currentAura.gameObject);
        }

        // Instantiate the aura prefab and attach it to the weapon
        currentAura = Instantiate(currentStats.auraPrefab, transform); // Instantiate the prefab
        currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area); // Scale it
        currentAura.transform.localPosition = Vector3.zero; // Ensure it's centered on the player

        currentAura.weapon = this; // Assign the weapon reference
        currentAura.owner = owner; // Set the owner

        // Add a CircleCollider2D for detecting enemy collisions
        CircleCollider2D auraCollider = currentAura.gameObject.AddComponent<CircleCollider2D>();
        auraCollider.isTrigger = true;  // Make sure it's a trigger collider
        auraCollider.radius = currentStats.area;  // Match the collider to the area stat
    }

    public override void OnEquip()
    {
        if (currentStats.auraPrefab)
        {
            if (currentAura) Destroy(currentAura);
            currentAura = Instantiate(currentStats.auraPrefab, transform);
            currentAura.weapon = this;
            currentAura.owner = owner;
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
    }

    public override void OnUnequip()
    {
        // Destroy the aura when the weapon is unequipped
        if (currentAura)
        {
            Destroy(currentAura.gameObject);
            currentAura = null; // Clear reference after destruction
        }
    }

    public override bool DoLevelUp()
    {
        if (!base.DoLevelUp()) return false;

        // Adjust the aura's scale when the weapon levels up
        if (currentAura)
        {
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }

        return true;
    }
}
