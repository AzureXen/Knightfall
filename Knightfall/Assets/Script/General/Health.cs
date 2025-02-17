using System;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;

    // used to instantiate
    public GameObject popUpDamage;
    // used to make the text follow
    private GameObject popUpText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }
    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        if(popUpText != null)
        {
            popUpText.transform.position = transform.position;
        }
    }
    // Update is called once per frame
    public void TakeDamage(int amount)
    {
        health -= amount;
        popUpText = Instantiate(popUpDamage, transform.position, Quaternion.identity) as GameObject;
        TextMeshPro damageDisplayMesh = popUpText.transform.GetChild(0).GetComponent<TextMeshPro>();
        damageDisplayMesh.text = amount.ToString();
        if (amount == 0)
        {
            damageDisplayMesh.color = Color.blue;
        }
    }
}
