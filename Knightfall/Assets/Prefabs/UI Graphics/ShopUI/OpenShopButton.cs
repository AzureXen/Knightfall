using UnityEngine;
using UnityEngine.UI;

public class OpenShopButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        OpenShopUI();
    }

    private void OpenShopUI()
    {
        ShopManager.OpenShop();
    }


}
