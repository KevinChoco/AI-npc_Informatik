using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public DialogueManager manager;

    // Call this for AI response
    public void TriggerDialogueAI()
    {
        TriggerDialogue(true); // true for AI
    }

    // Call this for prewritten response
    public void TriggerDialoguePrewritten()
    {
        TriggerDialogue(false); // false for prewritten
    }

    // Original function with bool parameter
    public void TriggerDialogue(bool useAI) 
    {
        Debug.Log("TriggerDialogue called with useAI = " + useAI);

        // Check if manager is assigned
        if (manager == null)
        {
            Debug.LogError("DialogueManager is not assigned!");
            return;
        }

        // Check if dialogue is assigned
        if (dialogue == null)
        {
            Debug.LogError("Dialogue object is not assigned!");
            return;
        }

        // If using AI, enqueue AI generated response
        if (useAI)
        {
            string aiResponse = "AI generated response here";  // Replace with actual AI call
            manager.EnqueueAndDisplay(aiResponse);  // Enqueue AI response
        }
        else
        {
            // Enqueue prewritten response
            manager.StartDialogue(dialogue);  // Start the prewritten dialogue
        }
    }
}
