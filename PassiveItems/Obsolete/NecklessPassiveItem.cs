using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecklessPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentRecovery += passiveItemData.Multiplier / 100f;
    }
}
