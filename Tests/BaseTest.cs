using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using Serilog;
using System;
using System.IO;
using ApiTestingFramework.Core;
using ApiTestingFramework.Business;

namespace ApiTestingFramework.Tests
{
    public abstract class BaseTest
    {
        protected IConfiguration Configuration { get; private set; }
        protected ILogger Logger { get; private set; }
        protected ApiClient ApiClient { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console()
                .WriteTo.File("logs/apitesting.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Logger = Log.ForContext(GetType());
            Logger.Information("Test setup started");

            if (GetType().Namespace.Contains("Ui"))
            {
                var driver = BrowserFactory.CreateDriver(Configuration, Logger);
                SingletonDriver.Instance.SetDriver(driver);
            }

            ApiClient = new ApiClient(Configuration);
        }

        [TearDown]
        public void TearDown()
        {
            Logger.Information("Test teardown started");

            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed &&
                GetType().Namespace.Contains("Ui"))
            {
                var driver = SingletonDriver.Instance.GetDriver();
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                var screenshotPath = Path.Combine("screenshots", $"failed_{TestContext.CurrentContext.Test.Name}_{timestamp}.png");
                Directory.CreateDirectory("screenshots");
                ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(screenshotPath);
                Logger.Error("Test failed, screenshot saved: {ScreenshotPath}", screenshotPath);
            }

            SingletonDriver.Instance.Quit();
            Logger.Information("Test teardown completed");
        }
    }
}