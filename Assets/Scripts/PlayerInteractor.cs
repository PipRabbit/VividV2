using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    [Header("UI")]
    public Image interactIconImage;
    public Animator uiAnimator;

    [Header("Settings")]
    public float interactTime = 1f;

    private ItemPickup currentItem;
    private ConsoleInteractable currentConsole;

    private float holdTimer;
    private bool hasInteracted;
    private bool promptVisible;

    bool HasTarget => currentItem != null || currentConsole != null;

    void Start()
    {
        if (uiAnimator != null && uiAnimator.runtimeAnimatorController != null)
        {
            foreach (AnimationClip clip in uiAnimator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "E_Interact_Fill")
                {
                    interactTime = clip.length;
                    break;
                }
            }
        }

        ShowPrompt(false);
    }

    void Update()
    {
        HandleInput();
    }

    void LateUpdate()
    {
        if (interactIconImage == null || !HasTarget) return;

        Transform target = currentItem != null
            ? currentItem.transform
            : currentConsole.transform;

        interactIconImage.transform.position = target.position + Vector3.up * 0.3f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            ItemPickup item = other.GetComponentInParent<ItemPickup>();
            if (item == null) return;

            currentItem = item;
            currentConsole = null;
            BeginTarget();
        }
        else if (other.CompareTag("Console"))
        {
            ConsoleInteractable console = other.GetComponentInParent<ConsoleInteractable>();
            if (console == null) return;

            currentConsole = console;
            currentItem = null;
            BeginTarget();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            if (currentItem != null && other.GetComponentInParent<ItemPickup>() == currentItem)
                ClearTarget();
        }
        else if (other.CompareTag("Console"))
        {
            if (currentConsole != null && other.GetComponentInParent<ConsoleInteractable>() == currentConsole)
                ClearTarget();
        }
    }

    void HandleInput()
    {
        if (!HasTarget)
        {
            if (promptVisible) ClearTarget();
            return;
        }

        bool dialogueOpen = DialogueSystem.instance != null && DialogueSystem.instance.IsOpen;

        if (Input.GetKey(KeyCode.E) && !dialogueOpen)
        {
            SetHoldAnimation(true);

            holdTimer += Time.deltaTime;

            if (!hasInteracted && holdTimer >= interactTime)
            {
                hasInteracted = true;
                Interact();
            }
        }
        else
        {
            holdTimer = 0f;
            hasInteracted = false;
            SetHoldAnimation(false);
        }
    }

    void Interact()
    {
        if (currentItem != null)
        {
            if (!HotbarInventory.instance.AddItem(currentItem.itemType))
                return;

            Debug.Log("Picked up: " + currentItem.name);
            Destroy(currentItem.gameObject);
        }

        else if (currentConsole != null)
        {
            Debug.Log("Console Activated");
            currentConsole.Activate();
        }

        ClearTarget();
    }

    void BeginTarget()
    {
        holdTimer = 0f;
        hasInteracted = false;
        ShowPrompt(true);
    }

    void ClearTarget()
    {
        currentItem = null;
        currentConsole = null;
        holdTimer = 0f;
        hasInteracted = false;
        ShowPrompt(false);
    }

    void ShowPrompt(bool show)
    {
        promptVisible = show;

        if (interactIconImage != null)
            interactIconImage.gameObject.SetActive(show);

        if (!show)
            SetHoldAnimation(false);
    }

    void SetHoldAnimation(bool holding)
    {
        if (uiAnimator != null)
            uiAnimator.SetBool("holdE", holding);
    }
}
