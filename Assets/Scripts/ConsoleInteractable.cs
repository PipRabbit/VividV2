using UnityEngine;

public class ConsoleInteractable : MonoBehaviour, IWorldInteractable
{
    [Tooltip("Stays off until the story unlocks it (after the player reads the journal in the living room).")]
    public bool interactionEnabled = false;

    public bool IsAvailable => interactionEnabled;

    public void EnableInteraction()
    {
        interactionEnabled = true;
    }

    public void Activate()
    {
        if (!interactionEnabled) return;

        if (MinigameManager.instance != null)
        {
            MinigameManager.instance.StartMinigame();
        }
    }
}
