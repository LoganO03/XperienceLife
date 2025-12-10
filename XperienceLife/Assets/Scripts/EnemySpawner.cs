using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private float timeBetweenWaves = 10f;
    [SerializeField] private int startEnemiesPerWave = 3;
    [SerializeField] private int enemiesPerWaveIncrease = 1;

    private int currentWave = 0;
    private bool isSpawning = true;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (isSpawning)
        {
            currentWave++;

            // difficultyBonus: +0 on wave 1, +1 on wave 2, etc.
            int difficultyBonus = Mathf.Max(0, currentWave - 1);

            int enemiesThisWave = startEnemiesPerWave + enemiesPerWaveIncrease * (currentWave - 1);

            Debug.Log($"Spawning wave {currentWave} (bonus {difficultyBonus}) with {enemiesThisWave} enemies.");

            for (int i = 0; i < enemiesThisWave; i++)
            {
                if (!isSpawning) yield break;

                SpawnEnemy(difficultyBonus);
                yield return new WaitForSeconds(0.2f);
            }

            float t = 0f;
            while (t < timeBetweenWaves && isSpawning)
            {
                t += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void SpawnEnemy(int difficultyBonus)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        GameObject enemyObj = Instantiate(prefab, sp.position, Quaternion.identity);

        // Apply difficulty scaling to this enemy instance
        var health = enemyObj.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.ApplyDifficultyBonus(difficultyBonus);
        }

        var melee = enemyObj.GetComponent<EnemyMeleeAttack>();
        if (melee != null)
        {
            melee.ApplyDifficultyBonus(difficultyBonus);
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }
}
