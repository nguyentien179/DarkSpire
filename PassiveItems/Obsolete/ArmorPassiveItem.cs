using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        float oldMaxHealth = player.CurrentMaxHealth;
        // Increase max health
        player.CurrentMaxHealth *= 1 + passiveItemData.Multiplier / 100f;

        // Calculate the amount of health added
        float healthAdded = player.CurrentMaxHealth - oldMaxHealth;

        // Heal the player by the amount added to max health
        player.CurrentHealth += healthAdded;

        // Ensure current health does not exceed max health
        if (player.CurrentHealth > player.CurrentMaxHealth)
        {
            player.CurrentHealth = player.CurrentMaxHealth;
        }
    }
}
