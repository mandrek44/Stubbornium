using System;
using System.Collections.Generic;
using OpenQA.Selenium.Remote;

namespace Stubbornium.Configuration
{
    public class StubbornConfiguration
    {
        public static StubbornConfiguration Default { get; } = new StubbornConfiguration();

        public List<Action<RemoteWebDriver>> BeforeDoActions { get; } = new List<Action<RemoteWebDriver>>();

        public ILogger Log { get; set; } = new EmptyLogger();
    }
}