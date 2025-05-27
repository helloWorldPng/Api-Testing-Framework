using OpenQA.Selenium;
using Serilog;

namespace ApiTestingFramework.Business
{
    public class SamplePage
    {
        private readonly IWebDriver _driver;
        private readonly ILogger _logger;
        private readonly By _usernameInput = By.Id("username");
        private readonly By _passwordInput = By.Id("password");
        private readonly By _loginButton = By.CssSelector("button[type='submit']");
        private readonly By _successMessage = By.CssSelector(".flash.success");

        public SamplePage(IWebDriver driver, ILogger logger)
        {
            _driver = driver;
            _logger = logger.ForContext<SamplePage>();
        }

        public void NavigateTo()
        {
            _logger.Information("Navigating to login page");
            _driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/login");
        }

        public void EnterCredentials(string username, string password)
        {
            _logger.Information("Entering credentials: Username={Username}", username);
            _driver.FindElement(_usernameInput).SendKeys(username);
            _driver.FindElement(_passwordInput).SendKeys(password);
        }

        public void ClickLogin()
        {
            _logger.Information("Clicking login button");
            _driver.FindElement(_loginButton).Click();
        }

        public bool IsLoginSuccessful()
        {
            var isDisplayed = _driver.FindElement(_successMessage).Displayed;
            _logger.Information("Checking login success: IsDisplayed={IsDisplayed}", isDisplayed);
            return isDisplayed;
        }
    }
}