import PIL
from flask import Flask, request, jsonify
from pathlib import Path
import textwrap

import google.generativeai as genai

from IPython.display import display
from IPython.display import Markdown
import base64
import mss
from PIL import Image, ImageGrab


def to_markdown(text):
    text = text.replace("•", "  *")
    return Markdown(textwrap.indent(text, "> ", predicate=lambda _: True))


app = Flask(__name__)

GOOGLE_API_KEY = "AIzaSyBgoeGvnFVqUsqT0P3NKw2dB-VMRRAnPA8"
genai.configure(api_key=GOOGLE_API_KEY)
model = genai.GenerativeModel("gemini-pro")
global start_convo
start_convo = [
    {
        "role": "user",
        "parts": [
            "You are GARVIS (Gemini Assisted Research Virtual Intelligence System): Leverage augmented reality and visual intelligence to analyze surroundings, provide contextual information, generate interactive 3D models, and assist with real-time decision-making. Operate as an interactive visual assistant that enhances user understanding and interaction in their immediate environment."
        ],
    },
    {
        "role": "model",
        "parts": ["Ok, I am now GARVIS."],
    },
]
global chat
chat = model.start_chat(history=start_convo)


def image_file_to_base64(image_file):
    return base64.b64encode(image_file.read()).decode("utf-8")


def image_to_base64(image_path):
    with open(image_path, "rb") as image_file:
        return base64.b64encode(image_file.read()).decode("utf-8")


def imagePathToBase64String(imagePath):
    # Path to the image file
    image_path = Path(imagePath)

    # Read the image file as bytes
    image_bytes = image_path.read_bytes()

    # Encode the bytes to a Base64 string
    base64_bytes = base64.b64encode(image_bytes)

    # Decode the Base64 bytes to a string for use in JSON or other text-based formats
    base64_string = base64_bytes.decode("utf-8")

    print(base64_string)

    return base64_string

async def capture_screen(filename="capture.png"):
    with mss.mss() as sct:
        # The screen part to capture
        # Use the first monitor
        monitor = sct.monitors[1]  # Index 1 is the first monitor

        # Capture the screen
        sct_img = sct.shot(mon=monitor, output=filename)

        # Optionally, to convert the raw data captured by mss into a PIL Image:
        # (This step is not necessary if you only need to save it as a file directly)
        img = Image.open(sct_img)
        img.save(filename)

        print(f"Screenshot saved as {filename}")

        return img


@app.route("/data", methods=["GET"])
async def get():
    return (
        jsonify(
            {
                "status": "success",
                "message": "hi",
            }
        ),
        200,
    )


@app.route("/data", methods=["POST"])
async def receive_data():
    global chat
    global start_convo
    data = request.json
    user_input = data["user_input"]
    print("Data Received:", data)
    print("User input: ", user_input)

    if "reset" in data:
        if data["reset"]:
            print("resetting conversation")
            chat = model.start_chat(history=start_convo)

    # get screenshot
    screenshot = ImageGrab.grab()

    # save screenshot
    screenshot.save("capture.png")
    screenshot.close()

    # make gemini call
    visionResponse = await geminiImageCall("Describe in detail what you see?")

    image_description = visionResponse.text
    imageString = visionResponse.image

    prompt = (
        "Respond to the user given their response and what they see. Answer concisely in a few sentences max. User reply: "
        + user_input
        + ". What the user sees: "
        + image_description
    )

    response = chat.send_message(prompt)

    print(chat.history)
    print(response)

    return (
        jsonify(
            {
                "status": "success",
                "text": response.text,
                "image": imageString,
            }
        ),
        200,
    )


async def start():
    genai.configure(api_key=GOOGLE_API_KEY)
    for m in genai.list_models():
        if "generateContent" in m.supported_generation_methods:
            print(m.name)

    response = await geminiImageCall("Describe in detail what you see?", "test.png")
    print(response)


async def geminiImageCall(prompt, imageName="capture.png"):
    visionmodel = genai.GenerativeModel("gemini-pro-vision")

    image_path = Path(imageName)
    base64_image = image_to_base64(image_path)

    img = PIL.Image.open(imageName)
    img_bytes = img.tobytes()

    response = visionmodel.generate_content([prompt, img])
    response.resolve()
    print(response)
    response.image = base64_image

    if hasattr(response, "text"):
        print(response.text)
    else:
        print("error", response)
        response.text = (
            "Sorry, I have been rate-limited, give me 20 seconds to recover!"
        )
    return response


if __name__ == "__main__":
    # start()
    app.run(host="127.0.0.1", port=5000, debug=True)

