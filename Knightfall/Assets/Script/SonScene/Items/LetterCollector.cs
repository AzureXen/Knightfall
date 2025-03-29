using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterCollectorCustom : MonoBehaviour
{
    public int totalLetters = 8; // Total letters required
    private int collectedLetters = 0;
    public TextMeshProUGUI letterCountText;

    public EndGameManagement endGameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateLetterUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollectLetter()
    {
        collectedLetters++;
        UpdateLetterUI();

        if (collectedLetters >= totalLetters)
        {
            endGameManager.WinLevel();
        }
    }

    void UpdateLetterUI()
    {
        letterCountText.text = $"{collectedLetters}/{totalLetters} letters";
    }

    public bool HasCollectedAllLetters()
    {
        return collectedLetters >= totalLetters;
    }
}
