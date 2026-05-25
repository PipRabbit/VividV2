using System.Collections;
using UnityEngine;

public class RoomTransitionManager : MonoBehaviour
{
    public static RoomTransitionManager instance;

    [Header("Player")]
    public Transform player;
    public Transform livingRoomSpawnPoint;

    [Header("Effects")]
    public TirednessManager tirednessManager;

    private bool transitioning = false;

    void Awake()
    {
        instance = this;
    }

    public void BeginTransition()
    {
        if (transitioning) return;

        transitioning = true;
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        tirednessManager.StartBlackout();

        yield return new WaitForSeconds(2.5f);

        player.position = livingRoomSpawnPoint.position;

        yield return new WaitForSeconds(0.3f);

        tirednessManager.ResetTiredness();

        transitioning = false;
    }
}