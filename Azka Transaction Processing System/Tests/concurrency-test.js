import http from 'k6/http';
import { check, sleep } from 'k6';
import { CONFIG, getAuthHeaders } from './config.js';
import { createTransactionPayload, parseJsonResponse, DuplicateReceiptsCount } from './helpers.js';

export const options = {
    scenarios: {
        concurrent_transactions: {
            executor: 'per-vu-iterations',
            vus: 100,
            iterations: 1,
            maxDuration: '30s',
        },
    },
    thresholds: {
        duplicate_receipts_count: ['count==0'],
        http_req_failed: ['rate<0.01'],
    },
};

// Map to track receipts per VU execution session
const receiptTracker = new Set();

export default function () {
    const url = `${CONFIG.BASE_URL}/api/Transactions`;
    const payload = createTransactionPayload();
    const headers = getAuthHeaders();

    const res = http.post(url, payload, { headers });

    const isSuccess = check(res, {
        'Status is 200 or 201': (r) => r.status === 200 || r.status === 201,
    });

    if (isSuccess) {
        const body = parseJsonResponse(res);
        if (body && body.data && body.data.receiptNumber) {
            const receipt = body.data.receiptNumber;
            console.log(`[VU ${__VU}] Generated Receipt: ${receipt}`);

            if (receiptTracker.has(receipt)) {
                console.error(`[DUPLICATE DETECTED] Receipt: ${receipt}`);
                DuplicateReceiptsCount.add(1);
            } else {
                receiptTracker.add(receipt);
            }

            check(receipt, {
                'Receipt number is unique': () => !DuplicateReceiptsCount.value,
            });
        }
    }
}
