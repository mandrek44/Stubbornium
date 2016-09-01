using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;

namespace Stubbornium
{
    public class StubbornBrowser : IWrapsDriver
    {
        private readonly RemoteWebDriver _browser;

        public StubbornBrowser(RemoteWebDriver browser)
        {
            _browser = browser;
        }

        public StubbornFinder Find => new StubbornFinder(_browser, _browser);

        IWebDriver IWrapsDriver.WrappedDriver => _browser;
    }
}