using UnityEngine;
using UnityEngine.UI;

public class BookTrigger : MonoBehaviour
{
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public GameObject bossHealthUI; // Gán GameObject có BossHealthBar (ví dụ Canvas)

    public AudioClip sfx;
    public AudioClip bossBGM;
    public GameObject bgmObject;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered || !collision.CompareTag("Player")) return;
        triggered = true;

        // Stop current music
        if (bgmObject != null)
        {
            AudioSource bgmSource = bgmObject.GetComponent<AudioSource>();
            if (bgmSource != null)
                bgmSource.Stop();
        }

        // Play boss music
        if (bossBGM != null)
            AudioSource.PlayClipAtPoint(bossBGM, transform.position);

        // Spawn boss
        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

        // Gán BossHealth vào UI
        if (bossHealthUI != null)
        {
            BossHealthBar healthBar = bossHealthUI.GetComponent<BossHealthBar>();
            Health bossHealth = boss.GetComponent<Health>();

            if (healthBar != null && bossHealth != null)
            {
                healthBar.bossHealth = bossHealth;
                healthBar.enabled = true; // đảm bảo script bật
            }
        }

        // SFX nhặt sách
        if (sfx != null)
            AudioSource.PlayClipAtPoint(sfx, transform.position);

        Destroy(gameObject); // Xoá sách
    }
}
