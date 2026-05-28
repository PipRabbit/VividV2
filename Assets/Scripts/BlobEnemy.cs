using UnityEngine;

public class BlobEnemy : MonoBehaviour
{
    public float speed = 2.5f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
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

    [System.Obsolete]
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MinigameManager.instance.Fail();
        }
    }
}