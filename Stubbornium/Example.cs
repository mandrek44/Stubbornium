using OpenQA.Selenium;

namespace Stubbornium
{
    public class Example
    {
        public void Test()
        {


            /*
             * Original code
             * 
            Extensions.Repeat(() => _app.Browser
                            .FindElementById("checkInUpload")
                            .SendKeys(fileUploadPath));

            Extensions.Repeat(() =>
            {
                _app.Browser
                    .FindElementById("checkInDocumentButton")
                    .Click();


            }, () => _app.WaitFor(ExpectedConditions.ElementIsVisible(By.Id("overConfirm"))));

            Extensions.Repeat(() =>
            {
                _app.Browser
                    .FindElementById("overConfirm")
                    .Click();

                Extensions.WaitFor(() => _app.IsElementMissing(By.Id("checkInDocumentButton"))).ShouldBeTrue();
            });

            return _documentPage;
             * 
             */

            var browser = new StubbornBrowser(null);
            string fileUploadPath = "";

            browser.Find.Id("checkInUpload").SetText(fileUploadPath);

            var confirmationButton = browser.Find.Id("checkInDocumentButton").ClickToOpen(By.Id("overConfirm"));
            confirmationButton.ClickToClose(By.Id("checkInDocumentButton"));
        }
    }
}