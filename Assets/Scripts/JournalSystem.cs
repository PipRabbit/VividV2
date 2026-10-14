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

    public void ShowNurseryEntry()
    {
        bookText.text =
            "Do you know where you are?\n\n" +
            "Of course you do.\n\n" +
            "Doesn't seem unlikely for you to forget.\n\n" +
            "After all, this was how you came to be.";
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