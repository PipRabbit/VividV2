using UnityEngine;

public class ConsoleInteractable : MonoBehaviour
{
    [Tooltip("Stays off until the story unlocks it (after the player reads the journal in the living room).")]
    public bool interactionEnabled = false;

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