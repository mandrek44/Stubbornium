using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

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
            catch (StaleElementReferenceException)
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

        public static TResult Until<TResult>(this IWait<IWebDriver> wait, Func<Func<IWebElement>, TResult> condition, Func<IWebElement> elementSource )
        {
            return wait.Until(_ => condition(elementSource));
        }

        public static IWebDriver Driver(this IWebElement webElement)
        {
            var wrapsDriver = webElement as IWrapsDriver;
            if (wrapsDriver != null)
                return wrapsDriver.WrappedDriver;

            throw new InvalidOperationException("webElement must implement IWrapsDriver");
        }

        //public static Func<Func<IWebElement>, TResult> ByWebElement<TResult>(this Func<IWebDriver, TResult> condition)
        //{
        //    return webElement => condition(webElement().Driver());
        //}

        public static Func<Func<IWebElement>, TResult> ByWebElement<TResult>(this Func<IWebDriver, TResult> condition, IWrapsDriver browser)
        {
            return webElement => condition(browser.WrappedDriver);
        }
    }
}