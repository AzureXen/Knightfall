using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UndeadBossHealth : Health
{
    public float delayDestroyObject = 1f;

    public Boolean isDead = false;

    public Boolean inflictPain = false;

    public event Action OnDeath;

    Animator animator;

    // used to instantiate
    public GameObject popUpDamagePixel;
    // used to make the text follow
    private GameObject TextpopUp;

    public GameObject healthBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
    }
    public override void Update()
    {
        if(health > 0) { healthBar.SetActive(true); }
        if (health <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(Die(delayDestroyObject));
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
            VanSoundManager.PlaySound(SoundType.UNDEADHURT, 0.4f);
            TextpopUp = Instantiate(popUpDamagePixel, transform.position, Quaternion.identity) as GameObject;
            TextpopUp.transform.position += new Vector3(0, 4, 0);
            TextMeshPro damageDisplayMesh = TextpopUp.transform.GetChild(0).GetComponent<TextMeshPro>();
            damageDisplayMesh.outlineColor = Color.black;
            damageDisplayMesh.text = amount.ToString();
            if (amount == 0)
            {
                damageDisplayMesh.color = Color.blue;
            }
        }
    }

    private IEnumerator Die(float delay)
    {
        // Optionally, trigger a death animation here
        if (health <= 0 && !isDead)
        {
            isDead = true;
            animator.SetTrigger("dying");
            yield return new WaitForSeconds(1f);
            animator.SetTrigger("dead");
            OnDeath?.Invoke();
            VanSoundManager.PlaySound(SoundType.UDEADDEAD, 1f);
        }
        yield return new WaitForSeconds(delay);
        healthBar.SetActive(false);
        Destroy(gameObject);
    }
}
