using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required for UI Button interaction
using TMPro; // Required for TextMeshPro interaction
using System;

public class ScreenshotHandler : MonoBehaviour
{
    public Button captureButton; // Reference to the UI Button
    public TextMeshProUGUI captureButtonText; // Reference to the TextMeshPro UI component on the button
    public RawImage displayImage;  // Assign this in the inspector

    void Start()
    {
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
