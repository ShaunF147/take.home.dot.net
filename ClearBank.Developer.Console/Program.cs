using System.Configuration;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest;

var service = new PaymentService(accountDataStore(), new PaymentRequestValidator());
var result = service.MakePayment(new MakePaymentRequest()
{
    Amount = 100m,
    DebtorAccountNumber = "98765432",
    PaymentScheme = PaymentScheme.Bacs,
});

Console.WriteLine(result.Success ? "Payment successful" : "Payment failed");

static IAccountDataStore accountDataStore()
{
    if (ConfigurationManager.AppSettings["DataStoreType"] == "Backup")
    {
        return new BackupAccountDataStore();
    }
    else
    {
        return new StubAccountDataStore();
    }
}
