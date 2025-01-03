using System.Collections.Generic;
using UnityEngine;

public class Aura : WeaponEffect
{
    private Dictionary<EnemyStat, float> affectedTargets = new Dictionary<EnemyStat, float>();
    private HashSet<EnemyStat> targetsToUneffect = new HashSet<EnemyStat>(); // Using HashSet for faster lookup

    // Update is called once per frame
    void Update()
    {
        UpdateAffectedTargets();
    }

    // Update the status of affected targets
    private void UpdateAffectedTargets()
    {
        List<EnemyStat> targetsToRemove = new List<EnemyStat>(); // To track targets to remove after the loop

        foreach (var pair in new Dictionary<EnemyStat, float>(affectedTargets)) // Copy to avoid modifying while iterating
        {
            affectedTargets[pair.Key] -= Time.deltaTime; // Decrease time remaining

            if (affectedTargets[pair.Key] <= 0)
            {
                // Check if the target should be uneffected
                if (targetsToUneffect.Contains(pair.Key))
                {
                    targetsToRemove.Add(pair.Key);
                }
                else
                {
                    DealDamage(pair.Key); // Deal damage to the enemy
                }
            }
        }

        // Remove targets that should no longer be affected
        foreach (var target in targetsToRemove)
        {
            affectedTargets.Remove(target);
            targetsToUneffect.Remove(target);
        }
    }

    // Deal damage to the target and reset cooldown
    private void DealDamage(EnemyStat enemy)
    {
        var stats = weapon.GetStat();
        affectedTargets[enemy] = stats.cooldown; // Reset cooldown
        enemy.TakeDamage(GetDamage()); // Deal damage to the enemy
    }

    // Called when an enemy enters the aura
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyStat enemyStat) && !affectedTargets.ContainsKey(enemyStat))
        {
            affectedTargets.Add(enemyStat, 0); // Add new target with initial time
            targetsToUneffect.Remove(enemyStat); // Ensure it's not in the uneffect list
        }
    }

    // Called when an enemy exits the aura
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyStat enemyStat) && affectedTargets.ContainsKey(enemyStat))
        {
            targetsToUneffect.Add(enemyStat); // Mark enemy to stop taking damage
        }
    }
}
