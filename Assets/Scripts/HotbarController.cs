using UnityEngine;

public class HotbarController : MonoBehaviour
{
    public RectTransform selector;
    public RectTransform[] slots;

    int currentIndex = -1;

    void Start()
    {
        HideSelector();
        HotbarInventory.instance.selectedSlot = -1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ToggleSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ToggleSlot(2);
    }

    void ToggleSlot(int index)
    {
        if (currentIndex == index)
        {
            currentIndex = -1;
            HotbarInventory.instance.selectedSlot = -1;
            HideSelector();
            return;
        }

        currentIndex = index;
        HotbarInventory.instance.selectedSlot = currentIndex;

        MoveSelector();
    }

    void MoveSelector()
    {
        selector.gameObject.SetActive(true);
        selector.position = slots[currentIndex].position;
    }

    void HideSelector()
    {
        selector.gameObject.SetActive(false);
    }
}