using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Potion : MonoBehaviour
{
    public int healAmount = 30;
    public AudioClip healSFX; 
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                if (playerHealth.health >= playerHealth.maxHealth) return;
                playerHealth.Heal(healAmount);

                if (healSFX != null)
                    audioSource.PlayOneShot(healSFX);
                Destroy(gameObject, healSFX != null ? healSFX.length : 0f);
            }
        }
    }
}
