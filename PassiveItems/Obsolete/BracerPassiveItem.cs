using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BracerPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMight *= 1 + passiveItemData.Multiplier / 100f;
    }
}
