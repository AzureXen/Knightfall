using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GoblinAnimator : MonoBehaviour
{
    Animator am;
    GoblinMovement gm;
    SpriteRenderer sr;

    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        am = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        gm = GetComponent<GoblinMovement>();

        StartCoroutine(FindPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        // Check if goblin is moving or not
        if (gm.moveDir != Vector2.zero)
        {
            am.SetBool("isWalking", true);
            UpdateAnimationState();
        }
        else
        {
            am.SetBool("isWalking", false);
        }
    }
    // changes animation based on direction goblin is walking towards.
    void UpdateAnimationState()
    {
        if (player == null) return; // Safety check

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        bool isHorizontal = Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.y);

        am.SetBool("isLookUp", !isHorizontal && directionToPlayer.y > 0);
        am.SetBool("isLookDown", !isHorizontal && directionToPlayer.y < 0);
        am.SetBool("isLookHorizon", isHorizontal);

        sr.flipX = isHorizontal && directionToPlayer.x > 0;
    }
    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = gm.player.transform;
            yield return null;
        }
    }
}
