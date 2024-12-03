using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterAvatarImage;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;
    
    public bool isDialogueActive = false;
    public float typingSpeed = 0.2f;

    public Animator animator;

    private Queue<DialogueLine> dialogueLines = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && isDialogueActive)
        {
            DisplayNextLine();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        animator.Play("Show");

        dialogueLines.Clear();

        foreach (DialogueLine line in dialogue.dialogueLines)
        {
            dialogueLines.Enqueue(line);
        }

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = dialogueLines.Dequeue();

        characterAvatarImage.sprite = currentLine.character.avatar;
        characterNameText.text = currentLine.character.name;

        StopAllCoroutines();

        StartCoroutine(TypeLine(currentLine));
    }

    public IEnumerator TypeLine(DialogueLine line)
    {
        dialogueText.text = "";
        foreach(char letter in line.line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        animator.Play("Hide");
    }
}
