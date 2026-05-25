using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TirednessManager : MonoBehaviour
{
    [Header("Volume")]
    public Volume volume;

    [Header("Tiredness Speed")]
    public float tiredSpeed = 0.02f;

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

    void Start()
    {
        volume.profile.TryGet(out colorAdjustments);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out lensDistortion);

        ApplyEffects();
    }

    void Update()
    {
        if (blackoutActive)
        {
            // FAST blackout ramp
            tiredness += Time.deltaTime * 1.5f;
            tiredness = Mathf.Clamp01(tiredness);

            ApplyEffects();
            return;
        }

        // NORMAL gameplay tiredness (slow build up)
        tiredness += Time.deltaTime * tiredSpeed;
        tiredness = Mathf.Clamp01(tiredness);

        ApplyEffects();
    }

    void ApplyEffects()
    {
        colorAdjustments.postExposure.value =
            Mathf.Lerp(0f, maxExposureReduction, tiredness);

        vignette.intensity.value =
            Mathf.Lerp(0f, maxVignette, tiredness);

        float baseDistortion =
            Mathf.Lerp(0f, maxDistortion, tiredness);

        float pulse =
            Mathf.Sin(Time.time * pulseSpeed) * pulseAmount * tiredness;

        lensDistortion.intensity.value = baseDistortion + pulse;
    }

    public void StartBlackout()
    {
        blackoutActive = true;
    }

    public void ResetTiredness()
    {
        blackoutActive = false;
        tiredness = 0f;

        ApplyEffects();
    }
}