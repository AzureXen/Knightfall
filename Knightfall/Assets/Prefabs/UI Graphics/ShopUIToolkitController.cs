using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopUIToolkitController : MonoBehaviour
{
    [Header("UI Toolkit Configuration")]
    [SerializeField] private VisualTreeAsset upgradeCardTemplate;   

    [Header("Shop Content")]
    [SerializeField] private List<ShopUpgradeData> availableUpgrades;

    private VisualElement upgradeList;

    private void OnEnable()
    {
        GetComponent<PanelRenderer>().RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDisable()
    {
        GetComponent<PanelRenderer>().UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root)
    {
        upgradeList = root.Q<VisualElement>("player_upgrade_list");
        upgradeList?.Clear();

        List<ShopUpgradeData> mockData = new List<ShopUpgradeData>();

        ShopUpgradeData temp1 = ScriptableObject.CreateInstance<ShopUpgradeData>();
        temp1.upgradeName = "Fireball Boost";
        temp1.upgradeDescription = "Increases explosive blast radius by +25%";
        temp1.upgradePrice = 75;
        mockData.Add(temp1);

        ShopUpgradeData temp2 = ScriptableObject.CreateInstance<ShopUpgradeData>();
        temp2.upgradeName = "Ice Shield";
        temp2.upgradeDescription = "Generates a freezing barrier that absorbs 10 damage.";
        temp2.upgradePrice = 120;
        mockData.Add(temp2);

        ShopUpgradeData temp3 = ScriptableObject.CreateInstance<ShopUpgradeData>();
        temp3.upgradeName = "Ice Shield 2";
        temp3.upgradeDescription = "2 2 2Generates a freezing barrier that absorbs 10 damage.";
        temp3.upgradePrice = 130;
        mockData.Add(temp3);


        ShowUpgrades(mockData);
    }

    public void ShowUpgrades(List<ShopUpgradeData> upgrades)
    {
        if (upgradeList == null) return;

        upgradeList.Clear();

        foreach (ShopUpgradeData upgrade in upgrades)
        {
            var card = upgradeCardTemplate.Instantiate();



            card.Q<Label>("upgrade_name").text = upgrade.upgradeName;
            card.Q<Label>("upgrade_description_text").text = upgrade.upgradeDescription;

            var btn = card.Q<UnityEngine.UIElements.Button>("upgrade_buy_btn");
            btn.text = $"{upgrade.upgradePrice}g";
            btn.clicked += () => OnPurchase(upgrade);

            upgradeList.Add(card);
        }
    }

    private void OnPurchase(ShopUpgradeData upgrade) {
        Debug.Log("Purchase button clicked");
    }

}
