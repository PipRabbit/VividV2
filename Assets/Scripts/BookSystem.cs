using UnityEngine;

public class BookSystem : MonoBehaviour
{
    public static BookSystem instance;

    public GameObject openBookUI;
    public GameObject bookButtonIcon;

    private bool bookUnlocked = false;
    private bool bookInteractionUnlocked = false;
    private bool hasReadBookOnce = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        openBookUI.SetActive(false);
        bookButtonIcon.SetActive(false);
    }

    void Update()
    {
        HandleInput();
    }

    public void UnlockBookInteraction()
    {
        bookInteractionUnlocked = true;
    }

    public void ForceUnlock()
    {
        bookInteractionUnlocked = true;
        bookUnlocked = true;
        hasReadBookOnce = true;

        bookButtonIcon.SetActive(true);
        JournalSystem.instance.ShowFirstEntry();
    }

    void HandleInput()
    {
        bool gPressed = Input.GetKeyDown(KeyCode.G);

        if (!bookInteractionUnlocked)
        {
            if (gPressed) Debug.Log("Book blocked: interaction is not unlocked yet (flashlight not picked up).");
            return;
        }

        if (DialogueSystem.instance != null && DialogueSystem.instance.IsOpen)
        {
            if (gPressed) Debug.Log("Book blocked: a dialogue is open.");
            return;
        }

        if (!bookUnlocked)
        {
            if (gPressed) Debug.Log("Book blocked: it has not been opened for the first time yet (select it and press E).");

            if (Input.GetKeyDown(KeyCode.E) && HotbarInventory.instance.SelectedSlotHasBook())
            {
                FirstTimeOpen();
            }
        }
        else
        {
            if (gPressed)
            {
                ToggleBook();
            }
        }
    }

    void FirstTimeOpen()
    {
        openBookUI.SetActive(true);

        bookUnlocked = true;

        HotbarInventory.instance.RemoveBookFromHotbar();
        bookButtonIcon.SetActive(true);

        JournalSystem.instance.ShowFirstEntry();
    }

    void ToggleBook()
    {
        bool isClosing = openBookUI.activeSelf;

        openBookUI.SetActive(!openBookUI.activeSelf);

        Debug.Log("Book " + (openBookUI.activeSelf ? "opened" : "closed"));

        if (isClosing)
        {
            JournalSystem.instance.OnJournalClosedInternal();

            if (bookUnlocked && !hasReadBookOnce)
            {
                hasReadBookOnce = true;
                PillManager.instance.OnBookClosedAfterReading();
            }
        }
    }
}
