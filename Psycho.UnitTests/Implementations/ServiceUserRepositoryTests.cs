using MongoDB.Driver;
using NUnit.Framework;
using Psycho.Common.Domain.AdminArea;
using Psycho.Service.Implementations;
using System.IO;

namespace Psycho.UnitTests.Implementations
{
    [TestFixture]
    public class ServiceUserRepositoryTests
    {
        private IMongoDatabase subMongoDatabase;
        private MongoClient _client;

        [SetUp]
        public void TestInitialize()
        {
            _client = new MongoClient("mongodb://localhost:27017");
            subMongoDatabase = _client.GetDatabase("test");
            subMongoDatabase.DropCollection(nameof(ServiceUser));
        }

        [TearDown]
        public void Cleanup()
        {
            subMongoDatabase.DropCollection(nameof(ServiceUser));
        }

        [Test]
        public void CheckUser_DoNotPass()
        {
            ServiceUserRepository serviceUserRepository = this.CreateServiceUserRepository();

            var res = serviceUserRepository.CheckUser(Path.GetRandomFileName(), "B");

            Assert.IsFalse(res);
        }

        [Test]
        public void CheckUser_Pass()
        {
            ServiceUserRepository serviceUserRepository = this.CreateServiceUserRepository();
            string username = Path.GetRandomFileName();
            var serviceUsers = subMongoDatabase.GetCollection<ServiceUser>(nameof(ServiceUser));
            serviceUsers.InsertOne(new ServiceUser { Username = username, Password = "B" });

            var res = serviceUserRepository.CheckUser(username, "B");

            Assert.IsTrue(res);
        }

        private ServiceUserRepository CreateServiceUserRepository()
        {
            return new ServiceUserRepository(
                this.subMongoDatabase);
        }
    }
}
