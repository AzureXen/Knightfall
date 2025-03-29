using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RoninSeverance : MonoBehaviour
{
    private RoninAction roninAction;
    private RoninSFX roninSFX;
    private RoninAnimator roninAnimator;

    public GameObject attackZone;
    public GameObject SeveranceVisual;

    [SerializeField] private float severanceSize;

    public float attackDuration = 1f;
    public int severanceDamage;
    public float knockbackForce;
    public float knockbackDuration;

    public float followPlayerDuration;
    public float delayAfterFollowDuration;

    public float attackCooldown = 5f;
    public float attackCoolDownTimer;

    public Transform attackTransform;
    private GameObject player;

    private GameObject attackInstance;
    private GameObject visualInstance;
    private Coroutine attackCoroutine;

    public Boolean isAttacking = false;


    private void Start()
    {
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
            roninAnimator.canChangeDirection = true;
            isAttacking = true;
            if (attackInstance != null)
            {
                Destroy(attackInstance);
            }

            if (visualInstance !=null)
            {
                Destroy(visualInstance);
            }
            if (player == null)
            {
                Debug.Log("Cannot find player.");
                isAttacking = false;
                yield break;
            }
            attackInstance = Instantiate(attackZone, player.transform.position, Quaternion.identity);
            attackInstance.transform.localScale = new Vector3(severanceSize, severanceSize, severanceSize);
            
            AttackZoneScript attackZoneScript = attackInstance.GetComponent<AttackZoneScript>();
            attackZoneScript.damage = severanceDamage;
            attackZoneScript.knockbackForce = knockbackForce;
            attackZoneScript.knockbackDuration = knockbackDuration;

            // Positions at player for an amount of time
            float followPlayerTimer = 0f;
            while (followPlayerTimer <= followPlayerDuration)
            {
                followPlayerTimer += Time.deltaTime;

                Vector3 playerPosFollow = player.transform.position;
                attackInstance.transform.position = playerPosFollow;
                yield return null;
            }

            // Delays for while
            roninSFX.playAttackWarning(2);
            roninAction.ChangeAnimationState("RoninSeveranceWarning");
            yield return new WaitForSeconds(delayAfterFollowDuration);
            // Attack
            visualInstance = Instantiate(SeveranceVisual, attackInstance.transform.position, Quaternion.identity);
            visualInstance.transform.localScale = new Vector3(severanceSize * 0.15f, severanceSize * 0.15f, severanceSize * 0.15f);
            attackZoneScript.showHitbox = false;

            roninAction.ChangeAnimationState("RoninSeverance");
            roninSFX.playSeverance();
            attackZoneScript.EnableDamage();
            yield return new WaitForSeconds(attackDuration);
            attackZoneScript.DisableDamage();
            visualInstance.SetActive(false);
            Destroy(attackInstance);

            // relax bruh
            yield return new WaitForSeconds(0.5f);
        }
        finally
        {
            roninAnimator.canChangeDirection = false;
            roninAction.ChangeAnimationState("RoninIdle");
            if (attackInstance)
            {
                Destroy(attackInstance);
            }
            if (visualInstance)
            {
                Destroy(visualInstance);
            }
            isAttacking = false;
            attackCoolDownTimer = attackCooldown;
        }
    }

}
