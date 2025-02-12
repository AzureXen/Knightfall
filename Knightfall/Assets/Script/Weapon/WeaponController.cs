using UnityEngine;


/// <summary>
/// Base script for all weapon controllers
/// </summary>
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public GameObject prefab;
    public float damage;
    public float speed;
    public float cooldownDuraion;
    public float currentCooldown;
    public int pierce;
    protected virtual void Start()
    {
        currentCooldown = cooldownDuraion;
    }


    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0f)
        {
            Attack();
        }
    }
    protected virtual void Attack()
    {
        currentCooldown = cooldownDuraion;
    }
}
