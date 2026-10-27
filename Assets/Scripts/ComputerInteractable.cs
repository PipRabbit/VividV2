using UnityEngine;

public class ComputerInteractable : MonoBehaviour, IWorldInteractable
{
    public bool interactionEnabled = false;
    public string[] inactiveLines;

    public bool IsAvailable =>
        interactionEnabled || (inactiveLines != null && inactiveLines.Length > 0);

    public void EnableInteraction()
    {
        interactionEnabled = true;
    }

    public void DisableInteraction()
    {
        interactionEnabled = false;
    }

    public void Activate()
    {
        if (!interactionEnabled)
        {
            if (inactiveLines != null && inactiveLines.Length > 0 && DialogueSystem.instance != null)
                DialogueSystem.instance.StartDialogue(inactiveLines);

            return;
        }

        if (ComputerManager.instance != null)
            ComputerManager.instance.OpenComputer();
    }
}
