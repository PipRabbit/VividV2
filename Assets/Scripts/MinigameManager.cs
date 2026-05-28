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

    [Header("Player")]
    public Transform player;
    public Transform playerSpawnPoint;

    [Header("Enemy")]
    public GameObject blobPrefab;
    public Transform[] spawnPoints;

    private bool gameActive;
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

    [System.Obsolete]
    public void StartMinigame()
    {
        if (gameActive) return;

        gameActive = true;
        timer = surviveTime;

        // SAVE WHERE PLAYER WAS
        savedPlayerPosition = player.position;

        // ENTER MINIGAME
        minigameUI.SetActive(true);
        player.position = playerSpawnPoint.position;

        StartCoroutine(TimerRoutine());
        StartCoroutine(SpawnRoutine());
    }

    [System.Obsolete]
    IEnumerator TimerRoutine()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();

            yield return null;
        }

        End(true);
    }

    [System.Obsolete]
    IEnumerator SpawnRoutine()
    {
        while (gameActive)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(blobPrefab, spawn.position, Quaternion.identity);

            yield return new WaitForSeconds(1.2f);
        }
    }

    [System.Obsolete]
    public void Fail()
    {
        if (!gameActive) return;

        End(false);
    }

    [System.Obsolete]
    void End(bool won)
    {
        gameActive = false;

        StopAllCoroutines();

        foreach (BlobEnemy b in FindObjectsOfType<BlobEnemy>())
            Destroy(b.gameObject);

        minigameUI.SetActive(false);

        // RETURN PLAYER TO ORIGINAL ROOM POSITION
        player.position = savedPlayerPosition;

        if (won)
        {
            Debug.Log("Player survived minigame");
            // future story progression hook
        }
        else
        {
            Debug.Log("Player failed minigame");
            // future fail state hook
        }
    }
}