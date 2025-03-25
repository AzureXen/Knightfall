using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UndeadHealth : Health
{
    public float delayDestroyObject = 1f;
    public Boolean isDead = false;
    private Boolean isInterupted = false;

    public Boolean inflictPain = false;

    Animator animator;

    // used to instantiate
    public GameObject popUpDamagePixel;
    // used to make the text follow
    private GameObject TextpopUp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
    }
    public override void Update()
    {

        if (health <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(Die(delayDestroyObject));
        }

        if (isInterupted)
        {
            StartCoroutine(Disrupted());
        }
    }
    public override void FixedUpdate()
    {
        if (TextpopUp != null)
        {
            TextpopUp.transform.position = transform.position;
        }
    }
    // Update is called once per frame
    public override void TakeDamage(int amount)
    {
        if (amount < 0) return;

        if (!isDead)
        {
            health -= amount;
            isInterupted = true;
            animator.SetTrigger("Hurt");
            TextpopUp = Instantiate(popUpDamagePixel, transform.position, Quaternion.identity) as GameObject;
            TextMeshPro damageDisplayMesh = TextpopUp.transform.GetChild(0).GetComponent<TextMeshPro>();
            damageDisplayMesh.outlineColor = Color.black;
            damageDisplayMesh.text = amount.ToString();
            if (amount == 0)
            {
                damageDisplayMesh.color = Color.blue;
            }
        }
    }

    private IEnumerator Disrupted()
    {
        // Optionally, trigger a death animation here
        inflictPain = true;
        yield return new WaitForSeconds(0.4f); // Wait 2 seconds before destroying
        isInterupted=false;
        inflictPain = false;
    }

    private IEnumerator Die(float delay)
    {
        // Optionally, trigger a death animation here
        animator.SetTrigger("Dead");
        yield return new WaitForSeconds(delay); // Wait 2 seconds before destroying
        Destroy(gameObject);
    }
}
