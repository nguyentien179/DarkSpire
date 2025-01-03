using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeOption : Upgrade
{
    public UpgradeOptionData data;
    [SerializeField] CharacterData.Stats currentBoosts;

    [System.Serializable]
    public struct Modifier
    {
        public string name, description;
        public CharacterData.Stats boosts;
    }

    public virtual void Initialise(UpgradeOptionData data)
    {
        base.Initialise(data);
        this.data = data;
        currentBoosts = data.baseStats.boosts;
    }

    public virtual CharacterData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    public override bool DoLevelUp()
    {
        base.DoLevelUp();
        if (!CanLevelUp())
        {
            return false;
        }

        currentBoosts += data.GetLevelData(++currentUpgradeLevel).boosts;
        return true;
    }
}
