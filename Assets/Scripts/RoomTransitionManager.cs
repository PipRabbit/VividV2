using System.Collections;
using UnityEngine;

public class RoomTransitionManager : MonoBehaviour
{
    public static RoomTransitionManager instance;

    [Header("Player")]
    public Transform player;

    [Header("Room Spawn Points")]
    public Transform livingRoomSpawnPoint;
    public Transform nurserySpawnPoint;

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
        BeginTransition(livingRoomSpawnPoint, () => StoryFlowController.instance.OnEnteredNewRoom());
    }

    public void BeginTransition(Transform destination, System.Action onArrived)
    {
        if (transitioning) return;

        if (destination == null)
        {
            Debug.LogWarning("RoomTransitionManager: destination spawn point is not assigned.");
            return;
        }

        transitioning = true;
        StartCoroutine(Transition(destination, onArrived));
    }

    IEnumerator Transition(Transform destination, System.Action onArrived)
    {
        if (tirednessManager != null)
            tirednessManager.StartBlackout();

        yield return new WaitForSeconds(2.5f);

        PlayerMovement.Teleport(player, destination.position);

        yield return new WaitForSeconds(0.3f);

        if (tirednessManager != null)
            tirednessManager.ResetTiredness();

        onArrived?.Invoke();

        transitioning = false;
    }
}