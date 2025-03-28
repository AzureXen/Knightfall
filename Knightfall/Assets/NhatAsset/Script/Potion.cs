using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Potion : MonoBehaviour
{
    public int healAmount = 30;
    public AudioClip healSFX; // 🎵 Kéo sound effect vào đây
    private AudioSource audioSource;

    private void Start()
    {
        // Tạo audio source nếu chưa có
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
                // ❌ Không cho nhặt nếu máu đầy
                if (playerHealth.health >= playerHealth.maxHealth) return;

                // ✅ Heal và phát âm
                playerHealth.Heal(healAmount);

                if (healSFX != null)
                    audioSource.PlayOneShot(healSFX);

                // 🕓 Delay huỷ để âm thanh phát xong
                Destroy(gameObject, healSFX != null ? healSFX.length : 0f);
            }
        }
    }
}
