using System;
using System.Collections;
using UnityEngine;

public class WaveSystemTrigger : MonoBehaviour
{
    public EventHandler OnPlayerEnterTrigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerEnterTrigger?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }

}
