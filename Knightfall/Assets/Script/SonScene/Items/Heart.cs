using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private int healAmount = 20; // Adjust healing amount

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        GameObject target = collision.gameObject;
        if (target.CompareTag("Player")) // Make sure your player has the tag "Player"
        {
            Health player = target.GetComponent<Health>();
            if (player != null)
            {
                player.health += healAmount;
                Destroy(gameObject); // Remove heart after healing
            }
        }
    }
}
