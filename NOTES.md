# Instructions

Built and tested against: `8.0.413`, use the included dev container if required.

```
dotnet restore
dotnet build
dotnet test
```

Alternatively you can run `dotnet run --project ClearBank.Developer.Console/ClearBank.Developer.Console.csproj` and modify the request within `Program.cs` to test various flows.

See below for some notes that tracked my thoughts and process during this task. I'm happy to go into more detail on them as needed.

Also included is a "More time" section - a rough summary of things I'd cover or include in a more realistic scenario.

## Notes on process and workflow

I started by creating a devcontainer so I had some way of running the code, installed a few extensions.

Does the project build first off? - yes.

Created a readme to record steps and ideas.

Viewed the code and noticed some obvious issues from the get go that would prevent me from adding some test coverage before starting to refactor.

First goal is to get a working end to end test covering the code to verify that any refactoring does not cause an issue. Given the branches we'd need at least 4 tests to gain enough confidence. One for each payment scheme and the failure flow when no update is made. This is not exhaustive code coverage but good enough to unlock some refactoring steps later.

Relying on leaning on the compiler and a quick debugging session I could see that the account data store was a problem. I extracted an interface and pushed the logic for determining the store to use up a layer. By commenting this out and adding a stub I was able to test at least the happy path flow for BACS. I repeated this for the other 2 scenarios. The datastore to use is fixed once the app starts so pushing it up to the composition root makes sense, either the real datastore is used or the backup. By relying on the interface however the service is unaware of the concrete implementation used.

The final part of the happy path scenario is to cover that an account is updated if successful validated. There are other branches not tested here currently but with my plan to extract the logic to standalone classes I'll add the missing coverage as part of that. The safety net of the top level tests against the payment service give me some confidence however. In order to do this I introduced the use of a mock object (Moq) to verify the account update is actually called. Once this scenario was covered the other two were just repeated.

I introduced a console app to run the app for manual verification. While the automated tests are easier this just ensures I don't break the object graph and provides some extra compile type safety. In a real scenario this would correspond to `Main` or your application root.

The next obvious thing to tackle was the validation logic. There was a clear pattern here and while only three examples now the validation logic would be more complex in a real life scenario. I started by introducing a common interface that would work and added 3 classes that were responsible for purely validating the payment they know about. The use of this started off inline and ran side by side the existing code. With the tests working I was able to comment out the old flow and see the tests still passing. A minor snag around the defaulting of the validation result was detected and fixed here so this intermediate step proved useful.

With the validation now standalone the next bit of duplication comes from repeatedly checking if the account is not null. I originally enabled nullable reference types in the csproj to help detect this. I'm quite a fan of this in TypeScript so this was a simple way to achieve a similar setup, however it dawned on me after that this causes changes within the request class which could be seen as a breaking change. After reverting this I coded the check manually but would encourage this use of this with more time.

With the changes stable again I wrapped some tests around the individual payment request validators. In a real scenario this logic would be more complex but for now this is similar to the original code but with the excessive null checking removed. I also introduced the `AccountBuilder` as a test util to make test setup quicker and easier once I noticed duplication between some of the tests.

Finally I extracted a composite validator for use within the payment service. I was treating this service as an application service, so while a bit overkill now the validator can be provided as a dependency. In a real scenario there would likely be external dependencies that each validator rely on also. The composite validator allows the logic to change or share common rules without impacting the overall service logic. Note the original tests have been preserved that were used as part of the refactor. These are operating as acceptance tests now, so while they don't cover all the flows for each scheme – they provide value by ensuring the core journey is still hanging together.

To show an example of the common logic I started by added validation that the amount on the payment request is not 0 or negative. This is prevent the balance increasing as the payment logic should only deduct. Extra common validation would be added but due to time constraints I've not included this. E.g. payment date must be now or within a range, amounts must be within a range and so on.

During manual testing I noticed a bug with the use of the enum flags. At the same time I wrapped up the core code changes by introducing some basic exception handling.

The last step was to conclude the readme and move some of the files and types around to more sensible locations prior to review.

## More time

The result of successful validation is the same for all schemes. In the likelihood of this being different per payment scheme this could be replaced with an object that performed an action instead. This would be responsible for the balance update and any other steps needed.

There is no logging, during development I leaned on the debugger for a couple of failing tests. For a real scenario some form of structured logger would be nice which for local dev could simply write out to stdout.

`MakePaymentResult` is a type containing a boolean flag. It may be clearer to use `SuccessfulPaymentResult` or `FailedPaymentResult` instead. This removes the boolean and provides a slightly more friendly type to use. Later on this could be extended to include the error details and so on. As the instructions said to not modify this I held off for now.

I included some comments as I went. Depending on the project guidelines this might require proper XML documentation comments on all the public types.

There's no linter/style configured. I simply relied on my editors default settings. This could be configured to ensure pre commit the same style is used and various linting checks are used.

I'd split acceptance tests vs unit tests. While I favour sociable tests vs solitary tests, the top level acceptance tests provide value when trying to validate the feature as close to end to end as possible. A new csproj could be created to include these.

`AllowedPaymentSchemes` is a enum but makes use of bitwise operators. Some engineers can find this confusing or uncommon. An alternative would be to use a "schemes" collection that includes a collection of schemes an account supports. This could be typed to include some friendly DX to perform checks ideally matching the domain language used. Due to the restriction of the requirements the input is subject to a bug also, you can provide a flag of schemes for a payment request. This causes the validation lookup to fail as it expects a single scheme per payment. I'd suggest simplifying this as previously mentioned or validating within the service that only a single scheme is requested for payment, while the underlying account could indeed support multiple different schemes.

The instructions state not to change the signature of the method on the service, so I've not touched the `MakePaymentRequest` type however it relies on a few primitive types e.g account numbers as strings. I'd introduce a value type here to represent an account and use that instead. This would be a string internally but allow validation and formatting to ensure we always have a valid account number. The same could be done for the other types such as amounts could be `Currency` and so on. There is a bit up front cost here but it pays of later once the whole code base is using it.

As mentioned above, I'd opt to enable nullable reference types in the csproj. The DX here is great and combined with good tests provides nice benefit for developers coming into this codebase fresh.

The validation in the validators themselves is very similar to the original setup. Given the simplicity of this for now I'm OK with this as is. However for a real scenario with more complex rules this could be replaced with a proper validation library like FluentValidation or similar. The errors could be more structured, translated and so on.

There's a few potential places where more recent C# language features could be used, e.g. Primary constructors. Personally I'm not too familiar with these so left them out. In a real scenario we'd discuss this with the team or follow the code guidelines in place but otherwise I'm considering them fairly subjective.

The composite `PaymentRequestValidator` could be enhanced with some tests that detect if another payment scheme has been added. In that case the test would fail and this guard rail would ensure that the dev adds a corresponding validator for the scheme and updates other areas as needed.

I'm relying on a top level try/catch for the service to ensure the client of this will always get a valid or invalid response, never a unhandled exception. The assumption here is that the other parts of the code such as the data access are handling errors and throwing. One alternative would be to replace this with a `Result` pattern instead if this was preferred with the team. We could be more specific on the type of error caught too, but as the data access code is not implemented I went with a top level exception type instead.

Some of the tests are starting to show patterns with test data. We could switch some of these out for more data driven tests or use TestCase. See
`ClearBank.DeveloperTest.Tests/Validators/PaymentRequestValidator.Test.cs` for an example.
