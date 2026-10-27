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
            "But I don't think I'm ready to sleep yet."
        }, ShowControlsTutorial);
    }

    void ShowControlsTutorial()
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Press 1, 2, or 3 to use the item in that slot.",
            "Press E to continue dialogue, and hold E to pick things up."
        });
    }

    public void PlayFlashlightDialogue()
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Ah, there it is.",
            "But I feel dizzy..."
        });

        BookSystem.instance.UnlockBookInteraction();
    }

    public void PlayPillPickupDialogue()
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "My medication. The last one of the day.",
            "The label says to take it with water."
        });
    }

    public void PlayForgotPillDialogue(Action onAfterDialogue)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Ah... I completely forgot about that."
        }, onAfterDialogue);
    }

    public void PlayDizzinessDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Now I'm feeling even more dizzy",
            "I-Migh-t pass- o-ut....",
            "...."
        }, onAfterDialogue);
    }

    public void PlayPostTransitionDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Wait... where am I??",
            "I was in my bedroom before, now I'm in a different room...",
            "I don't get what just happened...",
            "Wait a minute... that looks like my game console.",
            "What is it doing here?",
            "I feel like I'm forgetting things... drastically."
        }, onAfterDialogue);
    }

    public void PlayScribbleJournalDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "I don't remember writing this at all...",
            "Someone has been scribbling in my journal...",
            "It wants me to play my game console...",
            "Since there's nothing else to do, I might as well follow it."
        }, onAfterDialogue);
    }

    public void PlayNurseryArrivalDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Wait... I'm in a new room again.",
            "But this doesn't look like a room from my house.",
            "Where am I?"
        }, onAfterDialogue);
    }

    public void PlayNurseryCheckJournalDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "I need some answers.",
            "Maybe my journal has something about this place."
        }, onAfterDialogue);
    }

    public void PlayNurseryJournalReactionDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "WHO IS WRITING IN MY JOURNAL?!",
            "WHO ARE YOU?! WHAT HAVE YOU DONE TO ME?!",
            "...",
            "Calm down... I have to calm down.",
            "Okay. Let me assess the situation."
        }, onAfterDialogue);
    }

    public void PlayMinigameWinDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "That felt too real...",
            "How was I able to enter the game?",
            "I have no idea what's going on.",
            "I feel on edge, yet my senses are dulling.",
            "I have no idea how to stop this.",
            "Oh no, I feel-- di-zzy agai-nnn...."
        }, onAfterDialogue);
    }

    public void PlayChecklistFoundDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "It's in my handwriting."
        }, onAfterDialogue);
    }

    public void PlayChecklistUsedDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "It's a list of steps... and the first one is collecting a blood sample.",
            "I'll need a syringe and a test tube."
        }, onAfterDialogue);
    }

    public void PlayToolsCollectedDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "That's everything I need for the first step."
        }, onAfterDialogue);
    }

    public void PlayBloodCollectedDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "I got a sample...",
            "I don't know why I'm doing this, but it feels like I've done it before."
        }, onAfterDialogue);
    }

    public void PlayComputerShutdownDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "...What just happened?",
            "The screen shut off by itself.",
            "Wait, something just came out of it.",
            "These look like the results."
        }, onAfterDialogue);
    }

    public void PlayNurseryConclusionDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "None of this belongs in a nursery.",
            "Wait... there's a checklist over there.",
            "Maybe I can use it."
        }, onAfterDialogue);
    }
}
