using System.Collections;
using UnityEngine;
using static RoninAction;


public enum NightBorneState
{
    IDLE,
    DECIDING,
    CHASE,
    WARP_ATTACK,
    ATTACK
}
public class NightBorneAI : MonoBehaviour
{   
    [SerializeField] private NightBorneState currentState;
    NightBorneAttack basicAttack;
    NightBorneWarp nightWarp;
    NightBorneMovement movement;
    [SerializeField] private float distanceFromPlayer;
    private GameObject player;
    private NightBorneAnimator am;

    [SerializeField] private float warpAttackCooldown = 10f;
    [SerializeField] private float warpAttackCooldownTimer;

    private SpriteRenderer sr;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        am = GetComponent<NightBorneAnimator>();
        basicAttack = GetComponent<NightBorneAttack>();
        nightWarp = GetComponent<NightBorneWarp>();
        movement = GetComponent<NightBorneMovement>();
        currentState = NightBorneState.DECIDING;
        player = GameManagerScript.Instance.player;
        if(player == null)
        {
            GameManagerScript.OnPlayerReady += RegisterPlayer;
        }
        warpAttackCooldownTimer = warpAttackCooldown;
    }
    private void RegisterPlayer(GameObject player)
    {
        this.player = player;
    }
    void Update()
    {
        if(player != null && currentState == NightBorneState.DECIDING)
        {
            if(warpAttackCooldownTimer <= 0)
            {
                currentState = NightBorneState.WARP_ATTACK;
                StartCoroutine(Warp());
            }else StartCoroutine(BasicAttack());
        }
        if (player != null)
        {
            distanceFromPlayer = Vector3.Distance(sr.bounds.center, player.transform.position);
        }
        if(warpAttackCooldownTimer > 0)
        {
            warpAttackCooldownTimer -= Time.deltaTime;
            warpAttackCooldownTimer = Mathf.Clamp(warpAttackCooldownTimer, 0f, warpAttackCooldown);
        }
    }

    private IEnumerator Warp()
    {
        warpAttackCooldownTimer = warpAttackCooldown;
        yield return nightWarp.Execute();
        currentState = NightBorneState.ATTACK;
        yield return basicAttack.Attack();
        yield return new WaitForSeconds(1.4f);
        currentState = NightBorneState.DECIDING;
    }

    private IEnumerator BasicAttack()
    {
        float stopChaseDistance = 3f;
        am.canChangeDirection = true;
        while (distanceFromPlayer > stopChaseDistance)
        {
            am.PlayAnimation(NightBorneAnimator.NIGHT_BORNE_RUN);
            currentState = NightBorneState.CHASE;
            movement.isMoving = true;
            movement.isChasing = true;
            yield return null;
        }
        am.canChangeDirection = false;
        movement.isMoving = false;
        movement.isChasing = false;
        currentState = NightBorneState.ATTACK;
        yield return basicAttack.Attack();
        yield return new WaitForSeconds(0.75f);
        currentState = NightBorneState.DECIDING;

    }
}
