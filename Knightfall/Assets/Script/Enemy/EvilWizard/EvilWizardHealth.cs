using System.Collections;
using UnityEngine;

public class EvilWizardHealth : Health
{
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public override void Update()
    {
        if (health <= 0)
        {
            animator.SetTrigger("isDead");
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
