using System;
using UnityEngine;

public class ShopManager
{
    public static event Action OnShopOpen;
    
    public static event Action OnShopClose;

    public static void OpenShop() => OnShopOpen?.Invoke();
    
    public static void CloseShop() => OnShopClose?.Invoke();
}
