using UnityEngine;

public class ConsoleInteractable : MonoBehaviour
{
    public void Activate()
    {
        if (MinigameManager.instance != null)
        {
            MinigameManager.instance.StartMinigame();
        }
    }
}
