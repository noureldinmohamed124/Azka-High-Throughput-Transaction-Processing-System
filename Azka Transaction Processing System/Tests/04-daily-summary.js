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
    const url = `${CONFIG.BASE_URL}/api/Transactions/daily-summary`;
    const headers = getAuthHeaders();

    group('04. GET /api/Transactions/daily-summary', function () {
        // 1. Valid Today Date
        {
            const today = new Date().toISOString().split('T')[0];
            const res = http.get(`${url}?date=${today}`, { headers });
            checkResponse(res, 200, 'Daily Summary Today');
            const body = parseJsonResponse(res);
            check(body, {
                'has totalTransactions field': (b) => b && b.data && b.data.totalTransactions !== undefined,
                'has totalAmount field': (b) => b && b.data && b.data.totalAmount !== undefined,
            });
        }

        // 2. Future Date
        {
            const res = http.get(`${url}?date=2099-12-31`, { headers });
            checkResponse(res, 200, 'Daily Summary Future Date');
        }

        // 3. Old Date
        {
            const res = http.get(`${url}?date=2000-01-01`, { headers });
            checkResponse(res, 200, 'Daily Summary Old Date');
        }

        // 4. Invalid Format
        {
            const res = http.get(`${url}?date=invalid-date`, { headers });
            check(res, { 'invalid date format returns 400': (r) => r.status === 400 });
        }
    });

    sleep(1);
}
