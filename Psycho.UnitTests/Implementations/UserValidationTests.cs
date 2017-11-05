using Serilog;
using NSubstitute;
using NUnit.Framework;
using Psycho.Common.Repository;
using Psycho.Service.Implementations;
using System;

namespace Psycho.UnitTests.Implementations
{
    [TestFixture]
    public class UserValidationTests
    {
        private IServiceUserRepository subServiceUserRepository;
        private ILogger _log;
        [SetUp]
        public void TestInitialize()
        {
            this.subServiceUserRepository = Substitute.For<IServiceUserRepository>();
            _log = Substitute.For<ILogger>();
        }

        [Test]
        public void Auth_DoNotPassUser()
        {
            UserValidation userValidation = this.CreateUserValidation();
            subServiceUserRepository.CheckUser("A", "B").Returns(false);

            Assert.Throws<UnauthorizedAccessException>(() => userValidation.Auth("A", "B"));
        }

        [Test]
        public void Auth_PassUser()
        {
            UserValidation userValidation = this.CreateUserValidation();
            subServiceUserRepository.CheckUser("A", "B").Returns(true);

            var token = userValidation.Auth("A", "B");

            Assert.IsNotEmpty(token);
        }

        [Test]
        public void ValidateToken_PassUser()
        {
            UserValidation userValidation = this.CreateUserValidation();
            subServiceUserRepository.CheckUser("A", "B").Returns(true);
            var token = userValidation.Auth("A", "B");

            var res = userValidation.ValidateToken(token);

            Assert.IsTrue(res);
        }

        [Test]
        public void ValidateToken_DontPassUser()
        {
            UserValidation userValidation = this.CreateUserValidation();
            var token = "an random string";

            var res = userValidation.ValidateToken(token);

            Assert.IsFalse(res);
        }

        private UserValidation CreateUserValidation()
        {
            return new UserValidation(
                this.subServiceUserRepository,
                _log);
        }
    }
}
