using System;
using TMPro;
using UnityEngine;

public class JournalSystem : MonoBehaviour
{
    public static JournalSystem instance;

    public TMP_Text bookText;

    public Action onJournalClosed;

    void Awake()
    {
        instance = this;
    }

    public void ShowFirstEntry()
    {
        bookText.text =
            "Note to self,\n\nI left the last pill I need to take on my bed.";
    }

    public void ShowLivingRoomEntry()
    {
        bookText.text =
            "Note to self,\n\nThis is my living room.\n\n" +
            "How about playing a quick game on my game console?";
    }

    public void TriggerScribbleEvent()
    {
        onJournalClosed = StoryFlowController.instance.OnJournalClosedAfterScribble;
    }

    public void OnJournalClosedInternal()
    {
        onJournalClosed?.Invoke();
        onJournalClosed = null;
    }

    public void Clear()
    {
        bookText.text = "";
    }
}