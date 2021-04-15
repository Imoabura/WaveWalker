using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            Debug.LogWarning("Instance Destroyed, there can only be 1 GameController!");
        }
        else
        {
            instance = this;
        }

        onEnemyDestroyedCallback += EnemyDied;
    }

    public delegate void OnEnemyDestroyed();
    public OnEnemyDestroyed onEnemyDestroyedCallback;

    public delegate void OnGameStateChanged(GameState state);
    public OnGameStateChanged onGameStateChangedCallback;

    public delegate void OnPlayerStateChange(PlayerController.PlayerState state);
    public OnPlayerStateChange onPlayerStateChangedCallback;

    [Header("Enemy Spawning Properties")]
    [SerializeField] GameObject enemyDummyPrefab;
    [SerializeField] int totalRounds = 1;
    [SerializeField] int enemiesPerRound = 1;
    [SerializeField] float timeBetweenEnemySpawns = 0f;
    [SerializeField] float timeBetweenRounds = 5f;

    [SerializeField, Range(0, 5f)] float minXSpawn = 5f;
    [SerializeField, Range(5, 20f)] float maxXSpawn = 20f;
    [SerializeField, Range(0, 5f)] float minZSpawn = 5f;
    [SerializeField, Range(5, 20f)] float maxZSpawn = 20f;

    Vector3 minSpawnRange = Vector3.zero;

    int currentRound = 0;
    int enemiesAlive = 0;
    bool isGameOver = false;

    GameState currentState;

    Coroutine slowTimeCoroutine = null;

    public enum GameState
    {
        GAME_START,
        BETWEEN_ROUNDS,
        SPAWNING_ENEMIES,
        ENEMIES_ALIVE,
        GAME_OVER
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.GAME_START;
        minSpawnRange = new Vector3(minXSpawn, 0, minZSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == GameState.ENEMIES_ALIVE)
        {
            if (enemiesAlive <= 0)
            {
                enemiesAlive = 0;
                if (currentRound >= totalRounds)
                {
                    TransitionState(GameState.GAME_OVER);
                }
                else
                {
                    TransitionState(GameState.BETWEEN_ROUNDS);
                }
            }
        }
    }

    public void SlowTime(float duration, float slowPercent, bool overwrite)
    {
        if (slowTimeCoroutine != null)
        {
            if (!overwrite)
            {
                return;
            }
            StopCoroutine(slowTimeCoroutine);
        }

        slowTimeCoroutine = StartCoroutine(SlowTimer(duration, slowPercent));
    }

    IEnumerator SlowTimer(float duration, float slowPercent)
    {
        float timeSlow = Mathf.Clamp(slowPercent, 0f, 1f);
        Time.timeScale = timeSlow;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        slowTimeCoroutine = null;
    }

    public void TransitionState(GameState newState)
    {
        if (newState == currentState)
        {
            return;
        }

        currentState = newState;
        switch (newState)
        {
            default:
                break;
            case GameState.BETWEEN_ROUNDS:
                StartWaitTime(timeBetweenRounds);
                break;
            case GameState.SPAWNING_ENEMIES:
                StartRound(enemiesPerRound, timeBetweenEnemySpawns);
                break;
            case GameState.GAME_OVER:
                GameOver();
                break;
        }

        onGameStateChangedCallback.Invoke(currentState);
    }

    public void StartGame()
    {
        TransitionState(GameState.BETWEEN_ROUNDS);
    }

    void StartWaitTime(float duration)
    {
        StartCoroutine(WaitTimer(duration));
    }

    IEnumerator WaitTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        TransitionState(GameState.SPAWNING_ENEMIES);
    }

    void StartRound(int enemyCount, float timeBetweenEnemies)
    {
        currentRound++;
        StartCoroutine(SpawnEnemies(enemyCount, timeBetweenEnemies));
    }

    void SpawnEnemy(Vector3 position)
    {
        GameObject enemy = Instantiate(enemyDummyPrefab, position, Quaternion.identity);
        Debug.Log($"Enemy spawned @ {position}");
        enemiesAlive++;
    }

    IEnumerator SpawnEnemies(int enemyCount, float timeBetweenSpawns)
    {
        Debug.Log($"maxX: {maxXSpawn} / minX: {minXSpawn} / maxZ: {maxZSpawn} / minZ: {minZSpawn}");

        Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < enemyCount; ++i)
        {
            float xPos = Mathf.Lerp(-maxXSpawn, maxXSpawn, Random.Range(0, 1f));

            float zPos = Mathf.Lerp(-maxZSpawn, maxZSpawn, Random.Range(0, 1f));

            Vector3 pos = new Vector3(xPos, 0, zPos);

            if (pos.sqrMagnitude < minSpawnRange.sqrMagnitude)
            {
                pos = pos.normalized * minSpawnRange.magnitude;
            }

            SpawnEnemy(pos + Vector3.up * .5f);

            if (enemiesAlive >= enemyCount)
            {
                break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        TransitionState(GameState.ENEMIES_ALIVE);
    }

    public void EnemyDied()
    {
        enemiesAlive--;
    }

    void GameOver()
    {
        isGameOver = true;
        StartCoroutine(StartGameOver());
    }

    IEnumerator StartGameOver()
    {
        yield return new WaitForSecondsRealtime(1f);
        UIManager.instance.GameOver();
    }
}
