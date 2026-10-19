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
        }, ShowControlsTutorial);
    }

    void ShowControlsTutorial()
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "Use 1, 2, and 3 to select items in your hotbar.",
            "Press E to use the selected item."
        });
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
            "I feel like I'm forgetting things... drastically.",
            "I should probably note everything down in my journal."
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

    public void PlayNurseryDiscoveryDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "This looks like... a nursery?",
            "But something feels really wrong about it.",
            "Why is there a sink in here? And a drain in the floor?",
            "And that table... it's covered in tools.",
            "None of this belongs in a baby's room...",
            "Wait... what's that on the blanket?",
            "Th-that's... that's BLOOD!",
            "Oh god... w-whose blood is that?!",
            "What is this place?!"
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

    public void PlayNurseryExploreDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "I can't just stand here.",
            "Let me look around. There has to be something useful in this room."
        }, onAfterDialogue);
    }

    public void PlayChecklistFoundDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "A checklist...",
            "Maybe I should take a closer look at it. (Select it and press E)"
        }, onAfterDialogue);
    }

    public void PlayChecklistUsedDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "It's a list of steps... and the first one is collecting a blood sample.",
            "I'll need a syringe and a test tube.",
            "They have to be somewhere in this room."
        }, onAfterDialogue);
    }

    public void PlayToolsCollectedDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "That's both of them.",
            "And that blood on the floor...",
            "I think I know what I have to do."
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

    public void PlayComputerHintDialogue(Action onAfterDialogue = null)
    {
        DialogueSystem.instance.StartDialogue(new string[]
        {
            "I have the sample now.",
            "That computer in the corner... maybe I can run it through there."
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
}
