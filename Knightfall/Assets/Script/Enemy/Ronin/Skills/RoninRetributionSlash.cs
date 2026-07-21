using System.Collections;
using UnityEngine;

public class RoninRetributionSlash : MonoBehaviour
{
    private RoninAction roninAction;

    public float rebtributionSlashLength;

    public GameObject retributionSlashPrefab;
    public GameObject retributionSlashSwingPrefab;


    public float attackDuration = 1f;
    public int Damage;
    public float knockbackForce;
    public float knockbackDuration;

    public float attackCooldown = 5f;
    public float attackCoolDownTimer;

    public Transform attackTransform;
    private GameObject player;

    private GameObject attackInstance;
    private GameObject visualInstance;
    private Coroutine attackCoroutine;

    private void Start()
    {
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
        if (visualInstance)
            { Destroy(visualInstance); }
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
            attackInstance = Instantiate(retributionSlashPrefab, attackTransform.position, Quaternion.identity);
            attackInstance.transform.localScale = new Vector3(rebtributionSlashLength * 2, rebtributionSlashLength, rebtributionSlashLength * 2);


            Vector3 bulletDirection = Vector3.up;
            GameObject targetPlayer = GameManagerScript.Instance?.player ?? player;

            if (targetPlayer != null)
            {
                bulletDirection = (targetPlayer.transform.position - attackTransform.position).normalized;
            }

            if (visualInstance != null)
            {
                Destroy(visualInstance);
            }

            float spawnOffsetDistance = 0.5f; 
            Vector3 spawnPosition = attackTransform.position + (bulletDirection * spawnOffsetDistance);


            visualInstance = Instantiate(retributionSlashSwingPrefab, spawnPosition, Quaternion.identity);

            if (visualInstance != null) {
                Destroy(visualInstance);
            }
            visualInstance = Instantiate(retributionSlashSwingPrefab, attackTransform.position, Quaternion.identity);

            if (visualInstance != null) {
                float rotation = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg;
                visualInstance.transform.rotation = Quaternion.Euler(0, 0, rotation);

                Destroy(visualInstance, 2f);
            }
        }
        finally
        {

        }
        yield return null;
    }

}
