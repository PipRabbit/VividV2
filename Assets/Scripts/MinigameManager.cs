using System.Collections;
using TMPro;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager instance;

    [Header("UI")]
    public GameObject minigameUI;
    public TMP_Text timerText;

    [Header("Gameplay")]
    public float surviveTime = 20f;
    public float spawnInterval = 1.2f;

    [Header("Player")]
    public Transform player;
    public Transform playerSpawnPoint;

    [Header("Enemy")]
    public GameObject blobPrefab;
    public Transform[] spawnPoints;

    private bool gameActive;
    private bool completed;
    private float timer;

    private Vector3 savedPlayerPosition;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        minigameUI.SetActive(false);
    }

    public void StartMinigame()
    {
        if (gameActive || completed) return;

        if (player == null || playerSpawnPoint == null || blobPrefab == null ||
            spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("MinigameManager: missing player, spawn point, prefab or enemy spawn points.");
            return;
        }

        gameActive = true;
        timer = surviveTime;

        savedPlayerPosition = player.position;

        minigameUI.SetActive(true);
        PlayerMovement.Teleport(player, playerSpawnPoint.position);

        StartCoroutine(TimerRoutine());
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator TimerRoutine()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(Mathf.Max(timer, 0f)).ToString();

            yield return null;
        }

        End(true);
    }

    IEnumerator SpawnRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        while (gameActive)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(blobPrefab, spawn.position, Quaternion.identity);

            yield return wait;
        }
    }

    public void Fail()
    {
        if (!gameActive) return;

        End(false);
    }

    void End(bool won)
    {
        gameActive = false;

        StopAllCoroutines();

        foreach (BlobEnemy b in FindObjectsByType<BlobEnemy>(FindObjectsSortMode.None))
            Destroy(b.gameObject);

        minigameUI.SetActive(false);

        PlayerMovement.Teleport(player, savedPlayerPosition);

        if (won)
        {
            Debug.Log("Player survived minigame");

            completed = true;

            if (StoryFlowController.instance != null)
                StoryFlowController.instance.OnMinigameWon();
        }
        else
        {
            Debug.Log("Player failed minigame");
        }
    }
}