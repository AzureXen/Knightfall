using UnityEngine;

[CreateAssetMenu(fileName = "MeleeDamageUpgrade", menuName = "Shop/Upgrades/Melee Damage")]
public class MeleeDamageUpgrade : ShopUpgradeData
{
    public int bonusDamage;
    public override void Apply()
    {
        PlayerManager.Instance.IncreaseMeleeDamage(bonusDamage);
    }
}
