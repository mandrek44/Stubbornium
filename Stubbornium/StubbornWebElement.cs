using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Stubbornium.Configuration;

namespace Stubbornium
{
    public class StubbornWebElement : IClickable, IButtonClickable, ISearchContext
    {
        private readonly RemoteWebDriver _browser;
        private readonly ISearchContext _parent;
        private readonly By _selector;
        private readonly Func<IWebElement, bool> _collectionPredicate;
        private readonly int _elementIndex;
        private readonly StubbornConfiguration _configuration;

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

        public RemoteWebDriver Browser => _browser;

        public StubbornFinder Find => new StubbornFinder(_browser, this);

        public IWebElement Element
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
                () =>
                {
                    Element.Clear();
                    Element.SendKeys(content);
                },
                _ => Element.Value() == content,
                ExpectedConditions.ElementIsVisible(_selector));
        }

        public void Click<TResult>(Func<IWebDriver, TResult> expectedConditionAfterAction)
        {
            Do(
                () => Element.Click(),
                expectedConditionAfterAction,
                ExpectedConditions.ElementIsVisible(_selector));
        }

        public void ClickButton<TResult>(Func<IWebDriver, TResult> expectedConditionAfterAction)
        {
            Do(
                () => Element.ClickButton(),
                expectedConditionAfterAction,
                ExpectedConditions.ElementIsVisible(_selector));
        }

        public void RightClick<TResult>(Func<IWebDriver, TResult> expectedConditionAfterAction)
        {
            Do(
                () => new Actions(_browser).ContextClick(Element).Build().Perform(),
                expectedConditionAfterAction,
                ExpectedConditions.ElementIsVisible(_selector));
        }

        public void AssertExists(string message = "")
        {
            Assert(e =>
            {
                Assertions.AreNotEqual(null, e, message);
                return true;
            });
        }

        public void AssertIsMissing()
        {
            Do(
                () => { },
                browser => browser.IsElementMissing(_selector));
        }

        public void AssertIsVisible()
        {
            Do(
                () => Assertions.AreEqual(true, Element.Displayed),
                _ => true,
                ExpectedConditions.ElementIsVisible(_selector));
        }

        public void Assert(Func<IWebElement, bool> assertion)
        {
            Do(() => { },
                _ => assertion(Element));
        }

        public void Do<TResult>(Action seleniumAction,
            Func<IWebDriver, TResult> expectedConditionAfterAction,
            int maxRetries = 10,
            [CallerMemberName] string caller = "")
        {
            Do(seleniumAction, expectedConditionAfterAction, (Func<IWebDriver, bool>)null, maxRetries, caller);
        }

        public void Do<TResult1, TResult2>(Action seleniumAction,
            Func<IWebDriver, TResult1> expectedConditionAfterAction,
            Func<IWebDriver, TResult2> errorWaitCondition = null,
            int maxRetries = 10,
            [CallerMemberName] string caller = "",
            string logMessage = "")
        {
            Do(_browser, seleniumAction, expectedConditionAfterAction, errorWaitCondition, maxRetries, caller, _configuration);
        }

        public static void Do<TResult1, TResult2>(RemoteWebDriver browser, Action seleniumAction,
            Func<IWebDriver, TResult1> expectedConditionAfterAction,
            Func<IWebDriver, TResult2> errorWaitCondition = null,
            int maxRetries = 10,
            [CallerMemberName] string caller = "",
            StubbornConfiguration configuration = null)
        {
            configuration = StubbornConfiguration.Default;

            configuration.BeforeDoActions.ForEach(action => action(browser));

            var wait = new WebDriverWait(browser, WaitTime.Short.ToTimeSpan());
            int attemptNo = 0;
            while (true)
            {
                var actionException = Try(seleniumAction);
                var expectedConditionException = Try(() => wait.Until(expectedConditionAfterAction));

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