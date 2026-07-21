using System;
using UnityEngine;


// In RoninAnimator, there is only implementation for ronin to look at player.
// Currently, Ronin Animations are managed in RoninAction
public class RoninAnimator : MonoBehaviour
{
    private RoninSFX roninSFX;
    private GameObject player;
    private SpriteRenderer sr;
    private RoninBasicAttack roninBasicAttack;
    private Animator am;
    // canChangeDirection: Ronin's sprite will flip based on position of player, making him look at player.
    public Boolean canChangeDirection { get; set; } = false;
    void Start()
    {
        roninSFX = GetComponent<RoninSFX>();
        am = GetComponentInChildren<Animator>();
        roninBasicAttack = GetComponent<RoninBasicAttack>();
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(canChangeDirection)
        {
            UpdateDirection();
        }
    }
    private void UpdateDirection()
    {
        if (player != null)
        {
            Vector3 playerPos = player.transform.position;
            Vector3 playerDir = playerPos - transform.position;

            Vector3 localScale = transform.localScale;

            if (playerDir.x > 0)
            {
                localScale.x = Mathf.Abs(localScale.x);
            }
            else if (playerDir.x < 0)
            {
                localScale.x = -Mathf.Abs(localScale.x);
            }
            transform.localScale = localScale;
        }
    }

    public void DashTowardsPlayer()
    {
        roninBasicAttack.DashTowardsPlayer();
    }

    public void EnableHitbox1()
    {
        roninBasicAttack.EnableHitbox1();
    }
    public void EnableHitbox2()
    {
        roninBasicAttack.EnableHitbox2();
    }
    public void DisableHitbox1()
    {
        roninBasicAttack.DisableHitbox1();
    }
    public void DisableHitbox2()
    {
        roninBasicAttack.DisableHitbox2();
    }

    public void EnableChangeDirection()
    {
        canChangeDirection = true;
    }

    public void DisableChangeDirection()
    {
        canChangeDirection = false;
    }

    public void PlayAttackWarning2()
    {
        roninSFX.playAttackWarning(2);
    }

    public bool IsAnimationFinished(string animationName)
    {
        AnimatorStateInfo stateInfo = am.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f;
    }

}
