using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Shop/Upgrade")]
[System.Serializable]
public abstract class ShopUpgradeData : ScriptableObject
{
    public string upgradeName;
    public string upgradeDescription;
    public float upgradePrice;

    public abstract void Apply();
}
