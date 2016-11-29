using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Stubbornium.Configuration;

namespace Stubbornium
{
    public class StubbornWebElement : IClickable, IButtonClickable, ISearchContext, IWrapsDriver
    {
        private readonly RemoteWebDriver _browser;
        private readonly ISearchContext _parent;
        private readonly By _selector;
        private readonly Func<IWebElement, bool> _collectionPredicate;
        private readonly int _elementIndex;
        private readonly StubbornConfiguration _configuration;

        public StubbornWebElement(By selector, StubbornWebElement parent, int elementIndex = 1, StubbornConfiguration configuration = null)
            : this(selector, parent._browser, parent._browser, null, elementIndex, configuration)
        {
        }

        public StubbornWebElement(By selector, RemoteWebDriver browser, ISearchContext parent = null, int elementIndex = 1, StubbornConfiguration configuration = null)
            : this(selector, browser, parent ?? browser, null, elementIndex, configuration)
        {
        }

        public StubbornWebElement(By selector, RemoteWebDriver browser, ISearchContext parent, Func<IWebElement, bool> collectionPredicate, int elementIndex, StubbornConfiguration configuration = null)
        {
            _browser = browser;
            _parent = parent;
            _selector = selector;
            _collectionPredicate = collectionPredicate;
            _elementIndex = elementIndex;
            _configuration = configuration ?? StubbornConfiguration.Default;
        }
        
        public By Selector => _selector;

        public StubbornFinder Find => new StubbornFinder(_browser, this);

        private IWebElement Element
        {
            get
            {
                if (_collectionPredicate == null)
                {
                    if (_elementIndex == 1)
                        return _parent.FindElement(_selector);
                    else
                        return _parent.FindElements(_selector).ElementAtOrDefault(_elementIndex);
                }
                else if (_elementIndex == 1)
                    return _parent.FindElements(_selector).FirstOrDefault(_collectionPredicate);
                else
                    return _parent.FindElements(_selector).Where(_collectionPredicate).ElementAtOrDefault(_elementIndex);
            }
        }

        public void SetText(string content)
        {
            Do(
                element =>
                {
                    element().Clear();
                    element().SendKeys(content);
                },
                _ => Element.Value() == content,
                ExpectedConditions.ElementIsVisible(_selector),
                logMessage: "\"" + content + "\"");
        }

        public void Click<TResult>(Func<Func<IWebElement>, TResult> expectedConditionAfterAction)
        {
            Do(
                element => element().Click(),
                expectedConditionAfterAction,
                ExpectedConditions.ElementIsVisible(_selector));
        }

        public void ClickButton<TResult>(Func<Func<IWebElement>, TResult> expectedConditionAfterAction)
        {
            Do(
                element => element().ClickButton(),
                expectedConditionAfterAction,
                ExpectedConditions.ElementIsVisible(_selector));
        }

        public void RightClick<TResult>(Func<Func<IWebElement>, TResult> expectedConditionAfterAction)
        {
            Do(
                element => new Actions(element().Driver()).ContextClick(element()).Build().Perform(),
                expectedConditionAfterAction,
                ExpectedConditions.ElementIsVisible(_selector));
        }

        public void AssertExists(string message = "")
        {
            Assert(e =>
            {
                Assertions.AreNotEqual(null, e, message);
                return true;
            }, message ?? "Exists");
        }

        public void AssertIsMissing()
        {
            Assert(element => _browser.IsElementMissing(_selector), "Is missing");
        }

        public void AssertIsVisible()
        {
            Do(
                element => Assertions.AreEqual(true, element().Displayed),
                element => element().Displayed,
                ExpectedConditions.ElementIsVisible(_selector),
                logMessage: "Is visible");
        }

        public void AssertHasText(string expectedText)
        {
            Assert(e => e().Text == expectedText, $"Has text \"{expectedText}\"");
        }

        public void Assert(Func<Func<IWebElement>, bool> assertion, string logMessage)
        {
            Do( _ => { },
                _ => assertion(() => Element),
                logMessage: logMessage);
        }

        public void Do<TResult>(Action<Func<IWebElement>> seleniumAction,
            Func<Func<IWebElement>, TResult> expectedConditionAfterAction,
            int maxRetries = 10,
            WaitTime waitTime = WaitTime.Short,
            [CallerMemberName] string caller = "",
            string logMessage = null)
        {
            Do(seleniumAction, expectedConditionAfterAction, (Func<IWebDriver, bool>)null, maxRetries, waitTime, caller, logMessage);
        }

        public void Do<TResult1, TResult2>(Action<Func<IWebElement>> seleniumAction,
            Func<Func<IWebElement>, TResult1> expectedConditionAfterAction,
            Func<IWebDriver, TResult2> errorWaitCondition = null,
            int maxRetries = 10,
            WaitTime waitTime = WaitTime.Short,
            [CallerMemberName] string caller = "",
            string logMessage = null)
        {
            var fullLogMessage = $"{caller} - {_selector}";
            if (logMessage != null)
                fullLogMessage += " - " + logMessage;

            Do(_browser, () => Element, seleniumAction, expectedConditionAfterAction, errorWaitCondition, maxRetries, waitTime, caller, _configuration, fullLogMessage);
        }

        public static void Do<TResult1, TResult2>(RemoteWebDriver browser, 
            Func<IWebElement>  webElementSource,
            Action<Func<IWebElement>> seleniumAction,
            Func<Func<IWebElement>, TResult1> expectedConditionAfterAction,
            Func<IWebDriver, TResult2> errorWaitCondition = null,
            int maxRetries = 10,
            WaitTime waitTime = WaitTime.Short,
            [CallerMemberName] string caller = "",
            StubbornConfiguration configuration = null,
            string logMessage = null)
        {
            configuration = configuration ?? StubbornConfiguration.Default;
            
            configuration.Log.Info(logMessage ?? caller);

            var wait = new WebDriverWait(browser, waitTime.ToTimeSpan());
            int attemptNo = 0;
            while (true)
            {
                configuration.BeforeDoActions.ForEach(action => action(browser));

                var actionException = Try(() => seleniumAction(webElementSource));

                configuration.BetweenDoActions.ForEach(action => action(browser));

                var expectedConditionException = Try(() => wait.Until(expectedConditionAfterAction, webElementSource));

                if (actionException == null && expectedConditionException == null)
                    return;

                if (attemptNo > 0 && actionException != null && expectedConditionException == null)
                {
                    configuration.Log.Warning($"Action threw exception (\"{actionException.Message}\") but excepted condition is met");
                    return;
                }

                var relevantException = actionException ?? expectedConditionException;
                if (attemptNo >= maxRetries)
                    ExceptionDispatchInfo.Capture(relevantException).Throw();

                attemptNo++;
                try
                {
                    if (errorWaitCondition != null)
                        wait.Until(errorWaitCondition);
                    else
                        Thread.Sleep(WaitTime.Short.ToTimeSpan());
                }
                catch (Exception)
                {
                    // Ignore wait errors - just try to perform the core action again
                }

                configuration.Log.Warning($"Repeating {caller}");
            }
        }

        IWebElement ISearchContext.FindElement(By @by)
        {
            return Element.FindElement(@by);
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements(By @by)
        {
            return Element.FindElements(@by);
        }

        IWebDriver IWrapsDriver.WrappedDriver => _browser;

        private static Exception Try(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }
    }
}