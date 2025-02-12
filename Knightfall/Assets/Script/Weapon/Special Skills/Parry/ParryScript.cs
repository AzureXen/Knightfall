using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParryScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerBullet playerBullet = collision.gameObject.GetComponent<PlayerBullet>();
        if(playerBullet != null)
        {
            Debug.Log("Parry!");
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            if (audioClips.Length > 0)
            {
                int random = Random.Range(0, audioClips.Length);
                audioSource.clip = audioClips[random];
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("Audio for ParryHit not found.");
            }
            playerBullet.DestroyBullet();
        }
        else
        {
            Debug.Log("playerBullet not found.");
        }
    }
}
