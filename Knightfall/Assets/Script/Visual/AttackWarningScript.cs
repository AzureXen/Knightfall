using System.Collections;
using UnityEngine;

public class AttackWarningScript : MonoBehaviour
{
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    public void DelayedDestroy(float delay)
    {
        StartCoroutine(DestroyOnDelay(delay));
    }
    IEnumerator DestroyOnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
