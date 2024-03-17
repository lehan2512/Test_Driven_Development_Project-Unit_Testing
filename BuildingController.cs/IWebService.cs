namespace G21097711
{
    public interface IWebService
    {
        public void LogStateChange(string logDetails);
        public void LogEngineerRequired(string logDetails);
        public void LogFireAlarm(string logDetails);
    }
}