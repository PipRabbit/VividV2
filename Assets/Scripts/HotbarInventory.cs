using UnityEngine;
using UnityEngine.UI;

public class HotbarInventory : MonoBehaviour
{
    public static HotbarInventory instance;

    public Image[] slotIcons;

    public Sprite bookSprite;
    public Sprite waterSprite;
    public Sprite flashlightSprite;
    public Sprite pillSprite;

    public Sprite checklistSprite;
    public Sprite syringeSprite;
    public Sprite testTubeSprite;
    public Sprite syringeBloodSprite;

    [Tooltip("Optional. If left empty it is found automatically at runtime.")]
    public FlashlightSpawner flashlightSpawner;

    public int selectedSlot = -1;

    public System.Action<ItemType> onItemAdded;

    public System.Action<ItemType> onItemUsed;

    private bool hasBook = false;
    private bool hasWater = false;

    private bool triggeredFlashlightDialogue = false;
    private bool triggeredFinalDialogue = false;

    private bool waterUnlocked = false;   // water can only be used once the pill-pickup dialogue has played

    private bool pillConsumed = false;
    private bool waterConsumed = false;
    private bool dizzinessTriggered = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            slotIcons[i].sprite = null;
            slotIcons[i].color = new Color(1, 1, 1, 0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryConsumeSelectedItem();
        }
    }

    /// <summary>Returns false (and changes nothing) if the item could not be added, e.g. hotbar full.</summary>
    public bool AddItem(ItemType item)
    {
        Sprite spriteToUse = GetSprite(item);
        if (spriteToUse == null) return false;

        int freeSlot = FindFreeSlot();
        if (freeSlot < 0)
        {
            Debug.Log("Hotbar is full, could not pick up: " + item);
            return false;
        }

        slotIcons[freeSlot].sprite = spriteToUse;
        slotIcons[freeSlot].color = Color.white;

        if (item == ItemType.Book) hasBook = true;
        if (item == ItemType.Water) hasWater = true;

        // FLASHLIGHT TRIGGER
        if (hasBook && hasWater && !triggeredFlashlightDialogue)
        {
            triggeredFlashlightDialogue = true;

            if (flashlightSpawner == null)
                flashlightSpawner = FindFirstObjectByType<FlashlightSpawner>();

            DialogueSystem.instance.StartDialogue(new string[]
            {
                "Where could I have misplaced my flashlight?"
            }, SpawnFlashlight);
        }

        if (item == ItemType.Flashlight && !triggeredFinalDialogue)
        {
            triggeredFinalDialogue = true;
            DialogueStarter.instance.PlayFlashlightDialogue();
        }

        if (item == ItemType.Pill_1)
        {
            waterUnlocked = true;
            DialogueStarter.instance.PlayPillPickupDialogue();
        }

        onItemAdded?.Invoke(item);

        return true;
    }

    int FindFreeSlot()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (slotIcons[i].sprite == null)
                return i;
        }
        return -1;
    }

    void SpawnFlashlight()
    {
        if (flashlightSpawner != null)
            flashlightSpawner.SpawnFlashlight();
    }

    void TryConsumeSelectedItem()
    {
        if (selectedSlot < 0 || selectedSlot >= slotIcons.Length) return;
        if (slotIcons[selectedSlot].sprite == null) return;

        // Don't let items be used while a dialogue is on screen (would overwrite it).
        if (DialogueSystem.instance != null && DialogueSystem.instance.IsOpen) return;

        Sprite selected = slotIcons[selectedSlot].sprite;

        if (selected == pillSprite)
        {
            ConsumePill();
        }
        else if (selected == waterSprite)
        {
            if (!waterUnlocked) return;

            ConsumeWater();
        }
        else if (selected == checklistSprite)
        {
            ConsumeChecklist();
        }
    }

    void ConsumePill()
    {
        ClearSelectedSlot();
        pillConsumed = true;
        CheckDizzinessTrigger();
    }

    void ConsumeWater()
    {
        ClearSelectedSlot();
        waterConsumed = true;
        CheckDizzinessTrigger();
    }

    void ConsumeChecklist()
    {
        ClearSelectedSlot();
        onItemUsed?.Invoke(ItemType.Checklist);
    }

    void ClearSelectedSlot()
    {
        slotIcons[selectedSlot].sprite = null;
        slotIcons[selectedSlot].color = new Color(1, 1, 1, 0);
    }

    void CheckDizzinessTrigger()
    {
        if (dizzinessTriggered) return;

        if (pillConsumed && waterConsumed)
        {
            dizzinessTriggered = true;
            DialogueStarter.instance.PlayDizzinessDialogue(StartBlackoutTransition);
        }
    }

    void StartBlackoutTransition()
    {
        RoomTransitionManager.instance.BeginTransition();
    }

    Sprite GetSprite(ItemType item)
    {
        switch (item)
        {
            case ItemType.Book: return bookSprite;
            case ItemType.Water: return waterSprite;
            case ItemType.Flashlight: return flashlightSprite;
            case ItemType.Pill_1: return pillSprite;
            case ItemType.Checklist: return checklistSprite;
            case ItemType.Syringe: return syringeSprite;
            case ItemType.TestTube: return testTubeSprite;
            case ItemType.SyringeWithBlood: return syringeBloodSprite;
        }
        return null;
    }

    public bool HasItem(ItemType item)
    {
        Sprite s = GetSprite(item);
        if (s == null) return false;

        for (int i = 0; i < slotIcons.Length; i++)
            if (slotIcons[i].sprite == s) return true;

        return false;
    }

    public bool RemoveItem(ItemType item)
    {
        Sprite s = GetSprite(item);
        if (s == null) return false;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (slotIcons[i].sprite == s)
            {
                slotIcons[i].sprite = null;
                slotIcons[i].color = new Color(1, 1, 1, 0);
                return true;
            }
        }
        return false;
    }

    public bool SelectedSlotHasBook()
    {
        if (selectedSlot < 0 || selectedSlot >= slotIcons.Length) return false;

        return slotIcons[selectedSlot].sprite == bookSprite;
    }

    public void RemoveBookFromHotbar()
    {
        if (selectedSlot < 0 || selectedSlot >= slotIcons.Length) return;

        if (slotIcons[selectedSlot].sprite == bookSprite)
        {
            ClearSelectedSlot();
        }
    }
}
