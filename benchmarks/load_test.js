import http from 'k6/http';
import { check } from 'k6';

const ENGINE_URL = __ENV.ENGINE_URL;
const MGMT_URL = __ENV.MGMT_URL;
const SLUG = '61d99749-9a83-4e4c-af73-d8ba4230056c';

export const options = {
  scenarios: {
    heavy_reads: {
      executor: 'constant-vus',
      vus: 80,
      duration: '1m',
      exec: 'redirect_logic',
    },
    constant_writes: {
      executor: 'constant-vus',
      vus: 20,
      duration: '1m',
      exec: 'create_logic',
    },
  },
  thresholds: {
    'http_req_duration{scenario:heavy_reads}': ['p(99)<20'],
    'http_req_duration{scenario:constant_writes}': ['p(99)<150'],
    'http_req_failed': ['rate<0.01'], 
  },
};

export function redirect_logic() {
  const res = http.get(`${ENGINE_URL}/api/links/${SLUG}`, { redirects: 0 });
  
  check(res, { 'redirect success': (r) => r.status === 303 });
}

export function create_logic() {
  const res = http.post(`${MGMT_URL}/api/links`, JSON.stringify({
    url: 'https://google.com',
  }), { 
    headers: { 'Content-Type': 'application/json' } 
  });

  check(res, { 'link created': (r) => r.status === 201 });
}
