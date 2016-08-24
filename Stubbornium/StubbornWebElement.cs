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

namespace Stubbornium
{
    public interface IClickable
    {
        void Click<TResult>(Func<IWebDriver, TResult> expectedConditionAfterAction);
    }

    public interface IButtonClickable
    {
        void ClickButton<TResult>(Func<IWebDriver, TResult> expectedConditionAfterAction);
    }

    public class StubbornWebElement : IClickable, IButtonClickable, ISearchContext
    {
        private readonly RemoteWebDriver _browser;
        private readonly ISearchContext _parent;
        private readonly By _selector;
        private readonly Func<IWebElement, bool> _collectionPredicate;
        private readonly int _elementIndex;

        public StubbornWebElement(RemoteWebDriver browser, ISearchContext parent, By selector, int elementIndex = 1) : this(browser, parent, selector, null, elementIndex)
        {
        }

        public StubbornWebElement(RemoteWebDriver browser, ISearchContext parent, By selector, Func<IWebElement, bool> collectionPredicate, int elementIndex)
        {
            _browser = browser;
            _parent = parent;
            _selector = selector;
            _collectionPredicate = collectionPredicate;
            _elementIndex = elementIndex;
        }

        public By Selector { get { return _selector; } }

        public RemoteWebDriver Browser
        {
            get { return _browser; }
        }

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

        public StubbornFinder Find
        {
            get { return new StubbornFinder(_browser, this); }
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

        //public void AssertIsVisible()
        //{
        //    Do(
        //        () => Element.Displayed.ShouldBeTrue(),
        //        _ => true,
        //        ExpectedConditions.ElementIsVisible(_selector));
        //}

        public void AssertIsMissing()
        {
            Do(
                () => { },
                browser => browser.IsElementMissing(_selector));
        }

        //public void AssertExists(string message = "")
        //{
        //    Assert(e => e.ShouldNotBeNull(message) != null);
        //}

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
            Do(_browser, seleniumAction, expectedConditionAfterAction, errorWaitCondition, maxRetries, caller);
        }

        public static void Do<TResult1, TResult2>(RemoteWebDriver browser, Action seleniumAction,
            Func<IWebDriver, TResult1> expectedConditionAfterAction,
            Func<IWebDriver, TResult2> errorWaitCondition = null,
            int maxRetries = 10,
            [CallerMemberName] string caller = "")
        {
            browser.WaitForAjaxCompletion();
            Extensions.WaitFor(() => browser.IsElementMissing(By.Id("blockMessage")), Extensions.DefaultWait);

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
                    //_log.Warn("Action threw exception (\"{0}\") but excepted condition is met", actionException.Message);
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

                // _log.Warn("Repeating {0}", caller);
            }
        }

        public IWebElement FindElement(By @by)
        {
            return Element.FindElement(@by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By @by)
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