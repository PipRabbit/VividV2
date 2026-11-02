using System.Collections;
using UnityEngine;

public class PillManager : MonoBehaviour
{
    public static PillManager instance;

    public GameObject pillObject;
    public Transform player;

    public bool useOpenPosition = true;
    public Vector3 landingOffset = new Vector3(0f, -0.4f, 0f);

    public float fallHeight = 0.4f;
    public float fallDuration = 0.3f;
    public float bounceHeight = 0.15f;
    public float bounceDuration = 0.2f;

    private bool bookReadTriggered = false;
    private Vector3 landingPosition;
    private bool hasLandingPosition = false;

    void Awake()
    {
        instance = this;
    }

    Transform GetPlayer()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) player = playerObject.transform;
        }

        return player;
    }

    public void RecordLandingPosition()
    {
        Transform p = GetPlayer();
        if (p == null) return;

        landingPosition = p.position;
        hasLandingPosition = true;
    }

    public void OnBookClosedAfterReading()
    {
        if (bookReadTriggered) return;

        bookReadTriggered = true;

        DialogueStarter.instance.PlayForgotPillDialogue(SpawnPill);
    }

    void SpawnPill()
    {
        if (pillObject == null) return;

        float originalZ = pillObject.transform.position.z;
        Vector3 target = pillObject.transform.position;
        Transform p = GetPlayer();

        if (useOpenPosition && hasLandingPosition)
            target = landingPosition + landingOffset;
        else if (p != null)
            target = p.position + landingOffset;

        target.z = originalZ;

        pillObject.transform.position = target + Vector3.up * fallHeight;
        pillObject.SetActive(true);

        StartCoroutine(Fall(target));

        HintSystem.Set(HintId.FindPill);
    }

    IEnumerator Fall(Vector3 target)
    {
        Collider2D[] colliders = pillObject.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D c in colliders)
            c.enabled = false;

        Vector3 start = pillObject.transform.position;
        float t = 0f;

        while (t < fallDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fallDuration);
            pillObject.transform.position = Vector3.Lerp(start, target, k * k);
            yield return null;
        }

        pillObject.transform.position = target;

        if (bounceHeight > 0f && bounceDuration > 0f)
        {
            t = 0f;

            while (t < bounceDuration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / bounceDuration);
                pillObject.transform.position = target + Vector3.up * (Mathf.Sin(k * Mathf.PI) * bounceHeight);
                yield return null;
            }

            pillObject.transform.position = target;
        }

        foreach (Collider2D c in colliders)
            c.enabled = true;
    }
}