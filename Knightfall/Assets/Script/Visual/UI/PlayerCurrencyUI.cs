using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerGoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ValueDisplay;

    private void OnEnable()
    {

        if (GameManagerScript.Instance != null && GameManagerScript.Instance.playerData != null)
        {
            GameManagerScript.Instance.playerData.OnGoldChanged += UpdateDisplay;
            UpdateDisplay(GameManagerScript.Instance.playerData.Gold);
        }
        else
        {
            StartCoroutine(WaitAndSubscribe());
        }
    }

    private void OnDisable()
    {
        if (GameManagerScript.Instance != null && GameManagerScript.Instance.playerData != null)
        {
            GameManagerScript.Instance.playerData.OnGoldChanged -= UpdateDisplay;
        }
    }

    private System.Collections.IEnumerator WaitAndSubscribe()
    {
        yield return null; 

        if (GameManagerScript.Instance != null && GameManagerScript.Instance.playerData != null)
        {
            GameManagerScript.Instance.playerData.OnGoldChanged += UpdateDisplay;
            UpdateDisplay(GameManagerScript.Instance.playerData.Gold);
        }
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
