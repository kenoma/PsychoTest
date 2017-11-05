using NSubstitute;
using NUnit.Framework;
using Psycho.Common.Repository;
using Psycho.Service;
using System;
using Serilog;
using Psycho.Service.Interfaces;
using Psycho.Common.Domain.UserData;
using System.Threading;
using Psycho.Service.Implementations;

namespace Psycho.UnitTests
{
    public class QuestionnaireServiceTest
    {
        private IUserValidation _userValidation;
        private IRespondentsRepository _respondentsRepository;
        private ILogger _log;
        private GatheringManager<SocialNetworkDataVkontakte> _vksocialNetworkGrabber;
        private GatheringManager<SocialNetworkDataFacebook> _fbsocialNetworkGrabber;
        private IQuestionnaireRepository _questionnaireRepository;
        private IOutcomeComputer _outcomputer;
        private CongruenceOutcomeComputer _congruenceOutcomeComputer;

        private QuestionnaireService Create()
        {
            _userValidation = Substitute.For<IUserValidation>();
            _respondentsRepository = Substitute.For<IRespondentsRepository>();
            _vksocialNetworkGrabber = Substitute.For<GatheringManager<SocialNetworkDataVkontakte >>();
            _fbsocialNetworkGrabber = Substitute.For<GatheringManager<SocialNetworkDataFacebook>>();
            _questionnaireRepository = Substitute.For<IQuestionnaireRepository>();
            _log = Substitute.For<ILogger>();
            _outcomputer = Substitute.For<IOutcomeComputer>();
            _congruenceOutcomeComputer = Substitute.For<CongruenceOutcomeComputer>();
            return new QuestionnaireService(_userValidation,
                _respondentsRepository,
                _questionnaireRepository,
                _fbsocialNetworkGrabber,
                _vksocialNetworkGrabber,
                _log,
                _outcomputer,
                _congruenceOutcomeComputer
                );
        }

        [Test]
        public void UpsertRespondentVkontakte_CorrectRoutinesCalled()
        {
            var service = Create();
            _userValidation.ValidateToken(Arg.Is<string>(z => z == "A")).Returns(true);
            RespondentUser respondentUser = new RespondentUser();
            _respondentsRepository.CreateNew().Returns(respondentUser);
            _respondentsRepository.FindById(Arg.Is(respondentUser.Id)).Returns(respondentUser);

            service.UpsertRespondentVkontakte("A", 120, "25081d7553e5fab");

            Thread.Sleep(10);
            _respondentsRepository.Received().FindByVkontakteId(Arg.Is(120));
            _vksocialNetworkGrabber.Received().Enqueue(Arg.Any<int>(), Arg.Is<string>("25081d7553e5fab"));
            _respondentsRepository.Received().Save(Arg.Any<RespondentUser>());
        }

        [Test]
        public void UpsertRespondentFacebook_CorrectRoutinesCalled()
        {
            var service = Create();
            _userValidation.ValidateToken(Arg.Is<string>(z => z == "A")).Returns(true);
            RespondentUser respondentUser = new RespondentUser();
            _respondentsRepository.CreateNew().Returns(respondentUser);
            _respondentsRepository.FindById(Arg.Is(respondentUser.Id)).Returns(respondentUser);

            service.UpsertRespondentFacebook("A", "2345234", "4");

            Thread.Sleep(10);
            _respondentsRepository.Received().FindByFacebookId(Arg.Is("2345234"));
            _fbsocialNetworkGrabber.Received().Enqueue(Arg.Any<int>(), Arg.Is<string>("4"));
            _respondentsRepository.Received().Save(Arg.Any<RespondentUser>());
        }
    }
}
