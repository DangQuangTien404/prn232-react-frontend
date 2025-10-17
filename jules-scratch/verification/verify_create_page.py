from playwright.sync_api import sync_playwright

def run(playwright):
    browser = playwright.chromium.launch(headless=True)
    context = browser.new_context()
    page = context.new_page()

    # Login
    page.goto("http://localhost:5173/login")
    page.fill('input[type="email"]', "teacher@example.com")
    page.fill('input[type="password"]', "password")
    page.click('button[type="submit"]')
    page.wait_for_url("http://localhost:5173/")

    # Go to create page
    page.goto("http://localhost:5173/create")
    page.screenshot(path="jules-scratch/verification/verification.png")

    browser.close()

with sync_playwright() as playwright:
    run(playwright)