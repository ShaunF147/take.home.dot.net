using ClearBank.DeveloperTest.Types;
using System.Collections.Generic;

namespace ClearBank.DeveloperTest.Services
{
    /// <summary>
    /// The payment request validator. A composite of one or more validators for each payment scheme. The correct validator will be used by type but this allows future changes to the validaton logic without modifying the payment service itself as long as the contract remains stable.
    /// </summary>
    public class PaymentRequestValidator : IPaymentValidator
    {
        private readonly Dictionary<PaymentScheme, IPaymentValidator> validators;

        public PaymentRequestValidator()
        {
            // configure each validator, this could be done by convention at the composition root
            // but given the simple setup now this can remain inline for now.
            this.validators = new Dictionary<PaymentScheme, IPaymentValidator>()
            {
                { PaymentScheme.Bacs, new BacsPaymentRequestValidator()},
                { PaymentScheme.FasterPayments, new FasterPaymentsRequestValidator()},
                { PaymentScheme.Chaps, new ChapsRequestValidator()},
            };
        }

        public MakePaymentResult ValidateRequest(Account account, MakePaymentRequest request)
        {
            // we don't handle missing validators for scheme types by default, let the error throw.
            // this should not happen during normal scenarios and we would enforce this by tests
            // or compile time checks.
            var validator = validators[request.PaymentScheme];

            // common validation would go here, this could be futher extracted as it became more complex.

            // prevent zero or negative payments which are not supported, otherwise the balance could increase.
            if (request.Amount <= 0)
            {
                return new MakePaymentResult() { Success = false };
            }

            // the composite validator gives scope for shared validation rules if needed, otherwise
            // the payment scheme specific validator is invoked. with more time we could add more common
            // request validation and finally invoke the correct scheme validator if needed.
            return validator.ValidateRequest(account, request);
        }
    }
}
