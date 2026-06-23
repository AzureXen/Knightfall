using UnityEngine;

[System.Serializable]
public class ShopUpgradeData : ScriptableObject
{
    public string upgradeName;
    public string upgradeDescription;
    public float upgradePrice;
    public ShopUpgradeData(string upgradeName, string description, float price)
    {
        this.upgradeName = upgradeName;
        this.upgradeDescription = description;
        this.upgradePrice = price;
    }
    public ShopUpgradeData() { }
}
