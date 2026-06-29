using UnityEngine;

[CreateAssetMenu(fileName = "MaxStaminaUpgrade", menuName = "Shop/Upgrades/Max Stamina")]
public class MaxStaminaUpgrade : ShopUpgradeData
{
    public float bonusStamina;
    public override void Apply()
    {
        PlayerManager.Instance.IncreaseMaxStamina(bonusStamina);
    }
}
