using UnityEngine;

public class StairMovement : MonoBehaviour
{
    public float stairSpeed = 3f; // Adjust movement speed when on stairs
    public float normalSpeed = 5f; // Default player speed
    public float normalGravity = 3f; // Default gravity
    private Rigidbody2D rb;
    private bool onStairs = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");

        // If the player is on stairs, adjust gravity and movement
        if (onStairs)
        {
            rb.gravityScale = 0; // Disable gravity
            rb.linearVelocity = new Vector2(moveX * stairSpeed, Input.GetAxis("Vertical") * stairSpeed);
        }
        else
        {
            rb.gravityScale = normalGravity; // Restore normal gravity
            rb.linearVelocity = new Vector2(moveX * normalSpeed, rb.linearVelocity.y);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Stair"))
        {
            onStairs = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Stair"))
        {
            onStairs = false;
        }
    }
}
