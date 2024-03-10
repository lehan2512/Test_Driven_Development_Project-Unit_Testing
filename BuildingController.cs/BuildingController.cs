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

        private ILightManager lightManager;
        private IFireAlarmManager fireAlarmManager;
        private IDoorManager doorManager;
        private IWebService webService;
        private IEmailService emailService;

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

        BuildingController(
            string id,
            ILightManager iLightManager,
            IFireAlarmManager iFireAlarmManager,
            IDoorManager iDoorManager,
            IWebService iWebService,
            IEmailService iEmailService)
        {
            buildingID = id.ToLower();
            currentState = OUT_OF_HOURS_STRING;

            // Assign dependencies
            lightManager = iLightManager ?? throw new ArgumentNullException(nameof(iLightManager));
            fireAlarmManager = iFireAlarmManager ?? throw new ArgumentNullException(nameof(iFireAlarmManager));
            doorManager = iDoorManager ?? throw new ArgumentNullException(nameof(iDoorManager));
            webService = iWebService ?? throw new ArgumentNullException(nameof(iWebService));
            emailService = iEmailService ?? throw new ArgumentNullException(nameof(iEmailService));
        }

        //Function to get buildingID
        public string getBuildingID()
        {
            return buildingID;
        }

        //Function to modify buildingID
        public void setBuildingID(string id)
        {
            buildingID = id;
            buildingID = buildingID.ToLower();
        }

        //Function to get currentState
        public string getCurrentState()
        {
            return currentState;
        }

        //Function to set currentState
        public bool setCurrentState(string newState)
        {
            string realState = getCurrentState();

            if (newState == realState)
            {
                return true;
            }
            else if (newState == CLOSED_STRING)
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
            else if (newState == OUT_OF_HOURS_STRING)
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
            else if (newState == OPEN_STRING)
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
            else if (newState == FIRE_DRILL_STRING)
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
            else if (newState == FIRE_ALARM_STRING)
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
    }
}
