using NUnit.Framework;
using NSubstitute;
using System.Xml;

namespace G21097711.Tests
{
    [TestFixture]
    public class BuildingControllerTests
    {
        private BuildingController buildingController;
        private BuildingController buildingController2;
        private BuildingController buildingController3;
        private BuildingController buildingController4;


        ILightManager LightManager = Substitute.For<ILightManager>();
        IDoorManager DoorManager = Substitute.For<IDoorManager>();
        IFireAlarmManager FireAlarmManager = Substitute.For<IFireAlarmManager>();
        IEmailService EmailService = Substitute.For<IEmailService>();
        IWebService WebService = Substitute.For<IWebService>();


        [SetUp]
        public void setup()
        {
            buildingController = new BuildingController("ucl");
            buildingController4 = new BuildingController("ucl", LightManager, FireAlarmManager, DoorManager, WebService, EmailService);

            LightManager.GetStatus().Returns("Lights, OK, OK, OK, OK, OK, OK, OK, OK, OK, OK,");
            DoorManager.GetStatus().Returns("Doors, OK, OK, OK, OK, OK, OK, OK, OK, OK, OK,");
            FireAlarmManager.GetStatus().Returns("Fire Alarms, OK, OK, OK, OK, OK, OK, OK, OK, OK, OK,");
        }

        //LEVEL 1 REQUIREMENTS

        //L1R1 - Testing if BuildingController() constructor creates a new instance of the class
        [Test]
        public void testBuildingControllerInitializationAndDecleration() //Test does not have an Arrange and an Act because it is just a null check
        {
            //Assert
            Assert.IsNotNull(buildingController, "BuildingController instance should not be null");
        }


        //L1R2 - Testing if getBuildingID() functions as expencted and if buildingID is stored in expected format
        [Test]
        [TestCase("ucl", "ucl")]
        public void testGetBuildingID(string initialBuildingID, string expectedBuildingID)
        {
            //Arrange
            buildingController.SetBuildingID(initialBuildingID);
            string realBuildingID;

            //Act
            realBuildingID = buildingController.GetBuildingID();

            //Assert
            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));
        }


        //L1R3 - Testing if () if buildingID is stored in expected format
        [Test]
        [TestCase("UCL", "ucl")]                //Testing behaviour with upper case input ID
        [TestCase("123", "123")]                //Testing behaviour with numeric input ID
        [TestCase("ucL - @#$123", "ucl - @#$123")]  //Testing behaviour with symbols in input ID
        public void testBuildingIDFormat(string initialBuildingID, string expectedBuildingID)
        {
            //Arrange
            buildingController.SetBuildingID(initialBuildingID);
            string realBuildingID;

            //Act
            realBuildingID = buildingController.GetBuildingID();

            //Assert
            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));
        }


        //L1R4 - Testing if setBuildingID() functions as expected
        [Test]
        [TestCase("ucl", "ucl123", "ucl123")]               //Test  9: Testing behaviour with new input ID
        [TestCase("ucl", "Ucl - 123!@#", "ucl - 123!@#")]   //Test 10: Testing behaviour with new input ID with uppercas letters, spaces, numbers and symbols
        public void testSetBuildingID(string initialBuildingID, string newBuildingID, string expectedBuildingID)
        {
            //Act
            buildingController.SetBuildingID(initialBuildingID);
            buildingController.SetBuildingID(newBuildingID);

            //Assert
            Assert.That(buildingController.GetBuildingID(), Is.EqualTo(expectedBuildingID));
        }


        //L1R5 - Testing the initialization and decleration of currentState
        [Test]
        [TestCase("out of hours")]
        public void testCurrentStateInitializationAndDecleration(string expectedCurrentState)
        {
            //Arrange
            string realCurrentState;

            //Act
            realCurrentState = buildingController.GetCurrentState();

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
        public void testGetCurrentState(string newCurrentState, string expectedCurrentState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController.SetCurrentState(newCurrentState);
            realCurrentState = buildingController.GetCurrentState();

            //Assert
            Assert.That(realCurrentState, Is.EqualTo(expectedCurrentState));
        }


        //L1R7 - Testing if setCurrentState() functions with valid input
        [Test]
        [TestCase("open")]
        [TestCase("closed")]
        [TestCase("fire drill")]
        [TestCase("fire alarm")]
        public void testSetCurrentStateWithValidInput(string newState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController.SetCurrentState(newState);
            realCurrentState = buildingController.GetCurrentState();

            //Assert
            Assert.That(realCurrentState, Is.EqualTo(newState));
        }


        //L1R7 - Testing if currentState remains unchanged if setCurrentState() is given invalid input
        [Test]
        [TestCase("Out of Hours")]
        [TestCase("outofhours")]
        [TestCase("OPEN")]
        [TestCase("123")]
        public void testSetCurrentStateWithInvalidInput(string newState)
        {
            //Arrange
            string realCurrentState;
            string initialState = "out of hours";

            //Act
            buildingController.SetCurrentState(initialState);
            buildingController.SetCurrentState(newState);
            realCurrentState = buildingController.GetCurrentState();

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
        public void testSetCurrentStateWithUnacceptableStateChange(string initialState, string newState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController.SetCurrentState(initialState);
            buildingController.SetCurrentState(newState);
            realCurrentState = buildingController.GetCurrentState();

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
        public void testFireDrillAndFireAlarm(string initialState, string fireState, string finalState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController.SetCurrentState(initialState);
            buildingController.SetCurrentState(fireState);
            buildingController.SetCurrentState(finalState);
            realCurrentState = buildingController.GetCurrentState();

            //Assert
            Assert.IsTrue(realCurrentState == finalState);
        }


        //L2R1 - Testing if fire alarm and fire drill state change with invalid input functions as expected
        [Test]
        [TestCase("open", "fire drill", "closed")]
        [TestCase("closed", "fire drill", "out of hours")]
        [TestCase("out of hours", "fire drill", "open")]
        [TestCase("open", "fire alarm", "out of hours")]
        [TestCase("closed", "fire alarm", "open")]
        [TestCase("out of hours", "fire alarm", "closed")]
        public void testFireDrillAndFireAlarmWithInvalidInput(string initialState, string fireState, string finalState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController.SetCurrentState(initialState);
            buildingController.SetCurrentState(fireState);
            buildingController.SetCurrentState(finalState);
            realCurrentState = buildingController.GetCurrentState();

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
        public void testStateChangeWhenCurrentStateIsInput(string initialState, string finalState)
        {
            //Arrange
            string realCurrentState;

            //Act
            buildingController.SetCurrentState(initialState);
            buildingController.SetCurrentState(finalState);
            realCurrentState = buildingController.GetCurrentState();

            //Assert
            Assert.IsTrue(realCurrentState == finalState);
        }

        //L2R3 - Testing if second Constructer of BuildingController functions as expected for valid input
        [Test]
        [TestCase("ucl", "open")]
        [TestCase("ucl", "out of hours")]
        [TestCase("ucl", "closed")]
        public void testSecondConstructerWithValidInput(string initialBuildingID, string validState)
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
        public void testSecondConstructerWithInvalidInput(string initialBuildingID, string invalidState)
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
        public void testConstructorWithDependenciesUsingValidInput()
        {
            //Arrange
            buildingController3 = new BuildingController("Ucl", LightManager, FireAlarmManager, DoorManager, WebService, EmailService);
            string expectedBuildingID;
            string realBuildingID;

            //Act
            expectedBuildingID = buildingController3.GetBuildingID();

            //Assert
            Assert.IsNotNull(buildingController);
            Assert.That(buildingController3.GetBuildingID(), Is.EqualTo(expectedBuildingID));
        }

        //L3R2 - Testing if getStatus() works for LightManager, DoorManager and FireAlarmManager
        [Test]
        [TestCase(
            "Lights, OK, OK, OK, OK, OK, OK, OK, OK, OK, OK,",
            "Doors, OK, OK, OK, OK, OK, OK, OK, OK, OK, OK,",
            "Fire Alarms, OK, OK, OK, OK, OK, OK, OK, OK, OK, OK,")]
        public void testIfLightManagersReturnExpectedStatus(
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

        //L3R4 - Testing GetStatusReport()
        [Test]
        [TestCase(
            "Lights, OK, OK, OK, OK, OK, OK, OK, OK, OK, OK,Doors, OK, OK, OK, OK, OK, OK, OK, OK, OK, OK,Fire Alarms, OK, OK, OK, OK, OK, OK, OK, OK, OK, OK,")]
        public void testGetStatusReport(string expectedReturnString)
        {
            //Arrange
            string realReturnString;

            //Act
            realReturnString = buildingController4.GetStatusReport();

            //Assert
            Assert.That(realReturnString, Is.EqualTo(expectedReturnString));
        }


        //L3R5 - Testing SetCurrentState("open") with OpenAllDoors() set to true and false
        [Test]
        [TestCase(true, "open", "open")]
        [TestCase(false, "open", "out of hours")]
        public void testSetCurrentStateToOpen(bool allDoorsOpen, string openState, string expectedState)
        {
            //Arrange
            DoorManager.OpenAllDoors().Returns(allDoorsOpen);
            string realState;

            //Act
            buildingController4.SetCurrentState(openState); //Knowing that contructor initializes the state to "out of hours"
            realState = buildingController4.GetCurrentState();

            //Assert
            Assert.That(expectedState, Is.EqualTo(realState));
        }







    }
}