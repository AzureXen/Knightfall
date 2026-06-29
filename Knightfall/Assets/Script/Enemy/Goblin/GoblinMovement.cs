using System;
using System.Collections;
using UnityEngine;

public class GoblinMovement : EntityMovement
{
    [NonSerialized] public GameObject player;
    [SerializeField] private float moveSpeed = 2;
    public Boolean canMove = true;
    [HideInInspector] public Vector2 moveDir;

    private GoblinManager goblinManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goblinManager = GetComponent<GoblinManager>();
        StartCoroutine(FindPlayer());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(player!= null)
        {
            if ( goblinManager != null && goblinManager.isAlive && canMove)
            {
                moveDir = (player.transform.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            }
            else moveDir = Vector2.zero;
        }
    }

    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            if (GameManagerScript.Instance != null && GameManagerScript.Instance.player != null)
                player = GameManagerScript.Instance.player;

            yield return new WaitForSeconds(0.5f);
        }
    }

    public override void DisableMovement()
    {
        canMove = false;
    }
    public override void EnableMovement()
    {
        canMove = true;
    }
}
