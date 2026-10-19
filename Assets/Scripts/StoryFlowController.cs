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

    public void OnMinigameWon()
    {
        if (minigameWonHandled) return;
        minigameWonHandled = true;

        StartCoroutine(PostMinigameSequence());
    }

    private IEnumerator PostMinigameSequence()
    {
        yield return new WaitForSeconds(0.8f);

        bool dialogueFinished = false;

        DialogueStarter.instance.PlayMinigameWinDialogue(() => dialogueFinished = true);

        yield return new WaitUntil(() => dialogueFinished);

        yield return new WaitForSeconds(0.2f);

        OnPostMinigameDialogueFinished();
    }

    private void OnPostMinigameDialogueFinished()
    {
        GoToNursery();
    }

    // ---------- STAGE: THE NURSERY ----------

    private void GoToNursery()
    {
        RoomTransitionManager rooms = RoomTransitionManager.instance;
        rooms.BeginTransition(rooms.nurserySpawnPoint, OnEnteredNursery);
    }

    [ContextMenu("DEBUG: Go to nursery")]
    private void DebugGoToNursery()
    {
        GoToNursery();
    }

    private void OnEnteredNursery()
    {
        StartCoroutine(NurserySequence());
    }

    private IEnumerator NurserySequence()
    {
        yield return new WaitForSeconds(0.8f);

        bool done = false;
        DialogueStarter.instance.PlayNurseryArrivalDialogue(() => done = true);
        yield return new WaitUntil(() => done);

        yield return new WaitForSeconds(0.4f);

        done = false;
        DialogueStarter.instance.PlayNurseryCheckJournalDialogue(() => done = true);
        yield return new WaitUntil(() => done);

        JournalSystem.instance.ShowNurseryEntry();

        bool journalClosed = false;
        JournalSystem.instance.onJournalClosed = () => journalClosed = true;
        yield return new WaitUntil(() => journalClosed);

        yield return new WaitForSeconds(0.5f);

        done = false;
        DialogueStarter.instance.PlayNurseryJournalReactionDialogue(() => done = true);
        yield return new WaitUntil(() => done);

        yield return new WaitForSeconds(0.3f);

        done = false;
        DialogueStarter.instance.PlayNurseryDiscoveryDialogue(() => done = true);
        yield return new WaitUntil(() => done);

        yield return new WaitForSeconds(0.2f);

        OnNurseryDiscoveryFinished();
    }

    private void OnNurseryDiscoveryFinished()
    {
        if (NurseryQuestManager.instance != null)
            NurseryQuestManager.instance.BeginQuest();
    }

    public void OnBloodCollected()
    {
        Debug.Log("Blood collected. Next stage goes here.");
    }
}