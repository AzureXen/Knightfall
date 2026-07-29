using UnityEngine;

public class FlyingEyeBullet : EnemyBullet
{
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        if (animator == null) Debug.Log("FlyingEyeBullet: animator not found!");
    }
    public override void DestroyBullet()
    {
        disabledDamage = true;
        animator.Play("FlyingEyeBulletDestroy");
        Destroy(this, 1f);
    }
}
