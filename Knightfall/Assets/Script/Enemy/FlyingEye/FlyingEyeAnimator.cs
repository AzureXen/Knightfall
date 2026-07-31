using System;
using UnityEngine;

public class FlyingEyeAnimator : MonoBehaviour
{
    public const String IDLE = "Idle";
    public const String ATTACK_1 = "Attack1";
    public const String ATTACK_2_START = "Attack2Start";
    public const String ATTACK_2_LOOP = "Attack2Loop";
    public const String ATTACK_2_END = "Attack2End";
    public const String ATTACK_3_WARNING = "Attack3Warning";
    public const String ATTACK_3_SHOOT = "Attack3Shoot";
    public const String ATTACK_4 = "Attack4";
    public const String DEATH_1 = "Death1";
    public const String DEATH_2 = "Death2";
    public const String HURT = "Hurt";

    private GameObject player;
    private SpriteRenderer sr;
    private Animator am;
    private FlyingEyeBite flyingEyeBite;
    private string currentState;

    public enum DirectionMode { None, FacePlayer, FaceMovement }
    public DirectionMode directionMode = DirectionMode.None;



    void Start()
    {
        am = GetComponentInChildren<Animator>();
        flyingEyeBite = GetComponent<FlyingEyeBite>();
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (directionMode == DirectionMode.FacePlayer)
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

    public void SetMoveDirection(Vector2 moveDir)
    {
        if (directionMode != DirectionMode.FaceMovement) return;
        if (moveDir == Vector2.zero) return;

        Vector3 localScale = transform.localScale;
        localScale.x = moveDir.x >= 0 ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }

    public void DashTowardsPlayer()
    {
        flyingEyeBite.DashTowardsPlayer();
    }

    public void EnableBiteHitbox()
    {
        flyingEyeBite.EnableBiteHitbox();
    }
    public void DisableBiteHitbox()
    {
        flyingEyeBite.DisableBiteHitbox();
    }

    public void ChangeDirectionFacePlayer()
    {
        directionMode = DirectionMode.FacePlayer;
    }

    public void ChangeDirectionFaceMovement()
    {
        directionMode = DirectionMode.FaceMovement;
    }

    public void ChangeDirectionNone()
    {
        directionMode = DirectionMode.None;
    }
    public void PlayAnimation(string newState)
    {
        if (currentState == newState) return;
        am.Play(newState);
        currentState = newState;
    }
    public bool IsAnimationFinished(string animationName)
    {
        AnimatorStateInfo stateInfo = am.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f;
    }
}
