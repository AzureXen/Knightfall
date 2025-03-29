using System;
using System.Collections;
using UnityEngine;

public class PlayerBullet : BulletScript
{
    // For Destroy Bullet
    public GameObject arrowHalvesPrefab;
    public float splitForce = 2f;
    public float splitRotateForce = 25f;
    public float destroyTime = 2f;
    public float fadeDuration = 1f;
    public PlayerSFX playerSFX;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Boolean successHit = false; 
        if (collision.CompareTag("Enemy"))
        {
            EntityManager enemyManager = collision.gameObject.GetComponent<EntityManager>();
            if (enemyManager != null)
            {
                successHit = enemyManager.TakeRangedHit(damage, transform.position, knockbackForce, knockbackDuration, this);
            }
            if (successHit && playerSFX!=null)
            {
                playerSFX.playArrowHit();
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
        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            sr.color = new Color(originColor.r, originColor.g, originColor.b, alpha);
            yield return null;
        }
        sr.color = new Color(originColor.r, originColor.g, originColor.b, 0f);
    }
}
