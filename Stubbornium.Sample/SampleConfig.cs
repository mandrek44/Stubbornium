using System;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using Stubbornium.Configuration;
using Stubbornium.Sample.Utils;

namespace Stubbornium.Sample
{
    [SetUpFixture]
    public class SampleConfig
    {
        public static IDisposable WebServer { get; private set; }
        public static StubbornBrowser Browser { get; private set; }
        public static ChromeDriver WebDriver { get; private set; }

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