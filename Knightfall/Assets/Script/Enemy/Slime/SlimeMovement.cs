using System.Collections;
using UnityEngine;

namespace Assets.Script.Enemy.Slime
{
    public class SlimeMovement : EntityMovement
    {
        public GameObject player;
        [SerializeField] private float moveSpeed = 2f;
        public bool canMove = true;
        [HideInInspector] public Vector2 moveDir;

        public float potionDetectionRadius = 2f;
        public float detectionRange = 5f;

        private GameObject targetPotion;
        private bool playerDetected = false;    

        // Abnormal slime properties
        public bool isAbnormal = false;
        private GameObject keyObject;
        private Vector3 escapePosition;
        [SerializeField] private GameObject slimePrefab;


        void Start()
        {
            StartCoroutine(FindPlayer());

            if (isAbnormal)
            {
                StartCoroutine(FindKey());
                escapePosition = new Vector3(15, -15, 0);
            }
        }

        void FixedUpdate()
        {
            if (isAbnormal)
            {
                if (keyObject != null)
                {
                    // Instantly move to the key
                    transform.position = keyObject.transform.position;
                    Destroy(keyObject); 

                    // Optional: Add a small delay before dashing away
                    StartCoroutine(DashToEscape());
                    SpawnNewSlime();
                    return;
                }
            }

            FindClosestPotion();

            if (canMove)
            {
                if (playerDetected)
                {
                    MoveTowards(player.transform.position);
                }
                else if (targetPotion != null)
                {
                    MoveTowards(targetPotion.transform.position);
                }
                else if (player != null && Vector2.Distance(transform.position, player.transform.position) <= detectionRange)
                {
                    playerDetected = true;
                }
                else
                {
                    moveDir = Vector2.zero;
                }
            }
        }

        private IEnumerator DashToEscape()
        {
            SlimeManager slimeManager = GetComponent<SlimeManager>();

            if (slimeManager != null)
            {
                slimeManager.SetInvincible(true); // Enable invincibility
            }

            yield return new WaitForSeconds(1f); // Small delay before dashing away
            moveSpeed *= 6f; // Increase speed

            while (Vector2.Distance(transform.position, escapePosition) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, escapePosition, moveSpeed * Time.deltaTime);
                yield return null; // Wait until next frame
            }

            if (slimeManager != null)
            {
                slimeManager.SetInvincible(false); // Disable invincibility after dashing
            }

            Destroy(gameObject);
            
        }





        private void MoveTowards(Vector2 targetPosition)
        {
            moveDir = (targetPosition - (Vector2)transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        private IEnumerator FindPlayer()
        {
            while (player == null && !isAbnormal)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                yield return null;
            }
        }

        private IEnumerator FindKey()
        {
            while (keyObject == null)
            {
                yield return new WaitForSeconds(0.8f);
                keyObject = GameObject.FindGameObjectWithTag("Key");
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
        private void SpawnNewSlime()
        {
            if (slimePrefab == null)
            {
                Debug.LogError("Slime prefab is not assigned!");
                return;
            }

            Vector3 spawnPosition = new Vector3(45, 15, 0);
            GameObject newSlime = Instantiate(slimePrefab, spawnPosition, Quaternion.identity);

            SlimeManager newSlimeManager = newSlime.GetComponent<SlimeManager>();
            SlimeMovement newSlimeScript = newSlime.GetComponent<SlimeMovement>();

            if (newSlimeManager != null && newSlimeScript != null)
            {
                newSlimeManager.SetInvincible(false);
                newSlimeManager.hasKey = true;
                newSlimeScript.isAbnormal = false;
                newSlimeScript.canMove = true;

                // Manually trigger initialization if Awake() didn't run correctly
                newSlimeManager.Awake();
                newSlimeScript.Start();
            }

            newSlime.SetActive(true);
        }


    }
}
