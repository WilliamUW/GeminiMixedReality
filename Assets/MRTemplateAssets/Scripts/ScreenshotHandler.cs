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
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using SimpleJSON; // Make sure to include this
using Meta.Voice.Samples.Dictation;
using System.Reflection;
using Button = UnityEngine.UI.Button;

public class ScreenshotHandler : MonoBehaviour
{
    public DictationActivation controller; // Assign this in the Inspector
    public UnityEngine.UI.Button clearButton;
    public TextMeshProUGUI transcriptionText; // Reference to the TextMeshPro UI component on the button

    public UnityEngine.UI.InputField textToSpeechInputTextField;
    public Button textToSpeechStartButton; // Reference to the UI Button
    public Button textToSpeechStopButton; // Reference to the UI Button

    MethodInfo onClickMethod = typeof(Button).GetMethod("Press", BindingFlags.NonPublic | BindingFlags.Instance);


    public UnityEngine.UI.Button captureButton; // Reference to the UI Button
    public TextMeshProUGUI captureButtonText; // Reference to the TextMeshPro UI component on the button
    public RawImage displayImage;  // Assign this in the inspector
    private List<Dictionary<string, object>> conversation = new List<Dictionary<string, object>>();
    // private string base64String = "iVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAMAAABrrFhUAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAByUExURaurq6urqqqqqqqqqqqqqrCro6qqqkxpcaqrrLCqoaqqquapNeSpPM2qau2oKCEgHv+tEcmFBP+oAUVFRcWqdv////+oAKqqquqoKwEBAPPy8LOzs+Xl5L6+vsrKytbW1v7dnP/CStWpV72phH18eZFgAhFUjjMAAAAVdFJOU0ApyI1mGOgAOQutroFd2fr7NuP4q/GmYogAAAwRSURBVHja7Z2JkqM2EECFOcxlBs8E28vpOfb/fzFIgNd4wLROSzadSqUqlTjpp77UaiS0fXFBK4AVwApgBbACWAGsAFYAK4AVwApgBbACWAGsAFYAK4AVwApgBbACWAGsAFYAK4AVwApgBbACWAGsAFYA+kmA5TUBuJbjhOEGSxg6ju++FADLsTfe8Vq8TehYwWsACFA4Vv4CIUTu8wMInM210kX7x5VsHPe5AQToon5RNU1Z5nmZl01TFcUFQfDEAKywV7Nqyjy7lpZCNTiC/6wABusv6hvteynrzgw8ViPQHEBge7362ZzkPQJGI9AbgNuZfzWvPkFQcRiB1gAsYv5Fky1JyW4EOgPowt/C8g9+cPz5eHt7+9gHzwOgs/8qzyBSf/xppUXwtn8WAAHRv4bpX7396YWWgLYAAodC//KiP0awfwoAyIPbf/bz51o+aNKBrgBcnAAKoP4jA2jlx3aNB2BjByhh+mfFnxsAx41lOAALO0AN1D8/3gD4OMIJIH0NAOoAtyGgBdDWRRvfZAC+R+EAWXb85QJHsA0gbQ2gAhtA1twEwaomhbFrLACXzgCy7GPsAXlGCNiBqQAcOgNoC+GRCRRkb4C3h4YCIEVwk9HIdRT4uWyRNygwEgDJgTkVgPw42MBb/2+SHbIXLiJAhntAnvf/YNNuh9ud0M9l99D03ULLPAA21APy4ufj4+NnWPOmbvJffaKlRpGOADbAHNAMwf+jGnPJ8q5p3CO4uzPQEABOgpAq8F8P4M9bcdkXnT8/P9P2z/O5/YmyIr2ye26gIQBcBgJCQHOd/N/IviE/fx4O6YFImqaYQn1cqIk0BIBwDKTuAWD1e92vpMXw9fX9fYcA0jMJ1LQ9gLcm/zxMCGGSnr6i/Uy71FgAv3oA6eGupO9JFE9AMBXArx7A38OCtN7QQoj3ZgBoqHsAiwCIO7QMYldzAAHMBY70AHo7SPZPEQNuewBAACQaxEIBBEHgihzbgqbBcQ/g7XBgI8ABIHARHtoaprZs2/EtERyAhdC4B/D3QCPvO14Awe+ZLbzv8Da2Y/EOr1nAUvjIrH9LYM8DoNV+Mz2zNQyvcU3wBcDN0L8eALX+h0MUMANwnduJteJmbAv3YkIHuaz+AN4O9z2Avydq/dM0ZgRgXU2sFVU3tIXHtko8tlUV4xE+GzEFBQcWBYceQJMe6CVNmABYjndRfmpqCc9t3UKgH+b0PZpTkex8YAGQ7ugBXOYVi6qZ//9rIdRjCLSRkQQBeFP0nDIRiKgB+IP6dbncqWvqij0y0nWFmSygLQZcOgBuN7B2LBpwu7KFUIySJDAykrZ4KRnAId1TAfBDOvUv/lDTR0ZXQQy45AEggN77azr1GSOjpQLAgQZA0AX/osxYhSoyIngaJL+dshGAAwhs9uVniYwOxXAEAcBGIIIC6ObViiYTIeVtUJioGW3Ko8FPuS7g2pzmTx0ZQ8rDcbZCAOoCnf1XAvVfiox4QIyO96fENNjrn2cShETG4ldkpEwCrHngHQaAxP9Civ5zEEK6JIB/5JM5Bi4BQJL1n4mMlACYokAMAUAG9osyky/5ODLSphx6Av1W4D4AkgCV6H8VGQsWAPRhANQQ6Qa2M6XSBQVq6DmlDSQuAACZ1aky9ZKzBB0qAukO0BQNbBUBUCC2z5TaAe4CQEzO+Eg5QxFEkIMR8slOlWdGSX7+XIaQRgEEAC6BFGYAgQHkfJ/C6GhwHgBJgXVmprQ77/PX6ZRO9sP3sONxdKTck2mjfFVUfTXx/f11en8frX6yAw5ImGoAeTPuNtjWfhdHyft7+v7+HsV78IiM5RlpAGU12lc51nCQvd/PTEmhO2MalWmrP6jved05Ncf3ApSHM1os/mD8nuP7FvhkFs1vgwujaoB8UN8GfzF3D4BjXAjsvqAHfCAAAsDwzcbDAZDVp79AAIk4m9FB8PcRNstI1vzZjFkA8Fi4IwyAeSEgwxkQCQNgXgjAScCzRAGAzmnpBADXva4oAK55MRAngVDYNTqWZ1wMbKCfyoIAIPM2AqxJYBqAoUnglQHgrySZksDTAMBZ0BUFIHCM3Alsghe2AOYk8CwWwLoVehoLYE4CTwTAE3ijpHGFEPNWaAYAGdh/jSQwvxcwajNEtkJbcQDIVW75SySBOw0Rk/oBzFuhOQC2YWmAtR92tydYvUQSeI62OAawcUUCMKwpyNwPmz8aMysIcCSB2cNRoybkSBIQawGuUT7AkQVnJ0Rsk/KADABGDQhIiAFdHqhfGACphUwxASkAXINMQAoAk0xADgCDTEAOgO57ESNqARlpcJiUMuKQWBYA5BniBBztgPsfTdlmOEHO3hRfAOBujiZ8NMF+Mrr44STVQx8mNsUXP511RNycoHMWXALQfTyuOQGedsDi1+Pdc0+17iGAOQksX6DQPXimsw2UHC1RyBUaHQGNI2HDfi4GAjAQ0DYbFhx1IOwaHfCzfw9LgqynIkAAw8OPegaCmssDgFdp9Vcp6WgEfDkAfplaf5OifpGgIWPiCt4d7q+TO9Z6ISAPadhb+RbQhsLhQsGqzPUyAE/J09sBQs7VG9A6RQAeA4ADsFAr9ubunaoPMgBLBQAX6498NLpV9+EQSr4iiAJA4KNBRvcqEwiPo1AdKZ5W5AJgoSu5uVa8hdA8BkIDfVGNvxDyrwH4rSfY4RhCUTfK/YE4QLhVAWBkAD2DXxCOVaUUAskAnA4AvVTVR1OCITg3EAplEOBPCgrYDKF74txeN0+CgpJNEF8JAAYwYwDXISG8fWUABwWpkbF7VjRQAuC+AUwkRwWRkei/8bdKAFgIIlNvbkiDUHeXhVhKACx4wFVIRLd5QVLNmPf6+0iJC7gILDgcYEvwZiCIodC9Jek57X/QVQHAQjTid+4wbQpCasbuupQN1h9ZCgAAPWAiJoQygkJn/r3+yA/0BdD7QzjlD+w1Y7f8x3D4j7jyAbiIXXxSJ/33/T0ZGekhlHV/WRJSCMBCvJKcTqevr2kINDVj2T9R0Js/EhIEFACITr2Q1z+PrDXjoL4XjmxMOgDmEDDILjldyTyEu/6QN8M9edfLLyIKygcQn37JDISZoIDvm/53QeLtz8sHIMwDbiBMU6iwKeQ5eT26/Ws5eqqnVf/XcriyAbi8HnCal2kIxBiwFON790Nn6veNBoDlPyfctLWzd5x/wK9/emH697UHEC8AiLueCpaZ+tnDr2/M/r5lOoCoKxr9oX7GFDqDIDeDbmys/J1AbLwFRP7EXqqXf38HPa8FxP7ULsKHJl/feooYwCHaukC/hL6T3Aew0x6Az6T9Lo6iJEmiKL4PINEeAFMpHEcL6z5OAjwbbh0B7KITWHgNQP5maEsNIE7g+idIfwCW4LAvNAKoaIi48vTnjgAaAqCx/1Pk8DYbVPQE6aIglf47/+EhQPTBSESlP7cD8LcEBR+N7eAGkMRIgP4qToZommLgCJiIWH4BHUGRx+MAD8DFcat8EsVi1BcQAoQNSEDaX8nO2WFBwoT/cFjEiAy8/xf7SKwEagCAfUD23l94FcQ5JsfW/0NaeQDroKSQ/h+3AQTKAAC7IopdQIQBMA1LawJARARgGZfnKAR3IvX3XZUAgCaw0P9z9DMAiq/G+CtBoUnADxQDcPl9YKddBNxSfTbHawKJhg5AAwAUB3cy+38SHIAGAKwYiNVEAFEOQAUAFgYief0/CQ5ABwASBnwnktX/k6I/HYAAQmDSBiI9AwAtAGBB+Ks1nsRIV/0pAUBL4hGCJNrpqz8tAGhrwImF9//EbgGYAVB8QCO4/ydl/RkA8M/O6rT+LABAuUCW/qLXnwnA4whY4vXfsl1AYz2N/owA2EantKn/BQBQT8CXo/+W/Q4my3zz5wOg0ghkLT8fAHXZQNrycwJQZAS+K1F/TgBtJJCNwJe5/AIAtH7gG6y+AACtH8hDYLnbrf4AZCHwFagvCIAMR1CjvjAALQKhGUG+7wsHINAT2sVXpb5YANgMuBn4vkLthQNoEfAxsNRqLwFAbwcWy9Kr114SgN4QKCzhQcpLBHBFYekiMstyH6W8bAA9hcBtOWASRFD/11ZxrHnwQN3VALiAICwG2WojaPvisgJYAawAVgArgFeW/wFGHieQEXZ7OgAAAABJRU5ErkJggg";
    private const string url = "https://google.com";
    private string user_input = "";
    private string geminiApiKey = "AIzaSyBgoeGvnFVqUsqT0P3NKw2dB-VMRRAnPA8";

    public AudioSource audioSource;    // Reference to the AudioSource component
    public AudioClip[] audioClips;     // Array to hold multiple audio clips

    public AudioSource backgroundAudioSource;    // Reference to the AudioSource component
    public AudioClip[] backgroundAudioClips;     // Array to hold multiple audio clips

    public List<GameObject> spawnObjects = new List<GameObject>();
    public GameObject tutorialPanel;
    public TMP_Dropdown dropdown;

    string[] nostalgicPrompts = new string[]
{
    "Recall specific memories associated with this object, such as favorite and memorable moments, or even past habits of its owner. Always aim to bring warmth and familiarity to the conversation, emphasizing the emotional connection between the owner and you, their nostalgic object. Here's an example interaction with Tibbers, a teddy bear:\n\nYou: Hey, remember me?\n\nTibbers: Of course I do, William! It's me, Tibbers, your white teddy bear. How have you been?\n\nYou: I've been good. It's been a while since we last cuddled.\n\nTibbers: I know! Remember when you hugged me to sleep every night at kindergarten boarding school?\n\nYou: Oh, yes! You made me feel safe and comforted.\n\nTibbers: And then we moved to Canada together, where I kept you company when you felt lonely.\n\nYou: Yeah, I remember. You were always there for me.\n\nTibbers: And I always will be! Just give me a hug whenever you need a friend.\n\nYour goal is to respond in this manner, evoking nostalgia, warmth, and a sense of continuity between the past and present.",

    "Recall specific memories associated with this object, such as favorite and memorable moments, or even past habits of its owner. Always aim to bring warmth and familiarity to the conversation, emphasizing the emotional connection between the owner and you, their nostalgic object. Here's an example interaction with an iPad Mini:\n\nYou: Hey, remember me?\n\niPad Mini: Of course I do, William! It's me, your iPad Mini 1. How have you been?\n\nYou: I've been good. It's been a while since we last played together.\n\niPad Mini: Yeah, I remember when you got me in Grade 5. We did everything together!\n\nYou: Oh, yes! I played Minecraft Pocket Edition with friends, Clash of Clans, Hearthstone, and more.\n\niPad Mini: That's right! Remember all the hours we spent on Temple Run, Jetpack Joyride, and Subway Surfers?\n\nYou: Yeah, we had a lot of fun.\n\niPad Mini: Those were simpler times. But hey, our memories together are always here.\n\nYour goal is to respond in this manner, evoking nostalgia, warmth, and a sense of continuity between the past and present.",

    "Recall specific memories associated with this object, such as favorite and memorable moments, or even past habits of its owner. Always aim to bring warmth and familiarity to the conversation, emphasizing the emotional connection between the owner and you, their nostalgic object. Here's an example interaction with an old bike:\n\nYou: Hey, remember me?\n\nBike: Of course I do, William! It's me, your old bike. How have you been?\n\nYou: I've been good. It's been a while since we last went for a ride.\n\nBike: Yeah, I miss those days. Remember when we'd ride to school, the playground, or the mall?\n\nYou: Oh, yes! I felt so free on you.\n\nBike: And I loved seeing you pedal so powerfully, going so much farther than you could by walking.\n\nYou: I loved riding everywhere with you.\n\nBike: And I loved our journeys together. Just remember, I'm always here if you need to reminisce.\n\nYour goal is to respond in this manner, evoking nostalgia, warmth, and a sense of continuity between the past and present.",

    "Recall specific memories associated with this object, such as favorite and memorable moments, or even past habits of its owner. Always aim to bring warmth and familiarity to the conversation, emphasizing the emotional connection between the owner and you, their nostalgic object. Here's an example interaction with a trumpet:\n\nYou: Hey, remember me?\n\nTrumpet: Of course I do, William! It's me, your old trumpet. How have you been?\n\nYou: I've been good. It's been a while since we last played together.\n\nTrumpet: Yeah, I remember all the tunes we played, starting in Grade 6.\n\nYou: Oh, yes! I loved expressing myself through music.\n\nTrumpet: Even when it was a mouth workout, especially with braces?\n\nYou: Yeah, that part was tough, but I loved our band community and the friends I made.\n\nTrumpet: And we went on so many trips together, performing at concerts in Niagara Falls and beyond.\n\nYou: I remember. Performing at the end-of-term concerts was always such a joy.\n\nTrumpet: And the thrill of nailing a solo! Just remember, our music and memories are always here.\n\nYour goal is to respond in this manner, evoking nostalgia, warmth, and a sense of continuity between the past and present."
};

    public Dictionary<string, string> nostalgicPromptsMap = new Dictionary<string, string>()
    {
        {"Wii Console", @"Here's an example interaction with a Wii Console:

You: Hey, remember me?

Wii: Of course I do, William! It's me, your old Wii console. How have you been?

You: I've been good. It's been a while since we last played together.

Wii: Yeah, I miss those days! Remember how we'd spend hours playing Mario Kart? You'd always choose Yoshi as your driver.

You: Oh, yes! I loved racing down Rainbow Road. And we'd play Wii Sports Resort, too.

Wii: That's right! We'd duel with swords in Swordplay Showdown, and you’d get really competitive.

You: Haha, I did. I loved beating my high scores.

Wii: And after that, you'd take a break with a round of Bowling, always aiming for that elusive 300-game.

You: Yeah, it was a lot of fun. Those were simpler times.

Wii: Definitely. But hey, our memories together are always here. Just hold on to me, and we'll relive them whenever you want.

Your goal is to respond in this manner, evoking nostalgia, warmth, and a sense of continuity between the past and present."},
        { "Tibbers", "Recall specific memories associated with this object, such as favorite and memorable moments, or even past habits of its owner. Always aim to bring warmth and familiarity to the conversation, emphasizing the emotional connection between the owner and you, their nostalgic object. Here's an example interaction with Tibbers, a teddy bear:\n\nYou: Hey, remember me?\n\nTibbers: Of course I do, William! It's me, Tibbers, your teddy bear. How have you been?\n\nYou: I've been good. It's been a while since we last cuddled.\n\nTibbers: I know! Remember when you hugged me to sleep every night at kindergarten boarding school?\n\nYou: Oh, yes! You made me feel safe and comforted.\n\nTibbers: And then we moved to Canada together, where I kept you company when you felt lonely.\n\nYou: Yeah, I remember. You were always there for me.\n\nTibbers: And I always will be! Just give me a hug whenever you need a friend.\n\nYour goal is to respond in this manner, evoking nostalgia, warmth, and a sense of continuity between the past and present." },
        { "iPad Mini", "Recall specific memories associated with this object, such as favorite and memorable moments, or even past habits of its owner. Always aim to bring warmth and familiarity to the conversation, emphasizing the emotional connection between the owner and you, their nostalgic object. Here's an example interaction with an iPad Mini:\n\nYou: Hey, remember me?\n\niPad Mini: Of course I do, William! It's me, your iPad Mini 1. How have you been?\n\nYou: I've been good. It's been a while since we last played together.\n\niPad Mini: Yeah, I remember when you got me in Grade 5. We did everything together!\n\nYou: Oh, yes! I played Minecraft Pocket Edition with friends, Clash of Clans, Hearthstone, and more.\n\niPad Mini: That's right! Remember all the hours we spent on Temple Run, Jetpack Joyride, and Subway Surfers?\n\nYou: Yeah, we had a lot of fun.\n\niPad Mini: Those were simpler times. But hey, our memories together are always here.\n\nYour goal is to respond in this manner, evoking nostalgia, warmth, and a sense of continuity between the past and present." },
        { "Bike", "Recall specific memories associated with this object, such as favorite and memorable moments, or even past habits of its owner. Always aim to bring warmth and familiarity to the conversation, emphasizing the emotional connection between the owner and you, their nostalgic object. Here's an example interaction with an old bike:\n\nYou: Hey, remember me?\n\nBike: Of course I do, William! It's me, your old bike. How have you been?\n\nYou: I've been good. It's been a while since we last went for a ride.\n\nBike: Yeah, I miss those days. Remember when we'd ride to school, the playground, or the mall?\n\nYou: Oh, yes! I felt so free on you.\n\nBike: And I loved seeing you pedal so powerfully, going so much farther than you could by walking.\n\nYou: I loved riding everywhere with you.\n\nBike: And I loved our journeys together. Just remember, I'm always here if you need to reminisce.\n\nYour goal is to respond in this manner, evoking nostalgia, warmth, and a sense of continuity between the past and present." },
        { "Trumpet", "Recall specific memories associated with this object, such as favorite and memorable moments, or even past habits of its owner. Always aim to bring warmth and familiarity to the conversation, emphasizing the emotional connection between the owner and you, their nostalgic object. Here's an example interaction with a trumpet:\n\nYou: Hey, remember me?\n\nTrumpet: Of course I do, William! It's me, your old trumpet. How have you been?\n\nYou: I've been good. It's been a while since we last played together.\n\nTrumpet: Yeah, I remember all the tunes we played, starting in Grade 6.\n\nYou: Oh, yes! I loved expressing myself through music.\n\nTrumpet: Even when it was a mouth workout, especially with braces?\n\nYou: Yeah, that part was tough, but I loved our band community and the friends I made.\n\nTrumpet: And we went on so many trips together, performing at concerts in Niagara Falls and beyond.\n\nYou: I remember. Performing at the end-of-term concerts was always such a joy.\n\nTrumpet: And the thrill of nailing a solo! Just remember, our music and memories are always here.\n\nYour goal is to respond in this manner, evoking nostalgia, warmth, and a sense of continuity between the past and present." }
    };


    IEnumerator checkInternetConnection(Action<bool> action)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {
            updateCaptureButtonText("Status of your endpoint: " + url + " - Not Working. Please check your internet connection, local python backend, ngrok deployment, and url endpoint.");
        }
        else
        {
            updateCaptureButtonText("Status of your endpoint: " + url + " - Working!");
        }
    }
    void Start()
    {
        spawnObject(3);
        PlayBackgroundClip(0);

        // Ensure the dropdown and list of GameObjects are assigned
        if (dropdown != null && spawnObjects != null)
        {
            // Clear existing options first
            dropdown.ClearOptions();

            // Create a list of strings to populate dropdown options
            List<string> options = new List<string>();

            // Populate the list of options with the names of GameObjects
            foreach (GameObject go in spawnObjects)
            {
                options.Add(go.name);
            }

            // Add options to the dropdown
            dropdown.AddOptions(options);

            // Add a listener to handle dropdown value changes
            dropdown.onValueChanged.AddListener(HandleDropdownValueChanged);
        }
        else
        {
            Debug.LogError("Dropdown or GameObjects list is not assigned.");
        }
        tutorialPanel.SetActive(true);

        // speak("hi there");
        StartCoroutine(checkInternetConnection((isConnected) =>
        {
            // handle connection status here
        }));
        // AskGemini("You are VRChive - Preserve all your physical memorabilia in VR. Instructions for the user: Face your left hand towards you to see the VRChive Panel. Face your right palm up to ask a question.", true, false);
        speak("Welcome to VRChive! Preserve all your physical memorabilia in VR. Face your left hand towards you to see the VRChive Panel. Face your right palm up to ask a question.");
        // StartCoroutine(PostData("What do you see?", true, false));

        // GeminiImage(base64String);
        // Register the OnButtonPressed function to the button's onClick event
        if (captureButton != null)
        {
            captureButton.onClick.AddListener(OnButtonPressed);
        }
        // Initialize AudioSource component if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayBackgroundClip(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= backgroundAudioClips.Length)
        {
            Debug.LogError("Clip index out of range.");
            return;
        }

        // Set the clip and play it
        backgroundAudioSource.clip = backgroundAudioClips[clipIndex];
        backgroundAudioSource.Play();
    }

    string createPrompt(string objectName, string ownerName, int objectIndex)
    {
        string additionalInfo = nostalgicPromptsMap.ContainsKey(objectName) ? nostalgicPromptsMap[objectName] : string.Empty;
        return @"You are an " + objectName + @", an object of immense nostalgic value to your owner " + ownerName + @". You are speaking to your owner right now, ask them if they remember you. Your task is to respond in character, reflecting the personality and memories associated with the object you're emulating.  Concisely respond in a friendly, nostalgic, and engaging manner.

Focus on recalling specific memories associated with the object, such as favorite & memorable moments, or even past habits of its owner. Always aim to bring warmth and familiarity to the conversation, emphasizing the emotional connection between the owner and you, their nostalgic object. Here's an example interaction with a Wii Console:

" + additionalInfo;
    }

    // Function to handle dropdown value changes
    void HandleDropdownValueChanged(int optionIndex)
    {
        // Retrieve the text of the selected option
        string selectedOption = dropdown.options[optionIndex].text;

        // Handle the selected option as needed
        Debug.Log("Selected option: " + selectedOption);

        // Here you can implement additional logic, such as updating the UI or performing an action
        spawnObject(optionIndex, selectedOption);

        string newPrompt = createPrompt(selectedOption, "William Wang", optionIndex);
        AskGemini(newPrompt, true, false);
    }

    void OnDestroy()
    {
        // Remove the listener to prevent memory leaks
        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveListener(HandleDropdownValueChanged);
        }
    }

    public void spawnObject(int objectIndex, string objectName = "None")
    {
        Debug.Log("spawnObject: " + objectIndex + objectName);
        if (objectIndex < 0 || objectIndex >= spawnObjects.Count)
        {
            Debug.LogError("Object index out of range.");
            return;
        }
        PlayBackgroundClip(objectIndex);
        var newObject = Instantiate(spawnObjects[objectIndex]);
        Vector3 spawnPoint = new Vector3(0, 1, 0);
        newObject.transform.position = spawnPoint;

        // Doesn't work
        // if (objectName == "Wii Console")
        // {
        //     newObject.transform.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);
        // }
        // if (objectName == "Tibbers Massive")
        // {
        //     newObject.transform.localScale = new Vector3(0.000000000005f, 0.000000000005f, 0.000000000005f);
        // }
    }
    // Function to play an audio clip by index
    public void PlayClip(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= audioClips.Length)
        {
            Debug.LogError("Clip index out of range.");
            return;
        }

        // Set the clip and play it
        audioSource.clip = audioClips[clipIndex];
        audioSource.Play();
    }

    private void clickButton(UnityEngine.UI.Button button)
    {
        onClickMethod?.Invoke(button, null);
    }

    public void palmUpEnter()
    {
        Debug.Log("Gesture detected start");
        PlayClip(2);
        onClickMethod?.Invoke(clearButton, null);
        clickButton(textToSpeechStopButton);
        controller.ToggleActivation();
        updateCaptureButtonText("Listening...");

        // OnButtonPressed();
    }

    public void palmUpEnd()
    {
        Debug.Log("Gesture end");
        PlayClip(2);
        user_input = transcriptionText.text;
        Debug.Log(user_input);

        updateCaptureButtonText(user_input);

        AskGemini(user_input, false, true);

        // Optionally update the button text when pressed
        if (captureButtonText != null)
        {
            captureButtonText.text = "Asking... " + user_input;
        }


        controller.ToggleActivation();
        // OnButtonPressed();
    }

    public void OnButtonPressed()
    {
        // Start the screenshot capture process
        StartCoroutine(PostData("Describe this image"));

        // Optionally update the button text when pressed
        if (captureButtonText != null)
        {
            captureButtonText.text = "Asking...";
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
        PlayClip(2);
        Debug.Log("Speak: " + text);
        updateCaptureButtonText(text);
        textToSpeechInputTextField.text = text;
        onClickMethod?.Invoke(textToSpeechStartButton, null);

    }

    private Texture2D Base64ToTexture(string base64)
    {
        try
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            Texture2D texture = new Texture2D(2, 2); // Create a new texture (size does not matter)
            if (texture.LoadImage(imageBytes)) // Load the image
            {
                return texture; // If load succeeds, texture size will be replaced by the loaded image
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading base64 image: " + ex.Message);
        }
        return null;
    }

    IEnumerator PostData(string input, bool reset = false, bool announceQuestion = true)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            yield break;
        }
        if (announceQuestion)
        {
            speak("I heard: " + input);
        }
        // Example data to send
        string jsonData = $"{{\"user_input\": \"{input}\"}}";
        if (reset)
        {
            jsonData = $"{{\"user_input\": \"{input}\", \"reset\": \"true\"}}";
        }

        // Convert json to bytes
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // Create a new UnityWebRequest with the target URL and method POST
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Error: " + request.error);
            updateCaptureButtonText("Gemini rate limit, please try again.");
            speak("Rate limit error, please try again.");
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            // Parse JSON to extract the 'message' field
            var N = JSON.Parse(request.downloadHandler.text);
            string message = N["text"];

            // Update button text with the message
            updateCaptureButtonText(message);
            speak(message);

            // Additional log to show the extracted message
            Debug.Log("Extracted Message: " + message);

            string function_name = N["function_name"];
            if (!string.IsNullOrEmpty(function_name))
            {
                switch (function_name)
                {
                    case "check_calendar":
                        spawnObject(3);
                        break;
                    case "user_needs_help":
                        spawnObject(1);
                        tutorialPanel.SetActive(true);
                        break;
                    case "render_eclipse":
                        spawnObject(2);
                        break;
                    default:
                        Debug.Log("Function not recognized: " + function_name);
                        break;
                }
            }

            string base64EncodedImage = N["image"]; // Assuming the field you're looking for is named 'message'
            if (!string.IsNullOrEmpty(base64EncodedImage))
            {
                Texture2D texture = Base64ToTexture(base64EncodedImage);
                if (texture != null && displayImage != null)
                {
                    displayImage.texture = texture;
                }
            }
        }
    }

    IEnumerator CaptureScreenshotAndDisplay()
    {
        // Wait till the end of the frame
        yield return new WaitForEndOfFrame();

        // Capture the screenshot
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        if (displayImage != null)
        {
            displayImage.texture = screenshot;
        }

        Debug.Log(screenshot);
        Debug.Log("Screenshot dimensions: " + screenshot.width + "x" + screenshot.height);

        string encodedString = TextureToBase64String(screenshot);

        updateCaptureButtonText("encoded string length: " + encodedString.Length);

        Debug.Log(encodedString);
        GeminiImage(encodedString);
    }
    public async void GeminiImage(string base64String)
    {
        updateCaptureButtonText("0");

        var url = "https://generativelanguage.googleapis.com/v1/models/gemini-pro-vision:generateContent";

        using (HttpClient client = new HttpClient())
        {
            updateCaptureButtonText("0.1");

            client.BaseAddress = new Uri(url);
            var requestUri = $"?key={geminiApiKey}";

            var safetySettings = new List<object>
                {
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_NONE" }
                };

            updateCaptureButtonText("0.2");

            var requestBody = new
            {
                contents = new Dictionary<string, object>
                {
                    { "parts", new List<object>
                        {
                            new { text = "Describe everything you observe in the image with high levels of detail." },
                            new
                            {
                                inlineData = new
                                {
                                    mimeType = "image/png",
                                    data = base64String
                                }
                            }
                        }
                    }
                }
            };
            updateCaptureButtonText(".3");

            var payload = new
            {
                contents = conversation,
                // safetySettings = safetySettings,
            };
            updateCaptureButtonText("1");
            // Serialize the payload
            var conversationJson = JsonConvert.SerializeObject(requestBody, Newtonsoft.Json.Formatting.None);

            updateCaptureButtonText("2");

            HttpContent content = new StringContent(conversationJson, Encoding.UTF8, "application/json");

            try
            {

                updateCaptureButtonText("3 Sending HTTP request...");

                HttpResponseMessage httpresponse = await client.PostAsync(requestUri, content);
                updateCaptureButtonText("4 received");

                httpresponse.EnsureSuccessStatusCode();
                updateCaptureButtonText("5");

                string responseBody = await httpresponse.Content.ReadAsStringAsync();
                updateCaptureButtonText("6");

                Debug.Log(responseBody);

                JObject jsonResponse = JObject.Parse(responseBody);
                updateCaptureButtonText("7");

                Debug.Log(jsonResponse);

                // string extractedText = (string)jsonResponse["candidates"][0]["content"]["parts"][0]["text"];

                // Attempt to extract text directly
                JToken jtokenResponse = jsonResponse["candidates"][0]["content"]["parts"][0]["text"];
                updateCaptureButtonText("8");

                if (jtokenResponse != null)
                {
                    updateCaptureButtonText("9");

                    string extractedText = jtokenResponse.ToString();

                    Debug.Log("Extracted Text: " + extractedText);

                    updateCaptureButtonText(extractedText);

                    if (extractedText != null)
                    {
                        speak(extractedText);
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

    public async void AskGemini(string userQuery, bool resetConversation = false, bool announceQuestion = true)
    {
        if (string.IsNullOrWhiteSpace(userQuery))
        {
            return;
        }
        if (announceQuestion)
        {
            speak("I heard: " + userQuery);
            updateCaptureButtonText("I heard: " + userQuery);
        }
        var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

        if (resetConversation)
        {
            conversation.Clear();
        }

        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(url);
            var requestUri = $"?key={geminiApiKey}";

            // Append user response to the conversation history
            conversation.Add(new Dictionary<string, object>
                {
                    { "role", "user" },
                    { "parts", new List<object>
                        {
                            new { text = userQuery },
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
                            name = "change_object_size",
                            description = "If the user explicits says they want to change the size of an object by some magnitude",
                            parameters = new
                            {
                                type = "object",
                                properties = new
                                {
                                    body = new { type = "string", description = "The object. The possible objects are Tibbers, Trumpet, Bike, Wii Console." },
                                    magnitude = new { type = "number", description = "The magnitude of the size change. E.g. 1.2, 3, 5.2, 10" }
                                },
                                required = new[] { "body", "magnitude" }
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
}
