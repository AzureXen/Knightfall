using System;
using System.Collections;
using UnityEngine;

public class NightBorneAnimator : MonoBehaviour
{
    [SerializeField] private NightBorneBasicAttackHitbox basicAttackHitbox;

    public const string NIGHT_BORNE_IDLE = "night-borne-idle";
    public const string NIGHT_BORNE_RUN = "night-borne-run";
    public const string NIGHT_BORNE_ATTACK = "night-borne-attack";
    public const string NIGHT_BORNE_HURT = "night-borne-hurt";
    public const string NIGHT_BORNE_DEATH = "night-borne-death";
    public const string NIGHT_BORNE_TELEPORT = "night-borne-teleport";
    public const string NIGHT_BORNE_APPEAR = "night-borne-appear";
    public const string NIGHT_BORNE_APPEAR_QUICK = "night-borne-appear-quick";
    public const string NIGHT_BORNE_TELEPORT_INSTANT = "night-borne-teleport-instant";


    Animator am;
    NightBorneMovement gm;
    SpriteRenderer sr;
    NightBorneManager nbManager;
    private Transform player;
    public Boolean canChangeDirection { get; set; } = false;

    private void Update()
    {
        if (canChangeDirection)
        {
            UpdateDirection();
        }
    }
    void Start()
    {
        am = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        gm = GetComponent<NightBorneMovement>();
        nbManager = GetComponent<NightBorneManager>();
        StartCoroutine(FindPlayer());
        canChangeDirection = true;

    }
    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            if (GameManagerScript.Instance != null && GameManagerScript.Instance.player != null)
                player = GameManagerScript.Instance.player.transform;

            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("NightBorne Animator: Found Player.");
    }

    public void PlayAnimation (string NewAnimation)
    {
        am.Play(NewAnimation);
    }

    public void EnableDamage()
    {
        basicAttackHitbox.EnableDamage();
    }   

    public void DisableDamage()
    {
        basicAttackHitbox.DisableDamage();
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

    public bool IsAnimationFinished(string animationName)
    {
        AnimatorStateInfo stateInfo = am.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f;
    }
}
