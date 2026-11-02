using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum HintId
{
    None,
    ExploreBedroom,
    FindFlashlight,
    ReadBook,
    CloseBook,
    FindPill,
    TakeMedication,
    CheckJournalLivingRoom,
    PlayConsole,
    CheckJournalNursery,
    ExploreNursery,
    FindChecklist,
    UseChecklist,
    FindTools,
    CollectBlood,
    UseComputer,
}

[System.Serializable]
public class HintEntry
{
    public HintId id;
    public string[] hints;
}

public class HintSystem : MonoBehaviour
{
    public static HintSystem instance;

    public RectTransform bubble;
    public TMP_Text bubbleText;
    public Transform player;
    public Vector3 worldOffset = new Vector3(0.9f, 1.1f, 0f);

    public float maxTextWidth = 380f;
    public float minWidth = 120f;
    public Vector2 padding = new Vector2(20f, 12f);

    public float firstHintDelay = 25f;
    public float hintDuration = 6f;
    public float repeatDelay = 20f;
    public float fadeTime = 0.3f;

    public bool separateLines = true;
    public float readTimePerCharacter = 0.06f;

    public List<HintEntry> entries = DefaultEntries();

    private HintEntry current;
    private HintId currentId = HintId.None;
    private int completedShows;
    private float waitTimer;
    private float showTimer;
    private bool showing;

    private CanvasGroup group;
    private Canvas canvas;

    public static void Set(HintId id)
    {
        if (instance != null) instance.SetObjective(id);
    }

    public static void Clear()
    {
        if (instance != null) instance.ClearObjective();
    }

    public static void ClearJournalHints()
    {
        if (instance == null) return;

        if (instance.currentId == HintId.CloseBook ||
            instance.currentId == HintId.CheckJournalLivingRoom ||
            instance.currentId == HintId.CheckJournalNursery)
        {
            instance.ClearObjective();
        }
    }

    public static void Nudge()
    {
        if (instance != null) instance.ResetTimer();
    }

    void Awake()
    {
        instance = this;

        if (bubble != null)
        {
            group = bubble.GetComponent<CanvasGroup>();
            if (group == null) group = bubble.gameObject.AddComponent<CanvasGroup>();

            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;

            canvas = bubble.GetComponentInParent<Canvas>();
            if (canvas != null) canvas = canvas.rootCanvas;

            bubble.anchorMin = new Vector2(0.5f, 0.5f);
            bubble.anchorMax = new Vector2(0.5f, 0.5f);

            if (bubbleText != null)
                bubbleText.enableAutoSizing = false;

            bubble.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) player = playerObject.transform;
        }
    }

    void SetObjective(HintId id)
    {
        currentId = id;
        current = null;

        foreach (HintEntry entry in entries)
        {
            if (entry.id == id && entry.hints != null && entry.hints.Length > 0)
            {
                current = entry;
                break;
            }
        }

        completedShows = 0;
        waitTimer = firstHintDelay;
        HideInstantly();
    }

    void ClearObjective()
    {
        current = null;
        currentId = HintId.None;
        HideInstantly();
    }

    void ResetTimer()
    {
        if (current == null) return;

        waitTimer = completedShows == 0 ? firstHintDelay : repeatDelay;
        HideInstantly();
    }

    void HideInstantly()
    {
        showing = false;

        if (group != null) group.alpha = 0f;
        if (bubble != null) bubble.gameObject.SetActive(false);
    }

    bool IsPaused()
    {
        if (DialogueSystem.instance != null && DialogueSystem.instance.IsOpen) return true;
        if (RoomTransitionManager.instance != null && RoomTransitionManager.instance.IsTransitioning) return true;
        if (ComputerManager.instance != null && ComputerManager.instance.IsOpen) return true;
        if (BookSystem.instance != null && BookSystem.instance.IsOpen) return true;

        return false;
    }

    void Update()
    {
        if (bubble == null || group == null) return;

        if (IsPaused())
        {
            if (showing)
            {
                waitTimer = Mathf.Max(waitTimer, 3f);
                HideInstantly();
            }
        }
        else if (current != null)
        {
            if (showing)
            {
                showTimer -= Time.deltaTime;

                if (showTimer <= 0f)
                {
                    showing = false;
                    completedShows++;
                    waitTimer = repeatDelay;
                }
            }
            else
            {
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                    ShowNext();
            }
        }

        float targetAlpha = showing ? 1f : 0f;
        group.alpha = fadeTime > 0f
            ? Mathf.MoveTowards(group.alpha, targetAlpha, Time.deltaTime / fadeTime)
            : targetAlpha;

        bubble.gameObject.SetActive(showing || group.alpha > 0f);
    }

    void LateUpdate()
    {
        if (bubble == null || !bubble.gameObject.activeSelf) return;

        PositionBubble();
    }

    void ShowNext()
    {
        int tier = Mathf.Min(completedShows, current.hints.Length - 1);

        string message = BuildText(tier);

        bubble.gameObject.SetActive(true);

        if (bubbleText != null)
        {
            bubbleText.text = message;
            ResizeBubble();
        }

        showing = true;
        showTimer = Mathf.Max(hintDuration, message.Length * readTimePerCharacter);

        PositionBubble();
    }

    string BuildText(int tier)
    {
        string text = current.hints[0];

        for (int i = 1; i <= tier; i++)
            text += (separateLines ? "\n" : " ") + current.hints[i];

        return text;
    }

    void ResizeBubble()
    {
        RectTransform textRect = bubbleText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(padding.x, padding.y);
        textRect.offsetMax = new Vector2(-padding.x, -padding.y);

        Vector2 preferred = bubbleText.GetPreferredValues(bubbleText.text, maxTextWidth, 0f);

        float width = Mathf.Max(minWidth, preferred.x + padding.x * 2f);
        float height = preferred.y + padding.y * 2f;

        bubble.sizeDelta = new Vector2(width, height);
    }

    void PositionBubble()
    {
        Camera cam = Camera.main;
        if (player == null || cam == null) return;

        Vector3 rightSide = player.position + worldOffset;
        Vector3 screenPoint = cam.WorldToScreenPoint(rightSide);

        float scale = canvas != null ? canvas.scaleFactor : 1f;
        bool flip = screenPoint.x + bubble.rect.width * scale > Screen.width;

        bubble.pivot = new Vector2(flip ? 1f : 0f, 0.5f);

        Vector3 world = flip
            ? player.position + new Vector3(-worldOffset.x, worldOffset.y, worldOffset.z)
            : rightSide;

        bool overlay = canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay;

        if (overlay)
        {
            Vector3 screen = cam.WorldToScreenPoint(world);
            screen.z = 0f;
            bubble.position = screen;
        }
        else
        {
            bubble.position = world;
        }
    }

    static List<HintEntry> DefaultEntries()
    {
        return new List<HintEntry>
        {
            new HintEntry { id = HintId.ExploreBedroom, hints = new[] {
                "Hmm... maybe I should look around the room.",
                "I was going to read before bed. And I'm thirsty." } },
            new HintEntry { id = HintId.FindFlashlight, hints = new[] {
                "Where did I leave my flashlight...?",
                "It must be somewhere in this room." } },
            new HintEntry { id = HintId.ReadBook, hints = new[] {
                "Maybe I should read my book.",
                "I can take it out from my hotbar." } },
            new HintEntry { id = HintId.CloseBook, hints = new[] {
                "I think I'm done reading for now.",
                "I should put the book away. The button is in the corner." } },
            new HintEntry { id = HintId.FindPill, hints = new[] {
                "Wait... did something just fall nearby?",
                "I should look around my feet." } },
            new HintEntry { id = HintId.TakeMedication, hints = new[] {
                "I should take my medication.",
                "The pill and the water. I can use them from my hotbar." } },
            new HintEntry { id = HintId.CheckJournalLivingRoom, hints = new[] {
                "Maybe I should check my journal.",
                "There might be something new written in it." } },
            new HintEntry { id = HintId.PlayConsole, hints = new[] {
                "Hmm... that game console over there.",
                "Maybe I should play something on it." } },
            new HintEntry { id = HintId.CheckJournalNursery, hints = new[] {
                "Maybe my journal has something about this place.",
                "Let me open it." } },
            new HintEntry { id = HintId.ExploreNursery, hints = new[] {
                "Let me take a closer look at what's in here.",
                "There's a lot in this room I haven't really looked at." } },
            new HintEntry { id = HintId.FindChecklist, hints = new[] {
                "That checklist over there... maybe it can help.",
                "I should pick it up." } },
            new HintEntry { id = HintId.UseChecklist, hints = new[] {
                "Maybe I should read through this checklist.",
                "I can use it from my hotbar." } },
            new HintEntry { id = HintId.FindTools, hints = new[] {
                "A syringe and a test tube... they must be in this room.",
                "I haven't looked everywhere yet." } },
            new HintEntry { id = HintId.CollectBlood, hints = new[] {
                "That stain on the floor...",
                "Maybe I can use what I've gathered on it." } },
            new HintEntry { id = HintId.UseComputer, hints = new[] {
                "That old computer might tell me something about this sample.",
                "I should go and check it out." } },
        };
    }
}