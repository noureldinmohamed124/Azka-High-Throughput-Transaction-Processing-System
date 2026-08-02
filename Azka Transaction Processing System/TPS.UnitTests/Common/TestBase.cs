using Moq;
using Azka_Transaction_Processing_System.Application.Abstractions.Common;
using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Application.Abstractions.Services;

namespace TPS.UnitTests.Common
{
    public abstract class TestBase
    {
        protected readonly Mock<ICustomerRepo> MockCustomerRepo;
        protected readonly Mock<IBranchRepo> MockBranchRepo;
        protected readonly Mock<IPaymentMethodRepo> MockPaymentMethodRepo;
        protected readonly Mock<ITransactionRepo> MockTransactionRepo;
        protected readonly Mock<IReceiptGenerator> MockReceiptGenerator;
        protected readonly Mock<IUnitOfWork> MockUnitOfWork;
        protected readonly Mock<ICurrentUserService> MockCurrentUserService;

        protected TestBase()
        {
            MockCustomerRepo = new Mock<ICustomerRepo>();
            MockBranchRepo = new Mock<IBranchRepo>();
            MockPaymentMethodRepo = new Mock<IPaymentMethodRepo>();
            MockTransactionRepo = new Mock<ITransactionRepo>();
            MockReceiptGenerator = new Mock<IReceiptGenerator>();
            MockUnitOfWork = new Mock<IUnitOfWork>();
            MockCurrentUserService = new Mock<ICurrentUserService>();
        }
    }
}
