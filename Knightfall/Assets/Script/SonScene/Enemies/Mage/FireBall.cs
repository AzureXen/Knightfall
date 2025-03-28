using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MageFireBall : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 15;
    private Vector2 targetDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)targetDirection * speed * Time.deltaTime;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Initialize(Vector3 targetPosition)
    {
        targetDirection = (targetPosition - transform.position).normalized;

        // Calculate the rotation angle
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, 3f); // Destroy after 3 seconds
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject target = collision.gameObject;
        if (target.CompareTag("Player"))
        {
            target.GetComponent<EntityManager>().TakeMeleeHit(damage, transform.position, 0.5f, 0.5f, null);
            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }
}
