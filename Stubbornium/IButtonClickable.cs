using System;
using OpenQA.Selenium;

namespace Stubbornium
{
    public interface IButtonClickable
    {
        void ClickButton<TResult>(Func<IWebElement, TResult> expectedConditionAfterAction);
    }
}