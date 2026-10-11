using System;
using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    public static DialogueStarter instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PlayIntro();
    }

    public void PlayIntro()
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "I'm tired...",
            "Those pills really were strong.",
            "Though according to my doctor, there are some side effects to the drug.",
            "I don't remember what he mentioned, but tiredness seems like the most evident symptom.",
            "I should probably head to bed.",
            "But I think I'll read my book and have a glass of water."
        });

        DialogueSystem.instance.onDialogueEnd = ShowControlsTutorial;
    }

    void ShowControlsTutorial()
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Use 1, 2, and 3 to select items in your hotbar.",
            "Press E to use the selected item."
        });

        DialogueSystem.instance.onDialogueEnd = null;
    }

    public void PlayFlashlightDialogue()
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Ah, there it is.",
            "But I feel dizzy...",
            "Did I get my book?"
        });

        BookSystem.instance.UnlockBookInteraction();
    }

    public void PlayPillPickupDialogue()
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "I should probably eat this.",
            "Luckily I have some water with me."
        });
    }

    public void PlayForgotPillDialogue(Action onAfterDialogue)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Ah I completely forgot about that",
            "Better go eat that last pill"
        });

        DialogueSystem.instance.onDialogueEnd = () =>
        {
            onAfterDialogue?.Invoke();
            DialogueSystem.instance.onDialogueEnd = null;
        };
    }

    public void PlayDizzinessDialogue()
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Now I'm feeling even more dizzy",
            "I-Migh-t pass- o-ut....",
            "...."
        });
    }

    public void PlayPostTransitionDialogue()
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Wait... where am I??",
            "I was in my bedroom before, now I'm in a different room...",
            "I don't get what just happened...",
            "Wait a minute... that looks like my game console.",
            "What is it doing here?",
            "I feel like I'm forgetting things... drastically.",
            "I should probably note everything down in my journal."
        });
    }

    public void PlayScribbleJournalDialogue()
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "I don't remember writing this at all...",
            "Someone has been scribbling in my journal...",
            "It wants me to play my game console...",
            "Since there's nothing else to do, I might as well do it."
        });
    }
}