using System;
using System.Collections;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance { get; private set; }

    public static event Action OnReady;

    public static event Action<GameObject> OnPlayerReady;

    public PlayerDataSO playerData;

    public GameObject player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (playerData == null)
            Debug.LogError("PlayerData is null! Assign PlayerStatsAsset in GameManager Inspector.");
        else
            playerData.ResetToDefaults();
    }

    private void Start()
    {
        StartCoroutine(FindPlayer());
        OnReady?.Invoke();
    }



    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.2f);
        }
        OnPlayerReady?.Invoke(player);
    }
}
