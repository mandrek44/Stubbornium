using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;

namespace Stubbornium
{
    public static class ClickableExtensions
    {
        public static StubbornWebElement ClickToAppear(this StubbornWebElement element, By expectedPopupElement,
            Func<IWebElement, bool> predicate = null)
        {
            return ClickToOpen(element, expectedPopupElement, predicate);
        }

        public static StubbornWebElement ClickToOpen(this StubbornWebElement element, By expectedPopupElement,
            Func<IWebElement, bool> predicate = null, WaitTime waitTime = WaitTime.Short)
        {
            element.Click(webElement => ElementIsVisible(webElement().Driver(), expectedPopupElement, predicate), waitTime: waitTime);
            return new StubbornWebElement(expectedPopupElement, element);
        }

        private static bool ElementIsVisible(IWebDriver driver, By selector, Func<IWebElement, bool> predicate)
        {
            predicate = predicate ?? (_ => true);
            var webElement = driver.FindElement(selector);
            return webElement.Displayed && predicate(webElement);
        }

        public static StubbornWebElement ClickToOpenFirstOf(this StubbornWebElement element, params By[] expectedPopupElements)
        {
            element.Click(webElement => expectedPopupElements.Any(selector => IsDisplayedSafe(() => webElement().Driver().FindElement(selector))));

            var displayedPopupElement = expectedPopupElements.First(selector => IsDisplayedSafe(() => GetDriver(element).FindElement(selector)));

            return new StubbornWebElement(displayedPopupElement, element);
        }

        public static void ClickToClose(this StubbornWebElement element, By expectedMissingElement = null)
        {
            if (expectedMissingElement == null)
                expectedMissingElement = element.Selector;
            element.Click(_ => ExpectedConditions.InvisibilityOfElementLocated(expectedMissingElement));
        }

        public static StubbornWebElement ClickButtonToAppear(this StubbornWebElement element, By expectedPopupElement,
            Func<IWebElement, bool> predicate = null)
        {
            return ClickButtonToOpen(element, expectedPopupElement, predicate);
        }

        public static StubbornWebElement ClickButtonToOpen(this StubbornWebElement element, By expectedPopupElement,
            Func<IWebElement, bool> predicate = null)
        {
            element.ClickButton(webElement => ElementIsVisible(webElement().Driver(), expectedPopupElement, predicate));

            return new StubbornWebElement(expectedPopupElement, element);
        }

        public static void ClickButtonToClose(this StubbornWebElement element, By expectedMissingElement = null)
        {
            if (expectedMissingElement == null)
                expectedMissingElement = element.Selector;
            element.ClickButton(_ => ExpectedConditions.InvisibilityOfElementLocated(expectedMissingElement));
        }

        public static StubbornWebElement RightClickToAppear(this StubbornWebElement element, By expectedPopupElement,
            Func<IWebElement, bool> predicate = null)
        {
            return RightClickToOpen(element, expectedPopupElement, predicate);
        }

        public static StubbornWebElement RightClickToOpen(this StubbornWebElement element, By expectedPopupElement,
            Func<IWebElement, bool> predicate = null)
        {
            element.RightClick(webElement => ElementIsVisible(webElement().Driver(), expectedPopupElement, predicate));
            return new StubbornWebElement(expectedPopupElement, element);
        }

        private static IWebDriver GetDriver(IWrapsDriver wrappedDriver) => wrappedDriver.WrappedDriver;

        private static bool IsDisplayedSafe(Func<IWebElement> elementFinder)
        {
            try
            {
                return elementFinder().Displayed;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}