using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Stubbornium
{
    public static class ClickableExtensions
    {
        public static StubbornWebElement ClickToOpen(this StubbornWebElement element, By expectedPopupElement)
        {
            element.Click(ExpectedConditions.ElementIsVisible(expectedPopupElement));
            return new StubbornWebElement(element.Browser, element.Browser, expectedPopupElement);
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
            return new StubbornWebElement(element.Browser, element.Browser, expectedPopupElement);
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
            return new StubbornWebElement(element.Browser, element.Browser, expectedPopupElement);
        }
    }
}