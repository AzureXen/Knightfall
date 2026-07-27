using UnityEngine;

public class FlyingEyeSFX : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip[] attackWarningClips;

    private void Start()
    {
        audioSource.volume = 0.4f;
    }

    public void playRandomAttackWarning()
    {
        if (attackWarningClips.Length > 0)
        {
            int random = UnityEngine.Random.Range(0, attackWarningClips.Length);
            audioSource.PlayOneShot(attackWarningClips[random]);
        }
        else
        {
            Debug.LogWarning("attackWarningClips is empty.");
        }
    }

    public void playAttackWarning(int a)
    {
        if (attackWarningClips.Length > 0)
        {
            audioSource.PlayOneShot(attackWarningClips[a]);
        }
        else
        {
            Debug.LogWarning("attackWarningClips is empty.");
        }
    }
}
