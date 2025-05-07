using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TMP_Text dialogueText;
    public AIManager aiManager; // Reference to AIManager

    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();
    }

    // Start the dialogue and display NPC's responses
    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting test dialogue with " + dialogue.name);

        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    // Display the next sentence from the queue
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        // Optional: clear text or disable dialogue UI here
    }

    // Enqueue a single response string (e.g., from AI) and display it
    public void EnqueueAndDisplay(string response)
    {
        sentences.Clear();
        sentences.Enqueue(response);
        DisplayNextSentence();
    }

    // Trigger AI response based on the player input or a prompt
    public void GenerateAIResponse(string playerInput)
    {
        aiManager.TriggerAIResponse(playerInput); // Call the TriggerAIResponse method in AIManager
    }
}
