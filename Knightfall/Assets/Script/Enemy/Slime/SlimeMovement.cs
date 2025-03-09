using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.Enemy.Slime
{
    public class SlimeMovement : EntityMovement
    {
        public GameObject player;
        [SerializeField] private float moveSpeed = 2;
        public Boolean canMove = true;
        [HideInInspector] public Vector2 moveDir;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(FindPlayer());
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (player != null)
            {
                if (canMove)
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
}
