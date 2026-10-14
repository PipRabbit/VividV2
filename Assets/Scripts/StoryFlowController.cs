using System.Collections;
using UnityEngine;

public class StoryFlowController : MonoBehaviour
{
    public static StoryFlowController instance;

    [Tooltip("The game console. Optional: found automatically if left empty.")]
    public ConsoleInteractable console;

    private void Awake()
    {
        instance = this;
    }

    public void OnEnteredNewRoom()
    {
        StartCoroutine(PostRoomSequence());
    }

    private IEnumerator PostRoomSequence()
    {
        yield return new WaitForSeconds(0.8f);

        bool dialogueFinished = false;

        DialogueStarter.instance.PlayPostTransitionDialogue(() => dialogueFinished = true);

        yield return new WaitUntil(() => dialogueFinished);

        yield return new WaitForSeconds(0.2f);

        JournalSystem.instance.ShowLivingRoomEntry();

        JournalSystem.instance.TriggerScribbleEvent();
    }

    public void OnJournalClosedAfterScribble()
    {
        // The console only becomes usable once the player has read the journal
        // and the follow-up dialogue has finished.
        DialogueStarter.instance.PlayScribbleJournalDialogue(UnlockConsole);
    }

    private void UnlockConsole()
    {
        if (console == null)
            console = FindFirstObjectByType<ConsoleInteractable>();

        if (console != null)
            console.EnableInteraction();
    }

    // ---------- STAGE: AFTER THE MINIGAME ----------

    private bool minigameWonHandled = false;

    // Called by MinigameManager when the player survives.
    public void OnMinigameWon()
    {
        if (minigameWonHandled) return;
        minigameWonHandled = true;

        StartCoroutine(PostMinigameSequence());
    }

    private IEnumerator PostMinigameSequence()
    {
        // Short beat so the player sees themselves back in the room first.
        yield return new WaitForSeconds(0.8f);

        bool dialogueFinished = false;

        DialogueStarter.instance.PlayMinigameWinDialogue(() => dialogueFinished = true);

        yield return new WaitUntil(() => dialogueFinished);

        yield return new WaitForSeconds(0.2f);

        OnPostMinigameDialogueFinished();
    }

    // NEXT-STAGE HOOK: runs right after "...dizzy agai-nnn....".
    // Put the next blackout / room change / story beat here.
    private void OnPostMinigameDialogueFinished()
    {
        Debug.Log("Post-minigame dialogue finished. Next stage goes here.");
    }
}