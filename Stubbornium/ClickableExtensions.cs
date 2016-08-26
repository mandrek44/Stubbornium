using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Stubbornium
{
    public static class ClickableExtensions
    {
        public static StubbornWebElement ClickToOpen(this StubbornWebElement element, By expectedPopupElement)
        {
            element.Click(ExpectedConditions.ElementIsVisible(expectedPopupElement));
            return new StubbornWebElement(expectedPopupElement, element.Browser, element.Browser);
        }

        public static StubbornWebElement ClickToOpenFirstOf(this StubbornWebElement element, params By[] expectedPopupElements)
        {
            element.Click(b => expectedPopupElements.Any(selector => IsDisplayedSafe(() => b.FindElement(selector))));

            var displayedPopupElement = expectedPopupElements.First(selector => IsDisplayedSafe(() => element.Browser.FindElement(selector)));

            return new StubbornWebElement(displayedPopupElement, element.Browser, element.Browser);
        }

        public static void ClickToClose(this StubbornWebElement element, By expectedMissingElement = null)
        {
            if (expectedMissingElement == null)
                expectedMissingElement = element.Selector;
            element.Click(browser => browser.IsElementMissing(expectedMissingElement));
        }

        public static StubbornWebElement ClickButtonToOpen(this StubbornWebElement element, By expectedPopupElement)
        {
            element.ClickButton(ExpectedConditions.ElementIsVisible(expectedPopupElement));
            return new StubbornWebElement(expectedPopupElement, element.Browser, element.Browser);
        }

        public static void ClickButtonToClose(this StubbornWebElement element, By expectedMissingElement = null)
        {
            if (expectedMissingElement == null)
                expectedMissingElement = element.Selector;
            element.ClickButton(browser => browser.IsElementMissing(expectedMissingElement));
        }

        public static StubbornWebElement RightClickToOpen(this StubbornWebElement element, By expectedPopupElement)
        {
            element.RightClick(ExpectedConditions.ElementIsVisible(expectedPopupElement));
            return new StubbornWebElement(expectedPopupElement, element.Browser, element.Browser);
        }

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