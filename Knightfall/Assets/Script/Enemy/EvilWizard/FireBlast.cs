using UnityEngine;

public class FireBlast : BulletScript
{
    [SerializeField] private float delayBeforeAppear = 0.5f; 
    [SerializeField] private float duration = 2f; 
    [SerializeField] private int fireDamage = 20; 
    private SpriteRenderer sr;
    private Collider2D col;

    protected  override void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        sr.enabled = false;
        col.enabled = false;

        Invoke(nameof(Appear), delayBeforeAppear);

        Destroy(gameObject, delayBeforeAppear + duration);
    }

    private void Appear()
    {
        sr.enabled = true;
        col.enabled = true;
    }

    protected  override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EntityManager player = collision.GetComponent<EntityManager>();
            if (player != null)
            {
                player.TakeRangedHit(fireDamage, transform.position, 0, 0);
            }
        }
    }
}
