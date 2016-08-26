using System;
using System.Collections.Generic;
using OpenQA.Selenium.Remote;

namespace Stubbornium.Configuration
{
    public class StubbornConfiguration
    {
        public static StubbornConfiguration Default { get; } = BuildDefaultConfiguration();

        public static StubbornConfiguration BuildDefaultConfiguration()
        {
            return new StubbornConfiguration()
                .WaitForAjax()
                .LogToConsole();
        }

        public List<Action<RemoteWebDriver>> BeforeDoActions { get; } = new List<Action<RemoteWebDriver>>();

        public ILogger Log { get; set; } = new EmptyLogger();
    }
}