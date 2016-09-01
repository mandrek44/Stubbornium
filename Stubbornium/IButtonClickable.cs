using System;
using OpenQA.Selenium;

namespace Stubbornium
{
    public interface IButtonClickable
    {
        void ClickButton<TResult>(Func<Func<IWebElement>, TResult> expectedConditionAfterAction);
    }
}