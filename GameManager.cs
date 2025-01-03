using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum GameState
    {
        Gameplay,
        Paused,
        LevelUp,
        GameOver
    }

    public GameState currentState;

    public GameState previousState;
    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("SCreens")]
    public GameObject pauseScreen;
    public GameObject resultScreen;
    public GameObject levelUpScreen;
    [Header("Souls")]
    public static int soul = 1000;


    [Header("Current Stat")]
    //Current Stat Display
    public TMPro.TMP_Text currentHealthDisplay;
    public TMPro.TMP_Text currentMoveSpeedDisplay;
    public TMPro.TMP_Text currentMightDisplay;
    public TMPro.TMP_Text currentRecoveryDisplay;
    public TMPro.TMP_Text currentProjectileSpeedDisplay;
    public TMPro.TMP_Text currentPickupRangeDisplay;


    [Header("Results")]
    public Image characterImage;
    public TMPro.TMP_Text characterName;
    public TMPro.TMP_Text levelReachedDisplay;
    public TMPro.TMP_Text timeSurvivedDisplay;

    public List<Image> chosenWeaponUI = new List<Image>(6);
    public List<Image> chosenPassiveItemUI;
    public TMPro.TMP_Text enemyKilledDisplay;
    [Header("Pause Screen")]
    public Image pauseCharacterImage;
    public TMPro.TMP_Text pauseCharacterName;

    [Header("Stop Watch")]
    public float timeLimit;
    float stopwatchTime;
    public TMPro.TMP_Text stopwatchDisplay;
    public TMP_Text healthDisplay;


    public bool isGameOver = false;

    public bool isChoosingUpgrade;

    public GameObject playerObject;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        soul = PlayerPrefs.GetInt("soul", 1001);
        DisableScreens();
    }
    void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                break;
            case GameState.LevelUp:
                if (!isChoosingUpgrade)
                {
                    isChoosingUpgrade = true;
                    Time.timeScale = 0f;
                }
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    DisplayResults();
                }
                break;
            default:
                break;
        }
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        enemyKilledDisplay.text = enemySpawner.enemyKilled.ToString();
    }

    public void AddSouls(int amount)
    {
        soul += amount;
        PlayerPrefs.SetInt("soul", soul);
        PlayerPrefs.Save();
    }

    public void SpendSouls(int amount)
    {
        soul -= amount;
        PlayerPrefs.SetInt("soul", soul);
        PlayerPrefs.Save();
    }
    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }
    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            DisableScreens();
            ChangeState(previousState);
            Time.timeScale = 1f;
        }
    }

    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        CharacterStat player = FindObjectOfType<CharacterStat>();
        timeSurvivedDisplay.text = stopwatchDisplay.text;
        AssignLevel(player.level);
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultScreen.SetActive(true);
    }

    public void AssignCharacterUI(CharacterData character)
    {
        characterImage.sprite = character.Icon;
        characterName.text = character.CharacterName;

        pauseCharacterImage.sprite = character.Icon;
        pauseCharacterName.text = character.CharacterName;
    }

    public void AssignLevel(int level)
    {
        levelReachedDisplay.text = level.ToString();
    }

    public void AssignChosenWeaponAndItemUI(List<PlayerInventory.Slot> chosenWeaponData, List<PlayerInventory.Slot> chosenPassiveItemData)
    {
        if (chosenWeaponData.Count != chosenWeaponUI.Count || chosenPassiveItemData.Count != chosenPassiveItemUI.Count)
        {
            return;
        }
        for (int i = 0; i < chosenWeaponUI.Count; i++)
        {
            if (chosenWeaponData[i].image.sprite)
            {
                chosenWeaponUI[i].enabled = true;
                chosenWeaponUI[i].sprite = chosenWeaponData[i].image.sprite;
            }
            else
            {
                chosenWeaponUI[i].enabled = false;
            }
        }
        for (int i = 0; i < chosenPassiveItemData.Count; i++)
        {
            if (chosenPassiveItemData[i].image.sprite)
            {
                chosenPassiveItemUI[i].enabled = true;
                chosenPassiveItemUI[i].sprite = chosenPassiveItemData[i].image.sprite;
            }
            else
            {
                chosenPassiveItemUI[i].enabled = false;
            }
        }
    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;

        UpdateStopwatchDisplay();

        if (stopwatchTime >= timeLimit)
        {
            GameOver();
        }
    }

    void UpdateStopwatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60f);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60f);

        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        levelUpScreen.SetActive(true);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }
    public void EndLevelUp()
    {
        isChoosingUpgrade = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }

    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        if (!instance.damageTextCanvas) return;
        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;
        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(text, target, duration, speed));
    }
    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if (textFont) tmPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        Destroy(textObj, duration);

        textObj.transform.SetParent(instance.damageTextCanvas.transform);
        textObj.transform.SetSiblingIndex(0);

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        while (t < duration)
        {

            if (textObj == null) yield break;
            if (target == null)
            {
                Destroy(textObj); // Destroy the text object immediately if the target is null
                yield break; // Exit the coroutine
            }

            // Check if the text object and its components are still valid
            if (tmPro == null || rect == null)
            {
                yield break; // Exit if the object has been destroyed
            }

            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, Mathf.Lerp(1f, 0f, t / duration));

            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(target.position + new Vector3(0, yOffset));

            yield return w;
            t += Time.deltaTime;
        }
    }

}
