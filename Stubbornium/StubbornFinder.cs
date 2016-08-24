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
            return new StubbornWebElement(_browser, _parent, selector, _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement Id(string idToFind)
        {
            return new StubbornWebElement(_browser, _parent, By.Id(idToFind), _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement LinkText(string linkTextToFind)
        {
            return new StubbornWebElement(_browser, _parent, By.LinkText(linkTextToFind), _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement Name(string nameToFind)
        {
            return new StubbornWebElement(_browser, _parent, By.Name(nameToFind), _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement XPath(string xpathToFind)
        {
            return new StubbornWebElement(_browser, _parent, By.XPath(xpathToFind), _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement ClassName(string classNameToFind)
        {
            return new StubbornWebElement(_browser, _parent, By.ClassName(classNameToFind), _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement PartialLinkText(string partialLinkTextToFind)
        {
            return new StubbornWebElement(_browser, _parent, By.PartialLinkText(partialLinkTextToFind), _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement TagName(string tagNameToFind)
        {
            return new StubbornWebElement(_browser, _parent, By.TagName(tagNameToFind), _collectionPredicate, _elementIndex);
        }

        public StubbornWebElement CssSelector(string cssSelectorToFind)
        {
            return new StubbornWebElement(_browser, _parent, By.CssSelector(cssSelectorToFind), _collectionPredicate, _elementIndex);
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