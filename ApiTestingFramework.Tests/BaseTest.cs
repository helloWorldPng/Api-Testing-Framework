using NUnit.Framework;
using Serilog;
using ApiTestingFramework.Core;

namespace ApiTests
{
    public abstract class BaseTest
    {
        protected ApiClient Client { get; private set; }
        protected ILogger Logger { get; private set; }

        [SetUp]
        public void BaseSetup()
        {
            try
            {
                Client = ApiClientFactory.CreateClient();
                Logger = Log.ForContext(GetType());
                Logger.Information("Starting test setup for {TestClass}", GetType().Name);
            }
            catch (Exception ex)
            {
                // Fallback to console logging
                Console.WriteLine($"Setup failed for {GetType().Name}: {ex.Message}");
                throw; // Rethrow to fail the test setup
            }
        }

        [TearDown]
        public void BaseTearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                (Logger ?? Log.ForContext(GetType())).Error("Test failed: {TestName}. Capturing response details.", TestContext.CurrentContext.Test.Name);
                (Logger ?? Log.ForContext(GetType())).Error("Response details not available in this context. Ensure response logging in test.");
            }
            (Logger ?? Log.ForContext(GetType())).Information("Test teardown completed for {TestClass}", GetType().Name);
        }
    }
}