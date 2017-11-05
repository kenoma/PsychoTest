using MongoDB.Driver;
using Serilog;
using NSubstitute;
using NUnit.Framework;
using Psycho.Common.Domain;
using Psycho.Service.Implementations;
using System.Collections.Generic;
using System.Linq;

namespace Psycho.UnitTests.Implementations
{
    [TestFixture]
    public class QuestionnaireRepositoryTests
    {
        private IMongoDatabase subMongoDatabase;
        private ILogger subLogger;

        [SetUp]
        public void TestInitialize()
        {
            this.subLogger = Substitute.For<ILogger>();
            var _client = new MongoClient("mongodb://localhost:27017");
            subMongoDatabase = _client.GetDatabase("test");
            subMongoDatabase.DropCollection(nameof(QuestionnaireScope));
        }

        [TearDown]
        public void Cleanup()
        {
            subMongoDatabase.DropCollection(nameof(QuestionnaireScope));
        }

        [Test]
        public void Add_SuccessfullyCreateNewRecord()
        {
            var questionnaireRepository = CreateQuestionnaireRepository();
            var scope = new QuestionnaireScope { Id = 1, Entries = new List<QuestionnaireEntry> { new QuestionnaireEntry { Answers = new List<QuestionnaireEntryAnswer> { new QuestionnaireEntryAnswer { Id = -1 } } } } };

            var res = questionnaireRepository.Add(scope);

            Assert.IsTrue(res);
            Assert.AreEqual(1, scope.Entries[0].Answers[0].Id);
        }

        [Test]
        public void Find_ReturnExistingScope()
        {
            var questionnaireRepository = CreateQuestionnaireRepository();
            var scope = new QuestionnaireScope { Id = 1, Entries = new List<QuestionnaireEntry> { new QuestionnaireEntry { Answers = new List<QuestionnaireEntryAnswer> { new QuestionnaireEntryAnswer { Id = -1 } } } } };
            questionnaireRepository.Add(scope);

            var res = questionnaireRepository.FindQuestionnaireById(scope.Id);

            Assert.AreEqual(scope.Id, res.Id);
            Assert.AreEqual(1, scope.Entries[0].Answers[0].Id);
        }

        [Test]
        public void GetAll_MapScopesToDTO()
        {
            var questionnaireRepository = CreateQuestionnaireRepository();
            
            questionnaireRepository.Add(new QuestionnaireScope { Id = 1, Name = "ABC", Entries = new List<QuestionnaireEntry> { new QuestionnaireEntry { Answers = new List<QuestionnaireEntryAnswer> { new QuestionnaireEntryAnswer { Id = -1 } } } } });
            questionnaireRepository.Add(new QuestionnaireScope { Id = 1, Name = "BAB", Entries = new List<QuestionnaireEntry> { new QuestionnaireEntry { Answers = new List<QuestionnaireEntryAnswer> { new QuestionnaireEntryAnswer { Id = -1 } } } } });
            questionnaireRepository.Add(new QuestionnaireScope { Id = 1, Name = "DAB", Entries = new List<QuestionnaireEntry> { new QuestionnaireEntry { Answers = new List<QuestionnaireEntryAnswer> { new QuestionnaireEntryAnswer { Id = -1 } } } } });

            var res = questionnaireRepository.GetAll();

            Assert.IsTrue(res.Any(z => z.Name == "ABC"));
            Assert.IsTrue(res.Any(z => z.Name == "BAB"));
            Assert.IsTrue(res.Any(z => z.Name == "DAB"));
        }

        [Test]
        public void RemoveQuestionnaire_ReturnNullAfterDeletion()
        {
            var questionnaireRepository = CreateQuestionnaireRepository();
            var scope = new QuestionnaireScope { Id = 1, Entries = new List<QuestionnaireEntry> { new QuestionnaireEntry { Answers = new List<QuestionnaireEntryAnswer> { new QuestionnaireEntryAnswer { Id = -1 } } } } };
            questionnaireRepository.Add(scope);

            var res = questionnaireRepository.RemoveQuestionnaire(scope.Id);
            var aftermath = questionnaireRepository.FindQuestionnaireById(scope.Id);

            Assert.IsTrue(res);
            Assert.IsNull(aftermath);
        }

        private QuestionnaireRepository CreateQuestionnaireRepository()
        {
            return new QuestionnaireRepository(
                this.subMongoDatabase,
                this.subLogger);
        }
    }
}
