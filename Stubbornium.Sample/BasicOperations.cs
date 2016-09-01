using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Stubbornium.Sample.Utils;

namespace Stubbornium.Sample
{
    public class BasicOperations
    {
        [SetUp]
        public void GoToIndexPage()
        {
            SampleConfig.WebDriver
                .Navigate().GoToUrl(SampleConfig.BaseAddress.Uri().Combine("index.html"));
        }

        [Test]
        public void Assert_title_after_load()
        {
            Browser.Find.TagName("h1")
                .Assert(element => element().Text == "Test", "H1 == Test");
        }

        [Test]
        public void Assert_title_after_clicking_link()
        {
            Browser.Find.TagName("h1")
                .AssertHasText("Test");

            Browser.Find.Id("change-title")
                .Click(element => element().Driver().FindElement(By.TagName("h1")).Text == "Hello");

            Browser.Find.TagName("h1")
                .AssertHasText("Hello");

            Browser.Find.Id("").Click(ExpectedConditions.InvisibilityOfElementLocated(By.Id("")).ByWebElement(Browser));
        }

        [Test]
        public void Assert_title_after_clicking_link_short()
        {
            Browser.Find.TagName("h1")
                .AssertHasText("Test");

            Browser.Find.Id("change-title")
                .ClickToAppear(By.TagName("h1"), e => e.Text == "Hello");

            Browser.Find.TagName("h1")
                .AssertHasText("Hello");
        }

        [Test]
        public void Assert_title_reset()
        {
            Browser.Find.TagName("h1")
                .AssertHasText("Test");

            Browser.Find.Id("change-title")
                .ClickToAppear(By.Id("reset-title"))
                .ClickToClose();

            Browser.Find.TagName("h1")
                .AssertHasText("Test");
        }

        [Test]
        public void Assert_title_changes_to_input_value()
        {
            Browser.Find.TagName("h1")
                .AssertHasText("Test");

            Browser.Find.Id("new-title")
                .SetText("Better title");

            Browser.Find.Id("change-title")
                .ClickToAppear(By.TagName("h1"), e => e.Text == "Better title");
        }

        private static StubbornBrowser Browser => SampleConfig.Browser;
    }
}
