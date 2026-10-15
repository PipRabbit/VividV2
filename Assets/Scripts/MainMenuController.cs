using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuController : MonoBehaviour
{
    [Header("Buttons (in order, top to bottom)")]
    public MainMenuButton[] buttons;
    public int defaultIndex = 0;

    [Header("Fade")]
    [Tooltip("A full-screen black Image, drawn on top of everything else in the Canvas.")]
    public Image fadeImage;
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

    [Header("Scene")]
    [Tooltip("Exact name of the game scene. It must be added in Build Settings.")]
    public string gameSceneName = "SampleScene";

    private int selectedIndex;
    private bool inputLocked = true;

    void Awake()
    {
        SetFade(1f);
    }

    IEnumerator Start()
    {
        selectedIndex = Mathf.Clamp(defaultIndex, 0, Mathf.Max(0, buttons.Length - 1));
        UpdateSelection();

        yield return Fade(1f, 0f, fadeInTime);

        inputLocked = false;
    }

    void Update()
    {
        if (inputLocked || buttons == null || buttons.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
                 Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelection(1);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            inputLocked = true;
            StartCoroutine(ActivateRoutine(buttons[selectedIndex]));
        }
    }

    void MoveSelection(int direction)
    {
        selectedIndex = (selectedIndex + direction + buttons.Length) % buttons.Length;
        UpdateSelection();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
                buttons[i].SetSelected(i == selectedIndex);
        }
    }

    IEnumerator ActivateRoutine(MainMenuButton button)
    {
        if (button.action == MenuAction.StartGame &&
            !Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            Debug.LogError("MainMenuController: scene '" + gameSceneName +
                           "' can't be loaded. Check the name and add it in Build Settings.");
            inputLocked = false;
            yield break;
        }

        yield return button.PlayPressAnimation();

        yield return Fade(0f, 1f, fadeOutTime);

        switch (button.action)
        {
            case MenuAction.StartGame:
                SceneManager.LoadScene(gameSceneName);
                break;

            case MenuAction.Quit:
                QuitGame();
                break;
        }
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            SetFade(to);
            yield break;
        }

        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            SetFade(Mathf.Lerp(from, to, Mathf.Clamp01(t / duration)));
            yield return null;
        }

        SetFade(to);
    }

    void SetFade(float alpha)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
