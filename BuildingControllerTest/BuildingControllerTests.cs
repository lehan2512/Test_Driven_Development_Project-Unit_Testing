using NUnit.Framework;

namespace G21097711.Tests
{
    public class BuildingControllerTests
    {
        [Test]
        public void TestBuildingControllerCreation()
        {
            string expectedBuildingID = "UCL";
            BuildingController A = new BuildingController(expectedBuildingID);
            Assert.IsNotNull(A, "BuildingController instance should not be null");
        }
        

        [Test]
        public void TestGetBuildingID()
        {
            string expectedBuildingID = "UCL";
            BuildingController A = new BuildingController(expectedBuildingID);
            Assert.That(A.GetBuildingID, Is.EqualTo(expectedBuildingID));
        }
    }
}   