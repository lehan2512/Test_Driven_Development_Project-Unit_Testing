using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G21097711
{
    public class BuildingController
    {
        //Declare attributes
        private string buildingID = string.Empty;
        private string currentState = string.Empty;

        //Declare states as constants
        public const string OUT_OF_HOURS_STRING = "out of hours";
        public const string CLOSED_STRING = "closed";
        public const string OPEN_STRING = "open";
        public const string FIRE_DRILL_STRING = "fire drill";
        public const string FIRE_ALARM_STRING = "fire alarm";

        //Constructor to populate buildingID
        public BuildingController(string id)
        {
            buildingID = id;
            buildingID = buildingID.ToLower();

            currentState = OUT_OF_HOURS_STRING;
        }

        //Constructor to get buildingID
        public string GetBuildingID()
        {
            return buildingID;
        }

        //Constructor to modify buildingID
        public void SetBuildingID(string ID)
        {
            buildingID = ID;
            buildingID = buildingID.ToLower();
        }

        //Constructor to get currentState
        public string GetCurrentState()
        {
            return currentState;
        }

        //Constructor to set currentState
        public bool SetCurrentState(string state)
        {
            switch (state)
            {
                case CLOSED_STRING:
                    currentState = CLOSED_STRING;
                    return true;
                case OUT_OF_HOURS_STRING: currentState = OUT_OF_HOURS_STRING;
                    return true;
                case OPEN_STRING: currentState = OPEN_STRING;
                    return true;
                case FIRE_DRILL_STRING: currentState = FIRE_DRILL_STRING;
                    return true;
                case FIRE_ALARM_STRING: currentState = FIRE_ALARM_STRING;
                    return true;
                default:
                    return false;
            }
        }
    }
}
