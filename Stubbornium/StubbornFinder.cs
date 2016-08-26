using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Stubbornium
{
    public class StubbornFinder
    {
        private readonly RemoteWebDriver _browser;
        private readonly ISearchContext _parent;
        private Func<IWebElement, bool> _collectionPredicate;
        private int _elementIndex = 1;

        public StubbornFinder(RemoteWebDriver browser, ISearchContext parent)
        {
            _browser = browser;
            _parent = parent;
        }

        public StubbornWebElement Element(By selector)
        {
            return new StubbornWebElement(selector, _browser, _parent, _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement Id(string idToFind)
        {
            return new StubbornWebElement(By.Id(idToFind), _browser, _parent, _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement LinkText(string linkTextToFind)
        {
            return new StubbornWebElement(By.LinkText(linkTextToFind), _browser, _parent, _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement Name(string nameToFind)
        {
            return new StubbornWebElement(By.Name(nameToFind), _browser, _parent, _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement XPath(string xpathToFind)
        {
            return new StubbornWebElement(By.XPath(xpathToFind), _browser, _parent, _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement ClassName(string classNameToFind)
        {
            return new StubbornWebElement(By.ClassName(classNameToFind), _browser, _parent, _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement PartialLinkText(string partialLinkTextToFind)
        {
            return new StubbornWebElement(By.PartialLinkText(partialLinkTextToFind), _browser, _parent, _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement TagName(string tagNameToFind)
        {
            return new StubbornWebElement(By.TagName(tagNameToFind), _browser, _parent, _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement CssSelector(string cssSelectorToFind)
        {
            return new StubbornWebElement(By.CssSelector(cssSelectorToFind), _browser, _parent, _collectionPredicate, _elementIndex);
        }

        public StubbornFinder First(Func<IWebElement, bool> collectionPredicate)
        {
            _collectionPredicate = collectionPredicate;
            _elementIndex = 1;
            return this;
        }
        
        public StubbornFinder Nth(int elementIndex, Func<IWebElement, bool> collectionPredicate = null)
        {
            _collectionPredicate = collectionPredicate;
            _elementIndex = elementIndex;
            return this;
        }
    }
}