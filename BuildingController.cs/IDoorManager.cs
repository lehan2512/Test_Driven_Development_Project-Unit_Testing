namespace G21097711
{
    public interface IDoorManager : IManager
    {
        public bool OpenDoor(int doorID);
        public bool LockDoor(int doorID);
        public bool OpenAllDoors();
        public bool LockAllDoors();
    }
}