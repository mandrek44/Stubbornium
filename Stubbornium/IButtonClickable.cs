using System;
using OpenQA.Selenium;

namespace Stubbornium
{
    public interface IButtonClickable
    {
        void ClickButton<TResult>(Func<IWebDriver, TResult> expectedConditionAfterAction);
    }
}