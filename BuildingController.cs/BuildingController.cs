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

        //Inheriting interfaces and declaring as nullable as some constructors do not inject dependencies
        public ILightManager? LightManager;
        public IFireAlarmManager? FireAlarmManager;
        public IDoorManager? DoorManager;
        public IWebService WebService;
        public IEmailService? EmailService;

        //Declare states as constants
        public const string OUT_OF_HOURS_STRING = "out of hours";
        public const string CLOSED_STRING = "closed";
        public const string OPEN_STRING = "open";
        public const string FIRE_DRILL_STRING = "fire drill";
        public const string FIRE_ALARM_STRING = "fire alarm";
        public string previousState = "";

        //Initialize and Declare bools required for getStatusReport()
        private bool lightHasFaults = false;
        private bool doorHasFaults = false;
        private bool fireAlarmHasFaults = false;
        private string lights_string = "";
        private string doors_string = "";
        private string fireAlarms_string = "";


        //Constructor to populate buildingID
        public BuildingController(string id)
        {
            buildingID = id;
            buildingID = buildingID.ToLower();

            currentState = OUT_OF_HOURS_STRING;
        }

        //Constructor that populates building ID and current state
        public BuildingController(string id, string startState)
        {
            buildingID = id.ToLower();

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
            buildingID = id.ToLower();
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

            if (state == realState) //when set current state gets the current state as input
            {
                return true;
            }
            else if (state == CLOSED_STRING)    //for changing state to closed state
            {
                if ((currentState == OUT_OF_HOURS_STRING) || (previousState == CLOSED_STRING))
                {
                    if(DoorManager != null && LightManager != null)
                    {
                        bool doorsLocked = DoorManager.LockAllDoors();
                        LightManager.SetAllLights(false);
                        if (doorsLocked) //changed to closed state only if allDoorsLocked() returns true
                        {
                            currentState = CLOSED_STRING;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        currentState = CLOSED_STRING;
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (state == OUT_OF_HOURS_STRING)  //for changing state to out of hours state
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
            else if (state == OPEN_STRING)  //for changing state to open state
            {
                if ((currentState == OUT_OF_HOURS_STRING) || (previousState == OPEN_STRING))
                {
                    if (DoorManager != null)
                    {
                        bool doorsOpened = DoorManager.OpenAllDoors();
                        if (doorsOpened)
                        {
                            // Doors opened successfully, proceed with state transition
                            currentState = OPEN_STRING;
                            return true;
                        }
                        else
                        {
                            // Failed to open all doors, return false and maintain current state
                            return false;
                        }
                    }
                    else
                    {
                        // DoorManager is not available, set state to open without opening doors
                        currentState = OPEN_STRING;
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (state == FIRE_DRILL_STRING)    //for changing state to fire drill state
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
            else if (state == FIRE_ALARM_STRING)    //for changing state to fire alarm state
            {
                if ((currentState == OUT_OF_HOURS_STRING) || (currentState == OPEN_STRING) || (currentState == CLOSED_STRING))
                {
                    try
                    {
                        if (DoorManager != null && LightManager != null && FireAlarmManager != null && WebService != null)
                        {
                            FireAlarmManager.SetAlarm(true);
                            DoorManager.OpenAllDoors();
                            LightManager.SetAllLights(true);
                            WebService.LogFireAlarm("fire alarm");
                            previousState = realState;
                            currentState = FIRE_ALARM_STRING;
                            return true;
                        }
                        else
                        {
                            previousState = realState;
                            currentState = FIRE_ALARM_STRING;
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"failed to log fire alarm: {ex.Message}");

                        // Send email with the exception message
                        if (EmailService != null)
                        {
                            string subject = "failed to log alarm";
                            string message = ex.Message;
                            EmailService.SendMail("smartbuilding@uclan.ac.uk", subject, message);
                        }
                        return false;
                    }
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
            if(lightStatus.Contains("FAULT"))
            {
                lightHasFaults = true;
                lights_string = "Lights,";
            }
            string doorsStatus = DoorManager.GetStatus();
            if (doorsStatus.Contains("FAULT"))
            {
                doorHasFaults = true;
                doors_string = "Doors,";
            }
            string fireAlarmStatus = FireAlarmManager.GetStatus();
            if (fireAlarmStatus.Contains("FAULT"))
            {
                fireAlarmHasFaults = true;
                fireAlarms_string = "Fire Alarms,";
            }

            WebService.LogEngineerRequired(lights_string + doors_string + fireAlarms_string);

            return lightStatus + doorsStatus + fireAlarmStatus;
        }
    }
}
