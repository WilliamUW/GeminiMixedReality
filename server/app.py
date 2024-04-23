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
    data = request.json
    user_input = data["user_input"]
    print("Data Received:", data)
    print("User input: ", user_input)

    # get screenshot
    screenshot = ImageGrab.grab()

    # save screenshot
    screenshot.save("capture.png")
    screenshot.close()

    # make gemini call
    response = await geminiImageCall(user_input)

    return (
        jsonify(
            {
                "status": "success",
                "text": response.text,
                "image": response.image,
            }
        ),
        200,
    )


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


async def start():
    genai.configure(api_key=GOOGLE_API_KEY)
    for m in genai.list_models():
        if "generateContent" in m.supported_generation_methods:
            print(m.name)

    response = await geminiImageCall("Describe in detail what you see?", "test.png")
    print(response)


async def geminiImageCall(prompt, imageName="capture.png"):
    genai.configure(api_key=GOOGLE_API_KEY)

    model = genai.GenerativeModel("gemini-pro-vision")

    # Path to the image file and conversion to base64
    image_path = Path(imageName)
    base64_image = image_to_base64(image_path)

    # Prepare the image data in the expected 'inline_data' format
    cookie_picture = {"inline_data": {"mime_type": "image/png", "data": base64_image}}

    # Prepare contents with the correct structure
    contents = {
        "parts": [
            {"text": "User question: " + prompt + ". Please use the image to assist the user, answer concisely in a few sentences max."},
            cookie_picture,  # No additional nesting under 'image_data' key, directly use the dictionary
        ]
    }

    # Generate content using the model
    response = model.generate_content(contents=contents)
    if hasattr(response, 'text'):
        print(response.text)
        response.image = base64_image
        return response
    else:
        print("error", response)
        return {"text_response": "Gemini Rate Limit Issue, please try again in 30 seconds!", "image": base64_image}


if __name__ == "__main__":
    # start()
    app.run(host="127.0.0.1", port=5000, debug=True)
