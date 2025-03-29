using UnityEngine;

public class BlackBanditSummoner : MonoBehaviour
{
    [Header("Summoning Settings")]
    public GameObject whiteBanditPrefab;
    public int maxSummons = 3;
    public float healthThresholdPercent = 0.3f;

    [Header("Voice Line")]
    public AudioClip summonVoiceClip;

    private Transform[] summonPoints;
    private Health health;
    private AudioSource audioSource;
    private int summons = 0;
    private int lastSummonHealth;

    void Start()
    {
        // Tìm 3 điểm spawn theo tên GameObject trong scene
        summonPoints = new Transform[3];
        summonPoints[0] = GameObject.Find("spawn1")?.transform;
        summonPoints[1] = GameObject.Find("spawn2")?.transform;
        summonPoints[2] = GameObject.Find("spawn3")?.transform;

        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        lastSummonHealth = health.health;
    }

    void Update()
    {
        if (summons >= maxSummons || health.health <= 0) return;

        int currentThreshold = Mathf.FloorToInt(health.maxHealth * (1 - (summons + 1) * healthThresholdPercent));
        if (health.health <= currentThreshold && health.health < lastSummonHealth)
        {
            SummonBandit();
            lastSummonHealth = health.health;
        }
    }

    void SummonBandit()
    {
        if (whiteBanditPrefab == null || summonPoints[summons % summonPoints.Length] == null) return;

        Transform point = summonPoints[summons % summonPoints.Length];
        Instantiate(whiteBanditPrefab, point.position, Quaternion.identity);
        summons++;

        PlayVoice(summonVoiceClip);
    }

    void PlayVoice(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
