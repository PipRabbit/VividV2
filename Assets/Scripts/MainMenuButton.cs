using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum MenuAction
{
    StartGame,
    Quit
}
public class MainMenuButton : MonoBehaviour
{
    [Header("Visuals")]
    [Tooltip("The Image that displays the button art. Leave empty to use the Image on this object.")]
    public Image visual;
    public Sprite normalSprite;
    public Sprite selectedSprite;
    public Sprite clickedSprite;

    [Tooltip("Keeps the size you set in the editor for the Normal sprite, and sizes the other sprites " +
             "proportionally (so the clicked sprite is smaller). Turn off to leave the size alone.")]
    public bool scaleFromEditorSize = true;

    [Header("Press Animation")]
    [Tooltip("How long the small clicked sprite is shown.")]
    public float pressTime = 0.12f;
    [Tooltip("How long the button stays back at its normal selected look before the fade starts.")]
    public float releasePause = 0.15f;

    [Header("What this button does")]
    public MenuAction action = MenuAction.StartGame;

    private bool selected;
    private bool showClicked;

    private Vector2 sizeScale = Vector2.one;
    private bool scaleReady;

    void Awake()
    {
        if (visual == null)
            visual = GetComponent<Image>();

        CaptureEditorSize();
        Refresh();
    }

    void CaptureEditorSize()
    {
        if (!scaleFromEditorSize || visual == null || normalSprite == null) return;

        Vector2 spriteSize = normalSprite.rect.size;
        Vector2 currentSize = visual.rectTransform.rect.size;

        if (spriteSize.x <= 0f || spriteSize.y <= 0f) return;

        sizeScale = new Vector2(currentSize.x / spriteSize.x, currentSize.y / spriteSize.y);
        scaleReady = true;
    }

    public void SetSelected(bool value)
    {
        selected = value;
        Refresh();
    }

    public IEnumerator PlayPressAnimation()
    {
        showClicked = true;
        Refresh();

        yield return new WaitForSecondsRealtime(pressTime);

        showClicked = false;
        Refresh();

        yield return new WaitForSecondsRealtime(releasePause);
    }

    void Refresh()
    {
        if (visual == null) return;

        Sprite sprite = normalSprite;

        if (showClicked)
            sprite = clickedSprite;
        else if (selected)
            sprite = selectedSprite;

        if (sprite == null)
            sprite = normalSprite;

        visual.sprite = sprite;

        if (scaleFromEditorSize && scaleReady && sprite != null)
        {
            RectTransform rt = visual.rectTransform;
            Vector2 size = new Vector2(sprite.rect.width * sizeScale.x,
                                       sprite.rect.height * sizeScale.y);

            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
    }
}