using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerGoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ValueDisplay;


    void Start()
    {
        GameManagerScript.Instance.playerData.OnGoldChanged += UpdateDisplay;
        UpdateDisplay(GameManagerScript.Instance.playerData.GetGold());
    }
    private void UpdateDisplay(float gold)
    {
        ValueDisplay.SetText(gold.ToString());
    }

    private void OnDestroy()
    {
        if (GameManagerScript.Instance != null)
            GameManagerScript.Instance.playerData.OnGoldChanged -= UpdateDisplay;
    }

}
