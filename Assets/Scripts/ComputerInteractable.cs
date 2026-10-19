using UnityEngine;

public class ComputerInteractable : MonoBehaviour
{
    public bool interactionEnabled = false;

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
        if (!interactionEnabled) return;

        if (ComputerManager.instance != null)
            ComputerManager.instance.OpenComputer();
    }
}
