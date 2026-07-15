using System;
using System.Collections;
using UnityEngine;

public class RoninRetriSlashScript : BulletScript
{
    public float fadeDuration = 1f;
    public Boolean playerEntered = false;
    public EntityManager playerManager;

    private void Update()
    {
        if(playerEntered)
        {
            if (playerManager != null)
            {
                playerManager.TakeRangedHit(damage, transform.position, knockbackForce, knockbackDuration, this);
            }
        }
    }
    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector3 bulletDirection = Vector3.up;

        GameObject player = GameManagerScript.Instance?.player;
        if (player != null)
        {
            bulletDirection = (player.transform.position - transform.position).normalized;
        }

        Debug.Log($"bullet direction: {bulletDirection}, player null: {player == null}");
        Debug.Log($"bullet pos: {transform.position}, player pos: {player.transform.position}");

        rb.linearVelocity = new Vector2(bulletDirection.x, bulletDirection.y) * force;

        float rotation = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotation);

        Destroy(gameObject, despawnTime);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            playerManager = collision.gameObject.GetComponent<EntityManager>();
            playerEntered = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerManager = collision.gameObject.GetComponent<EntityManager>();
            playerEntered = false;
        }
    }

    private IEnumerator FadeAway(SpriteRenderer sr)
    {
        Color originColor = sr.color;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            sr.color = new Color(originColor.r, originColor.g, originColor.b, alpha);
            yield return null;
        }
        sr.color = new Color(originColor.r, originColor.g, originColor.b, 0f);
    }
}
