using UnityEngine;

public class KeyDropOnDeath : MonoBehaviour
{
    public GameObject keyPrefab;         // Prefab chìa khóa
    [Range(0f, 1f)]
    public float dropChance = 0.33f;     // Tỉ lệ rớt key (ví dụ 33%)

    private Health health;
    private bool hasDropped = false;

    void Start()
    {
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (!hasDropped && health != null && health.health <= 0)
        {
            hasDropped = true;

            float rand = Random.value;
            if (rand <= dropChance && keyPrefab != null)
            {
                Instantiate(keyPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
