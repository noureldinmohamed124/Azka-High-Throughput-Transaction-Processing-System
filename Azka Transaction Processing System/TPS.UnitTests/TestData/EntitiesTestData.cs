using Azka_Transaction_Processing_System.Domain.Entities;

namespace TPS.UnitTests.TestData
{
    public static class CustomerTestData
    {
        public static Customer CreateValidCustomer(int id = 1, string name = "John Doe")
        {
            return new Customer
            {
                Id = id,
                FullName = name,
                Email = "john.doe@example.com",
                Phone = "+1234567890"
            };
        }
    }

    public static class BranchTestData
    {
        public static Branch CreateValidBranch(int id = 1, string name = "Main Branch")
        {
            return new Branch
            {
                Id = id,
                Name = name,
                Code = "MB-01"
            };
        }
    }

    public static class PaymentMethodTestData
    {
        public static PaymentMethod CreateValidPaymentMethod(int id = 1, string name = "Visa Card")
        {
            return new PaymentMethod
            {
                Id = id,
                Name = name
            };
        }
    }
}
