using System.Collections;
using TMPro;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private GameObject door;

    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject Skeleton;
    [SerializeField] private GameObject Necromancer;
    [SerializeField] private GameObject Skull;
    [SerializeField] private WaveSystemTrigger waveTrigger;

    [SerializeField] private GameObject skullKnightPrefab; // Final boss
    [SerializeField] private GameObject waveState;

    private int wave = 0;
    private int baseSkeletonCount = 6;
    private int baseNecromancerCount = 4;
    private int baseSkullCount = 10;
    private float spawnInterval = 2f;
    private int baseSpawnBatchSize = 10;

    private int remainingSkeletons;
    private int remainingNecromancers;
    private int remainingSkulls;
    private int spawnBatchSize;
    private bool isSpawning = false;
    private bool hasStarted = false;
    private bool finalBossSpawned = false;

    private void Start()
    {
        if (waveTrigger != null)
        {
            waveTrigger.OnPlayerEnterTrigger += (sender, args) => StartCoroutine(WaveBreak());
        }
    }

    private IEnumerator WaveBreak()
    {
        if (hasStarted) yield break;
        hasStarted = true;

        door.SetActive(true);
        VanSoundManager.StopBGM();
        VanSoundManager.PlayBGM(SoundType.WAVEBATTLEBGM, 0.4f);

        while (wave < 3) // Stops after 3 waves
        {
            StartCoroutine(WaveCount(wave));
            yield return new WaitForSeconds(2f); // 2-second break before each wave starts
            StartNextWave();
            yield return new WaitUntil(() => !isSpawning && GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
        }

        // After all waves, spawn the Final Boss
        if (wave == 3 && !finalBossSpawned)
        {
            SpawnFinalBoss();
            StartCoroutine(BossWave());
            StartCoroutine(DoorOpen());
        }
    }

    private void StartNextWave()
    {
        wave++;
        remainingSkeletons = baseSkeletonCount * (int)Mathf.Pow(4, wave - 1);
        remainingNecromancers = baseNecromancerCount * (int)Mathf.Pow(2, wave - 1);
        remainingSkulls = baseSkullCount * (int)Mathf.Pow(4, wave - 1);
        spawnBatchSize = baseSpawnBatchSize + (wave - 1) * 5;

        isSpawning = true;
        InvokeRepeating(nameof(SpawnBurst), 0f, spawnInterval);
    }

    private void SpawnBurst()
    {
        if (remainingSkeletons <= 0 && remainingNecromancers <= 0 && remainingSkulls <= 0)
        {
            CancelInvoke(nameof(SpawnBurst)); // Stop spawning
            isSpawning = false;
            return;
        }

        for (int i = 0; i < spawnBatchSize; i++)
        {
            int enemyType = Random.Range(0, 3); // 0 = Skeleton, 1 = Necromancer, 2 = Skull

            if (enemyType == 0 && remainingSkeletons > 0)
            {
                SpawnEnemy(Skeleton);
                remainingSkeletons--;
            }
            else if (enemyType == 1 && remainingNecromancers > 0)
            {
                SpawnEnemy(Necromancer);
                remainingNecromancers--;
            }
            else if (enemyType == 2 && remainingSkulls > 0)
            {
                SpawnEnemy(Skull);
                remainingSkulls--;
            }
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 spawnPosition = spawnPos.position + new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), 0);
        GameObject spawnInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private void SpawnFinalBoss()
    {
        finalBossSpawned = true;
        VanSoundManager.PlaySound(SoundType.BOSSSPAWNS);
        skullKnightPrefab.SetActive(true);
        UndeadBossHealth bossHealth = skullKnightPrefab.GetComponent<UndeadBossHealth>();
        if (bossHealth != null)
        {
            bossHealth.OnDeath += OnFinalBossDefeated; // Subscribe to event
            VanSoundManager.StopBGM();
            VanSoundManager.PlayBGM(SoundType.BOSSBGM, 0.4f);
        }
    }

    private void OnFinalBossDefeated()
    {
        StartCoroutine(DoorOpen());
    }

    private IEnumerator DoorOpen()
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

        VanSoundManager.StopBGM();
        VanSoundManager.PlayBGM(SoundType.OPENINGBGM, 0.4f);
        Animator doorAni = door.GetComponent<Animator>();
        doorAni.SetTrigger("open");
        yield return new WaitForSeconds(2f);
        door.SetActive(false);
    }

    private IEnumerator WaveCount(int waveCount)
    {
        TextMeshProUGUI textMeshPro = waveState.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMeshPro.text = "Wave " + (waveCount + 1).ToString();
        yield return new WaitForSeconds(1.5f);
        textMeshPro.text = null;
    }

    private IEnumerator BossWave()
    {
        TextMeshProUGUI textMeshPro = waveState.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMeshPro.text = "FINAL BOSS";
        yield return new WaitForSeconds(1.5f);
        textMeshPro.text = null;
    }
}
