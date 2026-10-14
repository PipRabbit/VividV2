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

    public bool IsTransitioning => transitioning;

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
        if (tirednessManager != null)
            tirednessManager.StartBlackout();

        yield return new WaitForSeconds(2.5f);

        PlayerMovement.Teleport(player, livingRoomSpawnPoint.position);

        yield return new WaitForSeconds(0.3f);

        if (tirednessManager != null)
            tirednessManager.ResetTiredness();

        StoryFlowController.instance.OnEnteredNewRoom();

        transitioning = false;
    }
}
