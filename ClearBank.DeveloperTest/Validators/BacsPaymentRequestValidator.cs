using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class BacsPaymentRequestValidator : IPaymentValidator
    {
        public MakePaymentResult ValidateRequest(Account account, MakePaymentRequest request)
        {
            var result = new MakePaymentResult() { Success = true };

            if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
            {
                result.Success = false;
            }

            return result;
        }
    }
}
