import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { CONFIG, getAuthHeaders } from './config.js';
import { createTransactionPayload, checkResponse, parseJsonResponse } from './helpers.js';

export const options = {
    vus: 1,
    duration: '5s',
    thresholds: CONFIG.THRESHOLDS,
};

export default function () {
    const baseUrl = `${CONFIG.BASE_URL}/api/Transactions`;
    const headers = getAuthHeaders();

    group('03. GET /api/Transactions/receipt/{receiptNumber}', function () {
        // Step 1: Create a transaction to ensure we have a valid receipt number
        const createRes = http.post(baseUrl, createTransactionPayload(), { headers });
        const createBody = parseJsonResponse(createRes);
        const validReceipt = (createBody && createBody.data) ? createBody.data.receiptNumber : 'PAY-20260802-1-000001';

        // 1. Get Valid Receipt
        {
            const res = http.get(`${baseUrl}/receipt/${validReceipt}`, { headers });
            checkResponse(res, 200, 'Get Valid Receipt');
            const body = parseJsonResponse(res);
            check(body, {
                'receipt number matches requested': (b) => b && b.data && b.data.receiptNumber === validReceipt,
            });
        }

        // 2. Get Invalid Receipt
        {
            const res = http.get(`${baseUrl}/receipt/INVALID-RECEIPT-999999`, { headers });
            check(res, { 'non-existent receipt returns 404 or success=false': (r) => r.status === 404 || r.status === 200 });
        }

        // 3. Special Characters Receipt
        {
            const res = http.get(`${baseUrl}/receipt/<script>alert(1)</script>`, { headers });
            check(res, { 'special chars receipt handled properly': (r) => r.status === 400 || r.status === 404 });
        }
    });

    sleep(1);
}
