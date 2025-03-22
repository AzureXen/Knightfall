using System.Collections;
using UnityEngine;

public class NhatMovement : EntityMovement
{
    [SerializeField] private float moveSpeed = 2;
    public bool canMove = true;
    public bool isRanged = false; // ✅ Nếu true → quái tầm xa, false → quái cận chiến

    private Vector2 randomDirection;
    private float changeDirectionTime = 2f; // Đổi hướng mỗi 2 giây
    private float nextDirectionChange;

    private GameObject player;

    void Start()
    {
        StartCoroutine(FindPlayer());
        if (isRanged)
        {
            ChangeDirection(); // Nếu là quái tầm xa, nó sẽ đi vòng quanh
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            if (isRanged) // ✅ Nếu là quái tầm xa, chỉ đi vòng vòng
            {
                if (Time.time >= nextDirectionChange)
                {
                    ChangeDirection();
                    nextDirectionChange = Time.time + changeDirectionTime;
                }

                transform.position += (Vector3)randomDirection * moveSpeed * Time.deltaTime;
            }
            else // ✅ Nếu là quái cận chiến, sẽ dí theo Player
            {
                Vector2 moveDir = (player.transform.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    private void ChangeDirection()
    {
        // Random hướng đi trong phạm vi 360 độ
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
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
