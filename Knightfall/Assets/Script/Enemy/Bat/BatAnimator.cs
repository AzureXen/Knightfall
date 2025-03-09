﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Script.Enemy.Slime;
using UnityEngine;

namespace Assets.Script.Enemy.Bat
{
    internal class BatAnimator : MonoBehaviour
    {
        Animator am;
        BatMovement bm;
        SpriteRenderer sr;

        private Transform player;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            am = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
            bm = GetComponent<BatMovement>();

            StartCoroutine(FindPlayer());
        }

        // Update is called once per frame
        void Update()
        {
            // Check if goblin is moving or not
            if (bm.moveDir != Vector2.zero)
            {
                am.SetBool("isWalking", true);
                UpdateAnimationState();
            }
            else
            {
                am.SetBool("isWalking", false);
            }
        }
        // changes animation based on direction goblin is walking towards.
        void UpdateAnimationState()
        {
            if (player == null) return; // Safety check

            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            bool isHorizontal = Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.y);

            am.SetBool("isLookUp", !isHorizontal && directionToPlayer.y > 0);
            am.SetBool("isLookDown", !isHorizontal && directionToPlayer.y < 0);
            am.SetBool("isLookHorizon", isHorizontal);

            sr.flipX = isHorizontal && directionToPlayer.x > 0;
        }
        private IEnumerator FindPlayer()
        {
            while (player == null)
            {
                player = bm.player.transform;
                yield return null;
            }
        }
    }
}
