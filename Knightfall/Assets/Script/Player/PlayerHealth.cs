using System;
using TMPro;
using UnityEngine;

public class PlayerHealth : Health
{
    private PlayerDataSO playerData;

    public override event Action OnDeath;

    public override void Start()
    {
        base.Start();
        playerData = GameManagerScript.Instance.playerData;
    }

    public override void TakeDamage(int amount)
    {
        playerData.TakeDamage(amount);
        popUpText = Instantiate(popUpDamage, transform.position, Quaternion.identity) as GameObject;
        TextMeshPro damageDisplayMesh = popUpText.transform.GetChild(0).GetComponent<TextMeshPro>();
        damageDisplayMesh.text = amount.ToString();
        if (amount == 0)
        {
            damageDisplayMesh.color = Color.blue;
        }
        if (health <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}
