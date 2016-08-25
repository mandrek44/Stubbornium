using OpenQA.Selenium.Support.UI;
using Should;

namespace Stubbornium.Should
{
    public static class StubbornWebElementShouldExtensions
    {
        public static void AssertIsVisible(this StubbornWebElement element)
        {
            element.Do(
                () => element.Element.Displayed.ShouldBeTrue(),
                _ => true,
                ExpectedConditions.ElementIsVisible(element.Selector));
        }

        public static void AssertExists(this StubbornWebElement element, string message = "")
        {
            element.Assert(e => e.ShouldNotBeNull(message) != null);
        }
    }
}
