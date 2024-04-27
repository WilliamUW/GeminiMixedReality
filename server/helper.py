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
import requests
from dotenv import load_dotenv
import os
import os
import requests
import base64
import asyncio
# Load environment variables from .env file
load_dotenv()

def to_markdown(text):
    text = text.replace("•", "  *")
    return Markdown(textwrap.indent(text, "> ", predicate=lambda _: True))


app = Flask(__name__)


def get_tutorial():
    print("Getting tutorial")



GPT4V_KEY = os.getenv("OPENAI_API_KEY")



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
                "message": "this endpoint is working!",
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
            # TODO

    # get screenshot
    screenshot = ImageGrab.grab()

    # save screenshot
    screenshot.save("capture.png")
    screenshot.close()

    image_path = Path("capture.png")
    imageString = image_to_base64(image_path)

    image_description = "No image description."  # visionResponse.text

    prompt = user_input

    response = azureImageCall(prompt)

    print(response)
    # if response.parts[0].function_call:
    #     function_call = response.parts[0].function_call
    #     function_name = function_call.name
    #     additional_information = ""
    #     match (function_name):
    #         case "user_needs_help":
    #             additional_information = "I have rendered a 3d model of the object you need help with, as well as tutorial video."

    #         case "check_calendar":
    #             additional_information = "The user has a flight to New York's LaGuardia Airport tomorrow at 8am. I have rendered a 3d map of NYC to better assist your travels including the location of your hotel in Soho, your upcoming meetings at the World Trade Center, and your upcoming dinner in Brooklyn."

    #         case "render_eclipse":
    #             additional_information = (
    #                 "I have rendered a 3d model of the eclipse for you to visualize."
    #             )
    #     afterFunctionResponse = chat.send_message(
    #         "Respond to the user that the action has been performed. Additional information: "
    #         + additional_information,
    #         tools=[],
    #     )
    #     print(afterFunctionResponse)

    #     return (
    #         jsonify(
    #             {
    #                 "status": "success",
    #                 "type": "function",
    #                 "function_name": function_name,
    #                 "text": afterFunctionResponse.text,
    #                 "image": imageString,
    #             }
    #         ),
    #         200,
    #     )
    # else:
    #     return (
    #         jsonify(
    #             {
    #                 "status": "success",
    #                 "type": "text",
    #                 "text": response.text,
    #                 "image": imageString,
    #             }
    #         ),
    #         200,
    #     )






async def start():
    await azureImageCall("What do you see?", "./test.png")


#if __name__ == "__main__":
    
    #print("starting server")
    # app.run(host="127.0.0.1", port=5000, debug=True)


async def azureImageCall(prompt, IMAGE_PATH="./test.png"):
    # Configuration
    encoded_image = base64.b64encode(open(IMAGE_PATH, 'rb').read()).decode('ascii')
    headers = {
        "Content-Type": "application/json",
        "api-key": GPT4V_KEY,
    }

    # Payload for the request
    payload = {
    "messages": [
        {
        "role": "system",
        "content": [
            {
            "type": "text",
            "text": "You are an AI assistant that helps people find information."
            }
        ]
        },
        {
        "role": "user",
        "content": [
            {
            "type": "image_url",
            "image_url": {
                "url": f"data:image/jpeg;base64,{encoded_image}"
            }
            },
            {
            "type": "text",
            "text": "what do you see?"
            }
        ]
        },
    ],
    "temperature": 0.7,
    "top_p": 0.95,
    "max_tokens": 800
    }

    GPT4V_ENDPOINT = "https://test833138126439.openai.azure.com/openai/deployments/gpt-4v/chat/completions?api-version=2024-02-15-preview"

    # Send request
    try:
        response = requests.post(GPT4V_ENDPOINT, headers=headers, json=payload)
        response.raise_for_status()  # Will raise an HTTPError if the HTTP request returned an unsuccessful status code
    except requests.RequestException as e:
        raise SystemExit(f"Failed to make the request. Error: {e}")

    # Handle the response as needed (e.g., print or process)
    print(response.json())

asyncio.run(azureImageCall("What do you see?", "./capture.png"))
