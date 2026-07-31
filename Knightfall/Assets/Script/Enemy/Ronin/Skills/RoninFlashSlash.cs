using System;
using System.Collections;
using Unity.AI.Assistant.Agents;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineInternal;

public class RoninFlashSlash : MonoBehaviour
{
    [SerializeField] private GameObject flashSlashVisual;
    private RoninManager manager;

    private RoninHealth health;
    private RoninSFX roninSFX;
    private RoninAnimator roninAnimator;
    private RoninAction roninAction;
    private GameObject player;
    //private Coroutine attackAnimationCoroutine;

    public GameObject attackZone;
    public float attackDuration = 1f;
    public int Damage;
    public float knockbackForce;
    public float knockbackDuration;

    public float flashSpeed = 5f;
    public float flashSlashRange = 5f;
    public float flashSlashWidth = 1f;

    public Transform attackTransform;

    public float attackCooldown = 5f;
    public float attackCoolDownTimer;

    private GameObject attackInstance;
    private Coroutine attackCoroutine;

    [SerializeField] private float followPlayerDuration = 0.5f;
    [SerializeField] private float delayAfterFollowDuration = 0.5f;

    [SerializeField] private float FollowUpFollowPlayerDuration = 0.5f;
    [SerializeField] private float FollowUpDelayAfterFollowDuration = 0.5f;
    private void Start()
    {
        health = GetComponent<RoninHealth>();
        manager = GetComponent<RoninManager>();
        roninSFX = GetComponent<RoninSFX>();
        roninAnimator = GetComponent<RoninAnimator>();
        roninAction = GetComponent<RoninAction>();
        player = GameManagerScript.Instance.player;
        if (player == null) GameManagerScript.OnPlayerReady += RegisterPlayer;
    }
    private void Update()
    {
        if (attackCoolDownTimer > 0)
        {
            attackCoolDownTimer -= Time.deltaTime;
            attackCoolDownTimer = Mathf.Clamp(attackCoolDownTimer, 0, attackCooldown);
        }
    }

    public Coroutine Attack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        return attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    public Coroutine AttackFollowUp()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        return attackCoroutine = StartCoroutine(AttackFollowUpCoroutine());
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


    private void RegisterPlayer(GameObject player)
    {
        this.player = player;
    }


    private IEnumerator AttackCoroutine()
    {
        try
        {
            bool playerIsToTheRight = player.transform.position.x > transform.position.x;
            Vector3 localScale = transform.localScale;
            localScale.x = playerIsToTheRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
            transform.localScale = localScale;

            roninAction.ChangeAnimationState("RoninUnsheathe1");
            if (attackInstance != null)
            {
                Destroy(attackInstance);
            }
            //Vector3 attackPos = new Vector3(transform.position.x, transform.position.y * 4f, transform.position.z);

            attackInstance = Instantiate(attackZone, transform.position, Quaternion.identity, attackTransform);
            attackInstance.transform.localScale = new Vector3(flashSlashWidth, flashSlashRange, flashSlashWidth);
            attackInstance.transform.position += new Vector3(0f, (attackInstance.transform.localScale.y / 2 + flashSlashRange * 0.15f), 0f);

            // Later on, we'll be reducing the attack length if it detects a wall, so we'll store the original length
            Vector3 originalStart = attackInstance.transform.position - attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
            Vector3 originalEnd = attackInstance.transform.position + attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
            float originalLength = Vector3.Distance(originalStart, originalEnd);

            // Aim attack at player.
            Vector3 playerPos = player.transform.position;

            Vector3 attackDir = (transform.position - playerPos).normalized;

            float rotateZ = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
            attackInstance.transform.RotateAround(transform.position, Vector3.forward, rotateZ + 90);

            FlashSlashAttackZoneScript attackZoneScript = attackInstance.GetComponent<FlashSlashAttackZoneScript>();
            attackZoneScript.damage = Damage;
            attackZoneScript.knockbackForce = knockbackForce;
            attackZoneScript.knockbackDuration = knockbackDuration;
            attackZoneScript.roninSFX = roninSFX;


            // Keep aiming at player for a specified amount of time + Delay for a while  
            float followPlayerTimer = 0f;
            while (followPlayerTimer <= followPlayerDuration)
            {
                followPlayerTimer += Time.deltaTime;

                Vector3 playerPosFollow = player.transform.position;
                if (playerPosFollow != playerPos)
                {
                    Vector3 attackDirFollow = (transform.position - playerPosFollow).normalized;
                    float rotateZFollow = Mathf.Atan2(attackDirFollow.y, attackDirFollow.x) * Mathf.Rad2Deg;
                    float rotationDifference = rotateZFollow - rotateZ;
                    attackInstance.transform.RotateAround(transform.position, Vector3.forward, rotationDifference);
                    rotateZ = rotateZFollow;
                    playerPos = playerPosFollow;
                }
                // Check if the hitbox went through a wall
                Vector3 attackDirection = attackInstance.transform.up;
                Vector3 raycastStart = transform.position;
                float raycastDistance = originalLength;
                RaycastHit2D hit = Physics2D.Raycast(raycastStart, attackDirection, raycastDistance, LayerMask.GetMask("Wall"));

                if (hit.collider != null && hit.collider.CompareTag("Wall"))
                {
                    float distanceToWall = hit.distance;

                    //Debug.DrawRay(hit.point, Vector3.right * 0.1f, Color.green, 0.1f);
                    //Debug.DrawRay(hit.point, Vector3.up * 0.1f, Color.green, 0.1f);

                    attackInstance.transform.localScale = new Vector3(flashSlashWidth, distanceToWall, flashSlashWidth);
                    attackInstance.transform.position = transform.position + (attackDirection * distanceToWall / 2);
                    //Debug.DrawRay(raycastStart, attackDirection * distanceToWall, Color.red);
                }
                else
                {
                    attackInstance.transform.localScale = new Vector3(flashSlashWidth, raycastDistance, flashSlashWidth);
                    attackInstance.transform.position = transform.position + (attackDirection * raycastDistance / 2);
                    //Debug.DrawRay(raycastStart, attackDirection * raycastDistance, Color.red);
                }

                yield return null;
            }

            roninAction.ChangeAnimationState("RoninFlashSlashWarning");
            roninSFX.playAttackWarning(2);
            roninAnimator.canChangeDirection = false;
            yield return new WaitForSeconds(delayAfterFollowDuration);

            // Detach the attackInstance + Move Enemy till the end of the hitbox over a time.
            attackInstance.transform.parent = null;
            Vector3 hitboxEnd = attackInstance.transform.position + attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
            // Start Attacking

            // play sfx
            roninSFX.playFlashSlash();
            roninSFX.playUnsheathe();

            roninAction.ChangeAnimationState("RoninAttack1Quick");

            // ignore collider so ronin does not send the player across the map

            Collider2D enemyBodyCollider = GetComponent<Collider2D>();

            Collider2D playerBodyCollider = player.GetComponent<Collider2D>();

            if (enemyBodyCollider != null && playerBodyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, true);
            }

            while (Vector3.Distance(transform.position, hitboxEnd) > 2.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, hitboxEnd, flashSpeed * Time.deltaTime);
                yield return null;
            }

            // turn on colliders again
            if (enemyBodyCollider != null && playerBodyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, false);
            }

            //attackAnimationCoroutine = StartCoroutine(playAttackAnimation(0.28f));


            // After ( Or mid way) the enemy reaches its destination, enable hitbox to deal damage.
            attackZoneScript.EnableDamage();
            attackZoneScript.showHitbox = false;
            SpawnFlashSlashEffect(attackInstance.transform.position, attackInstance.transform.rotation, null, attackInstance.transform.lossyScale);
            yield return new WaitForSeconds(attackDuration);
            attackZoneScript.DisableDamage();
            attackCoolDownTimer = attackCooldown;
            yield return new WaitUntil(() => roninAnimator.IsAnimationFinished("RoninAttack1Quick"));

            if (manager.IsInAbsoluteOffenseState) yield return StartCoroutine(AttackFollowUpCoroutine());
            else
            {
                roninAction.ChangeAnimationState("RoninSheathe");
                yield return new WaitUntil(() => roninAnimator.IsAnimationFinished("RoninSheathe"));
            }
            //if(attackAnimationCoroutine != null)
            //{
            //    StopCoroutine(attackAnimationCoroutine);
            //    attackAnimationCoroutine = null;
            //}
        }
        finally
        {
            roninAction.ChangeAnimationState("RoninIdle");
            if (attackInstance)
            {
                Destroy(attackInstance);
            }
        }
    }

    private IEnumerator AttackFollowUpCoroutine()
    {
        try
        {
            bool playerIsToTheRight = player.transform.position.x > transform.position.x;
            Vector3 localScale = transform.localScale;
            localScale.x = playerIsToTheRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
            transform.localScale = localScale;

            roninAction.ChangeAnimationState("RoninRetributionSlashWarning");
            roninSFX.playAttackWarning(3);
            if (attackInstance != null)
            {
                Destroy(attackInstance);
            }
            //Vector3 attackPos = new Vector3(transform.position.x, transform.position.y * 4f, transform.position.z);

            attackInstance = Instantiate(attackZone, transform.position, Quaternion.identity, attackTransform);
            attackInstance.transform.localScale = new Vector3(flashSlashWidth, flashSlashRange, flashSlashWidth);
            attackInstance.transform.position += new Vector3(0f, (attackInstance.transform.localScale.y / 2 + flashSlashRange * 0.15f), 0f);

            // Later on, we'll be reducing the attack length if it detects a wall, so we'll store the original length
            Vector3 originalStart = attackInstance.transform.position - attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
            Vector3 originalEnd = attackInstance.transform.position + attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
            float originalLength = Vector3.Distance(originalStart, originalEnd);

            // Aim attack at player.
            Vector3 playerPos = player.transform.position;

            Vector3 attackDir = (transform.position - playerPos).normalized;

            float rotateZ = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
            attackInstance.transform.RotateAround(transform.position, Vector3.forward, rotateZ + 90);

            FlashSlashAttackZoneScript attackZoneScript = attackInstance.GetComponent<FlashSlashAttackZoneScript>();
            attackZoneScript.damage = Damage;
            attackZoneScript.knockbackForce = knockbackForce;
            attackZoneScript.knockbackDuration = knockbackDuration;
            attackZoneScript.roninSFX = roninSFX;


            // Keep aiming at player for a specified amount of time + Delay for a while  
            float followPlayerTimer = 0f;
            while (followPlayerTimer <= FollowUpFollowPlayerDuration)
            {
                followPlayerTimer += Time.deltaTime;

                Vector3 playerPosFollow = player.transform.position;
                if (playerPosFollow != playerPos)
                {
                    Vector3 attackDirFollow = (transform.position - playerPosFollow).normalized;
                    float rotateZFollow = Mathf.Atan2(attackDirFollow.y, attackDirFollow.x) * Mathf.Rad2Deg;
                    float rotationDifference = rotateZFollow - rotateZ;
                    attackInstance.transform.RotateAround(transform.position, Vector3.forward, rotationDifference);
                    rotateZ = rotateZFollow;
                    playerPos = playerPosFollow;
                }
                // Check if the hitbox went through a wall
                Vector3 attackDirection = attackInstance.transform.up;
                Vector3 raycastStart = transform.position;
                float raycastDistance = originalLength;
                RaycastHit2D hit = Physics2D.Raycast(raycastStart, attackDirection, raycastDistance, LayerMask.GetMask("Wall"));

                if (hit.collider != null && hit.collider.CompareTag("Wall"))
                {
                    float distanceToWall = hit.distance;

                    //Debug.DrawRay(hit.point, Vector3.right * 0.1f, Color.green, 0.1f);
                    //Debug.DrawRay(hit.point, Vector3.up * 0.1f, Color.green, 0.1f);

                    attackInstance.transform.localScale = new Vector3(flashSlashWidth, distanceToWall, flashSlashWidth);
                    attackInstance.transform.position = transform.position + (attackDirection * distanceToWall / 2);
                    //Debug.DrawRay(raycastStart, attackDirection * distanceToWall, Color.red);
                }
                else
                {
                    attackInstance.transform.localScale = new Vector3(flashSlashWidth, raycastDistance, flashSlashWidth);
                    attackInstance.transform.position = transform.position + (attackDirection * raycastDistance / 2);
                    //Debug.DrawRay(raycastStart, attackDirection * raycastDistance, Color.red);
                }

                yield return null;
            }
            roninAnimator.canChangeDirection = false;
            yield return new WaitForSeconds(FollowUpDelayAfterFollowDuration);

            // Detach the attackInstance + Move Enemy till the end of the hitbox over a time.
            attackInstance.transform.parent = null;
            Vector3 hitboxEnd = attackInstance.transform.position + attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
            // Start Attacking

            // play sfx
            roninSFX.playFlashSlash();

            roninAction.ChangeAnimationState("RoninAttack2");

            // ignore collider so ronin does not send the player across the map

            Collider2D enemyBodyCollider = GetComponent<Collider2D>();

            Collider2D playerBodyCollider = player.GetComponent<Collider2D>();

            if (enemyBodyCollider != null && playerBodyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, true);
            }

            while (Vector3.Distance(transform.position, hitboxEnd) > 2.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, hitboxEnd, flashSpeed * Time.deltaTime);
                yield return null;
            }

            // turn on colliders again
            if (enemyBodyCollider != null && playerBodyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyBodyCollider, playerBodyCollider, false);
            }

            //attackAnimationCoroutine = StartCoroutine(playAttackAnimation(0.28f));


            // After ( Or mid way) the enemy reaches its destination, enable hitbox to deal damage.
            attackZoneScript.EnableDamage();
            attackZoneScript.showHitbox = false;
            SpawnFlashSlashEffect(attackInstance.transform.position, attackInstance.transform.rotation, null, attackInstance.transform.lossyScale);
            yield return new WaitForSeconds(attackDuration);
            attackZoneScript.DisableDamage();
            attackCoolDownTimer = attackCooldown;
            yield return new WaitUntil(() => roninAnimator.IsAnimationFinished("RoninAttack2"));
            //if(attackAnimationCoroutine != null)
            //{
            //    StopCoroutine(attackAnimationCoroutine);
            //    attackAnimationCoroutine = null;
            //}
        }
        finally
        {
            roninAction.ChangeAnimationState("RoninIdle");
            if (attackInstance)
            {
                Destroy(attackInstance);
            }
        }
    }

    private void SpawnFlashSlashEffect(Vector3 position, Quaternion rotation, Transform parent, Vector3 scale)
    {
        StartCoroutine(SpawnFlashSlashEffectCorountine( position,  rotation,  parent,  scale));
    }
    private IEnumerator SpawnFlashSlashEffectCorountine(Vector3 position, Quaternion rotation, Transform parent, Vector3 scale)
    {
        GameObject effect = Instantiate(flashSlashVisual, position, rotation, parent);

        effect.transform.localScale = new Vector3(
        Mathf.Abs(scale.x),
        Mathf.Abs(scale.y),
        Mathf.Abs(scale.z)
    );

        yield return new WaitForSeconds(2f);

        Destroy(effect);
    }
}