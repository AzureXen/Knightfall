using Assets.Script.Enemy.FlyingEye;
using System.Collections;
using UnityEngine;

public class FlyingEyeShoot : MonoBehaviour
{
    private FlyingEyeAnimator animator;
    private FlyingEyeManager manager;
    private GameObject player;

    private Coroutine attackCoroutine;

    private Rigidbody2D rb;

    [SerializeField] private GameObject bulletPrefab;

    [Header("Bullet Settings")]
    [SerializeField] private float bulletSpeed = 5;
    [SerializeField] private int damage = 5;
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float knockbackForce = 2f;
    [SerializeField] private float shootDelay = 0.5f;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilForce = 3f;
    [SerializeField] private float recoilDuration = 0.15f;
    private void Start()
    {
        manager = GetComponent<FlyingEyeManager>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<FlyingEyeAnimator>();
        player = GameManagerScript.Instance.player;
        if (player == null) GameManagerScript.OnPlayerReady += RegisterPlayer;

    }

    private void RegisterPlayer(GameObject player)
    {
        this.player = player;
    }
    public Coroutine Attack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        return attackCoroutine = StartCoroutine(AttackCoroutine());
    }
    public void StopAttackImmediate()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }


    private IEnumerator AttackCoroutine()
    {
        try
        {
            animator.PlayAnimation(FlyingEyeAnimator.ATTACK_3_WARNING);
            yield return new WaitUntil(() => animator.IsAnimationFinished(FlyingEyeAnimator.ATTACK_3_WARNING));

            yield return new WaitForSeconds(shootDelay);

            animator.PlayAnimation(FlyingEyeAnimator.ATTACK_3_SHOOT);

            yield return new WaitUntil(() => animator.IsAnimationFinished(FlyingEyeAnimator.ATTACK_3_SHOOT));

            yield return new WaitForSeconds(0.5f);
        }
        finally
        {
            animator.PlayAnimation(FlyingEyeAnimator.IDLE);
        }
    }

    public void ShootBulletAtPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.transform.position - transform.position).normalized;
        GameObject bulletInstance = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        FlyingEyeBullet bulletScript = bulletInstance.GetComponent<FlyingEyeBullet>();
        bulletScript.Init(direction, bulletSpeed, damage, knockbackForce, knockbackDuration);

        manager.TakeKnockBack(player.transform.position, recoilForce, recoilDuration);
    }
}
