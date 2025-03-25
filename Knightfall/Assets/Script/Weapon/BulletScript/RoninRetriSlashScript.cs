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

        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        // DEFAULT ENEMY BULLET DIRECTION IF PLAYER IS DESTROYED WHEN PROJECTILE IS SHOT
        Vector3 bulletDirection = Vector3.up;
        Vector3 playerPos = Vector3.up;
        // FOR ENEMY BULLETS, CHANGE THE BULLET DIRECTION TO PLAYER
        if (GameObject.FindGameObjectsWithTag("Player") != null)
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            bulletDirection = playerPos - transform.position;
        }
        rb.linearVelocity = new Vector2(bulletDirection.x, bulletDirection.y).normalized * force;

        Vector3 bulletRotation = transform.position - playerPos;
        float rotation = Mathf.Atan2(bulletRotation.y, bulletRotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotation + 180);
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
