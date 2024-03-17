using NUnit.Framework;
using NSubstitute;
using System.Xml;
using System.Globalization;

namespace G21097711.Tests
{
    [TestFixture]
    public class BuildingControllerTests
    {
        private BuildingController buildingController1;
        private BuildingController buildingController2;
        private BuildingController buildingController;
        private BuildingController buildingController_withFaultLights;
        private BuildingController buildingController_withFaultDoors;
        private BuildingController buildingController_withFaultFireAlarms;
        private BuildingController buildingController_withFaultLights_andFaultDoors;
        private BuildingController buildingController_withFaultLights_andFaultFireAlarms;
        private BuildingController buildingController_withFaultDoors_andFaultFireAlarms;
        private BuildingController buildingController_withAllFaultDevices;


        ILightManager LightManager = Substitute.For<ILightManager>();
        IDoorManager DoorManager = Substitute.For<IDoorManager>();
        IFireAlarmManager FireAlarmManager = Substitute.For<IFireAlarmManager>();
        IEmailService EmailService = Substitute.For<IEmailService>();
        IWebService WebService = Substitute.For<IWebService>();

        ILightManager LightManager_withFaults = Substitute.For<ILightManager>();
        IDoorManager DoorManager_withFaults = Substitute.For<IDoorManager>();
        IFireAlarmManager FireAlarmManager_withFaults = Substitute.For<IFireAlarmManager>();



        [SetUp]
        public void setup()
        {
            buildingController1 = new BuildingController("ucl");
            buildingController = new BuildingController("ucl", LightManager, FireAlarmManager, DoorManager, WebService, EmailService);
            buildingController_withFaultLights = new BuildingController("ucl", LightManager_withFaults, FireAlarmManager, DoorManager, WebService, EmailService);
            buildingController_withFaultDoors = new BuildingController("ucl", LightManager, FireAlarmManager, DoorManager_withFaults, WebService, EmailService);
            buildingController_withFaultFireAlarms = new BuildingController("ucl", LightManager, FireAlarmManager_withFaults, DoorManager, WebService, EmailService);
            buildingController_withFaultLights_andFaultDoors = new BuildingController("ucl", LightManager_withFaults, FireAlarmManager, DoorManager_withFaults, WebService, EmailService);
            buildingController_withFaultLights_andFaultFireAlarms = new BuildingController("ucl", LightManager_withFaults, FireAlarmManager_withFaults, DoorManager, WebService, EmailService);
            buildingController_withFaultDoors_andFaultFireAlarms = new BuildingController("ucl", LightManager, FireAlarmManager_withFaults, DoorManager_withFaults, WebService, EmailService);
            buildingController_withAllFaultDevices = new BuildingController("ucl", LightManager_withFaults, FireAlarmManager_withFaults, DoorManager_withFaults, WebService, EmailService);


            LightManager.GetStatus().Returns("Lights,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,");
            DoorManager.GetStatus().Returns("Doors,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,");
            FireAlarmManager.GetStatus().Returns("Fire Alarms,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,");

            LightManager_withFaults.GetStatus().Returns("Lights,OK,OK,FAULT,OK,OK,OK,OK,OK,OK,OK,");
            DoorManager_withFaults.GetStatus().Returns("Doors,FAULT,OK,OK,OK,OK,OK,OK,OK,OK,OK,");
            FireAlarmManager_withFaults.GetStatus().Returns("Fire Alarms,OK,OK,OK,OK,OK,OK,OK,OK,OK,FAULT,");
        }

        //LEVEL 1 REQUIREMENTS

        //L1R1 - Testing if BuildingController() constructor creates a new instance of the class
        [Test]
        public void Should_Initialize_BuildingController_Using_First_Constructor() //Test does not have Arrange and Act because already Arranged and Acted in [Setup]
        {
            //Assert
            Assert.IsNotNull(buildingController1, "BuildingController instance should not be null");
        }


        //L1R2 - Testing if getBuildingID() functions as expencted and if buildingID is stored in expected format
        [Test]
        [TestCase("ucl", "ucl")]
        public void Should_Return_BuildingID_When_Using_GetBuildingID(string initialBuildingID, string expectedBuildingID)
        {
            //Arrange
            buildingController1.SetBuildingID(initialBuildingID);
            string realBuildingID;

            //Act
            realBuildingID = buildingController1.GetBuildingID();

            //Assert
            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));
        }


        //L1R3 - Testing if () if buildingID is stored in expected format
        [Test]
        [TestCase("UCL", "ucl")]                //Testing behaviour with upper case input ID
        [TestCase("123", "123")]                //Testing behaviour with numeric input ID
        [TestCase("ucL - @#$123", "ucl - @#$123")]  //Testing behaviour with symbols in input ID
        public void Should_Store_BuildingID_In_Correct_Format_When_Input_Format_Is_Incorrect(string initialBuildingID, string expectedBuildingID)
        {
            //Arrange
            buildingController1.SetBuildingID(initialBuildingID);
            string realBuildingID;

            //Act
            realBuildingID = buildingController1.GetBuildingID();

            //Assert
            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));
        }


        //L1R4 - Testing if setBuildingID() functions as expected
        [Test]
        [TestCase("ucl", "ucl123", "ucl123")]               //Test  9: Testing behaviour with new input ID
        [TestCase("ucl", "Ucl - 123!@#", "ucl - 123!@#")]   //Test 10: Testing behaviour with new input ID with uppercas letters, spaces, numbers and symbols
        public void Should_Store_BuildingID_In_Correct_Format_When_SetBuildingID_Is_Called(string initialBuildingID, string newBuildingID, string expectedBuildingID)
        {
            //Act
            buildingController1.SetBuildingID(initialBuildingID);
            buildingController1.SetBuildingID(newBuildingID);

            //Assert
            Assert.That(buildingController1.GetBuildingID(), Is.EqualTo(expectedBuildingID));
        }


        //L1R5 - Testing the initialization and decleration of currentState
        [Test]
        [TestCase("out of hours")]
        public void Should_Initialize_CurrentState_To_Out_Of_Hours_When_New_BuildingController_Is_Created(string expectedCurrentState)
        {
            //Arrange
            string realCurrentState;

            //Act
            realCurrentState = buildingController1.GetCurrentState();

            //Assert
            Assert.That(realCurrentState, Is.EqualTo(expectedCurrentState));
        }


        //L1R6 - Testing getCurrentState()
        [Test]
        [TestCase("open", "open")]
        [TestCase("closed", "closed")]
        [TestCase("out of hours", "out of hours")]
        [TestCase("fire drill", "fire drill")]
        [TestCase("fire alarm", "fire alarm")]
        public void Should_Return_Current_State_When_GetCurrentState_Is_Called(string newCurrentState, string expectedCurrentState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController1.SetCurrentState(newCurrentState);
            realCurrentState = buildingController1.GetCurrentState();

            //Assert
            Assert.That(realCurrentState, Is.EqualTo(expectedCurrentState));
        }


        //L1R7 - Testing if setCurrentState() functions with valid input
        [Test]
        [TestCase("open")]
        [TestCase("closed")]
        [TestCase("fire drill")]
        [TestCase("fire alarm")]
        public void Should_Change_Current_State_When_SetCurrentState_Is_Called(string newState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController1.SetCurrentState(newState);
            realCurrentState = buildingController1.GetCurrentState();

            //Assert
            Assert.That(realCurrentState, Is.EqualTo(newState));
        }


        //L1R7 - Testing if currentState remains unchanged if setCurrentState() is given invalid input
        [Test]
        [TestCase("Out of Hours")]
        [TestCase("outofhours")]
        [TestCase("OPEN")]
        [TestCase("123")]
        public void Should_Not_Change_Current_State_When_SetCurretState_Is_Called_With_Invalid_Input(string newState)
        {
            //Arrange
            string realCurrentState;
            string initialState = "out of hours";

            //Act
            buildingController1.SetCurrentState(initialState);
            buildingController1.SetCurrentState(newState);
            realCurrentState = buildingController1.GetCurrentState();

            //Assert
            Assert.That(realCurrentState != newState);
            Assert.That(realCurrentState == initialState);
        }


        //LEVEL 2 REQUIREMENTS

        //L2R1 - Testing if SetCurrentState functions when input is valid but not a state it can change to
        [Test]
        [TestCase("open", "closed")]
        [TestCase("closed", "open")]
        [TestCase("fire drill", "fire alarm")]
        [TestCase("fire alarm", "fire drill")]
        public void Should_Not_Change_State_When_SetCurrentState_Is_Called_With_Unacceptable_Input(string initialState, string newState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController1.SetCurrentState(initialState);
            buildingController1.SetCurrentState(newState);
            realCurrentState = buildingController1.GetCurrentState();

            //Assert
            Assert.IsFalse(realCurrentState == newState);
            Assert.IsTrue(realCurrentState == initialState);
        }


        //L2R1 - Testing if fire alarm and fire drill state change functions as expected
        [Test]
        [TestCase("open", "fire drill", "open")]
        [TestCase("closed", "fire drill", "closed")]
        [TestCase("out of hours", "fire drill", "out of hours")]
        [TestCase("open", "fire alarm", "open")]
        [TestCase("closed", "fire alarm", "closed")]
        [TestCase("out of hours", "fire alarm", "out of hours")]
        public void Should_Change_State_Back_To_Previous_State_When_SetCurrentState_Is_Called_After_FireState(string initialState, string fireState, string finalState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController1.SetCurrentState(initialState);
            buildingController1.SetCurrentState(fireState);
            buildingController1.SetCurrentState(finalState);
            realCurrentState = buildingController1.GetCurrentState();

            //Assert
            Assert.IsTrue(realCurrentState == finalState); //From fire states state can only be changed back to the state it was before it moved to fire state
        }


        //L2R1 - Testing if fire alarm and fire drill state change with invalid input functions as expected
        [Test]
        [TestCase("open", "fire drill", "closed")]
        [TestCase("closed", "fire drill", "out of hours")]
        [TestCase("out of hours", "fire drill", "open")]
        [TestCase("open", "fire alarm", "out of hours")]
        [TestCase("closed", "fire alarm", "open")]
        [TestCase("out of hours", "fire alarm", "closed")]
        public void Should_Not_Change_State_When_SetCurrentState_Is_Called_After_FireState_Without_Previous_State(string initialState, string fireState, string finalState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController1.SetCurrentState(initialState);
            buildingController1.SetCurrentState(fireState);
            buildingController1.SetCurrentState(finalState);
            realCurrentState = buildingController1.GetCurrentState();

            //Assert
            Assert.IsFalse(realCurrentState == finalState);
            Assert.IsTrue(realCurrentState == fireState);
        }


        //L2R2 - Testing if  setCurrentState() returns true if state is changed to current state
        [Test]
        [TestCase("open", "open")]
        [TestCase("closed", "closed")]
        [TestCase("out of hours", "out of hours")]
        [TestCase("open", "open")]
        [TestCase("closed", "closed")]
        public void Should_Remain_In_Current_State_When_SetCurrentState_Is_Called_With_Current_State(string initialState, string finalState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController1.SetCurrentState(initialState);
            buildingController1.SetCurrentState(finalState);
            realCurrentState = buildingController1.GetCurrentState();

            //Assert
            Assert.IsTrue(realCurrentState == finalState);
        }

        //L2R3 - Testing if second Constructer of BuildingController functions as expected for valid input
        [Test]
        [TestCase("ucl", "open")]
        [TestCase("ucl", "out of hours")]
        [TestCase("ucl", "closed")]
        public void Should_Create_New_BuildingController_With_Assigned_State_When_Second_Constructor_Is_Used(string initialBuildingID, string validState)
        {
            //Arrange
            buildingController2 = new BuildingController(initialBuildingID, validState);
            string buildingID;
            string state;

            //Act
            buildingID = buildingController2.GetBuildingID();
            state = buildingController2.GetCurrentState();

            //Assert
            Assert.IsTrue(buildingID == initialBuildingID);
            Assert.IsTrue(state == validState);
        }


        //L2R3 - Testing if second Constructer of BuildingController functions as expected for invalid input
        [Test]
        [TestCase("ucl", "fire drill")]
        [TestCase("ucl", "fire alarm")]
        [TestCase("ucl", "Open")]
        public void Should_Throw_Exception_When_BuildingController_Initialized_With_Invalid_State(string initialBuildingID, string invalidState)
        {
            //Arrange
            string arguemrntExceptionMessage =
                "BuildingController can only be initialised to the following states: 'open', 'closed', 'out of hours'";

            //Act and Assert
            Assert.Throws<ArgumentException>(() => new BuildingController(initialBuildingID, invalidState), arguemrntExceptionMessage);
        }

        //LEVEL 3 REQUIREMENTS

        //L3R1 - Testing constructor with 6 parameters with valid input
        [Test]
        public void Should_Create_BuildingController_With_Dependencies_When_Created_Using_Third_Constructor()
        {
            //Assert
            Assert.That(buildingController.GetBuildingID(), Is.EqualTo("ucl")); // Building ID should be in lowercase
            Assert.That(buildingController.GetCurrentState(), Is.EqualTo(BuildingController.OUT_OF_HOURS_STRING)); // Current state should be "out of hours"
            Assert.That(buildingController.LightManager, Is.SameAs(LightManager)); // LightManager should be assigned
            Assert.That(buildingController.FireAlarmManager, Is.SameAs(FireAlarmManager)); // FireAlarmManager should be assigned
            Assert.That(buildingController.DoorManager, Is.SameAs(DoorManager)); // DoorManager should be assigned
            Assert.That(buildingController.WebService, Is.SameAs(WebService)); // WebService should be assigned
            Assert.That(buildingController.EmailService, Is.SameAs(EmailService)); //EmailService should be assigned
        }

        //L3R2 - Testing if getStatus() works for LightManager, DoorManager and FireAlarmManager
        [Test]
        [TestCase(
            "Lights,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,",
            "Doors,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,",
            "Fire Alarms,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,")]
        public void Should_Return_Expected_Message_When_GetStatus_Is_Called(
            string expectedReturnStringForLightManager,
            string expectedReturnStringForDoorManager,
            string expectedReturnStringForFireAlarmManager)
        {
            //Arrange
            string realReturnStringForLightManager;
            string realReturnStringForDoorManager;
            string realReturnStringForFireAlarmManager;

            //Act
            realReturnStringForLightManager = LightManager.GetStatus();
            realReturnStringForDoorManager = DoorManager.GetStatus();
            realReturnStringForFireAlarmManager = FireAlarmManager.GetStatus();


            //Assert
            Assert.That(realReturnStringForLightManager, Is.EqualTo(expectedReturnStringForLightManager));
            Assert.That(realReturnStringForDoorManager, Is.EqualTo(expectedReturnStringForDoorManager));
            Assert.That(realReturnStringForFireAlarmManager, Is.EqualTo(expectedReturnStringForFireAlarmManager));
        }

        //L3R3 - Testing GetStatusReport()
        [Test]
        [TestCase(
            "Lights,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,Doors,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,Fire Alarms,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,")]
        public void Should_Return_Expected_Message_When_GetStatusReport_Is_Called(string expectedReturnString)
        {
            //Arrange
            string realReturnString;

            //Act
            realReturnString = buildingController.GetStatusReport();

            //Assert
            Assert.That(realReturnString, Is.EqualTo(expectedReturnString));
        }


        [Test]
        //L3R4 - Setting state to "open" with AllDoorsOpen() returning false from "out of hours" state
        [TestCase(false, "out of hours", "open", "out of hours")]
        public void Should_Return_False_When_Set_To_OpenState_With_Doors_Closed(bool allDoorsOpen, string initialState, string newState, string expectedState)
        {
            //Arrange
            DoorManager.OpenAllDoors().Returns(allDoorsOpen);
            buildingController.SetCurrentState(initialState);
            string realState;

            //Act and Assert
            Assert.IsFalse(buildingController.SetCurrentState(newState));
            realState = buildingController.GetCurrentState();
            Assert.That(expectedState, Is.EqualTo(realState));
        }

        //L3R4 - Setting state to "open" with AllDoorsOpen() returning false  //////////////////////////////////////////////////////////////////////
        [TestCase(true, false, "open", "fire alarm")]   //from "fire alarm" state
        [TestCase(true, false, "open", "fire drill")]    //from "fire drill" state
        public void Should_Return_False_When_Set_To_OpenState_From_FireStates_With_Doors_Closed(bool openDoors, bool closeDoors, string openState, string fireState)
        {
            //Arrange
            DoorManager.OpenAllDoors().Returns(openDoors);  //setting OpenAllDoors() to true in order to setinital state to "open"
            buildingController.SetCurrentState(openState);
            String realState;

            //Act
            buildingController.SetCurrentState(fireState);
            DoorManager.OpenAllDoors().Returns(closeDoors);  //setting OpenAllDoors() to false to test state change

            //Assert
            Assert.IsFalse(buildingController.SetCurrentState(openState));
            realState= buildingController.GetCurrentState();
            Assert.That(realState, Is.EqualTo(fireState));
        }


        [Test]
        //L3R5 - Setting state to "open" with AllDoorsOpen() returning true from "out of hours" state
        [TestCase(true,"out of hours", "open", "open")]
        public void Should_Change_State_To_Open_When_SetCurrentState_To_Open_From_Out_Of_Hours_State_With_Doors_Open(bool allDoorsOpen,string initialState, string newState, string expectedState)
        {
            //Arrange
            DoorManager.OpenAllDoors().Returns(allDoorsOpen);
            DoorManager.LockAllDoors().Returns(false);
            buildingController.SetCurrentState(initialState);
            string realState;

            //Act and Assert
            Assert.IsTrue(buildingController.SetCurrentState(newState));
            realState = buildingController.GetCurrentState();
            Assert.That(expectedState, Is.EqualTo(realState));
        }

        //L3R4 - Setting state to open with AllDoorsOpen() returning true
        [TestCase(true, "open", "fire drill")]   //from "fire drill" state
        [TestCase(true, "open", "fire alarm")]   //from "fire alarm" state
        public void Should_Change_State_To_Open_When_SetCurrentState_To_Open_From_Fire_States_With_Doors_Open(bool allDoorsOpen, string openState, string newState)
        {
            //Arrange
            DoorManager.OpenAllDoors().Returns(true);   //setting OpenAllDoors() to true in order to setinital state to "open"
            buildingController.SetCurrentState(openState);
            DoorManager.OpenAllDoors().Returns(allDoorsOpen);   //setting OpenAllDoors() to false to test state change
            string realState;

            //Act
            buildingController.SetCurrentState(newState);

            //Assert
            Assert.IsTrue(buildingController.SetCurrentState(openState));
            realState = buildingController.GetCurrentState();
            Assert.That(openState, Is.EqualTo(realState));
        }


        //LEVEL 4 REQUIREMENTS

        //L4R1 - Testing if LockAllDoors() functions as expected
        [Test]
        [TestCase("out of hours", "closed")]
        public void Should_Return_True_When_LockAllDoors_Is_Called(string initialState, string newState)
        {
            //Arrange
            buildingController.SetCurrentState(initialState);

            //Act
            buildingController.SetCurrentState(newState);
            DoorManager.LockAllDoors().Returns(true);

            //Assert
            Assert.IsTrue(DoorManager.LockAllDoors());
        }


        //L4R1 - Testing if SetCurrentState("closed") functions as expected when CloseAllDoors() returns true
        [Test]
        [TestCase("out of hours", "closed")]
        public void Should_Change_State_To_Closed_Only_If_CloseAllDoors_Returns_True(string initialState, string newState)
        {
            //Arrange
            buildingController.SetCurrentState(initialState);

            //Act
            DoorManager.LockAllDoors().Returns(true);
            buildingController.SetCurrentState(newState);

            //Assert
            Assert.That(buildingController.GetCurrentState(), Is.EqualTo(newState));
        }


        //L4R1 - Testing if SetCurrentState("closed") returns false if CloseAllDoors() returns false
        [Test]
        [TestCase("out of hours", "closed")]
        public void Should_Not_Change_State_To_Closed_If_CloseAllDoors_Returns_False(string initialState, string newState)
        {
            //Arrange
            buildingController.SetCurrentState(initialState);

            //Act
            DoorManager.LockAllDoors().Returns(false);

            //Assert
            Assert.IsFalse(buildingController.SetCurrentState(newState));
        }


        //L4R2 - Testing if SetCurrentState("fire alarm") functions as expected from out of hours state
        [Test]
        [TestCase("out of hours", "fire alarm")]
        public void Should_SetAlarm_And_SetAllLight_On_When_SetCurrentState_To_Fire_Alarm_From_Out_Of_Hours_State(string initialState, string newState)
        {
            //Arrange
            buildingController.SetCurrentState(initialState);
            DoorManager.OpenAllDoors().Returns(true);


            //Act
            buildingController.SetCurrentState(newState);

            //Assert
            FireAlarmManager.Received().SetAlarm(true);
            LightManager.Received().SetAllLights(true);
            WebService.Received().LogFireAlarm("fire alarm");
            Assert.IsTrue(DoorManager.OpenAllDoors());
        }


        //L4R2 - Testing if SetCurrentState("fire alarm") functions as expected from closed state
        [TestCase("closed", "fire alarm")]
        public void Should_SetAlarm_And_SetAllLight_On_When_SetCurrentState_To_Fire_Alarm_From_Closed_State(string initialState, string newState)
        {
            //Arrange
            DoorManager.LockAllDoors().Returns(true);
            DoorManager.OpenAllDoors().Returns(false);
            buildingController.SetCurrentState(initialState);

            //Act
            DoorManager.LockAllDoors().Returns(false);
            DoorManager.OpenAllDoors().Returns(true);
            buildingController.SetCurrentState(newState);

            //Assert
            FireAlarmManager.Received().SetAlarm(true);
            LightManager.Received().SetAllLights(true);
            WebService.Received().LogFireAlarm("fire alarm");
            Assert.IsTrue(DoorManager.OpenAllDoors());
        }


        //L4R2 - Testing if SetCurrentState("fire alarm") functions as expected from open state
        [TestCase("open", "fire alarm")]
        public void Should_SetAlarm_And_SetAllLight_On_When_SetCurrentState_To_Fire_Alarm_From_Open_State(string initialState, string newState)
        {
            //Arrange
            DoorManager.LockAllDoors().Returns(false);
            DoorManager.OpenAllDoors().Returns(true);
            buildingController.SetCurrentState(initialState);

            //Act
            buildingController.SetCurrentState(newState);

            //Assert
            FireAlarmManager.Received().SetAlarm(true);
            LightManager.Received().SetAllLights(true);
            WebService.Received().LogFireAlarm("fire alarm");
            Assert.IsTrue(DoorManager.OpenAllDoors());
        }


        //L4R3 - Testing GetStatusReport() with fault Devices
        [Test]
        [TestCase(
            "Lights,OK,OK,FAULT,OK,OK,OK,OK,OK,OK,OK,Doors,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,Fire Alarms,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,",
            "Lights,")] //GetStatusReport() for faulty lights
        public void Should_Return_Expected_Message_When_GetStatusReport_Is_Called_When_There_Are_Faults_In_Lights(
            string expectedStatusReport,
            string expectedEngineerLog)
        {
            //Arrange
            string realReturnString;

            //Act
            realReturnString = buildingController_withFaultLights.GetStatusReport();

            //Assert
            WebService.Received().LogEngineerRequired(expectedEngineerLog);
            Assert.That(realReturnString, Is.EqualTo(expectedStatusReport));
        }

        [TestCase(
            "Lights,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,Doors,FAULT,OK,OK,OK,OK,OK,OK,OK,OK,OK,Fire Alarms,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,",
            "Doors,")]  //GetStatusReport() for faulty doors
        public void Should_Return_Expected_Message_When_GetStatusReport_Is_Called_When_There_Are_Faults_In_Doors(
            string expectedStatusReport, 
            string expectedEngineerLog)
        {
            //Arrange
            string realReturnString;

            //Act
            realReturnString = buildingController_withFaultDoors.GetStatusReport();

            //Assert
            WebService.Received().LogEngineerRequired(expectedEngineerLog);
            Assert.That(realReturnString, Is.EqualTo(expectedStatusReport));
        }

        [TestCase(
            "Lights,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,Doors,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,Fire Alarms,OK,OK,OK,OK,OK,OK,OK,OK,OK,FAULT,",
            "Fire Alarms,")] //GetStatusReport() for faulty fire alarms
        public void Should_Return_Expected_Message_When_GetStatusReport_Is_Called_When_There_Are_Faults_In_FireAlarms(
            string expectedStatusReport,
            string expectedEngineerLog)
        {
            //Arrange
            string realReturnString;

            //Act
            realReturnString = buildingController_withFaultFireAlarms.GetStatusReport();

            //Assert
            WebService.Received().LogEngineerRequired(expectedEngineerLog);
            Assert.That(realReturnString, Is.EqualTo(expectedStatusReport));
        }

        [TestCase(
            "Lights,OK,OK,FAULT,OK,OK,OK,OK,OK,OK,OK,Doors,FAULT,OK,OK,OK,OK,OK,OK,OK,OK,OK,Fire Alarms,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,",
            "Lights,Doors,")] //GetStatusReport() for faulty lights and doors
        public void Should_Return_Expected_Message_When_GetStatusReport_Is_Called_When_There_Are_Faults_In_Lights_And_Doors(
            string expectedStatusReport,
            string expectedEngineerLog)
        {
            //Arrange
            string realReturnString;

            //Act
            realReturnString = buildingController_withFaultLights_andFaultDoors.GetStatusReport();

            //Assert
            WebService.Received().LogEngineerRequired(expectedEngineerLog);
            Assert.That(realReturnString, Is.EqualTo(expectedStatusReport));
        }

        [TestCase(
            "Lights,OK,OK,FAULT,OK,OK,OK,OK,OK,OK,OK,Doors,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,Fire Alarms,OK,OK,OK,OK,OK,OK,OK,OK,OK,FAULT,",
            "Lights,Fire Alarms,")] //GetStatusReport() for faulty lights and fire alarms
        public void Should_Return_Expected_Message_When_GetStatusReport_Is_Called_When_There_Are_Faults_In_Lights_And_FireAlarms(
            string expectedStatusReport,
            string expectedEngineerLog)
        {
            //Arrange
            string realReturnString;

            //Act
            realReturnString = buildingController_withFaultLights_andFaultFireAlarms.GetStatusReport();

            //Assert
            WebService.Received().LogEngineerRequired(expectedEngineerLog);
            Assert.That(realReturnString, Is.EqualTo(expectedStatusReport));
        }

        [TestCase(
            "Lights,OK,OK,OK,OK,OK,OK,OK,OK,OK,OK,Doors,FAULT,OK,OK,OK,OK,OK,OK,OK,OK,OK,Fire Alarms,OK,OK,OK,OK,OK,OK,OK,OK,OK,FAULT,",
            "Doors,Fire Alarms,")] //GetStatusReport() for faulty doors and fire alarms
        public void Should_Return_Expected_Message_When_GetStatusReport_Is_Called_When_There_Are_Faults_In_Doors_And_FireAlarms(string expectedStatusReport, string expectedEngineerLog)
        {
            //Arrange
            string realReturnString;

            //Act
            realReturnString = buildingController_withFaultDoors_andFaultFireAlarms.GetStatusReport();

            //Assert
            WebService.Received().LogEngineerRequired(expectedEngineerLog);
            Assert.That(realReturnString, Is.EqualTo(expectedStatusReport));
        }

        [TestCase(
            "Lights,OK,OK,FAULT,OK,OK,OK,OK,OK,OK,OK,Doors,FAULT,OK,OK,OK,OK,OK,OK,OK,OK,OK,Fire Alarms,OK,OK,OK,OK,OK,OK,OK,OK,OK,FAULT,",
            "Lights,Doors,Fire Alarms,")]  //GetStatusReport() for all faulty devices
        public void Should_Return_Expected_Message_When_GetStatusReport_Is_Called_When_There_Are_Faults_In_All_Devices(
            string expectedStatusReport,
            string expectedEngineerLog)
        {
            //Arrange
            string realReturnString;

            //Act
            realReturnString = buildingController_withAllFaultDevices.GetStatusReport();

            //Assert
            WebService.Received().LogEngineerRequired(expectedEngineerLog);
            Assert.That(realReturnString, Is.EqualTo(expectedStatusReport));
        }

        [Test]
        public void Should_Send_Mail_When_State_Changed_To_Fire_Alarm_And_Log_Fire_Alarm_Fails()
        {
            //Arrange
            WebService.When(x => x.LogFireAlarm(Arg.Any<string>())).Do(x => throw new Exception("LogFireAlarm failed"));

            //Act
            bool result = buildingController.SetCurrentState(BuildingController.FIRE_ALARM_STRING);

            //Assert
            Assert.IsFalse(result); // Expecting the SetCurrentState method to return false due to the exception
            EmailService.Received().SendMail(
                "smartbuilding@uclan.ac.uk", // Expected recipient email
                "failed to log alarm", // Expected email subject
                Arg.Is<string>(x => x.Trim().ToLower() == "logfirealarm failed") // Case-insensitive and whitespace-trimmed comparison for the message
                );

        }
    }
}