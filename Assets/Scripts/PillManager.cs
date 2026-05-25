using UnityEngine;

public class PillManager : MonoBehaviour
{
    public static PillManager instance;

    public GameObject pillObject;

    private bool bookReadTriggered = false;

    void Awake()
    {
        instance = this;
    }

    public void OnBookClosedAfterReading()
    {
        if (bookReadTriggered) return;

        bookReadTriggered = true;

        DialogueStarter.instance.PlayForgotPillDialogue(SpawnPill);
    }

    void SpawnPill()
    {
        if (pillObject != null)
            pillObject.SetActive(true);
    }
}