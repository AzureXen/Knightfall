using System;
using UnityEngine;
using System.Collections;


namespace Assets.Script.Enemy.FlyingEye
{
    public class FlyingEyeMovement : EntityMovement
    {
        [SerializeField] private float moveSpeed = 2;
        public Boolean canMove = true;
        [HideInInspector] public Vector2 moveDir;
        public Boolean isChasingPlayer;
        public Boolean isMoving;

        [SerializeField] private float chaseSpeed = 5f;
        [SerializeField] private int maxWallAvoidAttempts = 5;
        [SerializeField] private float wallStopDistance = 3f;
        [SerializeField] private float wallDetectionDistance = 4f; 
        [SerializeField] private float directionChangeInterval = 2f;

        private Vector2 currentRandomDir;
        private float directionTimer = 0f;

        public GameObject player;
        private FlyingEyeManager flyingEyeManager;
        private FlyingEyeAnimator flyingEyeAnimator;
        void Start()
        {
            flyingEyeManager = GetComponent<FlyingEyeManager>();
            flyingEyeAnimator = GetComponent<FlyingEyeAnimator>();
            player = flyingEyeManager.Player;
            if(player == null)
            {
                flyingEyeManager.OnPlayerFound += RegisterPlayer;
            }
        }

        void FixedUpdate()
        {
            if (player == null) return;

            if (!canMove)
            {
                moveDir = Vector2.zero;
                flyingEyeAnimator?.SetMoveDirection(moveDir);
                return;
            }

            if (isMoving && isChasingPlayer)
            {
                moveDir = (player.transform.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    player.transform.position,
                    chaseSpeed * Time.fixedDeltaTime
                );
            }
            else if (isMoving)
            {
                directionTimer -= Time.fixedDeltaTime;
                if (directionTimer <= 0f || currentRandomDir == Vector2.zero)
                {
                    currentRandomDir = GetSafeRandomDirection();
                    directionTimer = directionChangeInterval;
                }

                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    currentRandomDir,
                    wallStopDistance,
                    LayerMask.GetMask("Walls")
                );

                if (hit.collider == null)
                {
                    moveDir = currentRandomDir;
                    transform.position = Vector2.MoveTowards(
                        transform.position,
                        (Vector2)transform.position + currentRandomDir,
                        moveSpeed * Time.fixedDeltaTime
                    );
                }
                else
                {
                    moveDir = Vector2.zero;
                }
            }
            else
            {
                moveDir = Vector2.zero;
            }
            flyingEyeAnimator?.SetMoveDirection(moveDir);
        }


        private Vector2 GetSafeRandomDirection()
        {
            Vector2 chosenDir = Vector2.zero;

            for (int i = 0; i < maxWallAvoidAttempts; i++)
            {
                float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
                Vector2 randomDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    randomDir,
                    wallDetectionDistance,
                    LayerMask.GetMask("Walls")
                );

                if (hit.collider == null)
                {
                    // found a clear direction
                    return randomDir;
                }

                // store last attempt in case we exhaust all attempts
                chosenDir = randomDir;
            }

            // max attempts reached, return last direction anyway
            // entity will stop when it gets within wallStopDistance
            return chosenDir;
        }

        private void RegisterPlayer(GameObject player)
        {
            this.player = player;
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
}