using ClearBank.DeveloperTest.Services;
using NUnit.Framework;
using ClearBank.DeveloperTest;

namespace ClearBank.DeveloperTest.Tests.Validators;

[TestFixture]
public class BacsPaymentRequestValidatorTest
{
    public class GivenAValidRequest
    {
        [Test]
        public void WhenValidatingThenTheResultIsSuccessful()
        {
            var subject = new BacsPaymentRequestValidator();

            var result = subject.ValidateRequest(
                new AccountBuilder().WithPaymentSchemes(Types.AllowedPaymentSchemes.Bacs).Build(),
                new Types.MakePaymentRequest() { DebtorAccountNumber = "987654321" });

            Assert.That(result.Success, Is.True);
        }
    }

    public class GivenAnInvalidRequest
    {
        [Test]
        public void WhenValidatingWithAnInvalidPaymentSchemeThenTheResultIsUnsuccessful()
        {
            var subject = new BacsPaymentRequestValidator();

            var result = subject.ValidateRequest(
                new AccountBuilder().WithPaymentSchemes(Types.AllowedPaymentSchemes.Chaps).Build(),
                new Types.MakePaymentRequest() { DebtorAccountNumber = "987654321" });

            Assert.That(result.Success, Is.False);
        }
    }
}
