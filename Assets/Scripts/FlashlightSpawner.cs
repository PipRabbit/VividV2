using UnityEngine;

public class FlashlightSpawner : MonoBehaviour
{
    public GameObject flashlightObject;

    private bool shouldSpawn = false;

    void Start()
    {
        if (flashlightObject != null)
            flashlightObject.SetActive(false);
    }

    public void EnableFlashlightSpawn()
    {
        shouldSpawn = true;

        DialogueSystem.instance.onDialogueEnd += SpawnFlashlight;
    }

    void SpawnFlashlight()
    {
        if (!shouldSpawn) return;

        flashlightObject.SetActive(true);

        shouldSpawn = false;

        DialogueSystem.instance.onDialogueEnd -= SpawnFlashlight;
    }
}