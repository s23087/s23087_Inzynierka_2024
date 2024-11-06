# Script for windows task scheduler. Update invoice status.
import os
import requests
from dotenv import load_dotenv
from pathlib import Path

pyFilePath = os.path.dirname(os.path.realpath(__file__))
os.chdir(pyFilePath)
envPath = Path('../web/handler_b2b/.env')
load_dotenv(dotenv_path=envPath)
API_DEST = os.getenv('API_DEST')
os.chdir("../database")
databaseDir = os.getcwd()

for dir in os.listdir(databaseDir):
    if dir != "template":
        url = f"{API_DEST}/{dir}/Invoices/update/status"
        response = requests.post(url)
        if (response.status_code != 200):
            logs = open(f"{pyFilePath}/scripts_logs.txt", "a")
            logs.write(f"Error for {dir} database: could not connect to database")
            logs.close()
        
