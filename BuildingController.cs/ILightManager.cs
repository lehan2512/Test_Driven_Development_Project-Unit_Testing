namespace G21097711
{
    public interface ILightManager : IManager
    {
        public void SetLight(bool isOn, int lightID);
        public void SetAllLights(bool isOn);
    }
}