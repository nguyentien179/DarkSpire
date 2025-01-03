using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Pickup
{
    int healthGranted;
    public override void Collect()
    {
        if (hasCollected) { return; }
        else
        {
            base.Collect();
        }
        CharacterStat player = FindObjectOfType<CharacterStat>();
        healthGranted = Mathf.FloorToInt(player.CurrentMaxHealth * 0.25f);
        if (player.CurrentHealth < player.CurrentMaxHealth)
        {
            player.Heal(healthGranted);
            Destroy(gameObject);
        }
    }
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Collect();
        }
    }
}
