using NUnit.Framework;
using System.Xml;

namespace G21097711.Tests
{
    public class BuildingControllerTests
    {
        //UNIT TESTS FOR L1 R1
        //Test 1: Testing if BuildingController() constructor initializes BuildingID
        [Test]
        public void TestBuildingControllerConstructor_1()
        {
            string inputBuildingID = "ucl";
            string expectedBuildingID = "ucl";

            BuildingController A = new BuildingController(inputBuildingID);
            string realBuildingID = A.GetBuildingID();

            Assert.IsNotNull(A, "BuildingController instance should not be null");
            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));
        }

        //Test 2: Testing Constructer for BuildingController to see if it accepts spaces, symbols and numbers
        [Test]
        public void TestBuildingControllerConstructor_2()
        {
            string expectedBuildingID = "ucl - 123";

            BuildingController A = new BuildingController(expectedBuildingID);
            string realBuildingID = A.GetBuildingID();

            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));
        }

        //UNIT TESTS FOR L1 R2
        //Test 1: Testing if GetBuildingID() functions
        [Test]
        public void TestGetBuildingID()
        {
            string expectedBuildingID = "ucl";

            BuildingController A = new BuildingController(expectedBuildingID);
            string realBuildingID = A.GetBuildingID();

            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));
        }

        //UNIT TESTS FOR L1 R3
        //Test 1: Testing if BuildingID is converted to Lowercase if input is all in uppercase
        [Test]
        public void TestToLowerCase_1()
        {
            string expectedBuildingID = "ucl";
            string inputBuildingID = "UCL";

            BuildingController A = new BuildingController(inputBuildingID);
            string realBuildingID = A.GetBuildingID();

            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));

        }

        //Test 2: Testing if BuildingID is converted to Lowercase if input has uppercase and lowercase
        [Test]
        public void TestToLowerCase_2()
        {
            string expectedBuildingID = "ucl";
            string inputBuildingID = "UcL";

            BuildingController A = new BuildingController(inputBuildingID);
            string realBuildingID = A.GetBuildingID();

            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));
        }

        //Test 3: Testing if BuildingID is converted to Lowercase if input is already lowercase
        [Test]
        public void TestToLowerCase_3()
        {
            string expectedBuildingID = "ucl";
            string inputBuildingID = "ucl";

            BuildingController A = new BuildingController(inputBuildingID);
            string realBuildingID = A.GetBuildingID();

            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));
        }

        //UNIT TESTS FOR L1 R4
        //Test 1: Testing if SetBuildingID() functions sets buildingID to new input
        [Test]
        public void TestSetBuildingID_1()
        {
            string initialBuildingID = "ucl";   //Initialized buildingID
            string newBuildingID = "ucl123";   //New buildingID

            BuildingController A = new BuildingController(initialBuildingID);
            A.SetBuildingID(newBuildingID);
            string realBuildingID = A.GetBuildingID();

            Assert.That(realBuildingID, Is.EqualTo(newBuildingID));
        }

        //Test 2: Testing if SetBuildingID() converts Uppercase characters to Lowercase
        [Test]
        public void TestSetBuildingID_2()
        {
            string initialBuildingID = "ucl";   //Initialized buildingID
            string newBuildingID = "UCL123";   //Changed buildingID
            string expectedBuildingID = "ucl123";   //Final expected buildingID

            BuildingController A = new BuildingController(initialBuildingID);
            A.SetBuildingID(newBuildingID);
            string realBuildingID = A.GetBuildingID();

            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));
        }

        //Test 3: Testing if SetBuildingID() accepts spaces, symbols and numbers
        [Test]
        public void TestSetBuildingID_3()
        {
            string initialBuildingID = "ucl";   //Initialized buildingID
            string newBuildingID = "ucl - 123";   //Changed buildingID
            string expectedBuildingID = "ucl - 123";   //Final expected buildingID

            BuildingController A = new BuildingController(initialBuildingID);
            A.SetBuildingID(newBuildingID);
            string realBuildingID = A.GetBuildingID();

            Assert.That(realBuildingID, Is.EqualTo(expectedBuildingID));
        }

        //UNIT TESTS FOR L1 R5 and L1 R6
        //Test 1: Testing if BuildingController() constructor initializes currentState to "out of hours"
        //Test 2: Testing if GetCurrentState() returns currentState
        [Test]
        public void TestBuildingControllerConstructor_3()
        {
            string inputBuildingID = "UCL";
            string expectedCurrentState = "out of hours";

            BuildingController A = new BuildingController(inputBuildingID);
            string realCurrentState = A.GetCurrentState();

            Assert.That(realCurrentState, Is.EqualTo(expectedCurrentState));
        }

        //UNIT TESTS FOR L1 R7
        //Test 1: Testing if SetCurrentState functions with valid input
        [Test]
        public void TestSetCurrentState_1()
        {
            string inputBuildingID = "ucl";

            BuildingController A = new BuildingController(inputBuildingID);
            string[] validInputsArray =
            {
                "open", "out of hours", "closed", "out of hours"
            };

            for (int i = 0; i < validInputsArray.Length; i++)
            {
                Assert.IsTrue(A.SetCurrentState(validInputsArray[i]));
                string realCurrentState = A.GetCurrentState();
                Assert.AreEqual(realCurrentState, validInputsArray[i]);
            }
        }

        //Test 2: Testing if SetCurrentState() functions if inputstate == currentState
        [Test]
        public void TestSetCurrentState_2()
        {
            string inputBuildingID = "ucl";
            string expectedCurrentState = "out of hours";
            string realCurrentState = "";

            BuildingController A = new BuildingController(inputBuildingID); //currentState is initialized to "out of hours" using constructor
            realCurrentState = A.GetCurrentState();
            if (realCurrentState == expectedCurrentState)
            {
                A.SetCurrentState(expectedCurrentState);
            }
            realCurrentState = A.GetCurrentState();

            Assert.IsTrue(realCurrentState == expectedCurrentState);
        }

        //Test 3: Testing if SetCurrentState() functions for invalid input
        [Test]
        public void TestSetCurrentState_3()
        {
            string inputBuildingID = "ucl";

            BuildingController A = new BuildingController(inputBuildingID);
            const string INVALID_INPUT_1 = "Out of Order";
            const string INVALID_INPUT_2 = "OPEN";
            const string INVALID_INPUT_3 = "closing";
            const string INVALID_INPUT_4 = "holiday";
            string[] invalidInputsArray =
            {
                INVALID_INPUT_1, INVALID_INPUT_2, INVALID_INPUT_3, INVALID_INPUT_4
            };

            for (int i = 0; i < invalidInputsArray.Length; i++)
            {
                Assert.IsFalse(A.SetCurrentState(invalidInputsArray[i]));
            }
        }

        //UNIT TESTS FOR L2 R1
        //Test 1: Testing if SetCurrentState functions when input is valid but not a state it can change to
        [Test]
        public void TestSetCurrentState_4()
        {
            string inputBuildingID = "ucl";

            BuildingController A = new BuildingController(inputBuildingID);

            A.SetCurrentState("open");
            Assert.IsFalse(A.SetCurrentState("closed"));    //if current state is open, SetCurrentState("closed") should return false

            A.SetCurrentState("out of hours");
            A.SetCurrentState("closed");
            Assert.IsFalse(A.SetCurrentState("open"));  //if current state is closed, SetCurrentState("open") should return false
        }

        //Test 2: Testing if SetCurrentState functions for fire alarm and fire drill with valid input
        [Test]
        public void TestSetCurrentState_5()
        {
            string inputBuildingID = "ucl";

            BuildingController A = new BuildingController(inputBuildingID);

            Assert.IsTrue(A.SetCurrentState("fire drill"));

            A.SetCurrentState("out of hours");
            A.SetCurrentState("closed");

            Assert.IsTrue(A.SetCurrentState("fire alarm"));

            A.SetCurrentState("closed");
            A.SetCurrentState("out of hours");
            A.SetCurrentState("open");

            Assert.IsTrue(A.SetCurrentState("fire drill"));
        }










    }
}