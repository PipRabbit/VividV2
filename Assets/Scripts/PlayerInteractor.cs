using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    [Header("UI")]
    public Image interactIconImage;
    public Animator uiAnimator;

    [Header("Settings")]
    public float interactTime = 1f;

    private readonly List<MonoBehaviour> nearby = new List<MonoBehaviour>();
    private MonoBehaviour target;

    private float holdTimer;
    private bool promptVisible;
    private bool waitForERelease;

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
        if (interactIconImage == null || target == null) return;

        interactIconImage.transform.position = target.transform.position + Vector3.up * 0.3f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        MonoBehaviour candidate = FindCandidate(other);

        if (candidate != null && !nearby.Contains(candidate))
            nearby.Add(candidate);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        MonoBehaviour candidate = FindCandidate(other);

        if (candidate != null)
            nearby.Remove(candidate);
    }

    MonoBehaviour FindCandidate(Collider2D other)
    {
        if (!other.CompareTag("Interactable") && !other.CompareTag("Console"))
            return null;

        ItemPickup item = other.GetComponentInParent<ItemPickup>();
        if (item != null) return item;

        return other.GetComponentInParent<IWorldInteractable>() as MonoBehaviour;
    }

    bool IsBlocked()
    {
        if (DialogueSystem.instance != null && DialogueSystem.instance.IsOpen) return true;
        if (RoomTransitionManager.instance != null && RoomTransitionManager.instance.IsTransitioning) return true;
        if (ComputerManager.instance != null && ComputerManager.instance.IsOpen) return true;

        return false;
    }

    bool IsAvailable(MonoBehaviour candidate)
    {
        if (candidate is ItemPickup) return true;

        IWorldInteractable world = candidate as IWorldInteractable;
        return world != null && world.IsAvailable;
    }

    void PickTarget()
    {
        MonoBehaviour best = null;
        float bestDistance = float.MaxValue;

        for (int i = nearby.Count - 1; i >= 0; i--)
        {
            MonoBehaviour candidate = nearby[i];

            if (candidate == null || !candidate.gameObject.activeInHierarchy)
            {
                nearby.RemoveAt(i);
                continue;
            }

            if (!IsAvailable(candidate)) continue;

            float distance = ((Vector2)candidate.transform.position - (Vector2)transform.position).sqrMagnitude;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = candidate;
            }
        }

        if (best != target)
        {
            target = best;
            holdTimer = 0f;
        }
    }

    void HandleInput()
    {
        bool blocked = IsBlocked();

        if (blocked)
            waitForERelease = true;
        else if (!Input.GetKey(KeyCode.E))
            waitForERelease = false;

        PickTarget();

        bool show = target != null && !blocked;

        if (show != promptVisible)
        {
            holdTimer = 0f;
            ShowPrompt(show);
        }

        if (!show) return;

        if (Input.GetKey(KeyCode.E) && !waitForERelease)
        {
            SetHoldAnimation(true);

            holdTimer += Time.deltaTime;

            if (holdTimer >= interactTime)
            {
                holdTimer = 0f;
                waitForERelease = true;
                SetHoldAnimation(false);
                Interact();
            }
        }
        else
        {
            holdTimer = 0f;
            SetHoldAnimation(false);
        }
    }

    void Interact()
    {
        ItemPickup item = target as ItemPickup;

        if (item != null)
        {
            if (!HotbarInventory.instance.HasFreeSlot()) return;

            if (item.pickupLines != null && item.pickupLines.Length > 0)
                DialogueSystem.instance.StartDialogue(item.pickupLines);

            if (!HotbarInventory.instance.AddItem(item.itemType)) return;

            Debug.Log("Picked up: " + item.name);

            nearby.Remove(item);
            Destroy(item.gameObject);
            return;
        }

        IWorldInteractable world = target as IWorldInteractable;

        if (world != null)
            world.Activate();
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
