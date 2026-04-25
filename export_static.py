import os
import requests
import shutil
from bs4 import BeautifulSoup
import time

BASE_URL = "http://localhost:5205"
OUT_DIR = "../static_frontend"

if not os.path.exists(OUT_DIR):
    os.makedirs(OUT_DIR)
else:
    for item in os.listdir(OUT_DIR):
        if item == '.git': continue
        item_path = os.path.join(OUT_DIR, item)
        if os.path.isfile(item_path): os.remove(item_path)
        elif os.path.isdir(item_path): shutil.rmtree(item_path)
time.sleep(1)

shutil.copytree("wwwroot/css", os.path.join(OUT_DIR, "css"))
shutil.copytree("wwwroot/js", os.path.join(OUT_DIR, "js"))
if os.path.exists("wwwroot/images"):
    shutil.copytree("wwwroot/images", os.path.join(OUT_DIR, "images"))

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
        elif tag['href'] == '/Account/Login': tag['href'] = 'login.html'
        elif tag['href'] == '/Index' or tag['href'] == '/': tag['href'] = 'index.html'
    
    for tag in soup.find_all(src=True):
        if tag['src'].startswith('/js/'): tag['src'] = tag['src'].lstrip('/')
        elif tag['src'].startswith('/images/'): tag['src'] = tag['src'].lstrip('/')
        elif tag['src'].startswith('~/images/'): tag['src'] = tag['src'].replace('~/', '')
    
    # Change logout form action to index.html
    for form in soup.find_all('form'):
        if form.get('action') == '/Account/Logout':
            form['action'] = 'index.html'
            form['method'] = 'get'

    # Hardcode demo login links in login.html to make it easy for users to view the demos
    if filename == "login.html":
        # Add a demo section to the login card
        card = soup.find('div', class_='premium-card')
        if card:
            demo_html = """
            <div class="mt-4 pt-3 border-top border-secondary border-opacity-25 text-center">
                <p class="text-secondary small mb-2">View Interactive Demos:</p>
                <div class="d-flex flex-wrap gap-2 justify-content-center">
                    <a href="admin.html" class="btn btn-sm btn-outline-light" style="background: rgba(255,217,61,0.1); border-color: rgba(255,217,61,0.3); color: #ffd93d;">Admin</a>
                    <a href="company.html" class="btn btn-sm btn-outline-light" style="background: rgba(255,107,107,0.1); border-color: rgba(255,107,107,0.3); color: #ff6b6b;">Company</a>
                    <a href="supervisor.html" class="btn btn-sm btn-outline-light" style="background: rgba(108,142,255,0.1); border-color: rgba(108,142,255,0.3); color: #6c8eff;">Supervisor</a>
                    <a href="student.html" class="btn btn-sm btn-outline-light" style="background: rgba(78,205,196,0.1); border-color: rgba(78,205,196,0.3); color: #4ecdc4;">Student</a>
                </div>
            </div>
            """
            demo_soup = BeautifulSoup(demo_html, 'html.parser')
            card.append(demo_soup)

    with open(os.path.join(OUT_DIR, filename), 'w', encoding='utf-8') as f:
        f.write(str(soup))

# 1. Landing Page
r_landing = session.get(f"{BASE_URL}/")
save_html(r_landing.text, "index.html")

# 2. Login Page
r_login = session.get(f"{BASE_URL}/Account/Login")
save_html(r_login.text, "login.html")

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

# 3. Export dashboards
export_dashboard("admin@uni.edu", "/Admin/Index", "admin.html")
export_dashboard("dr.smith@uni.edu", "/Supervisor/Dashboard", "supervisor.html")
export_dashboard("hr@techcorp.com", "/Company/Dashboard", "company.html")
export_dashboard("student1@uni.edu", "/Student/Dashboard", "student.html")

print("Export completed to static_frontend!")
