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
    const url = `${CONFIG.BASE_URL}/api/PaymentMethods`;
    const headers = getAuthHeaders();

    group('05. GET /api/PaymentMethods', function () {
        const res = http.get(url, { headers });
        checkResponse(res, 200, 'Get Payment Methods');

        const body = parseJsonResponse(res);
        check(body, {
            'response has success=true': (b) => b && b.success === true,
            'data is non-empty array': (b) => b && Array.isArray(b.data) && b.data.length > 0,
        });
    });

    sleep(1);
}
