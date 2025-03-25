using System;
using System.Collections;
using UnityEngine;

public class EnemyBullet : BulletScript
{
    public GameObject arrowHalvesPrefab;
    public float splitForce = 2f;
    public float splitRotateForce = 25f;
    public float destroyTime = 2f;
    public float fadeDuration = 1f;
    //FOR ERNEMY BULLET, ADD PLAYER POSITION

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
        transform.rotation = Quaternion.Euler(0, 0, rotation + 90);
        Destroy(gameObject, despawnTime);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Boolean successHit = false;
        if (collision.CompareTag("Player"))
        {
            EntityManager enemyManager = collision.gameObject.GetComponent<EntityManager>();
            if (enemyManager != null)
            {
                successHit = enemyManager.TakeRangedHit(damage, transform.position, knockbackForce, knockbackDuration, this);
            }
        }
    }
    public override void BreakBullet()
    {
        GameObject arrowHalves = Instantiate(arrowHalvesPrefab, transform.position, transform.rotation);
        Transform half1 = arrowHalves.transform.GetChild(0);
        Transform half2 = arrowHalves.transform.GetChild(1);

        Rigidbody2D rb1 = half1.GetComponent<Rigidbody2D>();
        Rigidbody2D rb2 = half2.GetComponent<Rigidbody2D>();

        SpriteRenderer sr1 = half1.GetComponent<SpriteRenderer>();
        SpriteRenderer sr2 = half2.GetComponent<SpriteRenderer>();

        if (sr1 == null || sr2 == null)
        {
            Debug.LogError("SpriteRenderer not found on arrow halves.");
            return;
        }

        //Vector2 splitDirection = Vector2.Perpendicular(transform.up).normalized;
        //Vector2 splitDirectionRandomized = new Vector2(splitDirection.x - UnityEngine.Random.Range(-1f, 0f), splitDirection.y);
        Vector2 splitRandom1 = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1));
        Vector2 splitRandom2 = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1));

        //rb1.AddForce(-splitDirectionRandomized * splitForce, ForceMode2D.Impulse);
        //rb2.AddForce(splitDirectionRandomized * splitForce, ForceMode2D.Impulse);

        rb1.AddForce(splitRandom1 * splitForce, ForceMode2D.Impulse);
        rb2.AddForce(splitRandom2 * splitForce, ForceMode2D.Impulse);

        rb1.AddTorque(-splitRotateForce, ForceMode2D.Impulse);
        rb2.AddTorque(splitRotateForce, ForceMode2D.Impulse);
        StartCoroutine(FadeAway(sr1));
        StartCoroutine(FadeAway(sr2));

        Destroy(gameObject);

        Destroy(arrowHalves, destroyTime);
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
