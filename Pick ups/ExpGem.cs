using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpGem : Pickup
{
    public int expGranted;
    public override void Collect()
    {
        if (hasCollected) { return; }
        else
        {
            base.Collect();
        }
        CharacterStat player = FindObjectOfType<CharacterStat>();
        if (player != null)
        {
            player.IncreaseExp(expGranted);
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
