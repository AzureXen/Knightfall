using System.Collections;
using UnityEngine;

public class EnemyManager : EntityManager
{
    [SerializeField] public bool touchDamageEnabled = true;
    public int touchDamage = 5;
    public int touchKnockbackForce = 5;
    public float touchKnockbackDuration = 1.5f;
    public GameObject Player { get; private set; }



    [SerializeField] protected float goldDrop = 5;

    public override void Start()
    {
        entityHealth = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        defaultColor = new Color(1, 1, 1, 1);


        takeDamageCooldown = 0.2f;
        flashDuration = 2.5f;

        if (entityHealth != null)
        {
            entityHealth.OnDeath += HandleDeathEvent;
        }
        Player = GameManagerScript.Instance.player;
        if(Player== null) GameManagerScript.OnPlayerReady += RegisterPlayer;
    }
    private void RegisterPlayer(GameObject player)
    {
        this.Player = player;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject target = collision.gameObject;
        if (target.CompareTag("Player") && touchDamageEnabled && isAlive)
        {
            EntityManager targetManager = target.GetComponent<EntityManager>();
            targetManager.TakeMeleeHit(touchDamage, transform.position, touchKnockbackForce, touchKnockbackDuration, this);
        }
    }

    // drop loot, play death sound, notify wave manager, etc.
    protected override void Die()
    {
        base.Die();

        Collider2D entityCollider = GetComponent<Collider2D>();
        if (entityCollider != null)
        {
            entityCollider.enabled = false;
        }

        sr.color = Color.black;
        if (GameManagerScript.Instance != null)
        {
            GameManagerScript.Instance.playerData.AddGold(goldDrop);
        }
        else Debug.Log("Enemy death: GameManagerInstance not found!");
    }

}