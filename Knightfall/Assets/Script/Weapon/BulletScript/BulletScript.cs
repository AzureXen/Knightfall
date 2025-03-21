using System;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;
    public float despawnTime = 1f;
    public int damage = 5;
    public float knockbackForce = 0f;
    public float knockbackDuration = 1.5f;
    protected virtual void Start()
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
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EntityManager enemyManager = collision.gameObject.GetComponent<EntityManager>();
            if (enemyManager != null)
            {
                enemyManager.TakeRangedHit(damage, transform.position, knockbackForce, knockbackDuration, this);
            }
        }
    }
    public virtual void DestroyBullet()
    {
        Destroy(gameObject);
    }
    public virtual void BreakBullet()
    {
        Destroy(gameObject);
    }
}
