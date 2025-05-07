using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    [Header("UI Components")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Animator animator;

    [Header("Settings")]
    public string npcName = "AI NPC";
    [TextArea(3, 10)]
    public string prompt = "You are a helpful NPC in a medieval village.";
    public string openAIApiKey;

    private Queue<string> sentences;
    private HttpClient httpClient;
    
    private bool useAI = true; // This variable determines whether to use AI

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            sentences = new Queue<string>();
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAIApiKey}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Trigger AI response and handle dialogue display
    public async void TriggerAIResponse(string playerInput)
    {
        if (useAI) // Check if AI responses are enabled
        {
            animator.SetBool("IsOpen", true);
            nameText.text = npcName;

            sentences.Clear();
            string aiResponse = await GetAIResponse(playerInput, prompt);
            sentences.Enqueue(aiResponse);

            DisplayNextSentence();
        }
    }

    // Display the next sentence in the queue
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

    // Coroutine to display the sentence one character at a time
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    // End the dialogue when it's finished
    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
    }

    // Get AI's response from OpenAI API
    private async Task<string> GetAIResponse(string input, string context)
    {
        var requestData = new
        {
            model = "gpt-3.5-turbo",
            messages = new[] 
            {
                new { role = "system", content = context },
                new { role = "user", content = input }
            },
            max_tokens = 100,
            temperature = 0.7f
        };

        string json = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        string responseText = await response.Content.ReadAsStringAsync();

        // Deserialize to strongly-typed object
        var aiResponse = JsonConvert.DeserializeObject<AIResponse>(responseText);
        return aiResponse.choices[0].message.content;
    }

    // Define the structure of the response
    public class Choice
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
    }

    public class AIResponse
    {
        public List<Choice> choices { get; set; }
    }
}
