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

    public int selectedSlot = -1;

    private bool hasBook = false;
    private bool hasWater = false;

    private bool triggeredFlashlightDialogue = false;
    private bool triggeredFinalDialogue = false;

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
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryConsumeSelectedItem();
        }
    }

    [System.Obsolete]
    public void AddItem(ItemType item)
    {
        Sprite spriteToUse = GetSprite(item);
        if (spriteToUse == null) return;

        if (item == ItemType.Book) hasBook = true;
        if (item == ItemType.Water) hasWater = true;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (slotIcons[i].sprite == null)
            {
                slotIcons[i].sprite = spriteToUse;
                slotIcons[i].color = Color.white;
                break;
            }
        }

        // FLASHLIGHT TRIGGER
        if (hasBook && hasWater && !triggeredFlashlightDialogue)
        {
            triggeredFlashlightDialogue = true;

            DialogueSystem.instance.onDialogueEnd = null;

            DialogueSystem.instance.StartDialogue(new string[]
            {
                "Where could I have misplaced my flashlight?"
            });

            FindObjectOfType<FlashlightSpawner>().EnableFlashlightSpawn();
        }

        if (item == ItemType.Flashlight && !triggeredFinalDialogue)
        {
            triggeredFinalDialogue = true;
            DialogueStarter.instance.PlayFlashlightDialogue();
        }

        if (item == ItemType.Pill_1)
        {
            DialogueStarter.instance.PlayPillPickupDialogue();
        }
    }

    void TryConsumeSelectedItem()
    {
        if (selectedSlot < 0) return;
        if (slotIcons[selectedSlot].sprite == null) return;

        if (slotIcons[selectedSlot].sprite == pillSprite)
        {
            ConsumePill();
        }
        else if (slotIcons[selectedSlot].sprite == waterSprite)
        {
            ConsumeWater();
        }
    }

    void ConsumePill()
    {
        slotIcons[selectedSlot].sprite = null;
        slotIcons[selectedSlot].color = new Color(1, 1, 1, 0);

        pillConsumed = true;

        CheckDizzinessTrigger();
    }

    void ConsumeWater()
    {
        slotIcons[selectedSlot].sprite = null;
        slotIcons[selectedSlot].color = new Color(1, 1, 1, 0);

        waterConsumed = true;

        CheckDizzinessTrigger();
    }

    void CheckDizzinessTrigger()
    {
        if (dizzinessTriggered) return;

        if (pillConsumed && waterConsumed)
        {
            dizzinessTriggered = true;

            DialogueStarter.instance.PlayDizzinessDialogue();

            DialogueSystem.instance.onDialogueEnd = StartBlackoutTransition;
        }
    }

    void StartBlackoutTransition()
    {
        RoomTransitionManager.instance.BeginTransition();
        DialogueSystem.instance.onDialogueEnd = null;
    }

    Sprite GetSprite(ItemType item)
    {
        switch (item)
        {
            case ItemType.Book: return bookSprite;
            case ItemType.Water: return waterSprite;
            case ItemType.Flashlight: return flashlightSprite;
            case ItemType.Pill_1: return pillSprite;
        }
        return null;
    }

    public bool SelectedSlotHasBook()
    {
        if (selectedSlot < 0) return false;

        return slotIcons[selectedSlot].sprite == bookSprite;
    }

    public void RemoveBookFromHotbar()
    {
        if (selectedSlot < 0) return;

        if (slotIcons[selectedSlot].sprite == bookSprite)
        {
            slotIcons[selectedSlot].sprite = null;
            slotIcons[selectedSlot].color = new Color(1, 1, 1, 0);
        }
    }
}