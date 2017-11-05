using Serilog;
using NSubstitute;
using NUnit.Framework;
using Psycho.Service.Implementations;

namespace Psycho.UnitTests.Implementations
{
    [TestFixture]
    public class GrabberFBTests
    {
        private ILogger subLogger;

        [SetUp]
        public void TestInitialize()
        {
            subLogger = Substitute.For<ILogger>();
        }

        [Test, Category("Long runner"), Explicit]
        public void GetUserData()
        {
            var grabberFB = this.CreateGrabberFB();

            var data= grabberFB.GetUserData("AQDQeVCCiH_HXjQXMZ3mrW4VxqR3xQbW18I1dRx6OiUV55w4QIAuUxdpgHvgGSrtBEpz7xWAOohnNZ1wLMmO9MJUMrMJ34WT6WzL40W4S0lcBdUhii2M4yZpSp65rDATzYugMTHPGKs8_RTg5WiI35NsUTrt11R3rcyum3Y_kElWgtfEjxAP5Lczo_xW0HRkzHyCUM6r2Wdii-d1GVBkeEYt5pobhpYgfHl7qmYAatTxOPgML0uPyUK14YmU-rcc5TdSrCvbYUyp4iJ_ojiHv5oAOGaysJgWBRRrAi0I0qdeWge2WFkg9aZgnzei0oTAkl5vYrB3hMbfWqCjEf22Goeo");

            Assert.IsNotNull(data);
        }

        private GrabberFB CreateGrabberFB()
        {
            return new GrabberFB(
                this.subLogger);
        }
    }
}
