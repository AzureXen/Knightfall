using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopUIToolkitController : MonoBehaviour
{
    [Header("UI Toolkit Configuration")]
    [SerializeField] private VisualTreeAsset upgradeCardTemplate;   

    [Header("Shop Content")]
    [SerializeField] private UpgradePool upgradePool;

    private VisualElement upgradeList;
    private VisualElement root;
    private VisualElement closeButton;
    private Label goldLabel;

    // this is for gold flashes red if player try to purchase without sufficent gold
    private IVisualElementScheduledItem goldFlashSchedule;
    private Color defaultGoldColor;


    private void Start()
    {
        CloseShopUI();
    }

    private void Awake()
    {
        if (GameManagerScript.Instance != null) 
            GameManagerScript.Instance.playerData.OnGoldChanged += UpdateDisplay;
        else
            GameManagerScript.OnReady += OnGameManagerReady;

        ShopManager.OnShopClose += CloseShopUI;
        ShopManager.OnShopOpen += OpenShopUI;
    }

    private void OnGameManagerReady()
    {
        GameManagerScript.OnReady -= OnGameManagerReady;
        GameManagerScript.Instance.playerData.OnGoldChanged += UpdateDisplay;
    }

    private void OnDestroy()
    {
        GameManagerScript.Instance.playerData.OnGoldChanged -= UpdateDisplay;
        ShopManager.OnShopClose -= CloseShopUI;
        ShopManager.OnShopOpen -= OpenShopUI;
    }

    private void UpdateDisplay(float gold)
    {
        if (goldLabel != null)
        {
            goldLabel.text = $"{gold}g";
        }
        else Debug.LogError("gold label not found");
    }

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
        this.root = root;
        goldLabel = root.Q<Label>("gold_value");
        if (goldLabel != null)
        {
            // We use schedule.Execute once here because resolvedStyle sometimes takes 
            // a split frame to calculate layout values after an OnUIReload redraw
            goldLabel.schedule.Execute(() => {
                defaultGoldColor = goldLabel.resolvedStyle.color;
            });
        }

        UpdateDisplay(GameManagerScript.Instance.playerData.Gold);
        upgradeList = root.Q<VisualElement>("player_upgrade_list");
        upgradeList?.Clear();

        ShowUpgrades(upgradePool.GetRandom(3));

        closeButton = root.Q<VisualElement>("close_btn");
        closeButton.RegisterCallback<ClickEvent>(evt => CloseShopUI());
    }

    public void ShowUpgrades(List<ShopUpgradeData> upgrades)
    {
        if (upgradeList == null) return;

        upgradeList.Clear();

        foreach (ShopUpgradeData upgrade in upgrades)
        {
            var card = upgradeCardTemplate.Instantiate();
            Debug.Log("Upgrade found: " + upgrade);
                                                                                            


            card.Q<Label>("upgrade_name").text = upgrade.upgradeName;
            card.Q<Label>("upgrade_description_text").text = upgrade.upgradeDescription;

            var btn = card.Q<UnityEngine.UIElements.Button>("upgrade_buy_btn");
            btn.text = $"{upgrade.upgradePrice}g";
            btn.clicked += () => OnPurchase(upgrade);

            upgradeList.Add(card);
        }
        if(upgrades.Count < 1)
        {
            Debug.LogError("No upgrades found!");
        }
    }

    private void OpenShopUI()
    {
        gameObject.SetActive(true);
    }

    private void CloseShopUI()
    {
        //if (root != null)
        //{
        //    root.style.display = DisplayStyle.None;

        //}
        gameObject.SetActive(false);
    }

    private void OnPurchase(ShopUpgradeData upgrade) {
        float playerGold = GameManagerScript.Instance.playerData.Gold;
        if (playerGold < upgrade.upgradePrice)
        {
            if (goldLabel != null)
            {
                goldFlashSchedule?.Pause();

                goldLabel.style.color = Color.red;
                goldLabel.style.scale = new Scale(new Vector2(1.15f, 1.15f));

                goldFlashSchedule = goldLabel.schedule.Execute(() =>
                {
                    goldLabel.style.color = defaultGoldColor;
                    goldLabel.style.scale = StyleKeyword.Null;
                }).StartingIn(250); 
            }
        }
        else
        {
            GameManagerScript.Instance.playerData.RemoveGold(upgrade.upgradePrice);
            upgrade.Apply();
        }
    }


}
