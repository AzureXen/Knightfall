using UnityEngine;

public class FlyingEyeVFX : MonoBehaviour
{
    [SerializeField] private GameObject warninngFlashRed;

    private WarningFlashRedAnimator warningFlashRedAm;

    private void Start()
    {
        if (warninngFlashRed != null)
        {
            warningFlashRedAm = warninngFlashRed.GetComponent<WarningFlashRedAnimator>();
        }
        else Debug.Log("FlyingEyeVFX: warningFlashRed not set!");
    }

    public void FlashRedWarning()
    {
        warningFlashRedAm.PlayWarningFlashRed();
    }
}
