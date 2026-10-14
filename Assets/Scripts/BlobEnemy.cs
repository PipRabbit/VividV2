using UnityEngine;

public class BlobEnemy : MonoBehaviour
{
    public float speed = 2.5f;
    private Transform player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogWarning("BlobEnemy: no object tagged 'Player' found.");
    }

    void Update()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && MinigameManager.instance != null)
        {
            MinigameManager.instance.Fail();
        }
    }
}
