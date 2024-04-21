using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required for UI Button interaction
using TMPro; // Required for TextMeshPro interaction
using System;
using static UnityEngine.Rendering.DebugUI;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ScreenshotHandler : MonoBehaviour
{
    public UnityEngine.UI.Button captureButton; // Reference to the UI Button
    public TextMeshProUGUI captureButtonText; // Reference to the TextMeshPro UI component on the button
    public RawImage displayImage;  // Assign this in the inspector
    private List<Dictionary<string, object>> conversation = new List<Dictionary<string, object>>();

    void Start()
    {
        AskGemini();
        // Register the OnButtonPressed function to the button's onClick event
        if (captureButton != null)
        {
            captureButton.onClick.AddListener(OnButtonPressed);
        }
    }

    void OnButtonPressed()
    {
        // Start the screenshot capture process
        StartCoroutine(CaptureScreenshotAndDisplay());

        // Optionally update the button text when pressed
        if (captureButtonText != null)
        {
            captureButtonText.text = "0. Capturing...";
        }
    }

    void updateCaptureButtonText(string text)
    {
        // Update button text after screenshot is displayed
        if (captureButtonText != null)
        {
            captureButtonText.text = text;
        }
    }

    public string TextureToBase64String(Texture2D texture)
    {
        // Convert the texture to a byte array
        byte[] imageBytes = texture.EncodeToPNG();  // You can also use EncodeToJPG()

        // Convert the byte array to a Base64 string
        string base64String = Convert.ToBase64String(imageBytes);

        return base64String;
    }

    void speak(string text)
    {
        Debug.Log("Speak: " + text);
    }


    IEnumerator CaptureScreenshotAndDisplay()
    {
        if (captureButtonText != null)
        {
            captureButtonText.text = "1. Corountine started Button Pressed";
        }
        // Wait till the end of the frame
        yield return new WaitForEndOfFrame();

        if (captureButtonText != null)
        {
            captureButtonText.text = "2. wait for end of frame";
        }

        // Capture the screenshot
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        if (displayImage != null)
        {
            displayImage.texture = screenshot;
        }

        Debug.Log(screenshot);
        Debug.Log("Screenshot dimensions: " + screenshot.width + "x" + screenshot.height);

        if (captureButtonText != null)
        {
            captureButtonText.text = "3. obtained screenshot";
        }

        string encodedString = TextureToBase64String(screenshot);

        updateCaptureButtonText("encoded string length: " + encodedString.Length);

        Debug.Log(encodedString);



        // Display the screenshot in your MR environment
        // DisplayScreenshotInMR(screenshot);


    }

    public async void AskGemini()
    {
        var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-pro-latest:generateContent";
        var apiKey = "AIzaSyBgoeGvnFVqUsqT0P3NKw2dB-VMRRAnPA8";

        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(url);
            var requestUri = $"?key={apiKey}";

            // Append user response to the conversation history
            conversation.Add(new Dictionary<string, object>
                {
                    { "role", "user" },
                    { "parts", new List<object>
                        {
                            new { text = "Write me a poem about UWaterloo." }
                        }
                    }
                });

            Debug.Log(JsonConvert.SerializeObject(conversation, Newtonsoft.Json.Formatting.Indented));
            var safetySettings = new List<object>
                {
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_NONE" }
                };

            var functionDeclarations = new
            {
                function_declarations = new[]
        {
        new
        {
            name = "reset_position",
            description = "Resets the positions of the planets to their default states in the virtual scene.",
            parameters = new
            {
                type = "object",
                properties = new { }
            }
        }
    }
            };


            // Existing objects: conversation and safetySettings

            var requestBody = new
            {
                contents = conversation,
                safetySettings = safetySettings,
                tools = new[]
            {
                new
                {
                    function_declarations = new List<object>
                    {
                        new
                        {
                            name = "change_size",
                            description = "change the size of a planet or star by some magnitude",
                            parameters = new
                            {
                                type = "object",
                                properties = new
                                {
                                    body = new { type = "string", description = "The planet or star. The possible options are The Milky Way, The Sun, Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus Neptune, Pluto, Messier-87 Black Hole" },
                                    magnitude = new { type = "number", description = "The magnitude of the size change. E.g. 1.2, 3, 5.2, 10" }
                                },
                                required = new[] { "description" }
                            }
                        },
                    }
                }
            },
                tool_config = new
                {
                    function_calling_config = new
                    {
                        mode = "AUTO"
                    },
                }
            };

            var payload = new
            {
                contents = conversation,
                safetySettings = safetySettings,
            };
            // Serialize the payload
            var conversationJson = JsonConvert.SerializeObject(requestBody, Newtonsoft.Json.Formatting.None);
            HttpContent content = new StringContent(conversationJson, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage httpresponse = await client.PostAsync(requestUri, content);
                httpresponse.EnsureSuccessStatusCode();
                string responseBody = await httpresponse.Content.ReadAsStringAsync();
                Debug.Log(responseBody);

                JObject jsonResponse = JObject.Parse(responseBody);
                Debug.Log(jsonResponse);

                // string extractedText = (string)jsonResponse["candidates"][0]["content"]["parts"][0]["text"];

                // Attempt to extract text directly
                JToken jtokenResponse = jsonResponse["candidates"][0]["content"]["parts"][0]["text"];

                if (jtokenResponse != null)
                {
                    string extractedText = jtokenResponse.ToString();

                    Debug.Log("Extracted Text: " + extractedText);


                    // Parse and handle model response here if necessary
                    // Example: Append model response to the conversation
                    // This is a placeholder. You need to extract actual response from responseBody JSON.
                    conversation.Add(new Dictionary<string, object>
                    {
                        { "role", "model" },
                        { "parts", new List<object>
                            {
                                new { text = extractedText }
                            }
                        }
                    });

                    Debug.Log(JsonConvert.SerializeObject(conversation, Newtonsoft.Json.Formatting.Indented));

                    if (extractedText != null)
                    {
                        speak(extractedText);
                    }
                }
                else
                {
                    // Check if there is a function call
                    JToken functionCall = jsonResponse["candidates"][0]["content"]["parts"][0]["functionCall"];
                    if (functionCall != null)
                    {
                        string functionName = (string)functionCall["name"];
                        JObject args = (JObject)functionCall["args"];

                        // Based on function name, call the relevant method
                        Debug.Log($"Function Call Detected: {functionName}");
                        // ExecuteFunctionCall(functionName, args);
                        //if (functionName == "change_size")
                        //{
                        //    change_size(functionCall["args"]["magnitude"].ToString(), functionCall["args"]["body"].ToString());
                        //}
                    }
                    else
                    {
                        Debug.Log("No valid text or function call found in the response.");
                    }
                }

            }
            catch (HttpRequestException e)
            {
                Debug.Log("\nException Caught!");
                Debug.Log("Message :{0} " + e.Message);
            }
        }
    }


    void DisplayScreenshotInMR(Texture2D screenshot)
    {
        // Create or find a GameObject to display the screenshot
        GameObject displayPanel = GameObject.Find("ScreenshotDisplay");
        if (displayPanel == null)
        {
            displayPanel = new GameObject("ScreenshotDisplay");
            displayPanel.AddComponent<MeshRenderer>();
            displayPanel.transform.position = new Vector3(0, 0, 2); // Adjust as needed
        }

        // Create a material for the display panel
        Material displayMaterial = new Material(Shader.Find("Unlit/Texture"));
        displayMaterial.mainTexture = screenshot;
        displayPanel.GetComponent<MeshRenderer>().material = displayMaterial;

        updateCaptureButtonText("4. Display Screenshot");

    }
}
