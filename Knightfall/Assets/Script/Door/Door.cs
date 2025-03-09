using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Door
{
    using UnityEngine;

    public class Door : MonoBehaviour
    {
        private Collider2D doorCollider;
        private Animator animator;

        private void Start()
        {
            doorCollider = GetComponent<Collider2D>();
            animator = GetComponent<Animator>();
        }

        public void OpenDoor()
        {
            if (animator != null)
            {
                animator.SetTrigger("Open"); // Play opening animation
            }

            if (doorCollider != null)
            {
                doorCollider.enabled = false; // Remove collision
            }

            Debug.Log("Door is now open!");
        }
    }

}
