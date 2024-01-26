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
        public string previousState = "";

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
            string realState = GetCurrentState();

            if (state == realState)
            {
                return true;
            }
            else if (state == CLOSED_STRING)
            {
                if ((currentState == OUT_OF_HOURS_STRING) || (previousState == CLOSED_STRING))
                {
                    currentState = CLOSED_STRING;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (state == OUT_OF_HOURS_STRING)
            {
                currentState = OUT_OF_HOURS_STRING;
                return true;
            }
            else if (state == OPEN_STRING)
            {
                if ((currentState == OUT_OF_HOURS_STRING) || (previousState == OPEN_STRING))
                {
                    currentState = OPEN_STRING;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (state == FIRE_DRILL_STRING)
            {
                previousState = realState;
                currentState = FIRE_DRILL_STRING;
                return true;
            }
            else if (state == FIRE_ALARM_STRING)
            {
                previousState = realState;
                currentState = FIRE_ALARM_STRING;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
