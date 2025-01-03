using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterStat : MonoBehaviour
{
    public CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;
    float health;
    private Animator animator;
    #region StatProperties
    public float CurrentHealth
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = $"Health: {Mathf.Floor(health)} / {Mathf.Floor(actualStats.maxHealth)}";
                }
            }
        }
    }

    public float MaxHealth
    {
        get { return actualStats.maxHealth; }
        set
        {
            if (actualStats.maxHealth != value)
            {
                actualStats.maxHealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = $"Health: {Mathf.Floor(health)} / {Mathf.Floor(actualStats.maxHealth)}";
                }
            }
        }
    }

    public float CurrentMaxHealth
    {
        get { return MaxHealth; }
        set
        {
            MaxHealth = value;
        }
    }

    public float Recovery
    {
        get { return actualStats.recovery; }
        set
        {
            if (actualStats.recovery != value)
            {
                actualStats.recovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = $"Recovery: {actualStats.recovery}";
                }
            }
        }
    }
    public float CurrentRecovery
    {
        get { return Recovery; }
        set
        {
            Recovery = value;
        }
    }

    public float MoveSpeed
    {
        get { return actualStats.moveSpeed; }
        set
        {
            if (actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = $"Move Speed: {actualStats.moveSpeed}";
                }
            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set
        {
            MoveSpeed = value;
        }
    }

    public float Might
    {
        get { return actualStats.might; }
        set
        {
            if (actualStats.might != value)
            {
                actualStats.might = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = $"Might: {actualStats.might}";
                }
            }
        }
    }
    public float CurrentMight
    {
        get { return Might; }
        set
        {
            Might = value;
        }
    }

    public float Speed
    {
        get { return actualStats.speed; }
        set
        {
            if (actualStats.speed != value)
            {
                actualStats.speed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = $"Speed: {actualStats.speed}";
                }
            }
        }
    }
    public float CurrentProjectileSpeed
    {
        get { return Speed; }
        set
        {
            Speed = value;
        }
    }



    float PickupRange
    {
        get { return actualStats.pickupRange; }
        set
        {
            if (actualStats.pickupRange != value)
            {
                actualStats.pickupRange = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentPickupRangeDisplay.text = $"Pickup Range: {actualStats.pickupRange}";
                }
            }
        }
    }
    public float CurrentPickupRange
    {
        get { return PickupRange; }
        set
        {
            PickupRange = value;
        }
    }

    #endregion

    //experience and level
    [Header("Experience/Level")]
    public int experience;
    public int level;
    public int expCap;

    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int expCapIncrease;
    }

    //I-Frames
    [Header("I-Frames")]
    public float iFrameDuration;
    float iFrameTimer;
    bool isInvulnerable;


    public List<LevelRange> levelRanges;


    PlayerInventory inventory;
    public int weaponIndex;
    public int passiveItemIndex;
    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TMPro.TMP_Text levelDisplay;

    public static CharacterStat instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<PlayerInventory>();

        baseStats = actualStats = characterData.stats;
        health = actualStats.maxHealth;

        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        inventory.Add(characterData.StartingWeapon);

        expCap = levelRanges[0].expCapIncrease;

        GameManager.instance.AssignCharacterUI(characterData);

        UpdateHealthBar();
        UpdateExpBar();
        updateLevelDisplay();
        RecalculateStats();
    }

    void Update()
    {

        if (iFrameTimer > 0)
        {
            iFrameTimer -= Time.deltaTime;
        }
        else if (isInvulnerable)
        {
            isInvulnerable = false;
        }
        Recover();

        GameManager.instance.currentHealthDisplay.text = $"Health:{Math.Floor(CurrentHealth) + " / " + Math.Floor(MaxHealth)} ";
        GameManager.instance.currentRecoveryDisplay.text = $"Recovery: {CurrentRecovery}/s";
        GameManager.instance.currentMoveSpeedDisplay.text = $"Move Speed: {CurrentMoveSpeed}";
        GameManager.instance.currentMightDisplay.text = $"Might: {CurrentMight}";
        GameManager.instance.currentProjectileSpeedDisplay.text = $"Projectile Speed: {CurrentProjectileSpeed}";
        GameManager.instance.currentPickupRangeDisplay.text = $"Pickup Range: {CurrentPickupRange}";
        GameManager.instance.healthDisplay.text = $"Health:{Math.Floor(CurrentHealth) + " / " + Math.Floor(MaxHealth)} ";
    }
    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot slot in inventory.passiveSlots)
        {
            Passive passive = slot.item as Passive;
            if (passive)
            {
                actualStats += passive.GetBoosts();
            }
        }
    }

    public void IncreaseExp(int amount)
    {
        experience += amount;

        LevelUpChecker();
        UpdateExpBar();
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > actualStats.maxHealth)
        {
            CurrentHealth = actualStats.maxHealth;
        }
    }

    public void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth && CurrentRecovery > 0)
        {
            health += CurrentRecovery * Time.deltaTime;
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
            UpdateHealthBar();
        }

    }

    void LevelUpChecker()
    {
        if (experience >= expCap)
        {
            level++;
            experience -= expCap;

            int expCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    expCapIncrease = range.expCapIncrease;
                    break;
                }
            }
            expCap += expCapIncrease;
            updateLevelDisplay();

            GameManager.instance.StartLevelUp();
        }
    }
    public void TakeDamage(float dmg)
    {
        if (!isInvulnerable)
        {
            CurrentHealth -= dmg;
            animator.SetBool("Hit", true);
            StartCoroutine(ResetHitBool());
            iFrameTimer = iFrameDuration;
            isInvulnerable = true;
            UpdateHealthBar();

            if (CurrentHealth <= 0)
            {
                Kill();
            }
        }

    }
    public void Kill()
    {
        animator.SetBool("Death", true);
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator ResetHitBool()
    {
        // Wait for a short duration before resetting the hit bool
        yield return new WaitForSeconds(0.5f); // Adjust this duration as needed
        animator.SetBool("Hit", false);
    }
    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignChosenWeaponAndItemUI(inventory.weaponSlots, inventory.passiveSlots);
            GameManager.instance.AssignLevel(level);
            GameManager.instance.GameOver();
        }
    }

    public void SpawnWeapon(GameObject weapon)
    {

        if (weaponIndex >= inventory.weaponSlots.Count)
        {
            return;
        }
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);
        // inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());

        weaponIndex++;
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveItemIndex >= inventory.passiveSlots.Count - 1)
        {

            return;
        }
        GameObject spawnedItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedItem.transform.SetParent(transform);
        // inventory.AddPassiveItem(passiveItemIndex, spawnedItem.GetComponent<PassiveItem>());

        passiveItemIndex++;

    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = health / MaxHealth;
    }

    void UpdateExpBar()
    {
        expBar.fillAmount = (float)experience / expCap;
    }


    void updateLevelDisplay()
    {
        levelDisplay.text = "LV " + level.ToString();
    }
}
