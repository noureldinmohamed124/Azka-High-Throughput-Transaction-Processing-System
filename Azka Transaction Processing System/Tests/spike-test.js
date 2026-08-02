import http from 'k6/http';
import { sleep } from 'k6';
import { CONFIG, getAuthHeaders } from './config.js';
import { createTransactionPayload, checkResponse } from './helpers.js';

export const options = {
    stages: [
        { duration: '5s', target: 1 },
        { duration: '10s', target: 200 }, // Spike!
        { duration: '15s', target: 200 }, // Hold spike
        { duration: '5s', target: 1 },    // Recovery
        { duration: '5s', target: 0 },
    ],
    thresholds: CONFIG.THRESHOLDS,
};

export default function () {
    const url = `${CONFIG.BASE_URL}/api/Transactions`;
    const payload = createTransactionPayload();
    const headers = getAuthHeaders();

    const res = http.post(url, payload, { headers });
    checkResponse(res, [200, 201], 'Spike Test POST Transaction');

    sleep(0.2);
}
