using System;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Stubbornium.Configuration;
using Stubbornium.Sample.Utils;

namespace Stubbornium.Sample
{
    [SetUpFixture]
    public class SampleConfig
    {
        public static IDisposable WebServer { get; private set; }
        public static StubbornBrowser Browser { get; private set; }
        public static RemoteWebDriver WebDriver { get; private set; }

        public static string BaseAddress => "http://localhost:12345";

        [OneTimeSetUp]
        public void StartWebServer()
        {
            WebServer = WebApp.Start<Startup>(BaseAddress);
        }

        [OneTimeSetUp]
        public void StartWebDriver()
        {
            StubbornConfiguration.Default.Log = new ConsoleLogger();
            StubbornConfiguration.Default.WaitForAjax();

            WebDriver = new ChromeDriver();
            Browser = new StubbornBrowser(WebDriver);
        }

        [OneTimeTearDown]
        public void StopWebServer()
        {
            WebServer.Dispose();
        }

        [OneTimeTearDown]
        public void StopWebDriver()
        {
            WebDriver.Dispose();
        }
    }
}