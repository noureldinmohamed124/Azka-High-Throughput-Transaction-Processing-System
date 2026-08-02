import postTransactionTest from './01-post-transaction.js';
import searchTransactionsTest from './02-search-transactions.js';
import getReceiptTest from './03-get-receipt.js';
import dailySummaryTest from './04-daily-summary.js';
import paymentMethodsTest from './05-payment-methods.js';
import authSecurityTest from './06-auth-security-test.js';
import { CONFIG } from '../Config/config.js';

export const options = {
    scenarios: {
        high_throughput_suite: {
            executor: 'constant-vus',
            vus: 20,
            duration: '20s',
        },
    },
    thresholds: CONFIG.THRESHOLDS,
};

export default function () {
    postTransactionTest();
    searchTransactionsTest();
    getReceiptTest();
    dailySummaryTest();
    paymentMethodsTest();
    authSecurityTest();
}
