using System.Collections;
using UnityEngine;
using TMPro;
using System;

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

    public Action onDialogueEnd;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        dialogueBox.SetActive(false);
    }

    void Update()
    {
        if (!dialogueBox.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = lines[index];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue(string[] newLines)
    {
        dialogueBox.SetActive(true);

        lines = newLines;
        index = 0;

        typingCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in lines[index])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
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

        onDialogueEnd?.Invoke();
    }
}