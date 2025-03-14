using UnityEngine;

public class OrcAnimator : MonoBehaviour
{


    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Transform Player;
    private Vector2 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("PLayer").transform;
    }

    // Update is called once per frame
    void Update()
    {
        direction = (Player.position - transform.position).normalized;
        if(direction.x < 0)
        {
            sr.flipX = true;    
        }
        else if(direction.x > 0)
        {
            sr.flipX = false;
        }
    }
}
