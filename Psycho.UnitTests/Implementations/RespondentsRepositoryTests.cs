using MongoDB.Driver;
using Serilog;
using NSubstitute;
using NUnit.Framework;
using Psycho.Common.Domain.UserData;
using Psycho.Service.Implementations;

namespace Psycho.UnitTests.Implementations
{
    [TestFixture]
    public class RespondentsRepositoryTests
    {
        private IMongoDatabase subMongoDatabase;
        private ILogger subLogger;

        [SetUp]
        public void TestInitialize()
        {
            var _client = new MongoClient("mongodb://localhost:27017");
            subMongoDatabase = _client.GetDatabase("test");
            subMongoDatabase.DropCollection(nameof(RespondentUser));
            this.subLogger = Substitute.For<ILogger>();
        }

        [Test]
        public void CreateNew_NotNullUser()
        {
            var respondentsRepository = CreateRespondentsRepository();

            var user = respondentsRepository.CreateNew();

            Assert.IsNotNull(user);
        }


        [Test]
        public void SaveAndFindById_NotNullUser()
        {
            var respondentsRepository = CreateRespondentsRepository();
            var user = respondentsRepository.CreateNew();
            user.DataFacebook = new SocialNetworkDataFacebook();
            user.DataVkontakte = new SocialNetworkDataVkontakte();
            user.AttendedQuestionnairies.Add(new Common.Domain.UserData.Guts.ExtendedQuestionnaireChoices { AnswerIds = new int[] { 1, 2, 3 } });

            respondentsRepository.Save(user);
            var restored = respondentsRepository.FindById(user.Id);

            Assert.IsNotNull(restored);
            Assert.IsNotNull(restored.DataFacebook);
            Assert.IsNotNull(restored.DataVkontakte);
            Assert.IsNotNull(restored.AttendedQuestionnairies);
            Assert.AreEqual(3, restored.AttendedQuestionnairies[0].AnswerIds.Count);
        }


        [Test]
        public void FindByFacebookId_NotNullUser()
        {
            var respondentsRepository = CreateRespondentsRepository();
            var user = respondentsRepository.CreateNew();
            user.FbId = "1";
            user.DataFacebook = new SocialNetworkDataFacebook();
            respondentsRepository.Save(user);

            var restored = respondentsRepository.FindByFacebookId("1");

            Assert.IsNotNull(restored);
            Assert.IsNotNull(restored.DataFacebook);
        }

        [Test]
        public void FindByVkontakteId_NotNullUser()
        {
            var respondentsRepository = CreateRespondentsRepository();
            var user = respondentsRepository.CreateNew();
            user.VkId = 1;
            user.DataVkontakte = new SocialNetworkDataVkontakte();
            respondentsRepository.Save(user);

            var restored = respondentsRepository.FindByVkontakteId(1);

            Assert.IsNotNull(restored);
            Assert.IsNotNull(restored.DataVkontakte);
        }

        [Test]
        public void FindByVkontakteId_NullUser()
        {
            var respondentsRepository = CreateRespondentsRepository();

            var restored = respondentsRepository.FindByVkontakteId(1);

            Assert.IsNull(restored);
        }

        [Test]
        public void FindByFacebookId_NullUser()
        {
            var respondentsRepository = CreateRespondentsRepository();

            var restored = respondentsRepository.FindByFacebookId("1");

            Assert.IsNull(restored);
        }

        private RespondentsRepository CreateRespondentsRepository()
        {
            return new RespondentsRepository(
                this.subMongoDatabase,
                this.subLogger);
        }
    }
}
