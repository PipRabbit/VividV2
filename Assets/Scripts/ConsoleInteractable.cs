using UnityEngine;

public class ConsoleInteractable : MonoBehaviour
{
    [System.Obsolete]
    public void Activate()
    {
        if (MinigameManager.instance != null)
        {
            MinigameManager.instance.StartMinigame();
        }
    }
}