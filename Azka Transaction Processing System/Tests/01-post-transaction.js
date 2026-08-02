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
    const url = `${CONFIG.BASE_URL}/api/Transactions`;
    const headers = getAuthHeaders();

    group('01. POST /api/Transactions - Smoke Test', function () {
        const payload = createTransactionPayload();
        const res = http.post(url, payload, { headers });

        checkResponse(res, [200, 201], 'Smoke POST Transaction');
        
        const body = parseJsonResponse(res);
        check(body, {
            'response has success=true': (b) => b && b.success === true,
            'response has data.receiptNumber': (b) => b && b.data && b.data.receiptNumber !== undefined,
            'response amount equals 500': (b) => b && b.data && b.data.amount === 500,
            'response status is Pending': (b) => b && b.data && b.data.status === 'Pending',
        });
    });

    group('02. POST /api/Transactions - Validation Tests', function () {
        // Test 1: Negative Amount
        {
            const payload = createTransactionPayload({ amount: -100 });
            const res = http.post(url, payload, { headers });
            check(res, { 'negative amount fails (400/422/409)': (r) => r.status >= 400 });
        }

        // Test 2: Zero Amount
        {
            const payload = createTransactionPayload({ amount: 0 });
            const res = http.post(url, payload, { headers });
            check(res, { 'zero amount validation handled': (r) => r.status === 200 || r.status >= 400 });
        }

        // Test 3: Invalid Branch ID
        {
            const payload = createTransactionPayload({ branchId: 999999 });
            const res = http.post(url, payload, { headers });
            check(res, { 'invalid branchId returns 404/400': (r) => r.status >= 400 });
        }

        // Test 4: Invalid Payment Method ID
        {
            const payload = createTransactionPayload({ paymentMethodId: 999999 });
            const res = http.post(url, payload, { headers });
            check(res, { 'invalid paymentMethodId returns 404/400': (r) => r.status >= 400 });
        }

        // Test 5: Missing Required Fields
        {
            const payload = JSON.stringify({ amount: 100 });
            const res = http.post(url, payload, { headers });
            check(res, { 'missing fields returns >= 400': (r) => r.status >= 400 });
        }

        // Test 6: Invalid Transaction Type
        {
            const payload = createTransactionPayload({ transactionType: "InvalidType" });
            const res = http.post(url, payload, { headers });
            check(res, { 'invalid transactionType returns >= 400': (r) => r.status >= 400 });
        }

        // Test 7: Malformed JSON
        {
            const res = http.post(url, '{ malformed json: ', { headers });
            check(res, { 'malformed json returns 400': (r) => r.status === 400 });
        }

        // Test 8: Wrong Content-Type
        {
            const res = http.post(url, 'plain text', {
                headers: { 'Authorization': `Bearer ${CONFIG.TOKEN}`, 'Content-Type': 'text/plain' }
            });
            check(res, { 'wrong content type handled (415/400)': (r) => r.status >= 400 });
        }
    });

    sleep(1);
}
