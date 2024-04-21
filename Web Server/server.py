from flask import Flask, request, jsonify
from pathlib import Path
import textwrap

import google.generativeai as genai

from IPython.display import display
from IPython.display import Markdown
import base64


def to_markdown(text):
    text = text.replace("•", "  *")
    return Markdown(textwrap.indent(text, "> ", predicate=lambda _: True))


app = Flask(__name__)

GOOGLE_API_KEY = "AIzaSyBgoeGvnFVqUsqT0P3NKw2dB-VMRRAnPA8"


@app.route("/data", methods=["POST"])
def receive_data():
    data = request.json
    user_input = data["user_input"]
    print("Data Received:", data)
    print("User input: ", user_input)

    # get screenshot

    # save screenshot

    # make gemini call
    genai.configure(api_key=GOOGLE_API_KEY)

    model = genai.GenerativeModel("gemini-pro-vision")

    base64_string = imagePathToBase64String("test.png")

    cookie_picture = [{"mime_type": "image/png", "data": base64_string}]
    prompt = "What do you see?"

    response = model.generate_content(
        contents=[{"text": prompt}, {"inline_data": cookie_picture}]
    )
    print(response.text)

    return (
        jsonify({"status": "success", "message": "We have received " + user_input}),
        200,
    )


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


def start():
    genai.configure(api_key=GOOGLE_API_KEY)
    for m in genai.list_models():
        if "generateContent" in m.supported_generation_methods:
            print(m.name)
    model = genai.GenerativeModel("gemini-pro-vision")


    # Path to the image file and conversion to base64
    image_path = Path("test.png")
    base64_image = image_to_base64(image_path)

    # Prepare the image data in the expected 'inline_data' format
    cookie_picture = {"inline_data": {"mime_type": "image/png", "data": base64_image}}

    # Text prompt
    prompt = "What do you see?"

    # Prepare contents with the correct structure
    contents = {
        "parts": [
            {"text": prompt},
            cookie_picture,  # No additional nesting under 'image_data' key, directly use the dictionary
        ]
    }

    # Generate content using the model
    response = model.generate_content(contents=contents)
    print(response.text)




if __name__ == "__main__":
    start()
    app.run(host="127.0.0.1", port=5000, debug=True)
