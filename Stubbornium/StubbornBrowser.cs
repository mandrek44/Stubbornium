using OpenQA.Selenium.Remote;

namespace Stubbornium
{
    public class StubbornBrowser
    {
        readonly RemoteWebDriver _browser;

        public StubbornBrowser(RemoteWebDriver browser)
        {
            _browser = browser;
        }

        public StubbornFinder Find
        {
            get { return new StubbornFinder(_browser, _browser); }
        }
    }
}