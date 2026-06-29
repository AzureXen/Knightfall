using UnityEngine;

[CreateAssetMenu(fileName = "FullHealUpgrade", menuName = "Shop/Upgrades/Full Heal")]
public class FullHealUpgrade : ShopUpgradeData
{
    public override void Apply()
    {
        PlayerManager.Instance.FullHeal();
    }
}
