using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComputerManager : MonoBehaviour
{
    public static ComputerManager instance;

    public ComputerInteractable computer;

    public GameObject computerUI;
    public RectTransform screenArea;

    public GameObject bootRoot;
    public TMP_Text bootText;
    public string[] bootLines = new string[]
    {
        "BIOS v1.02  (C) 1989",
        "Memory test: 640K OK",
        "Detecting drives... OK",
        "Loading system...",
        "Welcome."
    };
    public float bootStartDelay = 0.8f;
    public float bootLineDelay = 0.45f;

    public GameObject desktopRoot;
    public RectTransform cursor;
    public float cursorSpeed = 1f;
    public RectTransform iconRect;
    public Image iconImage;
    public Sprite iconNormal;
    public Sprite iconHover;

    public GameObject scanRoot;
    public TMP_Text statusText;
    public Image progressImage;
    public Sprite[] progressSprites;
    public TMP_Text progressText;
    public float scanDuration = 6f;

    public GameObject shutdownOverlay;
    public float shutdownDelay = 1.2f;

    enum Stage { Closed, Booting, Desktop, Scanning, ShuttingDown }
    Stage stage = Stage.Closed;

    bool unlocked;
    bool completed;
    bool iconClicked;

    Vector2 lastMouse;
    Canvas rootCanvas;
    readonly Vector3[] corners = new Vector3[4];
    bool previousCursorVisible;
    CursorLockMode previousLockState;

    public bool IsOpen => stage != Stage.Closed;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (computerUI != null) computerUI.SetActive(false);
    }

    void OnDisable()
    {
        if (IsOpen) RestoreCursor();
    }

    public void UnlockComputer()
    {
        if (unlocked) return;
        unlocked = true;

        if (computer != null) computer.EnableInteraction();
    }

    public void OpenComputer()
    {
        if (IsOpen || completed) return;
        if (!HotbarInventory.instance.HasItem(ItemType.SyringeWithBlood)) return;

        stage = Stage.Booting;
        iconClicked = false;

        computerUI.SetActive(true);

        rootCanvas = computerUI.GetComponentInParent<Canvas>();
        if (rootCanvas != null) rootCanvas = rootCanvas.rootCanvas;

        previousCursorVisible = Cursor.visible;
        previousLockState = Cursor.lockState;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        bootRoot.SetActive(true);
        bootText.text = "";
        desktopRoot.SetActive(false);
        scanRoot.SetActive(false);
        shutdownOverlay.SetActive(false);
        cursor.gameObject.SetActive(false);
        SetProgress(0f);

        yield return new WaitForSeconds(bootStartDelay);

        foreach (string line in bootLines)
        {
            bootText.text += line + "\n";
            yield return new WaitForSeconds(bootLineDelay);
        }

        yield return new WaitForSeconds(0.6f);

        bootRoot.SetActive(false);
        desktopRoot.SetActive(true);

        cursor.anchoredPosition = Vector2.zero;
        cursor.gameObject.SetActive(true);
        lastMouse = Input.mousePosition;

        iconImage.sprite = iconNormal;
        stage = Stage.Desktop;

        yield return new WaitUntil(() => iconClicked);

        stage = Stage.Scanning;
        iconImage.sprite = iconNormal;
        scanRoot.SetActive(true);

        yield return TypeStatus("Beginning blood sample scan...");

        HotbarInventory.instance.RemoveItem(ItemType.SyringeWithBlood);

        float t = 0f;
        while (t < scanDuration)
        {
            t += Time.deltaTime;
            SetProgress(t / scanDuration);
            yield return null;
        }

        SetProgress(1f);
        statusText.text = "Scan complete.";

        yield return new WaitForSeconds(1f);

        stage = Stage.ShuttingDown;
        cursor.gameObject.SetActive(false);
        shutdownOverlay.SetActive(true);

        yield return new WaitForSeconds(shutdownDelay);

        CloseComputer();
    }

    IEnumerator TypeStatus(string message)
    {
        statusText.text = "";

        foreach (char c in message)
        {
            statusText.text += c;
            yield return new WaitForSeconds(0.04f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    void Update()
    {
        if (stage != Stage.Desktop && stage != Stage.Scanning) return;

        MoveCursor();

        if (stage != Stage.Desktop) return;

        bool hovering = IsCursorOver(iconRect);
        iconImage.sprite = hovering ? iconHover : iconNormal;

        if (hovering && Input.GetMouseButtonDown(0))
            iconClicked = true;
    }

    void MoveCursor()
    {
        Vector2 mouse = Input.mousePosition;
        Vector2 delta = (mouse - lastMouse) * cursorSpeed;
        lastMouse = mouse;

        Vector2 pos = cursor.anchoredPosition + delta / GetPixelsPerUnit();

        Rect r = screenArea.rect;
        pos.x = Mathf.Clamp(pos.x, -r.width * 0.5f, r.width * 0.5f);
        pos.y = Mathf.Clamp(pos.y, -r.height * 0.5f, r.height * 0.5f);

        cursor.anchoredPosition = pos;
    }

    Camera GetCanvasCamera()
    {
        if (rootCanvas == null || rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        if (rootCanvas.worldCamera != null)
            return rootCanvas.worldCamera;

        return Camera.main;
    }

    float GetPixelsPerUnit()
    {
        float width = screenArea.rect.width;
        if (width <= 0f) return 1f;

        screenArea.GetWorldCorners(corners);

        Camera cam = GetCanvasCamera();
        Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        Vector2 bottomRight = RectTransformUtility.WorldToScreenPoint(cam, corners[3]);

        float pixels = Mathf.Abs(bottomRight.x - bottomLeft.x) / width;
        return pixels > 0.0001f ? pixels : 1f;
    }

    bool IsCursorOver(RectTransform target)
    {
        Camera cam = GetCanvasCamera();

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, cursor.position);
        return RectTransformUtility.RectangleContainsScreenPoint(target, screenPoint, cam);
    }

    void SetProgress(float p)
    {
        p = Mathf.Clamp01(p);

        if (progressImage != null && progressSprites != null && progressSprites.Length > 0)
        {
            int index = Mathf.Min(progressSprites.Length - 1, Mathf.FloorToInt(p * progressSprites.Length));
            progressImage.sprite = progressSprites[index];
        }

        if (progressText != null)
            progressText.text = Mathf.RoundToInt(p * 100f) + "%";
    }

    void RestoreCursor()
    {
        Cursor.visible = previousCursorVisible;
        Cursor.lockState = previousLockState;
    }

    void CloseComputer()
    {
        computerUI.SetActive(false);
        RestoreCursor();

        stage = Stage.Closed;
        completed = true;

        if (computer != null) computer.DisableInteraction();

        DialogueStarter.instance.PlayComputerShutdownDialogue(GiveResults);
    }

    void GiveResults()
    {
        HotbarInventory.instance.AddItem(ItemType.PrintedResults);

        if (StoryFlowController.instance != null)
            StoryFlowController.instance.OnResultsPrinted();
    }
}
