using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Updgrade Data", menuName = "2D RougeLike/Upgrade Data")]
public class UpgradeOptionData : UpgradeData
{
    public UpgradeOption.Modifier baseStats;
    public UpgradeOption.Modifier[] growth;

    public UpgradeOption.Modifier GetLevelData(int level)
    {
        if (level - 2 < growth.Length)
            return growth[level - 2];
        return new UpgradeOption.Modifier();
    }
}
