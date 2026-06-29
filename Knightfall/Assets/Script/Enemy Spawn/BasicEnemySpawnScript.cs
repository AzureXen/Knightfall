using System.Collections;
using UnityEngine;

// This is basic enemy spawn script, and can be used right away.
public class BasicEnemySpawnScript : EnemySpawnScript
{
    [SerializeField] private GameObject spawnWarningPrefab;
    [SerializeField] private Color warningColor = Color.red;

    protected override void Start()
    {
        StartCoroutine(SpawnSequence());
    }

    private IEnumerator SpawnSequence()
    {
        GameObject warning = Instantiate(spawnWarningPrefab, transform.position, Quaternion.identity);
        SpriteRenderer sr = warning.GetComponent<SpriteRenderer>();

        float elapsed = 0f;
        while (elapsed < spawnDelayTime)
        {
            float alpha = Mathf.PingPong(elapsed * 4f, 1f);
            sr.color = new Color(warningColor.r, warningColor.g, warningColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(warning);
        SpawnEnemy();
    }

    protected override void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        SpriteRenderer enemySR = enemy.GetComponentInChildren<SpriteRenderer>();

        if (enemySR != null)
        {
            Debug.Log("found sr: " + enemySR.gameObject.name);
            StartCoroutine(FlashWhiteRoutine(enemySR));
        }
        else Debug.Log("Cant find enemy's sprite renderer :(");
    }

    private IEnumerator FlashWhiteRoutine(SpriteRenderer enemySR)
    {
        Color originalColor = enemySR.color;
        Material originalMaterial = enemySR.material;

        enemySR.material = new Material(Shader.Find("GUI/Text Shader"));

        enemySR.color = Color.white;

        yield return new WaitForSeconds(0.12f);

        if (enemySR != null)
        {
            enemySR.material = originalMaterial;
            enemySR.color = originalColor;
        }
    }
}
