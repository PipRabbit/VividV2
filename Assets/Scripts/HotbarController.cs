using UnityEngine;

public class HotbarController : MonoBehaviour
{
    public RectTransform selector;
    public RectTransform[] slots;

    public float showTime = 0f;

    private float hideTimer;
    private int currentIndex = -1;

    void Start()
    {
        HideSelector();
    }

    void Update()
    {
        if (HotbarInventory.instance == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) Press(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Press(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Press(2);

        if (showTime > 0f && selector != null && selector.gameObject.activeSelf)
        {
            hideTimer -= Time.deltaTime;

            if (hideTimer <= 0f)
                HideSelector();
        }
    }

    void Press(int index)
    {
        if (currentIndex == index)
        {
            HideSelector();
            return;
        }

        ShowSelector(index);
        HotbarInventory.instance.UseSlot(index);
    }

    void ShowSelector(int index)
    {
        if (selector == null || slots == null) return;
        if (index < 0 || index >= slots.Length || slots[index] == null) return;

        selector.gameObject.SetActive(true);
        selector.position = slots[index].position;
        currentIndex = index;
        hideTimer = showTime;
    }

    void HideSelector()
    {
        currentIndex = -1;

        if (selector != null)
            selector.gameObject.SetActive(false);
    }
}