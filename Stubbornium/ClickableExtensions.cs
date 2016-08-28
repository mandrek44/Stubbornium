using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;

namespace Stubbornium
{
    public static class ClickableExtensions
    {
        public static StubbornWebElement ClickToOpen(this StubbornWebElement element, By expectedPopupElement)
        {
            element.Click(ExpectedConditions.ElementIsVisible(expectedPopupElement).ByWebElement());
            return new StubbornWebElement(expectedPopupElement, element);
        }

        public static StubbornWebElement ClickToOpenFirstOf(this StubbornWebElement element, params By[] expectedPopupElements)
        {
            element.Click(webElement => expectedPopupElements.Any(selector => IsDisplayedSafe(() => webElement.Driver().FindElement(selector))));

            var displayedPopupElement = expectedPopupElements.First(selector => IsDisplayedSafe(() => GetDriver(element).FindElement(selector)));

            return new StubbornWebElement(displayedPopupElement, element);
        }

        public static void ClickToClose(this StubbornWebElement element, By expectedMissingElement = null)
        {
            if (expectedMissingElement == null)
                expectedMissingElement = element.Selector;
            element.Click(webElement => webElement.Driver().IsElementMissing(expectedMissingElement));
        }

        public static StubbornWebElement ClickButtonToOpen(this StubbornWebElement element, By expectedPopupElement)
        {
            element.ClickButton(ExpectedConditions.ElementIsVisible(expectedPopupElement).ByWebElement());
            return new StubbornWebElement(expectedPopupElement, element);
        }

        public static void ClickButtonToClose(this StubbornWebElement element, By expectedMissingElement = null)
        {
            if (expectedMissingElement == null)
                expectedMissingElement = element.Selector;
            element.ClickButton(webElement => webElement.Driver().IsElementMissing(expectedMissingElement));
        }

        public static StubbornWebElement RightClickToOpen(this StubbornWebElement element, By expectedPopupElement)
        {
            element.RightClick(ExpectedConditions.ElementIsVisible(expectedPopupElement).ByWebElement());
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