using System.Collections;
using UnityEngine;

public class NhatAttack : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab vi�n ??n
    public Transform firePoint; // ?i?m xu?t ph�t vi�n ??n
    public float bulletSpeed = 5f; // T?c ?? ??n
    public float fireRate = 1.5f; // Th?i gian gi?a m?i l?n b?n
    private float nextFireTime;

    private GameObject player;

    void Start()
    {
        StartCoroutine(FindPlayer());
    }

    void Update()
    {
        if (player != null && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (player.transform.position - firePoint.position).normalized;
                rb.linearVelocity = direction * bulletSpeed;
            }
        }
    }
}
