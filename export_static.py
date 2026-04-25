import os
import requests
import shutil
from bs4 import BeautifulSoup

BASE_URL = "http://localhost:5205"
OUT_DIR = "../static_frontend"

if os.path.exists(OUT_DIR):
    shutil.rmtree(OUT_DIR)
os.makedirs(OUT_DIR)

shutil.copytree("wwwroot/css", os.path.join(OUT_DIR, "css"))
shutil.copytree("wwwroot/js", os.path.join(OUT_DIR, "js"))

session = requests.Session()

def get_token(url):
    r = session.get(url)
    soup = BeautifulSoup(r.text, 'html.parser')
    token = soup.find('input', {'name': '__RequestVerificationToken'})
    return token['value'] if token else None

def save_html(html, filename):
    soup = BeautifulSoup(html, 'html.parser')
    # Fix paths
    for tag in soup.find_all(href=True):
        if tag['href'].startswith('/css/'): tag['href'] = tag['href'].lstrip('/')
        elif tag['href'] == '/Admin/Index': tag['href'] = 'admin.html'
        elif tag['href'] == '/Supervisor/Dashboard': tag['href'] = 'supervisor.html'
        elif tag['href'] == '/Company/Dashboard': tag['href'] = 'company.html'
        elif tag['href'] == '/Student/Dashboard': tag['href'] = 'student.html'
        elif tag['href'] == '/Account/Login': tag['href'] = 'index.html'
        elif tag['href'] == '/Index': tag['href'] = 'index.html'
    
    for tag in soup.find_all(src=True):
        if tag['src'].startswith('/js/'): tag['src'] = tag['src'].lstrip('/')
    
    # Change logout form action to index.html
    for form in soup.find_all('form'):
        if form.get('action') == '/Account/Logout':
            form['action'] = 'index.html'
            form['method'] = 'get'

    with open(os.path.join(OUT_DIR, filename), 'w', encoding='utf-8') as f:
        f.write(str(soup))

# 1. Index (Login)
r = session.get(f"{BASE_URL}/Account/Login")
save_html(r.text, "index.html")

def export_dashboard(email, dashboard_url, out_name):
    session.cookies.clear()
    token = get_token(f"{BASE_URL}/Account/Login")
    r = session.post(f"{BASE_URL}/Account/Login", data={
        "Input.Email": email,
        "Input.Password": "Password123!",
        "__RequestVerificationToken": token
    })
    r2 = session.get(f"{BASE_URL}{dashboard_url}")
    save_html(r2.text, out_name)

# Export dashboards
export_dashboard("admin@uni.edu", "/Admin/Index", "admin.html")
export_dashboard("dr.smith@uni.edu", "/Supervisor/Dashboard", "supervisor.html")
export_dashboard("hr@techcorp.com", "/Company/Dashboard", "company.html")
export_dashboard("student1@uni.edu", "/Student/Dashboard", "student.html")

print("Export completed to static_frontend!")
