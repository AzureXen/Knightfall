using System;
using System.Collections;
using UnityEngine;

public class RoninFlashSlash : MonoBehaviour
{
    private GameObject player;

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
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
    public IEnumerator AttackCoroutine()
    {
        Debug.Log("Firing Attack");
        if (attackInstance != null)
        {
            Destroy(attackInstance);
        }
        //Vector3 attackPos = new Vector3(transform.position.x, transform.position.y * 4f, transform.position.z);

        attackInstance = Instantiate(attackZone, transform.position, Quaternion.identity, attackTransform);
        attackInstance.transform.localScale = new Vector3(1, flashSlashRange, 1);
        attackInstance.transform.position += new Vector3(0f, (attackInstance.transform.localScale.y / 2 + flashSlashRange*0.15f), 0f);

        // Aim attack at player.
        Vector3 playerPos = player.transform.position;
        Vector3 attackDir = (attackTransform.position - playerPos).normalized;
        float rotateZ = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        attackInstance.transform.RotateAround(attackTransform.position, Vector3.forward, rotateZ + 90);

        AttackZoneScript attackZoneScript = attackInstance.GetComponent<AttackZoneScript>();
        attackZoneScript.damage = Damage;
        attackZoneScript.knockbackForce = knockbackForce;
        attackZoneScript.knockbackDuration = knockbackDuration;

        // Detach the attackInstance + Move Enemy till the end of the hitbox over a time.
        attackInstance.transform.parent = null;
        
        // Delay for a while + keep aiming at player for a specified amount of time
        float followPlayerTimer = 0f;
        while (followPlayerTimer <= followPlayerDuration)
        {
            followPlayerTimer += Time.deltaTime;

            Vector3 playerPosFollow = player.transform.position;
            if(playerPosFollow != playerPos)
            {
                Vector3 attackDirFollow = (attackTransform.position - playerPosFollow).normalized;
                float rotateZFollow = Mathf.Atan2(attackDirFollow.y, attackDirFollow.x) * Mathf.Rad2Deg;
                float rotationDifference = rotateZFollow - rotateZ;
                attackInstance.transform.RotateAround(attackTransform.position, Vector3.forward, rotationDifference);
                rotateZ = rotateZFollow;
                playerPos = playerPosFollow;
            }
            yield return null;
        }
        yield return new WaitForSeconds(delayAfterFollowDuration);
        Vector3 hitboxEnd = attackInstance.transform.position + attackInstance.transform.up * (attackInstance.transform.localScale.y / 2);
        // Start Attacking
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        while (Vector3.Distance(transform.position, hitboxEnd) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, hitboxEnd, flashSpeed * Time.deltaTime);
            yield return null;
        }
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        // After ( Or mid way) the enemy reaches its destination, enable hitbox to deal damage.
        attackZoneScript.EnableDamage();
        yield return new WaitForSeconds(attackDuration);
        attackZoneScript.DisableDamage();
        if (attackInstance)
        {
            Destroy(attackInstance);
        }
    }
}
