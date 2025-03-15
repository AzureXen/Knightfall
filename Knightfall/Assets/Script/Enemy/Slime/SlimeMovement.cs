using System.Collections;
using UnityEngine;

namespace Assets.Script.Enemy.Slime
{
    public class SlimeMovement : EntityMovement
    {
        [SerializeField] public GameObject player;
        [SerializeField] private float moveSpeed = 2f;
        public bool canMove = true;
        [HideInInspector] public Vector2 moveDir;

        public float potionDetectionRadius = 2f;
        public float detectionRange = 5f;

        private GameObject targetPotion;
        private bool playerDetected = false;

        void Start()
        {
            StartCoroutine(FindPlayer());
        }

        void FixedUpdate()
        {
            FindClosestPotion();

            if (canMove)
            {
                if (playerDetected)
                {
                    MoveTowards(player.transform.position); // Keep chasing the player once detected
                }
                else if (targetPotion != null)
                {
                    MoveTowards(targetPotion.transform.position); // Move to the potion if no player is detected
                }
                else if (player != null && Vector2.Distance(transform.position, player.transform.position) <= detectionRange)
                {
                    playerDetected = true; // Detect player and lock onto them
                }
                else
                {
                    moveDir = Vector2.zero;
                }
            }
        }

        private void MoveTowards(Vector2 targetPosition)
        {
            moveDir = (targetPosition - (Vector2)transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        private IEnumerator FindPlayer()
        {
            while (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                yield return null;
            }
        }

        private void FindClosestPotion()
        {
            Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, potionDetectionRadius);

            GameObject closestPotion = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider2D collider in nearbyObjects)
            {
                if (collider.CompareTag("Object"))
                {
                    float distanceToPotion = Vector2.Distance(transform.position, collider.transform.position);

                    if (distanceToPotion < closestDistance)
                    {
                        closestDistance = distanceToPotion;
                        closestPotion = collider.gameObject;
                    }
                }
            }

            targetPotion = closestPotion;
        }

        public override void DisableMovement()
        {
            canMove = false;
        }

        public override void EnableMovement()
        {
            canMove = true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, potionDetectionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}
