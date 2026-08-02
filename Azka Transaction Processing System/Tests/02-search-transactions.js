import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { CONFIG, getAuthHeaders } from './config.js';
import { checkResponse, parseJsonResponse } from './helpers.js';

export const options = {
    vus: 1,
    duration: '5s',
    thresholds: CONFIG.THRESHOLDS,
};

export default function () {
    const url = `${CONFIG.BASE_URL}/api/Transactions`;
    const headers = getAuthHeaders();

    group('02. GET /api/Transactions - Search Suite', function () {
        // 1. Get All Transactions
        {
            const res = http.get(url, { headers });
            checkResponse(res, 200, 'Search All Transactions');
            const body = parseJsonResponse(res);
            check(body, {
                'response has success=true': (b) => b && b.success === true,
                'data is array': (b) => b && Array.isArray(b.data),
            });
        }

        // 2. Search by Customer ID
        {
            const res = http.get(`${url}?customerId=1`, { headers });
            checkResponse(res, 200, 'Search by customerId');
        }

        // 3. Search by Date
        {
            const today = new Date().toISOString().split('T')[0];
            const res = http.get(`${url}?date=${today}`, { headers });
            checkResponse(res, 200, 'Search by date');
        }

        // 4. Search by Customer ID + Date
        {
            const today = new Date().toISOString().split('T')[0];
            const res = http.get(`${url}?customerId=1&date=${today}`, { headers });
            checkResponse(res, 200, 'Search by customerId + date');
        }

        // 5. Search with Invalid Parameter Types
        {
            const res = http.get(`${url}?customerId=invalid_id`, { headers });
            check(res, { 'invalid customerId param returns 400': (r) => r.status === 400 });
        }
    });

    sleep(1);
}
