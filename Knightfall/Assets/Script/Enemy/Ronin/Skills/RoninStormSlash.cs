using System;
using UnityEngine;
using System.Collections;

// Judgement Cut End :)
public class RoninStormSlash : MonoBehaviour
{
    private RoninAction roninAction;
    private RoninSFX roninSFX;
    private RoninAnimator roninAnimator;
    private RoninManager roninManager;
    private Rigidbody2D rb;

    public GameObject stormSlashAttackZone;

    [SerializeField] private float stormSize;

    public float attackDuration = 1f;
    public int damage;
    public float knockbackForce;
    public float knockbackDuration;

    public float attackCooldown = 90f;
    public float attackCoolDownTimer;

    public Transform attackTransform;
    private GameObject player;

    private GameObject attackInstance;
    private Coroutine attackCoroutine;

    public Boolean isAttacking = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackCoolDownTimer = attackCooldown;
        roninManager = GetComponent<RoninManager>();
        roninAnimator = GetComponent<RoninAnimator>();
        roninSFX = GetComponent<RoninSFX>();
        roninAction = GetComponent<RoninAction>();
        StartCoroutine(FindPlayer());
    }
    private void Update()
    {
        if (attackCoolDownTimer > 0)
        {
            attackCoolDownTimer -= Time.deltaTime;
            attackCoolDownTimer = Mathf.Clamp(attackCoolDownTimer, 0, attackCooldown);
        }
    }

    public void Attack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }
    public void StopAttackImmediate()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        if (attackInstance)
        {
            Destroy(attackInstance);
        }
    }
    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            yield return new WaitForSeconds(0.2f);
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
    private IEnumerator AttackCoroutine()
    {
        try
        {
            if (player == null)
            {
                Debug.Log("Cannot find player.");
                isAttacking = false;
                yield break;
            }
            roninManager.canDamage = false;
            roninAnimator.canChangeDirection = true;
            isAttacking = true;
            if (attackInstance != null)
            {
                Destroy(attackInstance);
            }
            rb.bodyType = RigidbodyType2D.Kinematic;
            roninAction.ChangeAnimationState("RoninFlashStepDisappear");

            yield return roninAnimator.IsAnimationFinished("RoninFlashStepDisappear");

            yield return new WaitForSeconds(2f);

            rb.bodyType = RigidbodyType2D.Dynamic;

            attackInstance = Instantiate(stormSlashAttackZone, player.transform.position, Quaternion.identity);

            attackInstance.transform.localScale = new Vector3(stormSize, stormSize, stormSize);

            StormSlashAttackZoneScript attackZoneScript = attackInstance.GetComponent<StormSlashAttackZoneScript>();
            attackZoneScript.attackVisualSize = stormSize/10;
            attackZoneScript.damage = damage;
            attackZoneScript.knockbackForce = knockbackForce;
            attackZoneScript.knockbackDuration = knockbackDuration;


            // Delays for while
            roninSFX.playAttackWarning(2);
            roninAction.ChangeAnimationState("RoninSeveranceWarning");
            yield return new WaitForSeconds(1f);
            // Attack
            attackZoneScript.showHitbox = true;
            roninAction.ChangeAnimationState("RoninSeverance");
            roninSFX.playSeverance();
            attackZoneScript.EnableDamage();
            yield return new WaitForSeconds(attackDuration);
            attackZoneScript.DisableDamage();
            Destroy(attackInstance);

            // relax bruh
            yield return new WaitForSeconds(0.5f);
        }
        finally
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            roninManager.canDamage = true;
            roninAnimator.canChangeDirection = false;
            roninAction.ChangeAnimationState("RoninIdle");
            if (attackInstance)
            {
                Destroy(attackInstance);
            }
            isAttacking = false;
            attackCoolDownTimer = attackCooldown;
        }
    }
}
