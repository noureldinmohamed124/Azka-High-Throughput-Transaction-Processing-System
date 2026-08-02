import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { CONFIG } from '../Config/config.js';
import { AUTH_TEST_TOKENS } from '../Utilities/auth-tokens.js';
import { SECURITY_PAYLOADS } from '../Utilities/security-payloads.js';
import { createTransactionPayload } from '../Utilities/helpers.js';

export const options = {
    vus: 1,
    duration: '5s',
    thresholds: CONFIG.THRESHOLDS,
};

export default function () {
    const url = `${CONFIG.BASE_URL}/api/Transactions`;

    group('06. Authentication & Security Testing Suite', function () {
        // 1. Missing Authorization Header
        {
            const res = http.post(url, createTransactionPayload(), {
                headers: { 'Content-Type': 'application/json' }
            });
            check(res, { 'missing JWT returns 401': (r) => r.status === 401 });
        }

        // 2. Expired JWT
        {
            const res = http.post(url, createTransactionPayload(), {
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${AUTH_TEST_TOKENS.EXPIRED_JWT}` }
            });
            check(res, { 'expired JWT returns 401': (r) => r.status === 401 });
        }

        // 3. Malformed JWT
        {
            const res = http.post(url, createTransactionPayload(), {
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${AUTH_TEST_TOKENS.MALFORMED_JWT}` }
            });
            check(res, { 'malformed JWT returns 401': (r) => r.status === 401 });
        }

        // 4. Invalid Signature JWT
        {
            const res = http.post(url, createTransactionPayload(), {
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${AUTH_TEST_TOKENS.INVALID_SIGNATURE_JWT}` }
            });
            check(res, { 'invalid signature JWT returns 401': (r) => r.status === 401 });
        }

        // 5. Authorization without Bearer prefix
        {
            const res = http.post(url, createTransactionPayload(), {
                headers: { 'Content-Type': 'application/json', 'Authorization': CONFIG.JWT_TOKEN }
            });
            check(res, { 'authorization without Bearer prefix returns 401': (r) => r.status === 401 });
        }

        // 6. SQL Injection Payload in search query
        {
            const sqli = SECURITY_PAYLOADS.SQL_INJECTION[0];
            const searchUrl = `${CONFIG.BASE_URL}/api/Transactions?customerId=${encodeURIComponent(sqli)}`;
            const res = http.get(searchUrl, {
                headers: { 'Authorization': `Bearer ${CONFIG.JWT_TOKEN}` }
            });
            check(res, { 'SQL injection in query returns 400': (r) => r.status === 400 || r.status === 404 });
        }

        // 7. XSS Injection Payload in receipt search
        {
            const xss = SECURITY_PAYLOADS.XSS_ATTACKS[0];
            const receiptUrl = `${CONFIG.BASE_URL}/api/Transactions/receipt/${encodeURIComponent(xss)}`;
            const res = http.get(receiptUrl, {
                headers: { 'Authorization': `Bearer ${CONFIG.JWT_TOKEN}` }
            });
            check(res, { 'XSS payload in URL handled safely': (r) => r.status === 400 || r.status === 404 });
        }
    });

    sleep(1);
}
