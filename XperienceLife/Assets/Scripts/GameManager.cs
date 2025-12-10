using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerMovement playerMovement;


    [SerializeField] private GameObject timerUIRoot;
    [SerializeField] private GameObject healthUIRoot;
    [SerializeField] private GameObject joystickUIRoot;
    [SerializeField] private GameObject attackButtonUIRoot;

    private int enemiesDefeated = 0;
    private bool gameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Make sure time is normal when starting a run
        Time.timeScale = 1f;
    }

    public void RegisterEnemyDefeated()
    {
        enemiesDefeated++;
    }

    public void OnTimerEnded()
    {
        if (gameOver) return;
        gameOver = true;
        EndRun(survived: true);
    }

    public void OnPlayerDied()
    {
        if (gameOver) return;
        gameOver = true;
        EndRun(survived: false);
    }

        private void EndRun(bool survived)
    {
        // Stop timer
        if (gameTimer != null)
            gameTimer.StopTimer();

        // Stop spawning new enemies
        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
            enemySpawner.enabled = false;
        }

        if (playerMovement != null)
        {
            var prb = playerMovement.GetComponent<Rigidbody2D>();
            if (prb != null)
            {
                // If you're using linearVelocity:
                prb.linearVelocity = Vector2.zero;

                // If not, or to be safe in general:
                prb.linearVelocity = Vector2.zero;
            }

            playerMovement.enabled = false;
        }


        // Freeze existing enemies
        EnemyMovement[] enemyMovements = FindObjectsOfType<EnemyMovement>();
        foreach (var em in enemyMovements)
        {
            var rb = em.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            em.enabled = false;
        }

        EnemyMeleeAttack[] enemyAttacks = FindObjectsOfType<EnemyMeleeAttack>();
        foreach (var ea in enemyAttacks)
        {
            ea.enabled = false;
        }

        // Final time
        float timeSurvived = gameTimer != null ? gameTimer.ElapsedTime : 0f;

        if (gameOverUI != null)
            gameOverUI.Show(survived, enemiesDefeated, timeSurvived);
    }


}
