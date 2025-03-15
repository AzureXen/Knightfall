using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Script.Potion;
using UnityEngine;

namespace Assets.Script.Object.Apple
{
    public class ApplePickup :MonoBehaviour
    {
        private bool isInRange = false;
        private Healing Healing;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            isInRange = true;
            Healing = other.GetComponentInChildren<Healing>();

            if (other.CompareTag("Enemy"))
            {
                Healing = other.GetComponent<Healing>();
                AutoPickup();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            isInRange = false;
            Healing = null;
        }
    }

    private void Update()
    {
        // Player needs to press E to pick up
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickupApple();
        }
    }

    private void AutoPickup()
    {
        if (Healing != null)
        {
            Healing.BuffEnemy();
        }

        Destroy(gameObject);
    }

    private void PickupApple()
    {
        if (Healing != null)
        {
            Healing.Heal();
        }

        Destroy(gameObject);
    }
}
}
