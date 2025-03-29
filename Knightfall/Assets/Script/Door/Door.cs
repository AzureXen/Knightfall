using UnityEngine;

namespace Assets.Script.Door
{
    public class Door : MonoBehaviour
    {
        private Collider2D doorCollider;
        private Animator animator;

        private void Awake() // Use Awake to initialize variables before anything calls OpenDoor
        {
            doorCollider = GetComponent<Collider2D>();
            animator = GetComponent<Animator>();
        }

        public void OpenDoor()
        {
            // Ensure animator and collider are assigned before using them
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            if (doorCollider == null)
            {
                doorCollider = GetComponent<Collider2D>();
            }

            if (animator != null)
            {
                animator.SetTrigger("Open"); // Play opening animation
            }
            else
            {
                Debug.LogError("Door: Animator is NULL!");
            }

            if (doorCollider != null)
            {
                doorCollider.enabled = false; // Disable collision
            }
            else
            {
                Debug.LogError("Door: Collider is NULL!");
            }

            Debug.Log("Door is now open!");
        }
    }
}
