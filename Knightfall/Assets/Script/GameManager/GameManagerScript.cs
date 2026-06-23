using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance { get; private set; }
    public PlayerData playerData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        playerData = new PlayerData();
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManager Awake - Instance set: " + (Instance != null));
    }

    public void AddGold(float amount)
    {
        playerData.AddGold(amount);
    }
    public void RemoveGold(float amount)
    {
        playerData.RemoveGold(amount);
    }
}
