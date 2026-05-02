const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage();
  const logs = [];
  const errors = [];
  const requests = [];

  page.on('console', msg => logs.push(`[${msg.type()}] ${msg.text()}`));
  page.on('pageerror', err => errors.push(`[pageerror] ${err.stack || err.message}`));
  page.on('response', response => {
    if (response.status() >= 400) {
      requests.push(`[${response.status()}] ${response.url()}`);
    }
  });

  try {
    await page.goto('http://127.0.0.1:5173/', { waitUntil: 'networkidle', timeout: 90000 });
    await page.waitForTimeout(10000);
    const html = await page.locator('#uno-body').evaluate(el => el.innerHTML).catch(() => '<uno-body not found>');
    console.log('HTML_START');
    console.log(html);
    console.log('HTML_END');
  } catch (err) {
    errors.push(`[goto] ${err.stack || err.message}`);
  }

  console.log('CONSOLE_START');
  for (const line of logs) console.log(line);
  console.log('CONSOLE_END');
  console.log('ERRORS_START');
  for (const line of errors) console.log(line);
  console.log('ERRORS_END');
  console.log('REQUESTS_START');
  for (const line of requests) console.log(line);
  console.log('REQUESTS_END');

  await browser.close();
})();

