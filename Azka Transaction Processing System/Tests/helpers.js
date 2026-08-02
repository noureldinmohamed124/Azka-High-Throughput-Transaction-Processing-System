import { check } from 'k6';
import { Trend, Counter, Rate } from 'k6/metrics';

// Custom Metrics
export const TransactionDuration = new Trend('transaction_duration');
export const FailedRequestsCount = new Counter('failed_requests_count');
export const SuccessRate = new Rate('success_rate');
export const DuplicateReceiptsCount = new Counter('duplicate_receipts_count');

// Helper to construct a valid Transaction Payload
export function createTransactionPayload(overrides = {}) {
    return JSON.stringify({
        transactionType: "Payment",
        branchId: 1,
        paymentMethodId: 1,
        amount: 500,
        settledOn: new Date().toISOString(),
        transactionStatus: "Pending",
        ...overrides
    });
}

// Standard HTTP Response Check
export function checkResponse(res, expectedStatus = [200, 201], testName = 'Request') {
    const statusArray = Array.isArray(expectedStatus) ? expectedStatus : [expectedStatus];
    const isSuccess = check(res, {
        [`${testName} status is ${statusArray.join(' or ')}`]: (r) => statusArray.includes(r.status),
        [`${testName} duration < 500ms`]: (r) => r.timings.duration < 500,
    });

    TransactionDuration.add(res.timings.duration);
    SuccessRate.add(isSuccess);

    if (!isSuccess) {
        FailedRequestsCount.add(1);
    }

    return isSuccess;
}

// Helper to safely parse JSON response
export function parseJsonResponse(res) {
    try {
        return JSON.parse(res.body);
    } catch (e) {
        return null;
    }
}
