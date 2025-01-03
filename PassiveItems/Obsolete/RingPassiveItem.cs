using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentPickupRange *= 1 + passiveItemData.Multiplier / 100f;
    }
}
