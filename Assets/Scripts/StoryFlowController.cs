using System.Collections;
using UnityEngine;

public class StoryFlowController : MonoBehaviour
{
    public static StoryFlowController instance;

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
        DialogueStarter.instance.PlayScribbleJournalDialogue();
    }


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
        Debug.Log("Post-minigame dialogue finished. Next stage goes here.");
    }
}