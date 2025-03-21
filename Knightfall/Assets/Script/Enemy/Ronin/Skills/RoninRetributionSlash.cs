using System.Collections;
using UnityEngine;

public class RoninRetributionSlash : MonoBehaviour
{
    public float rebtributionSlashLength;

    public GameObject retributionSlashPrefab;
    public float attackDuration = 1f;
    public int Damage;
    public float knockbackForce;
    public float knockbackDuration;

    public float attackCooldown = 5f;
    public float attackCoolDownTimer;

    public Transform attackTransform;
    private GameObject player;

    private GameObject attackInstance;
    private Coroutine attackCoroutine;

    private void Start()
    {
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
            if (attackInstance != null)
            {
                Destroy(attackInstance);
            }
            attackInstance = Instantiate(retributionSlashPrefab, attackTransform.position, Quaternion.identity, attackTransform);
            attackInstance.transform.localScale = new Vector3(1, rebtributionSlashLength, 1);
        }
        finally
        {

        }
        yield return null;
    }

}
