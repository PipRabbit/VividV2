using UnityEngine;

public class BloodInteractable : MonoBehaviour
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

        if (NurseryQuestManager.instance != null)
            NurseryQuestManager.instance.OnBloodInteracted();
    }
}
