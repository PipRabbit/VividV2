using UnityEngine;

public class NurseryQuestManager : MonoBehaviour
{
    public static NurseryQuestManager instance;
    public GameObject checklistObject;
    public GameObject syringeObject;
    public GameObject testTubeObject;
    public BloodInteractable blood;

    enum Stage { NotStarted, FindChecklist, UseChecklist, FindTools, CollectBlood, Done }
    Stage stage = Stage.NotStarted;

    bool hasSyringe;
    bool hasTestTube;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (checklistObject != null) checklistObject.SetActive(false);
        if (syringeObject != null) syringeObject.SetActive(false);
        if (testTubeObject != null) testTubeObject.SetActive(false);

        if (HotbarInventory.instance != null)
        {
            HotbarInventory.instance.onItemAdded += HandleItemAdded;
            HotbarInventory.instance.onItemUsed += HandleItemUsed;
        }
    }

    void OnDestroy()
    {
        if (HotbarInventory.instance != null)
        {
            HotbarInventory.instance.onItemAdded -= HandleItemAdded;
            HotbarInventory.instance.onItemUsed -= HandleItemUsed;
        }
    }
    public void BeginQuest()
    {
        if (stage != Stage.NotStarted) return;

        stage = Stage.FindChecklist;

        DialogueStarter.instance.PlayNurseryExploreDialogue(() =>
        {
            if (checklistObject != null) checklistObject.SetActive(true);
        });
    }

    void HandleItemAdded(ItemType item)
    {
        switch (item)
        {
            case ItemType.Checklist:
                if (stage == Stage.FindChecklist)
                {
                    stage = Stage.UseChecklist;
                    DialogueStarter.instance.PlayChecklistFoundDialogue();
                }
                break;

            case ItemType.Syringe:
                hasSyringe = true;
                CheckToolsCollected();
                break;

            case ItemType.TestTube:
                hasTestTube = true;
                CheckToolsCollected();
                break;
        }
    }

    void HandleItemUsed(ItemType item)
    {
        if (item == ItemType.Checklist && stage == Stage.UseChecklist)
        {
            stage = Stage.FindTools;
            DialogueStarter.instance.PlayChecklistUsedDialogue(RevealTools);
        }
    }

    void RevealTools()
    {
        if (syringeObject != null) syringeObject.SetActive(true);
        if (testTubeObject != null) testTubeObject.SetActive(true);
    }

    void CheckToolsCollected()
    {
        if (stage != Stage.FindTools) return;
        if (!hasSyringe || !hasTestTube) return;

        stage = Stage.CollectBlood;

        DialogueStarter.instance.PlayToolsCollectedDialogue(() =>
        {
            if (blood != null) blood.EnableInteraction();
        });
    }
    public void OnBloodInteracted()
    {
        if (stage != Stage.CollectBlood) return;
        stage = Stage.Done;

        HotbarInventory inv = HotbarInventory.instance;
        inv.RemoveItem(ItemType.Syringe);
        inv.RemoveItem(ItemType.TestTube);
        inv.AddItem(ItemType.SyringeWithBlood);

        if (blood != null) blood.DisableInteraction();

        DialogueStarter.instance.PlayBloodCollectedDialogue(() =>
        {
            if (StoryFlowController.instance != null)
                StoryFlowController.instance.OnBloodCollected();
        });
    }
}
