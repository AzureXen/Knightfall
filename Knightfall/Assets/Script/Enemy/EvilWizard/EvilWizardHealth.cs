using System.Collections;
using UnityEngine;

public class EvilWizardHealth : Health
{
    public override void Update()
    {
        if (health <= 0)
        {
            StartCoroutine(onDeath());
        }
    }
    IEnumerator onDeath()
    {
        GameObject sceneTrigger = GameObject.FindGameObjectWithTag("SceneTrigger");
        sceneTrigger.SetActive(true);
        yield return null;
        Destroy(gameObject);
    }
}
