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

        DialogueSystem.instance.onDialogueEnd = () =>
        {
            dialogueFinished = true;
        };

        DialogueStarter.instance.PlayPostTransitionDialogue();

        yield return new WaitUntil(() => dialogueFinished);

        yield return new WaitForSeconds(0.2f);

        JournalSystem.instance.ShowLivingRoomEntry();

        JournalSystem.instance.TriggerScribbleEvent();
    }

    public void OnJournalClosedAfterScribble()
    {
        DialogueStarter.instance.PlayScribbleJournalDialogue();
    }
}