using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] meleeHitClips;
    public AudioClip[] bowShootClips;
    public AudioClip[] arrowHitClips;

    private void Start()
    {
        audioSource.volume = 0.4f;
    }
    public void playMeleeHit()
    {
        //audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        if (meleeHitClips.Length > 0)
        {
            int random = UnityEngine.Random.Range(0, meleeHitClips.Length);
            audioSource.PlayOneShot(meleeHitClips[random]);
        }
        else
        {
            Debug.LogWarning("meleeHitClips is empty.");
        }
    }

    public void playBowShoot()
    {
        //audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        if (bowShootClips.Length > 0)
        {
            Debug.Log("Played Audio.");
            int random = UnityEngine.Random.Range(0, bowShootClips.Length);
            audioSource.PlayOneShot(bowShootClips[random]);
        }
        else
        {
            Debug.LogWarning("bowShootClips is empty.");
        }
    }
    public void playArrowHit()
    {
        //audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        if (arrowHitClips.Length > 0)
        {
            Debug.Log("Played Audio.");
            int random = UnityEngine.Random.Range(0, arrowHitClips.Length);
            audioSource.PlayOneShot(arrowHitClips[random]);
        }
        else
        {
            Debug.LogWarning("arrowHitClips is empty.");
        }
    }
}
