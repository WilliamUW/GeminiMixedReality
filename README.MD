1. Pre-requisites

```
cd server

choco install ngrok

ngrok config add-authtoken <token>
```

2. Start local python web server

In server folder
```
cd server
pip install -r requirements.txt
python app.py
```

3. Make public with ngrok

In separate terminal in server folder, run:
```
cd server
ngrok http http://127.0.0.1:5000
```

Set the ngrok forwarding public url as the Quest 3 app's "url" constant variable.

E.g. "https://83aa-99-227-197-26.ngrok-free.app"

4. Build and run to Quest 3.

Build "mainScene" to Quest 3.

5. Start casting to computer.

6. Talk to GARVIS by having your palm up.