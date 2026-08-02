import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 1,
    duration: '10s',

    thresholds: {
        http_req_duration: ['p(95)<500'],
        http_req_failed: ['rate<0.01'],
    },
};

const BASE_URL = 'http://localhost:5010';

const TOKEN = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJBemthX1RQUyIsImlhdCI6MTc4NTY5ODA4NCwiZXhwIjoxNzg1Njk5MzIzLCJhdWQiOiJBemthLmNvbSIsInN1YiI6IjEiLCJHaXZlbk5hbWUiOiJKb2hubnkiLCJTdXJuYW1lIjoiUm9ja2V0IiwiRW1haWwiOiJqcm9ja2V0QGV4YW1wbGUuY29tIiwiUm9sZSI6WyJNYW5hZ2VyIiwiUHJvamVjdCBBZG1pbmlzdHJhdG9yIl19.l8NPhdxI5gJDxwty4CDKoJSI9NluIz3i5uwFGGfvKs4';

export default function () {

    const payload = JSON.stringify({
        transactionType: "Payment",
        branchId: 1,
        paymentMethodId: 1,
        amount: 500,
        settledOn: new Date().toISOString(),
        transactionStatus: "Pending"
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${TOKEN}`
        }
    };

    const response = http.post(
        `${BASE_URL}/api/Transactions`,
        payload,
        params
    );

    console.log('Status: ' + response.status);
    console.log('Response: ' + response.body);

    check(response, {
        'Status is 200 or 201': (r) =>
            r.status === 200 || r.status === 201,

        'Response time أقل من 500ms': (r) =>
            r.timings.duration < 500,
    });

    sleep(1);
}