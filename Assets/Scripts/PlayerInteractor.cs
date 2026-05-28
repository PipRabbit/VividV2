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
    private bool isNearInteractable;
    private bool hasInteracted;
    private float animationLength;

    void Start()
    {
        if (uiAnimator != null)
        {
            RuntimeAnimatorController ac = uiAnimator.runtimeAnimatorController;

            foreach (AnimationClip clip in ac.animationClips)
            {
                if (clip.name == "E_Interact_Fill")
                {
                    animationLength = clip.length;
                    interactTime = animationLength;
                    break;
                }
            }
        }

        if (interactIconImage != null)
            interactIconImage.gameObject.SetActive(false);

        if (uiAnimator != null)
            uiAnimator.SetBool("holdE", false);
    }

    [System.Obsolete]
    void Update()
    {
        HandleInput();
    }

    void LateUpdate()
    {
        if (isNearInteractable)
        {
            if (currentItem != null)
                interactIconImage.transform.position =
                    currentItem.transform.position + Vector3.up * 0.3f;

            if (currentConsole != null)
                interactIconImage.transform.position =
                    currentConsole.transform.position + Vector3.up * 0.3f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            isNearInteractable = true;
            currentItem = other.GetComponent<ItemPickup>();
        }

        if (other.CompareTag("Console"))
        {
            isNearInteractable = true;
            currentConsole = other.GetComponent<ConsoleInteractable>();
        }

        if (interactIconImage != null)
            interactIconImage.gameObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentItem = null;
        }

        if (other.CompareTag("Console"))
        {
            currentConsole = null;
        }

        isNearInteractable = false;
        holdTimer = 0f;
        hasInteracted = false;

        if (interactIconImage != null)
            interactIconImage.gameObject.SetActive(false);

        if (uiAnimator != null)
            uiAnimator.SetBool("holdE", false);
    }

    [System.Obsolete]
    void HandleInput()
    {
        if (!isNearInteractable) return;

        bool hasTarget = currentItem != null || currentConsole != null;

        if (!hasTarget) return;

        if (Input.GetKey(KeyCode.E))
        {
            if (uiAnimator != null)
                uiAnimator.SetBool("holdE", true);

            holdTimer += Time.deltaTime;

            if (!hasInteracted && holdTimer >= interactTime)
            {
                Interact();
                hasInteracted = true;
            }
        }
        else
        {
            holdTimer = 0f;
            hasInteracted = false;

            if (uiAnimator != null)
                uiAnimator.SetBool("holdE", false);
        }
    }

    [System.Obsolete]
    void Interact()
    {
        // ITEM PICKUP
        if (currentItem != null)
        {
            Debug.Log("Picked up: " + currentItem.name);

            HotbarInventory.instance.AddItem(currentItem.itemType);
            Destroy(currentItem.gameObject);
        }

        // CONSOLE INTERACTION
        else if (currentConsole != null)
        {
            Debug.Log("Console Activated");

            currentConsole.Activate();
        }

        currentItem = null;
        currentConsole = null;
        isNearInteractable = false;
        holdTimer = 0f;
        hasInteracted = false;

        if (interactIconImage != null)
            interactIconImage.gameObject.SetActive(false);

        if (uiAnimator != null)
            uiAnimator.SetBool("holdE", false);
    }
}