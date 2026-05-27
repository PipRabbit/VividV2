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
}