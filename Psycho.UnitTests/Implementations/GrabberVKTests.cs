using Serilog;
using NSubstitute;
using NUnit.Framework;
using Psycho.Service.Implementations;

namespace Psycho.UnitTests.Implementations
{
    [TestFixture]
    public class GrabberVKTests
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
            var grabberVK = this.CreateGrabberVK();

            var data = grabberVK.GetUserData("161939b582580f7f7aba3ba1caa41bb7b54ea94e9a7dd147cffa7e4daf5d21c37c3504640a6c6ce108307");

            Assert.IsNotNull(data);
        }

        private GrabberVK CreateGrabberVK()
        {
            return new GrabberVK(subLogger);
        }
    }
}
