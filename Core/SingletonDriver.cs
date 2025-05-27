using OpenQA.Selenium;
using System;

namespace ApiTestingFramework.Core
{
    public sealed class SingletonDriver
    {
        private static readonly Lazy<SingletonDriver> lazy = new(() => new SingletonDriver());
        private IWebDriver _driver;

        public static SingletonDriver Instance => lazy.Value;

        private SingletonDriver() { }

        public void SetDriver(IWebDriver driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        public IWebDriver GetDriver()
        {
            return _driver ?? throw new InvalidOperationException("WebDriver not initialized");
        }

        public void Quit()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver = null;
            }
        }
    }
}