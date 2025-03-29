using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillSelectionScreen : MonoBehaviour
{
    [SerializeField] private GameObject skillSelectionUI;
    [SerializeField] private List<GameObject> skillSelections; // List of SkillSelection parents
    private int currentSkillIndex = 0;
    private int voiceLineIndex = 0;

    private PlayerSkillManager playerSkillManager;
    private PassiveHandler passiveHandler;

    private void Start()
    {
        playerSkillManager = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<PlayerSkillManager>();
        passiveHandler = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).transform.GetChild(0).GetComponent<PassiveHandler>();

        // Assign button listeners dynamically
        foreach (GameObject selection in skillSelections)
        {
            Button[] buttons = selection.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                string skillName = button.gameObject.name;
                button.onClick.AddListener(() => SelectSkill(skillName));
            }
        }
    }

    public void OpenSkillSelection()
    {
        skillSelectionUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game

        if (skillSelections.Count == 0)
        {
            Debug.LogError("SkillSelectionScreen: No skill selections assigned!");
            return;
        }

        ShowNextSkillSelection();
    }

    private void ShowNextSkillSelection()
    {
        HideAllSkillSelections();

        if (currentSkillIndex < skillSelections.Count)
        {
            skillSelections[currentSkillIndex].SetActive(true);

            SoundType[] voiceLineOrder = new SoundType[]
            {
                SoundType.SAGEVOICELINE1, SoundType.MONKVOICELINE1, SoundType.ANGELVOICELINE1,
                SoundType.SAGEVOICELINE2, SoundType.MONKVOICELINE2, SoundType.ANGELVOICELINE2,
                SoundType.SAGEVOICELINE3, SoundType.MONKVOICELINE3, SoundType.ANGELVOICELINE3
            };

            if (voiceLineIndex < voiceLineOrder.Length) // Ensure we don’t exceed array bounds
            {
                VanSoundManager.PlaySound(voiceLineOrder[voiceLineIndex]);
                voiceLineIndex++; // Move to the next voice line
            }

            currentSkillIndex = (currentSkillIndex + 1) % skillSelections.Count;
        }
        else
        {
            Debug.LogError("SkillSelectionScreen: currentSkillIndex is out of range!");
        }
    }

    private void HideAllSkillSelections()
    {
        foreach (var selection in skillSelections)
        {
            if (selection != null) selection.SetActive(false);
        }
    }

    public void SelectSkill(string skillName)
    {
        Button clickedButton = null;
        VanSoundManager.PlaySound(SoundType.LEVELUP, 0.5f);

        // Find the button that was clicked
        foreach (GameObject selection in skillSelections)
        {
            Button[] buttons = selection.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                if (button.gameObject.name == skillName)
                {
                    clickedButton = button;
                    break;
                }
            }
        }

        if (clickedButton != null)
        {
            clickedButton.interactable = false; // Disable the button
        }

        // Check if skill is an active skill or passive
        if (playerSkillManager != null && playerSkillManager.UnlockSkill(skillName))
        {
            Debug.Log("Active Skill Unlocked: " + skillName);
        }
        else if (passiveHandler != null && passiveHandler.UnlockPassive(skillName))
        {
            Debug.Log("Passive Skill Unlocked: " + skillName);
        }
        else
        {
            Debug.LogError("Skill not found in PlayerSkillManager or PassiveHandler: " + skillName);
        }

        skillSelectionUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }
}
