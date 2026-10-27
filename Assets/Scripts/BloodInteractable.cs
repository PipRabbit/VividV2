using UnityEngine;

public class BloodInteractable : MonoBehaviour, IWorldInteractable
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

    public bool IsAvailable =>
        interactionEnabled &&
        NurseryQuestManager.instance != null &&
        NurseryQuestManager.instance.CanCollectBlood;

    public void Activate()
    {
        if (!IsAvailable) return;

        if (NurseryQuestManager.instance != null)
            NurseryQuestManager.instance.OnBloodInteracted();
    }
}
