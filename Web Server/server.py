from flask import Flask, request, jsonify

app = Flask(__name__)

@app.route('/data', methods=['POST'])
def receive_data():
    data = request.json
    user_input = data["user_input"]
    print("Data Received:", data)
    print("User input: ", user_input)
    return jsonify({"status": "success", "message": "We have received " + user_input}), 200

if __name__ == '__main__':
    app.run(host='127.0.0.1', port=5000, debug=True)
