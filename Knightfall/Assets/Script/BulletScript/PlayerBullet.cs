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

    protected override void Start()
    {

        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 bulletDirection = mousePos - transform.position;
        rb.linearVelocity = new Vector2(bulletDirection.x, bulletDirection.y).normalized * force;

        Vector3 bulletRotation = transform.position - mousePos;
        float rotation = Mathf.Atan2(bulletRotation.y, bulletRotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotation + 90);
        Destroy(gameObject, despawnTime);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Boolean successHit = false; 
        if (!disabledDamage && collision.CompareTag("Enemy"))
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

        // get current bullet velocity as the base direction
        Vector2 currentVelocity = rb.linearVelocity;
        float currentSpeed = currentVelocity.magnitude;
        Vector2 currentDirection = currentVelocity.normalized;

        // perpendicular to the bullet's travel direction
        Vector2 perpendicular = Vector2.Perpendicular(currentDirection);

        // spread angle — how far each half can deviate from the original direction
        float spreadAngle = UnityEngine.Random.Range(15f, 45f);

        // half 1 spreads to the left of travel direction
        Vector2 dir1 = RotateVector(currentDirection, spreadAngle);
        // half 2 spreads to the right of travel direction  
        Vector2 dir2 = RotateVector(currentDirection, -spreadAngle);

        // halves travel at a fraction of the original speed, slightly randomized
        float speed1 = currentSpeed * UnityEngine.Random.Range(0.3f, 0.6f);
        float speed2 = currentSpeed * UnityEngine.Random.Range(0.3f, 0.6f);

        rb1.linearVelocity = dir1 * speed1;
        rb2.linearVelocity = dir2 * speed2;

        // spin opposite directions, intensity based on original speed
        float torque = currentSpeed * splitRotateForce * 0.1f;
        rb1.AddTorque(-torque, ForceMode2D.Impulse);
        rb2.AddTorque(torque, ForceMode2D.Impulse);

        StartCoroutine(FadeAway(sr1));
        StartCoroutine(FadeAway(sr2));
        Destroy(gameObject);
        Destroy(arrowHalves, destroyTime);
    }

    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
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
