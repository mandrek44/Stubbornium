using System;
using OpenQA.Selenium;

namespace Stubbornium
{
    public interface IClickable
    {
        void Click<TResult>(Func<IWebElement, TResult> expectedConditionAfterAction);
    }
}