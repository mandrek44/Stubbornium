using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Stubbornium
{
    public static class SeleniumExtensions
    {
        public static bool HasClass(this IWebElement el, string className)
        {
            return el.GetAttribute("class").Split(' ').Contains(className);
        }

        public static bool IsAjaxFinished(this IJavaScriptExecutor javaScriptExecutor)
        {
            return (bool)javaScriptExecutor.ExecuteScript("return jQuery.active == 0");
        }
        
        public static void WaitForAjaxCompletion(this RemoteWebDriver browser)
        {
            Extensions.WaitFor(browser.IsAjaxFinished, Extensions.DefaultWait);
        }

        public static bool IsElementMissing(this IWebDriver browser, By selector)
        {
            try
            {
                return !browser.FindElements(selector).Any() || !browser.FindElement(selector).Displayed;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
        }

        public static IWebElement ClickButton(this IWebElement el)
        {
            el.SendKeys(Keys.Enter);
            return el;
        }

        public static string Value(this IWebElement el)
        {
            return el.GetAttribute("value");
        }
    }
}