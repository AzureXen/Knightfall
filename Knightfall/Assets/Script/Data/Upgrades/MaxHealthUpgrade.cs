using UnityEngine;

[CreateAssetMenu(fileName = "MaxHealthUpgrade", menuName = "Shop/Upgrades/Max Health")]
public class MaxHealthUpgrade : ShopUpgradeData
{
    public int bonusHealth;
    public override void Apply()
    {
        PlayerManager.Instance.IncreaseMaxHealth(bonusHealth);
    }
}
