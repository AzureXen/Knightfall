using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class GoblinAnimator : MonoBehaviour
{
    Animator am;
    GoblinMovement gm;
    SpriteRenderer sr;
    GoblinManager goblinManager;
    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        am = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        gm = GetComponent<GoblinMovement>();
        goblinManager = GetComponent<GoblinManager>();
        StartCoroutine(FindPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        // Check if goblin is moving or not
        if(goblinManager!=null && goblinManager.isAlive)
        {
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
        else if (goblinManager != null && !goblinManager.isAlive)
        {
            am.SetBool("isLookUp", false);
            am.SetBool("isLookDown", false);
            am.SetBool("isLookHorizon", false);
            am.SetBool("isDead", true);
        }
    }
    // changes animation based on direction goblin is walking towards.
    void UpdateAnimationState()
    {
        if (player == null)
        {
            Debug.Log("UpdateAnimationState: player not found");
            return;
        }

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
            if (GameManagerScript.Instance != null && GameManagerScript.Instance.player != null)
                player = GameManagerScript.Instance.player.transform;

            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Goblin Animator: Found Player.");
    }
}
