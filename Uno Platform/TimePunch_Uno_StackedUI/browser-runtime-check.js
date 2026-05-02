const { chromium } = require('playwright');
const http = require('http');
const path = require('path');
const fs = require('fs');

const root = path.resolve(__dirname, '..', 'TimePunch_Uno_StackedUI_Demo', 'TimePunch_Uno_StackedUI_Demo', 'bin', 'tpuno', 'Debug', 'net10.0-browserwasm', 'wwwroot');
const port = 5007;

function contentType(filePath) {
  const ext = path.extname(filePath).toLowerCase();
  return {
    '.html': 'text/html; charset=utf-8',
    '.js': 'text/javascript; charset=utf-8',
    '.css': 'text/css; charset=utf-8',
    '.json': 'application/json; charset=utf-8',
    '.wasm': 'application/wasm',
    '.dll': 'application/octet-stream',
    '.pdb': 'application/octet-stream',
    '.dat': 'application/octet-stream',
    '.blat': 'application/octet-stream',
    '.woff': 'font/woff',
    '.woff2': 'font/woff2',
    '.png': 'image/png',
    '.jpg': 'image/jpeg',
    '.jpeg': 'image/jpeg',
    '.gif': 'image/gif',
    '.svg': 'image/svg+xml',
    '.ico': 'image/x-icon',
    '.txt': 'text/plain; charset=utf-8',
    '.webmanifest': 'application/manifest+json; charset=utf-8'
  }[ext] || 'application/octet-stream';
}

const server = http.createServer((req, res) => {
  const requestPath = decodeURIComponent((req.url || '/').split('?')[0]);
  const normalized = requestPath === '/' ? '/index.html' : requestPath;
  const candidate = path.normalize(path.join(root, '.' + normalized));

  if (!candidate.startsWith(root)) {
    res.statusCode = 403;
    res.end('Forbidden');
    return;
  }

  let filePath = candidate;
  if (fs.existsSync(filePath) && fs.statSync(filePath).isDirectory()) {
    filePath = path.join(filePath, 'index.html');
  }

  if (!fs.existsSync(filePath)) {
    res.statusCode = 404;
    res.end('Not Found');
    return;
  }

  res.setHeader('Content-Type', contentType(filePath));
  fs.createReadStream(filePath).pipe(res);
});

(async () => {
  const logs = [];
  const errors = [];
  const requests = [];

  await new Promise(resolve => server.listen(port, resolve));

  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage();

  page.on('console', msg => {
    logs.push(`[${msg.type()}] ${msg.text()}`);
  });

  page.on('pageerror', err => {
    errors.push(`[pageerror] ${err.stack || err.message}`);
  });

  page.on('response', response => {
    if (response.status() >= 400) {
      requests.push(`[${response.status()}] ${response.url()}`);
    }
  });

  try {
    await page.goto(`http://127.0.0.1:${port}/`, { waitUntil: 'networkidle', timeout: 60000 });
    await page.waitForTimeout(10000);
    const html = await page.locator('#uno-body').evaluate(el => el.innerHTML).catch(() => '<uno-body not found>');
    console.log('HTML_START');
    console.log(html);
    console.log('HTML_END');
  } catch (err) {
    errors.push(`[goto] ${err.stack || err.message}`);
  } finally {
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
    server.close();
  }
})();
