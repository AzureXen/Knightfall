using UnityEngine;

public class RoninSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] blockClips;
    public AudioClip[] deflectClips;
    public AudioClip[] deflectRangedClips;

    public AudioClip[] flashSlashClips;
    public AudioClip[] flashSlashHitClips;
    

    public AudioClip[] attackWarningClips;

    public AudioClip[] retributionSlashClips;
    private void Start()
    {
        audioSource.volume = 0.6f;
    }
    public void playDeflect()
    {
        // Random pitch if want
        //audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        if (deflectClips.Length > 0)
        {
            int random = UnityEngine.Random.Range(0, deflectClips.Length);
            audioSource.PlayOneShot(deflectClips[random]);
        }
        else
        {
            Debug.LogWarning("deflectClips is empty.");
        }
    }

    public void playDeflectRanged()
    {
        // Random pitch if want
        //audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        if (deflectRangedClips.Length > 0)
        {
            int random = UnityEngine.Random.Range(0, deflectRangedClips.Length);
            audioSource.PlayOneShot(deflectRangedClips[random]);
        }
        else
        {
            Debug.LogWarning("deflectRangedClips is empty.");
        }
    }


    public void playBlock()
    {
        //audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        if (blockClips.Length > 0)
        {
            int random = UnityEngine.Random.Range(0, blockClips.Length);
            audioSource.PlayOneShot(blockClips[random]);
        }
        else
        {
            Debug.LogWarning("blockClips is empty.");
        }
    }

    public void playFlashSlash()
    {
        if (flashSlashClips.Length > 0)
        {
            int random = UnityEngine.Random.Range(0, flashSlashClips.Length);
            audioSource.PlayOneShot(flashSlashClips[random]);
        }
        else
        {
            Debug.LogWarning("flashSlashClips is empty.");
        }
    }

    public void playFlashSlashHit()
    {
        if (flashSlashHitClips.Length > 0)
        {
            int random = UnityEngine.Random.Range(0, flashSlashHitClips.Length);
            audioSource.PlayOneShot(flashSlashHitClips[random]);
        }
        else
        {
            Debug.LogWarning("flashSlashHitClips is empty.");
        }
    }

    public void playAttackWarning()
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

    public void playerRetributionSlash()
    {
        if (retributionSlashClips.Length > 0)
        {
            int random = UnityEngine.Random.Range(0, retributionSlashClips.Length);
            audioSource.PlayOneShot(retributionSlashClips[random]);
        }
        else
        {
            Debug.LogWarning("retributionSlashClips is empty.");
        }
    }
}
