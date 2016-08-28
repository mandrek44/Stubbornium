using NUnit.Framework;
using Stubbornium.Sample.Utils;

namespace Stubbornium.Sample
{
    public class H1Sample
    {
        [Test]
        public void Do()
        {
            SampleConfig.WebDriver
                .Navigate().GoToUrl(SampleConfig.BaseAddress.Uri().Combine("index.html"));

            SampleConfig.Browser
                .Find.TagName("h1")
                .Assert(element => element.Text == "Test");
        }
    }
}
