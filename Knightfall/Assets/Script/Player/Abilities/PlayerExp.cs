using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerExp : MonoBehaviour
{
    public int level = 0;
    private int experience = 0;
    private int experienceToNextLevel = 10;

    public Image expBar;
    public TextMeshProUGUI levelText;
    [SerializeField] private GameObject arrowImg;
    [SerializeField] private GameObject skillSelectionScreen;

    private float targetFillAmount;
    private float fillSpeed = 0.6f; // Adjust speed for bar animation
    private bool maxLevelReached = false; // Track if max level is reached

    private void Start()
    {
        UpdateUI();
    }

    public void AddExperience(int amount)
    {
        if (maxLevelReached) return; // Stop gaining EXP if max level is reached

        experience += amount;

        while (experience >= experienceToNextLevel)
        {
            experience -= experienceToNextLevel;
            LevelUp();
        }

        targetFillAmount = (float)experience / experienceToNextLevel;
    }

    private void LevelUp()
    {
        level++;

        if (level >= 11) // If max level is reached
        {
            level = 10;
            maxLevelReached = true;
            levelText.text = "MAX"; // Set the level text to "MAX"
            expBar.fillAmount = 1f; // Keep EXP bar full
            return; // Stop further leveling
        }

        experienceToNextLevel += 40;
        UpdateUI();
        StartCoroutine(ArrowPop());
        VanSoundManager.PlaySound(SoundType.SKILLCHOSEN);
        OpenSkillSelection();
    }

    private void OpenSkillSelection()
    {
        if (maxLevelReached) return; // Prevent opening skill selection if max level is reached

        if (skillSelectionScreen != null)
        {
            skillSelectionScreen.SetActive(true);

            SkillSelectionScreen selectionScreen = skillSelectionScreen.GetComponent<SkillSelectionScreen>();
            if (selectionScreen != null)
            {
                selectionScreen.OpenSkillSelection();
            }

            Time.timeScale = 0f; // Pause game time
        }
    }

    private void UpdateUI()
    {
        if (levelText != null)
        {
            levelText.text = level.ToString();
        }
    }

    private void Update()
    {
        if (expBar != null)
        {
            expBar.fillAmount = Mathf.Lerp(expBar.fillAmount, targetFillAmount, Time.unscaledDeltaTime * fillSpeed);
        }
    }

    private IEnumerator ArrowPop()
    {
        arrowImg.SetActive(true);
        yield return new WaitForSecondsRealtime(0.3f);
        arrowImg.SetActive(false);
    }
}