using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ExpManager : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;

    private float detectionRange = 4f;

    public int expValue = 5;

    [SerializeField] private float chaseSpeed = 4f;
    private bool canMove = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            canMove = true;
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        if (!canMove) return;
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerExp playerExp = player.GetChild(1).GetComponent<PlayerExp>();
            playerExp.AddExperience(expValue);
            int randomRoll = Random.Range(1, 4); // Generates 1, 2, or 3
            switch (randomRoll)
            {
                case 1:
                    VanSoundManager.PlaySound(SoundType.PICKUP1, 0.8f);
                    break;
                case 2:
                    VanSoundManager.PlaySound(SoundType.PICKUP2, 0.8f);
                    break;
                case 3:
                    VanSoundManager.PlaySound(SoundType.PICKUP3, 0.8f);
                    break;
            }
            StartCoroutine(ExpDestroy());
        }
    }

    private IEnumerator ExpDestroy()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        canMove = false;
    }
}
