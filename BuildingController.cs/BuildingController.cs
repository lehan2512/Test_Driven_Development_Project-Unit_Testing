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

        private ILightManager LightManager;
        private IFireAlarmManager FireAlarmManager;
        private IDoorManager DoorManager;
        private IWebService WebService;
        private IEmailService EmailService;

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

        public BuildingController(string id, string startState)
        {
            buildingID = id;
            buildingID = buildingID.ToLower();

            if(startState == OUT_OF_HOURS_STRING || startState == CLOSED_STRING || startState == OPEN_STRING)
            {
                currentState = startState;
            }
            else
            {
                throw new ArgumentException("Argument Exception: BuildingController can only be initialised to the following states 'open', 'closed', 'out of hours'");
            }
        }

        public BuildingController(
            string id,
            ILightManager iLightManager,
            IFireAlarmManager iFireAlarmManager,
            IDoorManager iDoorManager,
            IWebService iWebService,
            IEmailService iEmailService)
        {
            buildingID = id.ToLower();
            currentState = OUT_OF_HOURS_STRING;

            LightManager = iLightManager;
            FireAlarmManager = iFireAlarmManager;
            DoorManager = iDoorManager;
            WebService = iWebService;
            EmailService = iEmailService;
        }

        //Function to get buildingID
        public string GetBuildingID()
        {
            return buildingID;
        }

        //Function to modify buildingID
        public void SetBuildingID(string id)
        {
            buildingID = id;
            buildingID = buildingID.ToLower();
        }

        //Function to get currentState
        public string GetCurrentState()
        {
            return currentState;
        }

        //Function to set currentState
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
                if ((previousState == OUT_OF_HOURS_STRING) || (currentState == OPEN_STRING) || (currentState == CLOSED_STRING))
                {
                    currentState = OUT_OF_HOURS_STRING;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (state == OPEN_STRING)
            {
                if ((currentState == OUT_OF_HOURS_STRING) || (previousState == OPEN_STRING))
                {
                    if (DoorManager.OpenAllDoors())
                    {
                        currentState = OPEN_STRING;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (state == FIRE_DRILL_STRING)
            {
                if ((currentState == OUT_OF_HOURS_STRING) || (currentState == OPEN_STRING) || (currentState == CLOSED_STRING))
                {
                    previousState = realState;
                    currentState = FIRE_DRILL_STRING;
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else if (state == FIRE_ALARM_STRING)
            {
                if ((currentState == OUT_OF_HOURS_STRING) || (currentState == OPEN_STRING) || (currentState == CLOSED_STRING))
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
            else
            {
                return false;
            }
        }

        public string GetStatusReport()
        {
            string lightStatus = LightManager.GetStatus();
            string doorsStatus = DoorManager.GetStatus();
            string fireAlarmStatus = FireAlarmManager.GetStatus();
            return lightStatus + doorsStatus + fireAlarmStatus;
        }
    }
}
