using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TirednessManager : MonoBehaviour
{
    [Header("Volume")]
    public Volume volume;

    [Header("Tiredness Speed")]
    public float tiredSpeed = 0.02f;
    public float blackoutSpeed = 1.5f;

    [Header("Recovery")]
    [Tooltip("Seconds to fade tiredness back to 0 after the blackout.")]
    public float recoverDuration = 1.5f;

    [Header("Effects Limits")]
    public float maxExposureReduction = -4f;
    public float maxVignette = 0.6f;
    public float maxDistortion = -0.5f;

    [Header("Pulse")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.08f;

    private ColorAdjustments colorAdjustments;
    private Vignette vignette;
    private LensDistortion lensDistortion;

    private float tiredness = 0f;

    private bool blackoutActive = false;
    private bool recovering = false;

    void Start()
    {
        if (volume == null || volume.profile == null)
        {
            Debug.LogWarning("TirednessManager: no Volume/profile assigned. Effects disabled.");
            enabled = false;
            return;
        }

        volume.profile.TryGet(out colorAdjustments);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out lensDistortion);

        if (colorAdjustments != null) colorAdjustments.postExposure.overrideState = true;
        if (vignette != null) vignette.intensity.overrideState = true;
        if (lensDistortion != null) lensDistortion.intensity.overrideState = true;

        ApplyEffects();
    }

    void Update()
    {
        if (recovering)
        {
            tiredness = Mathf.MoveTowards(
                tiredness, 0f, Time.deltaTime / Mathf.Max(0.01f, recoverDuration));

            if (tiredness <= 0f)
                recovering = false;
        }
        else if (blackoutActive)
        {
            tiredness += Time.deltaTime * blackoutSpeed;
        }
        else
        {
            tiredness += Time.deltaTime * tiredSpeed;
        }

        tiredness = Mathf.Clamp01(tiredness);

        ApplyEffects();
    }

    void ApplyEffects()
    {
        if (colorAdjustments != null)
            colorAdjustments.postExposure.value =
                Mathf.Lerp(0f, maxExposureReduction, tiredness);

        if (vignette != null)
            vignette.intensity.value =
                Mathf.Lerp(0f, maxVignette, tiredness);

        if (lensDistortion != null)
        {
            float baseDistortion =
                Mathf.Lerp(0f, maxDistortion, tiredness);

            float pulse =
                Mathf.Sin(Time.time * pulseSpeed) * pulseAmount * tiredness;

            lensDistortion.intensity.value = baseDistortion + pulse;
        }
    }

    public void StartBlackout()
    {
        recovering = false;
        blackoutActive = true;
    }

    public void ResetTiredness(bool instant = false)
    {
        blackoutActive = false;

        if (instant)
        {
            recovering = false;
            tiredness = 0f;
            ApplyEffects();
        }
        else
        {
            recovering = true;
        }
    }
}
