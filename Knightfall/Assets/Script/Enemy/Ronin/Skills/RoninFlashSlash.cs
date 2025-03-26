using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineInternal;

public class RoninFlashSlash : MonoBehaviour
{
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

    public Transform attackTransform;

    public float attackCooldown = 5f;
    public float attackCoolDownTimer;

    private GameObject attackInstance;
    private Coroutine attackCoroutine;

    [SerializeField] private float followPlayerDuration = 0.5f;
    [SerializeField] private float delayAfterFollowDuration = 0.5f;

    public Boolean isAttacking = false;
    private void Start()
    {
        roninSFX = GetComponent<RoninSFX>();
        roninAnimator = GetComponent<RoninAnimator>();
        roninAction = GetComponent<RoninAction>();
        StartCoroutine(FindPlayer());
    }
    private void Update()
    {
        if(attackCoolDownTimer > 0)
        {
            attackCoolDownTimer -= Time.deltaTime;
            attackCoolDownTimer = Mathf.Clamp(attackCoolDownTimer, 0, attackCooldown);
        }
    }

    public void Attack()
    {
        if(attackCoroutine != null) { 
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }
    public void StopAttackImmediate()
    {
        if (attackCoroutine != null) {
            StopCoroutine(attackCoroutine);
        }
        if (attackInstance)
        {
            Destroy(attackInstance);
        }
    }


    private IEnumerator FindPlayer()
    {
        while(player == null)
        {
            yield return new WaitForSeconds(0.2f);
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }


    private IEnumerator AttackCoroutine()
    {
        try
        {
            isAttacking = true;
            roninAnimator.canChangeDirection = true;
            roninAction.ChangeAnimationState("RoninUnsheathe1");
            if (attackInstance != null)
            {
                Destroy(attackInstance);
            }
            //Vector3 attackPos = new Vector3(transform.position.x, transform.position.y * 4f, transform.position.z);

            attackInstance = Instantiate(attackZone, transform.position, Quaternion.identity, attackTransform);
            attackInstance.transform.localScale = new Vector3(1, flashSlashRange, 1);
            attackInstance.transform.position += new Vector3(0f, (attackInstance.transform.localScale.y / 2 + flashSlashRange * 0.15f), 0f);

            // Later on, we'll be reducing the attack length if it detects a wall, so we'll store the original length
            Vector3 originalStart = attackInstance.transform.position - attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
            Vector3 originalEnd = attackInstance.transform.position + attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
            float originalLength = Vector3.Distance(originalStart, originalEnd);

            // Aim attack at player.
            Vector3 playerPos = player.transform.position;
            Vector3 attackDir = (attackTransform.position - playerPos).normalized;
            float rotateZ = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
            attackInstance.transform.RotateAround(attackTransform.position, Vector3.forward, rotateZ + 90);

            AttackZoneScript attackZoneScript = attackInstance.GetComponent<AttackZoneScript>();
            attackZoneScript.damage = Damage;
            attackZoneScript.knockbackForce = knockbackForce;
            attackZoneScript.knockbackDuration = knockbackDuration;

            attackZoneScript.attackZoneType = AttackZoneScript.AttackZoneType.FlashSlash;
            attackZoneScript.roninSFX = roninSFX;


            // Keep aiming at player for a specified amount of time + Delay for a while  
            float followPlayerTimer = 0f;
            while (followPlayerTimer <= followPlayerDuration)
            {
                followPlayerTimer += Time.deltaTime;

                Vector3 playerPosFollow = player.transform.position;
                if (playerPosFollow != playerPos)
                {
                    Vector3 attackDirFollow = (attackTransform.position - playerPosFollow).normalized;
                    float rotateZFollow = Mathf.Atan2(attackDirFollow.y, attackDirFollow.x) * Mathf.Rad2Deg;
                    float rotationDifference = rotateZFollow - rotateZ;
                    attackInstance.transform.RotateAround(attackTransform.position, Vector3.forward, rotationDifference);
                    rotateZ = rotateZFollow;
                    playerPos = playerPosFollow;
                }
                // Check if the hitbox went through a wall
                Vector3 attackDirection = attackInstance.transform.up;
                Vector3 raycastStart = attackTransform.position; 
                float raycastDistance = originalLength;
                RaycastHit2D hit = Physics2D.Raycast(raycastStart, attackDirection, raycastDistance, LayerMask.GetMask("Wall"));

                if (hit.collider != null && hit.collider.CompareTag("Wall"))
                {
                    float distanceToWall = hit.distance;

                    Debug.DrawRay(hit.point, Vector3.right * 0.1f, Color.green, 0.1f);
                    Debug.DrawRay(hit.point, Vector3.up * 0.1f, Color.green, 0.1f);

                    attackInstance.transform.localScale = new Vector3(1, distanceToWall, 1);
                    attackInstance.transform.position = attackTransform.position + (attackDirection * distanceToWall / 2);
                    Debug.DrawRay(raycastStart, attackDirection * distanceToWall, Color.red);
                }
                else
                {
                    attackInstance.transform.localScale = new Vector3(1, raycastDistance, 1);
                    attackInstance.transform.position = attackTransform.position + (attackDirection * raycastDistance / 2);
                    Debug.DrawRay(raycastStart, attackDirection * raycastDistance, Color.red);
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

            roninAction.ChangeAnimationState("RoninAttack2");
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
            while (Vector3.Distance(transform.position, hitboxEnd) > 2.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, hitboxEnd, flashSpeed * Time.deltaTime);
                yield return null;
            }
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

            //attackAnimationCoroutine = StartCoroutine(playAttackAnimation(0.28f));


            // After ( Or mid way) the enemy reaches its destination, enable hitbox to deal damage.
            attackZoneScript.EnableDamage();
            yield return new WaitForSeconds(attackDuration);
            attackZoneScript.DisableDamage();
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
            isAttacking = false;
        }
    }





    //private IEnumerator AttackCoroutine()
    //{
    //    try
    //    {
    //        roninAnimator.canChangeDirection = true;
    //        roninAction.ChangeAnimationState("RoninUnsheathe1");
    //        if (attackInstance != null)
    //        {
    //            Destroy(attackInstance);
    //        }
    //        //Vector3 attackPos = new Vector3(transform.position.x, transform.position.y * 4f, transform.position.z);

    //        attackInstance = Instantiate(attackZone, transform.position, Quaternion.identity, attackTransform);
    //        attackInstance.transform.localScale = new Vector3(1, flashSlashRange, 1);
    //        attackInstance.transform.position += new Vector3(0f, (attackInstance.transform.localScale.y / 2 + flashSlashRange * 0.15f), 0f);

    //        // Later on, we'll be reducing the attack length if it detects a wall, so we'll store the original length
    //        Vector3 originalStart = attackInstance.transform.position - attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
    //        Vector3 originalEnd = attackInstance.transform.position + attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
    //        float originalLength = Vector3.Distance(originalStart, originalEnd);

    //        // Aim attack at player.
    //        Vector3 playerPos = player.transform.position;
    //        Vector3 attackDir = (attackTransform.position - playerPos).normalized;
    //        float rotateZ = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
    //        attackInstance.transform.RotateAround(attackTransform.position, Vector3.forward, rotateZ + 90);

    //        AttackZoneScript attackZoneScript = attackInstance.GetComponent<AttackZoneScript>();
    //        attackZoneScript.damage = Damage;
    //        attackZoneScript.knockbackForce = knockbackForce;
    //        attackZoneScript.knockbackDuration = knockbackDuration;

    //        attackZoneScript.attackZoneType = AttackZoneScript.AttackZoneType.FlashSlash;
    //        attackZoneScript.roninSFX = roninSFX;


    //        // Keep aiming at player for a specified amount of time + Delay for a while  
    //        float followPlayerTimer = 0f;
    //        while (followPlayerTimer <= followPlayerDuration)
    //        {
    //            followPlayerTimer += Time.deltaTime;

    //            Vector3 playerPosFollow = player.transform.position;
    //            if (playerPosFollow != playerPos)
    //            {
    //                Vector3 attackDirFollow = (attackTransform.position - playerPosFollow).normalized;
    //                float rotateZFollow = Mathf.Atan2(attackDirFollow.y, attackDirFollow.x) * Mathf.Rad2Deg;
    //                float rotationDifference = rotateZFollow - rotateZ;
    //                attackInstance.transform.RotateAround(attackTransform.position, Vector3.forward, rotationDifference);
    //                rotateZ = rotateZFollow;
    //                playerPos = playerPosFollow;
    //            }
    //            // Check if the hitbox went through a wall
    //            //Vector3 raycastStart = attackInstance.transform.position - attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
    //            Vector3 raycastStart = attackInstance.transform.position - attackInstance.transform.up * (attackInstance.transform.localScale.y / 2f) - attackInstance.transform.up * 2f;

    //            // The end of the hitbox
    //            Vector3 raycastEnd = attackInstance.transform.position + attackInstance.transform.up * (attackInstance.transform.localScale.y / 2f) + attackInstance.transform.up * 2f;
    //            float raycastDistance = originalLength;
    //            Vector3 raycastDirection = (raycastEnd - raycastStart).normalized;
    //            RaycastHit2D hit = Physics2D.Raycast(raycastStart, raycastDirection, raycastDistance * 1.45f, LayerMask.GetMask("Wall"));

    //            if (hit.collider != null && hit.collider.CompareTag("Wall"))
    //            {
    //                //Debug.Log("Raycast hit: " + hit.collider.gameObject.name + "| On layer: " + hit.collider.gameObject.layer + "| With Tag:" +hit.collider.tag);
    //                raycastEnd = hit.point;
    //                raycastDistance = Vector3.Distance(raycastEnd, raycastStart);

    //                // Draw a small cross at the hit point
    //                Debug.DrawRay(hit.point, Vector3.right * 0.1f, Color.green, 0.1f);
    //                Debug.DrawRay(hit.point, Vector3.up * 0.1f, Color.green, 0.1f);
    //                attackInstance.transform.localScale = new Vector3(1, raycastDistance * 0.5f, 1);
    //                // Correct positioning so that the hitbox starts at Ronin
    //                attackInstance.transform.position = transform.position + attackInstance.transform.up * ((raycastDistance * 0.5f) / 1.5f);
    //                Debug.DrawRay(raycastStart, raycastDirection * raycastDistance, Color.red);
    //            }
    //            else
    //            {
    //                attackInstance.transform.localScale = new Vector3(1, raycastDistance, 1);
    //                // Correct positioning so that the hitbox starts at Ronin
    //                attackInstance.transform.position = transform.position + attackInstance.transform.up * (raycastDistance / 1.5f);
    //                Debug.DrawRay(raycastStart, raycastDirection * raycastDistance * 1.45f, Color.red);
    //            }

    //            yield return null;
    //        }

    //        roninAction.ChangeAnimationState("RoninUnsheathe2");
    //        roninAnimator.canChangeDirection = false;
    //        yield return new WaitForSeconds(delayAfterFollowDuration);

    //        // Detach the attackInstance + Move Enemy till the end of the hitbox over a time.
    //        attackInstance.transform.parent = null;
    //        Vector3 hitboxEnd = attackInstance.transform.position + attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
    //        // Start Attacking

    //        // play sfx
    //        roninSFX.playFlashSlash();

    //        roninAction.ChangeAnimationState("RoninAttack2");
    //        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
    //        while (Vector3.Distance(transform.position, hitboxEnd) > 0.1f)
    //        {
    //            transform.position = Vector3.MoveTowards(transform.position, hitboxEnd, flashSpeed * Time.deltaTime);
    //            yield return null;
    //        }
    //        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

    //        //attackAnimationCoroutine = StartCoroutine(playAttackAnimation(0.28f));


    //        // After ( Or mid way) the enemy reaches its destination, enable hitbox to deal damage.
    //        attackZoneScript.EnableDamage();
    //        yield return new WaitForSeconds(attackDuration);
    //        attackZoneScript.DisableDamage();
    //        //if(attackAnimationCoroutine != null)
    //        //{
    //        //    StopCoroutine(attackAnimationCoroutine);
    //        //    attackAnimationCoroutine = null;
    //        //}
    //    }
    //    finally
    //    {
    //        roninAction.ChangeAnimationState("RoninIdle");
    //        if (attackInstance)
    //        {
    //            Destroy(attackInstance);
    //        }
    //    }
    //}

    //IEnumerator playAttackAnimation(float duration)
    //{
    //    try
    //    {
    //        roninAction.ChangeAnimationState("RoninAttack2");
    //        yield return new WaitForSeconds(duration);
    //    }
    //    finally
    //    {
    //        roninAction.ChangeAnimationState("RoninUnsheathe2");
    //        attackAnimationCoroutine = null;
    //    }
    //}
}
