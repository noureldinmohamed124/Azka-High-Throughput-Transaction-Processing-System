import http from 'k6/http';
import { sleep } from 'k6';
import { CONFIG, getAuthHeaders } from './config.js';
import { createTransactionPayload, checkResponse } from './helpers.js';

export const options = {
    stages: [
        { duration: '10s', target: 10 },
        { duration: '15s', target: 25 },
        { duration: '15s', target: 50 },
        { duration: '20s', target: 100 },
        { duration: '20s', target: 200 },
        { duration: '20s', target: 300 },
        { duration: '20s', target: 500 },
        { duration: '10s', target: 0 },
    ],
    thresholds: CONFIG.THRESHOLDS,
};

export default function () {
    const url = `${CONFIG.BASE_URL}/api/Transactions`;
    const payload = createTransactionPayload();
    const headers = getAuthHeaders();

    const res = http.post(url, payload, { headers });
    checkResponse(res, [200, 201], 'Stress Test POST Transaction');

    sleep(0.5);
}
