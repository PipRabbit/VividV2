using UnityEngine;

public class FlashlightSpawner : MonoBehaviour
{
    public GameObject flashlightObject;

    void Start()
    {
        if (flashlightObject != null)
            flashlightObject.SetActive(false);
    }

    public void SpawnFlashlight()
    {
        if (flashlightObject != null)
            flashlightObject.SetActive(true);
    }
}
