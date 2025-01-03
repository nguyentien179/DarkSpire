using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if (item is Weapon)
            {
                Weapon weapon = item as Weapon;
                image.enabled = true;
                image.sprite = weapon.data.icon;
            }
            else
            {
                Passive passive = item as Passive;
                image.enabled = true;
                image.sprite = passive.data.icon;
            }
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty()
        {
            return item == null;
        }
    }
    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }
    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();
    public List<PassiveData> availablePassives = new List<PassiveData>();
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    CharacterStat player;

    void Start()
    {
        player = GetComponent<CharacterStat>();
    }

    public bool Has(ItemData type)
    {
        return Get(type);
    }

    public Item Get(ItemData type)
    {
        if (type is WeaponData) return Get(type as WeaponData);
        else if (type is PassiveData) return Get(type as PassiveData);
        return null;
    }

    public Passive Get(PassiveData type)
    {
        foreach (Slot slot in passiveSlots)
        {
            Passive passive = slot.item as Passive;
            if (passive.data == type) return passive;
        }
        return null;
    }

    public Weapon Get(WeaponData type)
    {
        foreach (Slot slot in weaponSlots)
        {
            Weapon weapon = slot.item as Weapon;
            if (weapon.data == type) return weapon;
        }
        return null;
    }
    public bool Remove(WeaponData weaponData, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availableWeapons.Remove(weaponData);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon weapon = weaponSlots[i].item as Weapon;
            if (weapon.data == weaponData)
            {
                weaponSlots[i].Clear();
                weapon.OnUnequip();
                Destroy(weapon.gameObject);
                return true;
            }
        }
        return false;
    }

    public bool Remove(PassiveData passiveData, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availablePassives.Remove(passiveData);

        for (int i = 0; i < passiveSlots.Count; i++)
        {
            Passive passive = passiveSlots[i].item as Passive;
            if (passive.data == passiveData)
            {
                passiveSlots[i].Clear();
                passive.OnUnequip();
                Destroy(passive.gameObject);
                return true;
            }
        }
        return false;
    }

    public bool Remove(ItemData itemData, bool removeUpgradeAvailability = false)
    {
        if (itemData is PassiveData) return Remove(itemData as PassiveData, removeUpgradeAvailability);
        else if (itemData is WeaponData) return Remove(itemData as WeaponData, removeUpgradeAvailability);
        return false;
    }

    public int Add(WeaponData weaponData)
    {
        int slotNum = -1;
        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }
        if (slotNum < 0)
        {
            return slotNum;
        }
        Type weaponType = Type.GetType(weaponData.behaviour);

        if (weaponType != null)
        {
            GameObject gameObject = new GameObject(weaponData.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)gameObject.AddComponent(weaponType);
            spawnedWeapon.Initialise(weaponData);
            spawnedWeapon.transform.SetParent(transform);
            spawnedWeapon.transform.localPosition = Vector2.zero;

            weaponSlots[slotNum].Assign(spawnedWeapon);

            if (GameManager.instance != null && GameManager.instance.isChoosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format("Invalid weapon for {0}", weaponData.name));
        }

        return -1;
    }

    public int Add(PassiveData passiveData)
    {
        int slotNum = -1;

        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }
        if (slotNum < 0)
        {
            return slotNum;
        }

        GameObject gameObject = new GameObject(passiveData.baseStats.name + " Passive");
        Passive passive = gameObject.AddComponent<Passive>();
        passive.Initialise(passiveData);
        passive.transform.SetParent(transform);
        passive.transform.localPosition = Vector2.zero;

        passiveSlots[slotNum].Assign(passive);
        if (GameManager.instance != null && GameManager.instance.isChoosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
        return slotNum;
    }
    public int Add(ItemData itemData)
    {
        if (itemData is PassiveData) return Add(itemData as PassiveData);
        else if (itemData is WeaponData) return Add(itemData as WeaponData);
        return -1;
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        Weapon weapon = weaponSlots[slotIndex].item as Weapon;
        if (!weapon.DoLevelUp()) return;
        if (GameManager.instance != null && GameManager.instance.isChoosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveSlots.Count > slotIndex)
        {
            Passive passive = passiveSlots[slotIndex].item as Passive;
            if (!passive.DoLevelUp()) return;
        }
        if (GameManager.instance != null && GameManager.instance.isChoosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
    }

    void ApplyUpgradeOption()
    {
        List<WeaponData> availableWeaponUpgrade = new List<WeaponData>(this.availableWeapons);
        List<PassiveData> availablePassiveUpgrade = new List<PassiveData>(this.availablePassives);

        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            if (availableWeaponUpgrade.Count == 0 && availablePassiveUpgrade.Count == 0)
            {
                return;
            }

            int upgradeType;
            if (availableWeaponUpgrade.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availablePassiveUpgrade.Count == 0)
            {
                upgradeType = 1;
            }

            else
            {
                upgradeType = UnityEngine.Random.Range(1, 3);
            }

            if (upgradeType == 1)
            {
                WeaponData chosenWeaponUpgrade = availableWeaponUpgrade[UnityEngine.Random.Range(0, availableWeaponUpgrade.Count)];
                availableWeaponUpgrade.Remove(chosenWeaponUpgrade);
                if (chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool isLevelUp = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        Weapon weapon = weaponSlots[i].item as Weapon;
                        if (weapon != null && weapon.data == chosenWeaponUpgrade)
                        {
                            if (chosenWeaponUpgrade.maxLevel <= weapon.currentLevel)
                            {
                                isLevelUp = true;
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i));
                            Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(weapon.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade));
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name;
                        upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                    }
                }
            }
            else if (upgradeType == 2)
            {
                PassiveData chosenPassiveUpgrade = availablePassiveUpgrade[UnityEngine.Random.Range(0, availablePassiveUpgrade.Count)];
                availablePassiveUpgrade.Remove(chosenPassiveUpgrade);
                if (chosenPassiveUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool isLevelUp = false;
                    for (int i = 0; i < passiveSlots.Count; i++)
                    {
                        Passive passive = passiveSlots[i].item as Passive;
                        if (passive != null && passive.data == chosenPassiveUpgrade)
                        {
                            if (chosenPassiveUpgrade.maxLevel <= passive.currentLevel)
                            {
                                isLevelUp = true;
                                break;
                            }
                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i));
                            Passive.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(passive.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }
                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade));
                        Passive.Modifier nextLevel = chosenPassiveUpgrade.baseStats;
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                        upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                        upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                    }
                }
            }
        }
    }

    void RemoveUpgradeOption()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOption();
        ApplyUpgradeOption();
    }

    void DisableUpgradeUI(UpgradeUI upgradeUI)
    {
        upgradeUI.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI upgradeUI)
    {
        upgradeUI.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}