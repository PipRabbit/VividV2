using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeIn : MonoBehaviour
{
    public static SceneFadeIn instance;

    // True from the moment the scene loads until the reveal has finished.
    public static bool IsFading => instance != null && instance.fading;

    [Tooltip("A full-screen black Image on its own Canvas with a high Sort Order.")]
    public Image fadeImage;

    [Tooltip("Seconds to stay fully black before the reveal starts.")]
    public float holdBlackTime = 0.5f;

    [Tooltip("Seconds the fade from black to clear takes.")]
    public float fadeInTime = 2f;

    private bool fading = true;

    void Awake()
    {
        instance = this;
        fading = true;

        SetAlpha(1f);
    }

    IEnumerator Start()
    {
        if (holdBlackTime > 0f)
            yield return new WaitForSecondsRealtime(holdBlackTime);

        float t = 0f;

        while (t < fadeInTime)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(1f - Mathf.Clamp01(t / fadeInTime));
            yield return null;
        }

        SetAlpha(0f);
        fading = false;
    }

    void SetAlpha(float alpha)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
