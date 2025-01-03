using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public int currentUpgradeLevel = 1, maxUpgradeLevel = 5;
    protected CharacterStat owner;
    // Start is called before the first frame update
    public virtual void Initialise(UpgradeData data)
    {
        maxUpgradeLevel = data.maxLevel;
        owner = FindObjectOfType<CharacterStat>();
    }

    public virtual bool CanLevelUp()
    {
        return currentUpgradeLevel <= maxUpgradeLevel;
    }

    public virtual bool DoLevelUp()
    {
        return true;
    }
}
