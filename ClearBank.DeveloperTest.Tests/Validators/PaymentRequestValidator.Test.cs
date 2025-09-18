using ClearBank.DeveloperTest.Services;
using NUnit.Framework;
using ClearBank.DeveloperTest;

namespace ClearBank.DeveloperTest.Tests.Validators;

[TestFixture]
public class PaymentRequestValidatorTest
{
    public class GivenAnInvalidRequest
    {
        [Test]
        [TestCase("0")]
        [TestCase("-1")]
        public void WhenValidatingWithAInvalidAmountThenTheResultIsUnsuccessful(decimal amount)
        {
            var subject = new PaymentRequestValidator();

            var result = subject.ValidateRequest(
                new AccountBuilder().WithPaymentSchemes(Types.AllowedPaymentSchemes.FasterPayments).Build(),
                new Types.MakePaymentRequest() { DebtorAccountNumber = "987654321", Amount = amount });

            Assert.That(result.Success, Is.False);
        }
    }
}
