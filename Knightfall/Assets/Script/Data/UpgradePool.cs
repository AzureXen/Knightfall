using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UpgradePool", menuName = "Shop/Upgrade Pool")]
public class UpgradePool : ScriptableObject
{
    public List<ShopUpgradeData> allUpgrades;

    public List<ShopUpgradeData> GetRandom(int count)
    {
        List<ShopUpgradeData> shuffled = new List<ShopUpgradeData>(allUpgrades);
        // fisher-yates shuffle
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        return shuffled.GetRange(0, Mathf.Min(count, shuffled.Count));
    }

}
