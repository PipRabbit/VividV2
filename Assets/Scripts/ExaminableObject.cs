using System;
using UnityEngine;
using UnityEngine.Events;

public class ExaminableObject : MonoBehaviour, IWorldInteractable
{
    public string[] lines;
    public string[] repeatLines;
    public bool interactionEnabled = true;
    public bool oneTimeOnly = true;
    public bool hideObjectAfter = false;
    public UnityEvent onFirstExamined;

    private bool examined;

    public bool IsAvailable => interactionEnabled;

    public bool Examined => examined;

    public void Activate()
    {
        if (!interactionEnabled) return;

        bool first = !examined;
        examined = true;

        string[] toShow = lines;

        if (!first && repeatLines != null && repeatLines.Length > 0)
            toShow = repeatLines;

        if (oneTimeOnly)
            interactionEnabled = false;

        Action onEnd = null;

        if (hideObjectAfter)
            onEnd = () => gameObject.SetActive(false);

        if (DialogueSystem.instance != null)
            DialogueSystem.instance.StartDialogue(toShow, onEnd);
        else
            onEnd?.Invoke();

        if (first)
            onFirstExamined?.Invoke();
    }
}
