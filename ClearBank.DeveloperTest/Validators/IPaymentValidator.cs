using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public interface IPaymentValidator
    {
        MakePaymentResult ValidateRequest(Account account, MakePaymentRequest makePaymentRequest);
    }
}
