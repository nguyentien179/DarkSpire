using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermesBootsPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMoveSpeed *= 1 + passiveItemData.Multiplier / 100f;
    }
}
