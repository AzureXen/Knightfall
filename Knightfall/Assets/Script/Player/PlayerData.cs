using System;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public event Action<float> OnGoldChanged;

    [SerializeField] private float Gold = 0;

    public float GetGold() {  return Gold; }
    public void SetGold(float value) { this.Gold = value; OnGoldChanged?.Invoke(Gold); }

    public void AddGold(float value)
    {
        Gold += value;
        OnGoldChanged?.Invoke(Gold);
    }

    public void RemoveGold(float value)
    { 
        Gold -= value;
        OnGoldChanged?.Invoke(Gold);
    }


}
