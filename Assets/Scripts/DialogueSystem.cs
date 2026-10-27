using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public float typingSpeed = 0.03f;
    private string[] lines;
    private int index;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private Action onDialogueEnd;

    private struct PendingDialogue
    {
        public string[] lines;
        public Action onEnd;
    }

    private readonly Queue<PendingDialogue> pending = new Queue<PendingDialogue>();

    public bool IsOpen => dialogueBox != null && dialogueBox.activeSelf;

    void Awake()
    {
        instance = this;
        dialogueBox.SetActive(false);
    }

    void Update()
    {
        if (!dialogueBox.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                dialogueText.text = lines[index];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue(string[] newLines, Action onEnd = null)
    {
        if (newLines == null || newLines.Length == 0)
        {
            onEnd?.Invoke();
            return;
        }

        if (IsOpen)
        {
            pending.Enqueue(new PendingDialogue { lines = newLines, onEnd = onEnd });
            return;
        }

        BeginDialogue(newLines, onEnd);
    }

    void BeginDialogue(string[] newLines, Action onEnd)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueBox.SetActive(true);

        lines = newLines;
        index = 0;
        onDialogueEnd = onEnd;

        typingCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        WaitForSeconds wait = new WaitForSeconds(typingSpeed);

        foreach (char c in lines[index])
        {
            dialogueText.text += c;
            yield return wait;
        }

        isTyping = false;
    }

    void NextLine()
    {
        index++;

        if (index >= lines.Length)
        {
            EndDialogue();
            return;
        }

        typingCoroutine = StartCoroutine(TypeLine());
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
        dialogueText.text = "";
        Action callback = onDialogueEnd;
        onDialogueEnd = null;
        callback?.Invoke();

        if (!IsOpen && pending.Count > 0)
        {
            PendingDialogue next = pending.Dequeue();
            BeginDialogue(next.lines, next.onEnd);
        }
    }
}
