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

    void HandleInput()
    {

        if (!bookInteractionUnlocked)
            return;


        if (!bookUnlocked)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (openBookUI.activeSelf)
                {
                    openBookUI.SetActive(false);
                    return;
                }

                if (HotbarInventory.instance.SelectedSlotHasBook())
                {
                    FirstTimeOpen();
                }
            }
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.G))
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


        if (isClosing && bookUnlocked && !hasReadBookOnce)
        {
            hasReadBookOnce = true;

            PillManager.instance.OnBookClosedAfterReading();
        }
    }
}