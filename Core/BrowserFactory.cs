using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Serilog;
using System;

namespace ApiTestingFramework.Core
{
    public static class BrowserFactory
    {
        public static IWebDriver CreateDriver(IConfiguration configuration, ILogger logger)
        {
            var browserType = configuration["BrowserSettings:BrowserType"]?.ToLower();
            var isRemote = bool.Parse(configuration["BrowserSettings:IsRemote"] ?? "false");
            var remoteUrl = configuration["BrowserSettings:RemoteUrl"];

            logger.Information("Creating WebDriver: BrowserType={BrowserType}, IsRemote={IsRemote}", browserType, isRemote);

            IWebDriver driver;
            if (isRemote)
            {
                var options = new ChromeOptions();
                driver = new RemoteWebDriver(new Uri(remoteUrl), options);
            }
            else
            {
                switch (browserType)
                {
                    case "chrome":
                        driver = new ChromeDriver();
                        break;
                    default:
                        throw new ArgumentException($"Unsupported browser type: {browserType}");
                }
            }

            driver.Manage().Window.Maximize();
            logger.Information("WebDriver created successfully");
            return driver;
        }
    }
}