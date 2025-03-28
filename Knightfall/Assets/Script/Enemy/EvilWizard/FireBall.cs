using UnityEngine;

public class FireBall : BulletScript
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int monsterDmg = 15;
    public int monsterKnockbackForce = 2;
    public float touchKnockbackDuration = 1f;
    private Vector2 direction;

    protected override void Start()
    {
        damage = monsterDmg;
        knockbackForce = monsterKnockbackForce;
        if(gameObject != null)
        {
            Destroy(gameObject, 5f);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EntityManager player = collision.GetComponent<EntityManager>();
            if (player != null)
            {
                player.TakeRangedHit(damage, transform.position, knockbackForce, touchKnockbackDuration, this);
            }
        }
    }
}
