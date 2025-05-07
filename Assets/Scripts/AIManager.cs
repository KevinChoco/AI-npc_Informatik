using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class AIManager : MonoBehaviour
{
    public DialogueManager dialogueManager;

    [TextArea(3, 10)]
    public string developerPrompt = "You are an NPC in a fantasy world called Jack Black. You know about the terrain, towns, factions, and quests.";

    private string apiKey = "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"; // ← Replace with your actual key

    public void GetAIResponse(string playerQuestion)
    {
        string finalPrompt = developerPrompt + "\n\nPlayer: " + playerQuestion + "\nNPC:";
        StartCoroutine(SendOpenAIRequest(finalPrompt));
    }

    IEnumerator SendOpenAIRequest(string prompt)
    {
        string endpoint = "https://api.openai.com/v1/chat/completions";

        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            max_tokens = 100
        };

        string jsonBody = JsonConvert.SerializeObject(requestBody);

        UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("AI Request Failed: " + request.error);
            dialogueManager.EnqueueAndDisplay("The NPC is silent... (Error from AI)");
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<OpenAIResponse>(jsonResponse);
            string aiReply = response.choices[0].message.content.Trim();
            dialogueManager.EnqueueAndDisplay(aiReply);
        }
    }

    [System.Serializable]
    public class OpenAIResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
}
