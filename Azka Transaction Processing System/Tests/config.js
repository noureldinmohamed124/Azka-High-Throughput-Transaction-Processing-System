export const CONFIG = {
    BASE_URL: __ENV.BASE_URL || 'http://localhost:5010',
    TOKEN: __ENV.JWT_TOKEN || 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJBemthX1RQUyIsImlhdCI6MTc4NTY5ODA4NCwiZXhwIjoxNzg1Njk5MzIzLCJhdWQiOiJBemthLmNvbSIsInN1YiI6IjEiLCJHaXZlbk5hbWUiOiJKb2hubnkiLCJTdXJuYW1lIjoiUm9ja2V0IiwiRW1haWwiOiJqcm9ja2V0QGV4YW1wbGUuY29tIiwiUm9sZSI6WyJNYW5hZ2VyIiwiUHJvamVjdCBBZG1pbmlzdHJhdG9yIl19.l8NPhdxI5gJDxwty4CDKoJSI9NluIz3i5uwFGGfvKs4',
    DEFAULT_HEADERS: {
        'Content-Type': 'application/json'
    },
    THRESHOLDS: {
        http_req_duration: ['p(95)<500'],
        http_req_failed: ['rate<0.01']
    }
};

export function getAuthHeaders() {
    return {
        ...CONFIG.DEFAULT_HEADERS,
        'Authorization': `Bearer ${CONFIG.TOKEN}`
    };
}
