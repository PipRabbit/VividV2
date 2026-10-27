using UnityEngine;

public class NurseryQuestManager : MonoBehaviour
{
    public static NurseryQuestManager instance;

    public ExaminableObject[] examinables;

    public GameObject checklistObject;
    public GameObject syringeObject;
    public GameObject testTubeObject;
    public BloodInteractable blood;

    enum Stage { NotStarted, Exploring, FindChecklist, UseChecklist, FindTools, CollectBlood, Done }
    Stage stage = Stage.NotStarted;

    bool hasSyringe;
    bool hasTestTube;

    public bool CanCollectBlood => stage == Stage.CollectBlood;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (checklistObject != null) checklistObject.SetActive(false);
        if (syringeObject != null) syringeObject.SetActive(false);
        if (testTubeObject != null) testTubeObject.SetActive(false);
        if (blood != null) blood.DisableInteraction();

        foreach (ExaminableObject examinable in examinables)
        {
            if (examinable == null) continue;

            examinable.interactionEnabled = false;
            examinable.onFirstExamined.AddListener(HandleExamined);
        }

        if (HotbarInventory.instance != null)
        {
            HotbarInventory.instance.onItemAdded += HandleItemAdded;
            HotbarInventory.instance.onItemUsed += HandleItemUsed;
        }
    }

    void OnDestroy()
    {
        foreach (ExaminableObject examinable in examinables)
        {
            if (examinable != null)
                examinable.onFirstExamined.RemoveListener(HandleExamined);
        }

        if (HotbarInventory.instance != null)
        {
            HotbarInventory.instance.onItemAdded -= HandleItemAdded;
            HotbarInventory.instance.onItemUsed -= HandleItemUsed;
        }
    }

    public void BeginQuest()
    {
        if (stage != Stage.NotStarted) return;

        stage = Stage.Exploring;

        Debug.Log("Nursery exploration started. Examinables to look at: " + CountExaminables());

        foreach (ExaminableObject examinable in examinables)
        {
            if (examinable != null)
                examinable.interactionEnabled = true;
        }

        CheckExplorationComplete();
    }

    int CountExaminables()
    {
        int count = 0;

        foreach (ExaminableObject examinable in examinables)
            if (examinable != null) count++;

        return count;
    }

    void HandleExamined()
    {
        CheckExplorationComplete();
    }

    void CheckExplorationComplete()
    {
        if (stage != Stage.Exploring) return;

        int examined = 0;

        foreach (ExaminableObject examinable in examinables)
        {
            if (examinable != null && examinable.Examined) examined++;
        }

        Debug.Log("Nursery examined " + examined + " of " + CountExaminables());

        if (examined < CountExaminables()) return;

        stage = Stage.FindChecklist;

        DialogueStarter.instance.PlayNurseryConclusionDialogue(() =>
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
