using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIManager : MonoBehaviour
{
    public UpgradeOptionData[] upgradeItems;
    public GameObject shopItemPrefab;
    public Transform shopContentPanel;
    public TMPro.TMP_Text soulDisplay;
    public CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;


    private void Start()
    {
        PopulateShop();
        UpdateSoulDisplay();
    }

    void PopulateShop()
    {
        foreach (UpgradeOptionData upgrade in upgradeItems)
        {
            GameObject itemUI = Instantiate(shopItemPrefab, shopContentPanel);
            itemUI.transform.Find("Icon").GetComponent<Image>().sprite = upgrade.icon;
            itemUI.transform.Find("Price").GetComponent<TMPro.TMP_Text>().text = upgrade.cost.ToString();
            itemUI.transform.Find("LV").GetComponent<TMPro.TMP_Text>().text = upgrade.currentLevel.ToString();

            // Configure the purchase button
            Button purchaseButton = itemUI.transform.Find("Button").GetComponent<Button>();
            purchaseButton.onClick.AddListener(() => PurchaseUpgrade(upgrade, itemUI));
        }
    }

    void PurchaseUpgrade(UpgradeOptionData upgrade, GameObject itemUI)
    {
        if (GameManager.soul < upgrade.cost)
        {
            return;
        }

        if (upgrade.currentLevel == upgrade.maxLevel)
        {
            return;
        }
        GameManager.instance.SpendSouls(upgrade.cost);
        upgrade.currentLevel++;
        UpgradeOption.Modifier level = upgrade.GetLevelData(upgrade.currentLevel);
        itemUI.transform.Find("LV").GetComponent<TMPro.TMP_Text>().text = upgrade.currentLevel.ToString();
        ApplyUpgradeToPlayer();
        UpdateSoulDisplay();
    }

    void UpdateSoulDisplay()
    {
        soulDisplay.text = GameManager.soul.ToString();
    }

    void ApplyUpgradeToPlayer()
    {
        CharacterData selectedCharacter = CharacterSelector.GetData();
        foreach (UpgradeOptionData upgrade in upgradeItems)
        {
            selectedCharacter.stats += upgrade.baseStats.boosts;
        }
    }
}
