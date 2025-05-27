using NUnit.Framework;
using OpenQA.Selenium;
using ApiTestingFramework.Business;
using ApiTestingFramework.Core;

namespace ApiTestingFramework.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [Category("UI")]
    public class SampleUiTests : BaseTest
    {
        private SamplePage _page;

        [SetUp]
        public void UiSetUp()
        {
            Logger.Information("Starting UI test setup");
            _page = new SamplePage(SingletonDriver.Instance.GetDriver(), Logger);
        }

        [Test]
        public void Validate_Login_Success()
        {
            Logger.Information("Starting test: Validate_Login_Success");

            _page.NavigateTo();
            _page.EnterCredentials("tomsmith", "SuperSecretPassword!");
            _page.ClickLogin();

            Logger.Information("Validating login success");
            Assert.IsTrue(_page.IsLoginSuccessful(), "Login should be successful");

            Logger.Information("Test completed successfully");
        }
    }
}