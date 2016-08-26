namespace Stubbornium.Configuration
{
    public class EmptyLogger : ILogger
    {
        public void Info(string message)
        {
        }

        public void Warning(string message)
        {
        }
    }
}