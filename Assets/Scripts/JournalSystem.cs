using UnityEngine;
using TMPro;

public class JournalSystem : MonoBehaviour
{
    public static JournalSystem instance;

    public TMP_Text bookText;

    void Awake()
    {
        instance = this;
    }

    public void ShowFirstEntry()
    {
        bookText.text = "Note to self,\n\nI left the last pill I need to take on my bed.";
    }

    public void Clear()
    {
        bookText.text = "";
    }
}