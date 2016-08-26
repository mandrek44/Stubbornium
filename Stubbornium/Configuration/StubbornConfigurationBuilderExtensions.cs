using OpenQA.Selenium;

namespace Stubbornium.Configuration
{
    public static class StubbornConfigurationBuilderExtensions
    {
        public static StubbornConfiguration WaitForAjax(this StubbornConfiguration config)
        {
            config.BeforeDoActions.Add(driver => driver.WaitForAjaxCompletion());
            return config;
        }

        public static StubbornConfiguration WaitForLoaderFinish(this StubbornConfiguration config, By loaderSelector)
        {
            config.BeforeDoActions.Add(driver => Extensions.WaitFor(() => driver.IsElementMissing(loaderSelector), Extensions.DefaultWait));
            return config;
        }

        public static StubbornConfiguration LogToConsole(this StubbornConfiguration config)
        {
            config.Log = new ConsoleLogger();
            return config;
        }
    }
}