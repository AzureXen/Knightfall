using System.Collections;
using UnityEngine;

public class RoninFlashSlash : MonoBehaviour
{
    public GameObject attackZone;
    private Vector3 playerPos;
    public float attackDuration = 1f;
    public int Damage;
    public float knockbackForce;
    public float knockbackDuration;

    public float attackCooldown = 5f;
    private float attackCoolDownTimer;

    private GameObject attackInstance;
    private Coroutine attackCoroutine;
    public void Attack()
    {
        Debug.Log("Firing Attack");
        if(attackInstance != null)
        {
            Destroy(attackInstance);
        }
        Vector3 attackPos = new Vector3(transform.position.x, transform.position.y * 4f, transform.position.z);
        attackInstance = Instantiate(attackZone, attackPos, Quaternion.identity);
        if(attackCoroutine != null) { 
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }
    public IEnumerator AttackCoroutine()
    {
        yield return null;
        AttackZoneScript attackZoneScript = attackInstance.GetComponent<AttackZoneScript>();
        attackInstance.transform.localScale = new Vector3(1, 6, 1);
        attackZoneScript.damage = Damage;
        attackZoneScript.knockbackForce = knockbackForce;
        attackZoneScript.knockbackDuration = knockbackDuration;
        attackZoneScript.hitBoxActive = true;
        yield return new WaitForSeconds(attackDuration);
        attackZoneScript.hitBoxActive = false;
        Destroy(attackInstance);
    }
}
