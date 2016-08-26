using System;
using OpenQA.Selenium;

namespace Stubbornium
{
    public interface IClickable
    {
        void Click<TResult>(Func<IWebDriver, TResult> expectedConditionAfterAction);
    }
}