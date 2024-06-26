# Welcome to GARVIS

## Important Note: This application is not standalone.

- It requires Oculus Casting and a local backend to process video feed from the deployed Meta Mixed Reality app.
- You need a PC running the local backend with ngrok, with the main display showing the Oculus Casting tab in full screen.

## Step by step setup

0. Pre-requisites

- Unity
- Meta Quest 3
- Gemini API Key
- ngrok
- Python

Run commmands:

```
cd server

choco install ngrok

ngrok config add-authtoken <token>

pip install -r requirements.txt
```

1. Create server/.env file.

If you have Gemini API Key:

```
GOOGLE_API_KEY=AIz...PA8
```


2. Start local python web server

Google Gemini Backend (Gemini Vision + Gemini Pro with Function Calling)

```
cd server && python gemini.py
```

3. Make public with ngrok

In separate terminal in server folder, run:

```
cd server && ngrok http http://127.0.0.1:5000
```

I run this command so it's set to "trusting-ostrich-measured.ngrok-free.app/data" by default.

```
cd server && ngrok http --domain=trusting-ostrich-measured.ngrok-free.app http://127.0.0.1:5000
```

4. Update the endpoint URL in the Meta Quest C# script.

https://github.com/WilliamUW/GARVIS/blob/135ddc978230c9b48d1adf15321834e65ca73656/Assets/MRTemplateAssets/Scripts/ScreenshotHandler.cs#L44


5. Build and run to Quest 3.

Build "mainScene" to Quest 3.

6. Start Oculus Casting to computer, enter full screen.

![casting](https://media.discordapp.net/attachments/1234940320408539178/1235329798377705602/oculuscasting.PNG?ex=6633fa07&is=6632a887&hm=a1f7fd844884e225a31e6b5f707521b1959e09afe8c8fc8f16fe6e8b0bf69fc5&=&format=webp&quality=lossless&width=1742&height=896)

![fullscreen](https://cdn.discordapp.com/attachments/1234940320408539178/1235329173619478668/fullscreencasting.png?ex=6633f972&is=6632a7f2&hm=26c6a470b007dc9a37f9d4c55f651ce8532f48ee764cc929449994d126a7ff2b&)

7. Talk to GARVIS by facing your palm up, stop recording your speech by facing your palm down.
