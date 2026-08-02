import http from 'k6/http';
import { sleep } from 'k6';
import { CONFIG, getAuthHeaders } from './config.js';
import { createTransactionPayload, checkResponse } from './helpers.js';

export const options = {
    stages: [
        { duration: '10s', target: 10 },
        { duration: '20s', target: 25 },
        { duration: '20s', target: 50 },
        { duration: '20s', target: 100 },
        { duration: '10s', target: 0 },
    ],
    thresholds: CONFIG.THRESHOLDS,
};

export default function () {
    const url = `${CONFIG.BASE_URL}/api/Transactions`;
    const payload = createTransactionPayload();
    const headers = getAuthHeaders();

    const res = http.post(url, payload, { headers });
    checkResponse(res, [200, 201], 'Load Test POST Transaction');

    sleep(1);
}
